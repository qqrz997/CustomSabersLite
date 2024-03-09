using BS_Utils.Gameplay;
using CustomSaber.Configuration;
using CustomSaber.Data;
using CustomSaber.Utilities;
using IPA.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomSaber.Components
{
    internal class CustomSaberManager : MonoBehaviour
    {
        public CustomSaber LeftSaber { get; private set; }

        public CustomSaber RightSaber { get; private set; }

        private ColorScheme colorScheme;

        private GameObject customSabersObject;

        private GameObject leftSaberObject;

        private GameObject rightSaberObject;

        private EventManager leftSaberEventManager;

        private EventManager rightSaberEventManager;

        private BeatmapObjectManager beatmapObjectManager;
        private ScoreController scoreController;
        private ComboController comboController;
        private ObstacleSaberSparkleEffectManager saberCollisionManager;
        private GameEnergyCounter gameEnergyCounter;
        private BeatmapCallbacksController beatmapCallback;
        private PlayerHeadAndObstacleInteraction playerHeadAndObstacleInteraction;
        private PauseController pauseController;
        private float lastNoteTime;

        private CustomSaberManager instance;

        public void Init()
        {
            Plugin.Log.Info("Game scene loaded, initializing the CustomSaberManager");

            GameObject go = GameObject.Find("VRGameCore");
            instance = go.AddComponent<CustomSaberManager>();
        }

        private void Awake()
        {
            colorScheme = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.colorScheme;

            if (customSabersObject)
            {
                Destroy(customSabersObject);
                customSabersObject = null;
            }

            if (CustomSaberConfig.Instance.CurrentlySelectedSaber == "Default")
            {
                CustomSaberAssetLoader.SelectedSaber = new CustomSaberData("DefaultSabers");
            }
            else
            {
                if (CustomSaberConfig.Instance.CurrentlySelectedSaber != CustomSaberAssetLoader.SelectedSaber?.FileName)
                {
                    // The saber was changed so load the new one
                    CustomSaberAssetLoader.SelectedSaber?.Destroy();
                    CustomSaberAssetLoader.SelectedSaber = CustomSaberAssetLoader.LoadSaberWithRepair(CustomSaberConfig.Instance.CurrentlySelectedSaber);
                }
            }

            CustomSaberData customSaberData = CustomSaberAssetLoader.SelectedSaber;

            if (customSaberData != null)
            {
                if (customSaberData.FileName != "DefaultSabers")
                {
                    if (customSaberData.SabersObject)
                    {
                        Plugin.Log.Debug($"Custom saber is selected, replacing sabers: {customSaberData.FileName}");
                        customSabersObject = Instantiate(customSaberData.SabersObject);
                        leftSaberObject = customSabersObject.transform.Find("RightSaber").gameObject;
                        rightSaberObject = customSabersObject.transform.Find("LeftSaber").gameObject;
                        LeftSaber = leftSaberObject.AddComponent<CustomSaber>();
                        RightSaber = rightSaberObject.AddComponent<CustomSaber>();
                    }

                    StartCoroutine(WaitForSabers(customSaberData.SabersObject));
                }
                else
                {
                    StartCoroutine(WaitForDefaultSabers());
                }
            }
            else
            {
                Plugin.Log.Error("Current CustomSaberData is null");
            }
        }

        private IEnumerator WaitForDefaultSabers()
        {
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<Saber>().Any());

            if (CustomSaberUtils.CheckMultiplayer())
            {
                yield break;
            }

            IEnumerable<Saber> defaultSabers = Resources.FindObjectsOfTypeAll<Saber>();

            // Hide the default trails if 'None' trail is selected
            // They stay disabled through switching levels so they need to be re-enabled if the setting is changed
            foreach (Saber saber in defaultSabers)
            {
                SaberTrail defaultTrail = GetVanillaTrail(saber);

                if (CustomSaberConfig.Instance.TrailType == TrailType.None)
                {
                    CustomSaberUtils.HideTrail(defaultTrail);
                }
                else
                {
                    CustomSaberUtils.SetTrailDuration(defaultTrail);
                    CustomSaberUtils.SetWhiteTrailDuration(defaultTrail);
                    defaultTrail.enabled = true;
                }
            }
        }

        private IEnumerator WaitForSabers(GameObject saberRoot)
        {
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<Saber>().Any());

            if (CustomSaberUtils.CheckMultiplayer())
            {
                DestroyImmediate(saberRoot);
                yield break;
            }

            AddEvents();

            IEnumerable<Saber> defaultSabers = Resources.FindObjectsOfTypeAll<Saber>();

            foreach (Saber defaultSaber in defaultSabers)
            {
                Plugin.Log.Debug($"Hiding default saber model for {defaultSaber.saberType}");

                // Hide each saber mesh
                IEnumerable<MeshFilter> meshFilters = defaultSaber.transform.GetComponentsInChildren<MeshFilter>();
                foreach (MeshFilter meshFilter in meshFilters)
                {
                    meshFilter.gameObject.SetActive(!saberRoot);

                    /*MeshFilter filter = meshFilter.GetComponentInChildren<MeshFilter>();
                    filter.gameObject.SetActive(!saberRoot);*/
                }

                Color saberColour = Color.white;
                GameObject customSaber = null;

                switch (defaultSaber.saberType)
                {
                    case SaberType.SaberA:
                        customSaber = leftSaberObject;
                        saberColour = colorScheme.saberAColor;
                        break;

                    case SaberType.SaberB:
                        customSaber = rightSaberObject;
                        saberColour = colorScheme.saberBColor;
                        break;
                }

                if (customSaber != null)
                {
                    customSaber.transform.SetParent(defaultSaber.transform);
                    customSaber.transform.position = defaultSaber.transform.position;
                    customSaber.transform.rotation = defaultSaber.transform.rotation;
                }
                else
                {
                    Plugin.Log.Error("Something went wrong when getting the custom saber instance"); yield break;
                }

                SetCustomSaberColour(customSaber, saberColour);

                SaberTrail defaultTrail = GetVanillaTrail(defaultSaber);

                switch (CustomSaberConfig.Instance.TrailType)
                {
                    case TrailType.Custom:
                        AddCustomSaberTrails(customSaber, saberColour, defaultSaber, defaultTrail);
                        break;

                    case TrailType.Vanilla:
                        CustomSaberUtils.SetTrailDuration(defaultTrail);
                        CustomSaberUtils.SetWhiteTrailDuration(defaultTrail);
                        break;

                    case TrailType.None:
                        CustomSaberUtils.HideTrail(defaultTrail);
                        break;
                }
            }
        }

        private void AddCustomSaberTrails(GameObject customSaber, Color saberColour, Saber defaultSaber, SaberTrail defaultTrail)
        {
            CustomTrail customTrail;
            try
            {
                customTrail = customSaber.GetComponent<CustomTrail>();
                Plugin.Log.Debug("Successfully got CustomTrail from custom saber.");
            }
            catch
            {
                customTrail = null;
            }

            if (customTrail == null)
            {
                Plugin.Log.Warn("No custom trails. Defaulting to existing saber trails.");
                CustomSaberUtils.SetTrailDuration(defaultTrail);
                CustomSaberUtils.SetWhiteTrailDuration(defaultTrail);
            }
            else
            {
                Plugin.Log.Debug($"Initializing custom trail to {defaultTrail.name}");

                var handler = new CustomTrailHandler(customSaber, customTrail);
                handler.CreateTrail(defaultTrail, saberColour);
            }
        }

        private void SetCustomSaberColour(GameObject saber, Color colour)
        {
            IEnumerable<Renderer> renderers = saber.GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderers)
            {
                if (renderer == null) continue;

                foreach (Material rendererMaterial in renderer.materials)
                {
                    if (rendererMaterial == null) continue;

                    if (rendererMaterial.HasProperty("_Color"))
                    {
                        if (rendererMaterial.HasProperty("_CustomColors"))
                        {
                            if (rendererMaterial.GetFloat("_CustomColors") > 0)
                            {
                                rendererMaterial.SetColor("_Color", colour);
                            }
                        }
                        else if (rendererMaterial.HasProperty("_Glow") && rendererMaterial.GetFloat("_Glow") > 0
                            || rendererMaterial.HasProperty("_Bloom") && rendererMaterial.GetFloat("_Bloom") > 0)
                        {
                            rendererMaterial.SetColor("_Color", colour);
                        }
                    }
                }
            }
        }

        private void OnDestroy() => RemoveEvents();

        private SaberTrail GetVanillaTrail(Saber defaultSaber)
        {
            SaberTrail trail;
            try
            {
                trail = defaultSaber.gameObject.GetComponentInChildren<SaberTrail>();
                Plugin.Log.Debug("Successfully got SaberTrail from default saber.");
            }
            catch
            {
                trail = null;
            }
            return trail;
        }

        private EventManager GetEventManagerByType(SaberType saberType)
        {
            EventManager eventManager = null;
            switch (saberType)
            {
                case SaberType.SaberA:
                    eventManager = leftSaberEventManager; break;

                case SaberType.SaberB:
                    eventManager = rightSaberEventManager; break;
            }
            return eventManager;
        }

        private void AddEvents()
        {
            leftSaberEventManager = leftSaberObject?.GetComponent<EventManager>();
            if (!leftSaberEventManager)
            {
                leftSaberEventManager = leftSaberObject.AddComponent<EventManager>();
            }

            rightSaberEventManager = rightSaberObject?.GetComponent<EventManager>();
            if (!rightSaberEventManager)
            {
                rightSaberEventManager = rightSaberObject.AddComponent<EventManager>();
            }

            if (leftSaberEventManager?.OnLevelStart == null
                || rightSaberEventManager?.OnLevelStart == null)
            {
                return;
            }

            leftSaberEventManager.OnLevelStart.Invoke();
            rightSaberEventManager.OnLevelStart.Invoke();

            try
            {
                scoreController = FindObjectsOfType<ScoreController>().FirstOrDefault();
                if (scoreController)
                {
                    scoreController.multiplierDidChangeEvent += MultiplierCallBack;
                }
                else
                {
                    Plugin.Log.Warn($"Failed to locate a suitable '{nameof(ScoreController)}'.");
                }

                beatmapObjectManager = ReflectionUtil.GetField<BeatmapObjectManager, ScoreController>(scoreController, "_beatmapObjectManager");
                if (beatmapObjectManager != null)
                {
                    beatmapObjectManager.noteWasCutEvent += SliceCallBack;
                    beatmapObjectManager.noteWasMissedEvent += NoteMissCallBack;
                }
                else
                {
                    Plugin.Log.Warn($"Failed to locate a suitable '{nameof(BeatmapObjectManager)}'.");
                }

                comboController = FindObjectsOfType<ComboController>().FirstOrDefault();
                if (scoreController)
                {
                    comboController.comboDidChangeEvent += ComboChangeEvent;
                }
                else
                {
                    Plugin.Log.Warn($"Failed to locate a suitable '{nameof(ScoreController)}'.");
                }

                saberCollisionManager = Resources.FindObjectsOfTypeAll<ObstacleSaberSparkleEffectManager>().FirstOrDefault();
                if (saberCollisionManager)
                {
                    saberCollisionManager.sparkleEffectDidStartEvent += SaberStartCollide;
                    saberCollisionManager.sparkleEffectDidEndEvent += SaberEndCollide;
                }
                else
                {
                    Plugin.Log.Warn($"Failed to locate a suitable '{nameof(ObstacleSaberSparkleEffectManager)}'.");
                }

                gameEnergyCounter = Resources.FindObjectsOfTypeAll<GameEnergyCounter>().FirstOrDefault();
                if (gameEnergyCounter)
                {
                    gameEnergyCounter.gameEnergyDidReach0Event += FailLevelCallBack;
                }
                else
                {
                    Plugin.Log.Warn($"Failed to locate a suitable '{nameof(GameEnergyCounter)}'.");
                }

                /*beatmapCallback = Resources.FindObjectsOfTypeAll<BeatmapCallbacksController>().FirstOrDefault();
                if (beatmapCallback)
                {
                    beatmapCallback.beatmapEventDidTriggerEvent += LightEventCallBack;
                }
                else
                {
                    Plugin.Log.Warn($"Failed to locate a suitable '{nameof(BeatmapObjectCallbackController)}'.");
                }*/

                playerHeadAndObstacleInteraction = scoreController.GetField<PlayerHeadAndObstacleInteraction, ScoreController>("_playerHeadAndObstacleInteraction");
                if (playerHeadAndObstacleInteraction == null)
                {
                    Plugin.Log.Warn($"Failed to locate a suitable '{nameof(PlayerHeadAndObstacleInteraction)}'.");
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.Error(ex);
                throw;
            }

            /*try
            {
                float LastTime = 0.0f;
                LevelData levelData = BS_Utils.Plugin.LevelData;
                BeatmapData beatmapData = levelData.GameplayCoreSceneSetupData.difficultyBeatmap.beatmapData;

                IReadOnlyList<IReadonlyBeatmapLineData> beatmapLinesData = beatmapData.beatmapLinesData;
                foreach (BeatmapLineData beatMapLineData in beatmapLinesData)
                {
                    IReadOnlyList<BeatmapObjectData> beatmapObjectsData = beatMapLineData.beatmapObjectsData;
                    for (int i = beatmapObjectsData.Count - 1; i >= 0; i--)
                    {
                        BeatmapObjectData beatmapObjectData = beatmapObjectsData[i];
                        if (beatmapObjectData.beatmapObjectType == BeatmapObjectType.Note
                            && ((NoteData)beatmapObjectData).colorType != global::ColorType.None)
                        {
                            if (beatmapObjectData.time > LastTime)
                            {
                                LastTime = beatmapObjectData.time;
                            }
                        }
                    }
                }

                lastNoteTime = LastTime;
            }
            catch (Exception ex)
            {
                Plugin.Log.Error(ex);
                throw;
            }*/
        }

        private void RemoveEvents()
        {
            if (pauseController != null)
            {
                pauseController.didResumeEvent += OnPauseResume;
            }

            if (beatmapObjectManager != null)
            {
                beatmapObjectManager.noteWasCutEvent -= SliceCallBack;
                beatmapObjectManager.noteWasMissedEvent -= NoteMissCallBack;
            }

            if (scoreController)
            {
                scoreController.multiplierDidChangeEvent -= MultiplierCallBack;
            }

            if (comboController)
            {
                comboController.comboDidChangeEvent -= ComboChangeEvent;
            }

            if (saberCollisionManager)
            {
                saberCollisionManager.sparkleEffectDidStartEvent -= SaberStartCollide;
                saberCollisionManager.sparkleEffectDidEndEvent -= SaberEndCollide;
            }

            if (gameEnergyCounter)
            {
                gameEnergyCounter.gameEnergyDidReach0Event -= FailLevelCallBack;
            }

            /*if (beatmapCallback != null)
            {
                beatmapCallback.beatmapEventDidTriggerEvent -= LightEventCallBack;
            }*/
        }

        private void OnPauseResume()
        {
            foreach (var saberTrailRenderer in Resources.FindObjectsOfTypeAll<SaberTrailRenderer>())
            {
                saberTrailRenderer.enabled = true;
            }
        }

        private void SliceCallBack(NoteController noteController, in NoteCutInfo noteCutInfo)
        {
            if (!noteCutInfo.allIsOK)
            {
                leftSaberEventManager?.OnComboBreak?.Invoke();
                rightSaberEventManager?.OnComboBreak?.Invoke();
                // StartCoroutine(CalculateAccuracyAndFireEvents());
            }
            else
            {
                EventManager eventManager = GetEventManagerByType(noteCutInfo.saberType);
                eventManager?.OnSlice?.Invoke();
                // noteCutInfo.swingRatingCounter.didFinishEvent += OnSwingRatingCounterFinished;
            }

            if (Mathf.Approximately(noteController.noteData.time, lastNoteTime))
            {
                lastNoteTime = 0;
                leftSaberEventManager?.OnLevelEnded?.Invoke();
                rightSaberEventManager?.OnLevelEnded?.Invoke();
            }
        }

        private void NoteMissCallBack(NoteController noteController)
        {
            if (noteController.noteData.colorType != global::ColorType.None)
            {
                leftSaberEventManager?.OnComboBreak?.Invoke();
                rightSaberEventManager?.OnComboBreak?.Invoke();
            }

            if (Mathf.Approximately(noteController.noteData.time, lastNoteTime))
            {
                lastNoteTime = 0;
                leftSaberEventManager?.OnLevelEnded?.Invoke();
                rightSaberEventManager?.OnLevelEnded?.Invoke();
            }

            // StartCoroutine(CalculateAccuracyAndFireEvents());
        }

        private void MultiplierCallBack(int multiplier, float progress)
        {
            if (multiplier > 1 && progress < 0.1f)
            {
                leftSaberEventManager?.MultiplierUp?.Invoke();
                rightSaberEventManager?.MultiplierUp?.Invoke();
            }
        }

        private void SaberStartCollide(SaberType saberType)
        {
            EventManager eventManager = GetEventManagerByType(saberType);
            eventManager?.SaberStartColliding?.Invoke();
        }

        private void SaberEndCollide(SaberType saberType)
        {
            EventManager eventManager = GetEventManagerByType(saberType);
            eventManager?.SaberStopColliding?.Invoke();
        }

        private void FailLevelCallBack()
        {
            leftSaberEventManager?.OnLevelFail?.Invoke();
            rightSaberEventManager?.OnLevelFail?.Invoke();
        }

        /*private void LightEventCallBack(BeatmapEventData songEvent)
        {
            if ((int)songEvent.type < 5)
            {
                if (songEvent. > 0 && songEvent.value < 4)
                {
                    leftSaberEventManager?.OnBlueLightOn?.Invoke();
                    rightSaberEventManager?.OnBlueLightOn?.Invoke();
                }

                if (songEvent.value > 4 && songEvent.value < 8)
                {
                    leftSaberEventManager?.OnRedLightOn?.Invoke();
                    rightSaberEventManager?.OnRedLightOn?.Invoke();
                }
            }
        }*/

        private void ComboChangeEvent(int combo)
        {
            leftSaberEventManager?.OnComboChanged?.Invoke(combo);
            rightSaberEventManager?.OnComboChanged?.Invoke(combo);
        }

        /*private IEnumerator CalculateAccuracyAndFireEvents()
        {
            yield return null;

            var rawScore = scoreController.modifiedScore;
            var maximumScore = ScoreModel.ComputeMaxMultipliedScoreForBeatmap(ReflectionUtil.GetField<int, ScoreController>(scoreController, "_cutOrMissedNotes"));
            var accuracy = (float)rawScore / (float)maximumScore;

            leftEventManager?.OnAccuracyChanged?.Invoke(accuracy);
            rightEventManager?.OnAccuracyChanged?.Invoke(accuracy);
        }*/

        /*private void OnSwingRatingCounterFinished(ISaberSwingRatingCounter afterCutRating)
        {
            afterCutRating.didFinishEvent -= OnSwingRatingCounterFinished;
            StartCoroutine(CalculateAccuracyAndFireEvents());
        }*/
    }
}

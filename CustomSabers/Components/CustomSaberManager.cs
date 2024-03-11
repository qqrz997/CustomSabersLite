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
        private RelativeScoreAndImmediateRankCounter relativeScoreCounter;
        private ObstacleSaberSparkleEffectManager saberCollisionManager;
        private GameEnergyCounter gameEnergyCounter;
        private float? lastNoteTime;
        private float previousScore;

        public void Init()
        {
            Plugin.Log.Info("Game scene loaded, initializing the CustomSaberManager");

            GameObject go = GameObject.Find("VRGameCore");
            go.AddComponent<CustomSaberManager>();
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

            if (CustomSaberConfig.Instance.CustomEventsEnabled) AddEvents();

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

        #region events
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

            Plugin.Log.Debug("Adding events");

            IReadonlyBeatmapData beatmapData = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData.transformedBeatmapData;

            lastNoteTime = GetLastNoteTime(beatmapData);

            try
            {
                scoreController = FindObjectsOfType<ScoreController>().FirstOrDefault(); //?
                beatmapObjectManager = ReflectionUtil.GetField<BeatmapObjectManager, ScoreController>(scoreController, "_beatmapObjectManager");
                comboController = FindObjectsOfType<ComboController>().FirstOrDefault(); //?
                saberCollisionManager = Resources.FindObjectsOfTypeAll<ObstacleSaberSparkleEffectManager>().FirstOrDefault();
                gameEnergyCounter = Resources.FindObjectsOfTypeAll<GameEnergyCounter>().FirstOrDefault();
                relativeScoreCounter = Resources.FindObjectsOfTypeAll<RelativeScoreAndImmediateRankCounter>().FirstOrDefault();
            }
            catch (Exception ex)
            {
                Plugin.Log.Error("Problem encountered when trying to get event objects");
                Plugin.Log.Error(ex);
            }

            if (scoreController)
            {
                scoreController.multiplierDidChangeEvent += MultiplierChanged;
            }

            if (beatmapObjectManager != null)
            {
                beatmapObjectManager.noteWasCutEvent += NoteWasCut;
                beatmapObjectManager.noteWasMissedEvent += NoteWasMissed;
            }

            if (scoreController)
            {
                comboController.comboDidChangeEvent += ComboChanged;
            }

            if (saberCollisionManager)
            {
                saberCollisionManager.sparkleEffectDidStartEvent += SaberStartedCollision;
                saberCollisionManager.sparkleEffectDidEndEvent += SaberEndedCollision;
            }

            if (gameEnergyCounter)
            {
                gameEnergyCounter.gameEnergyDidReach0Event += LevelWasFailed;
            }

            if (relativeScoreCounter)
            {
                relativeScoreCounter.relativeScoreOrImmediateRankDidChangeEvent += ScoreChangedEvent;
            }

            leftSaberEventManager.OnLevelStart.Invoke();
            rightSaberEventManager.OnLevelStart.Invoke();
        }

        private void OnDestroy() => RemoveEvents();

        private void RemoveEvents()
        {
            if (beatmapObjectManager != null)
            {
                beatmapObjectManager.noteWasCutEvent -= NoteWasCut;
                beatmapObjectManager.noteWasMissedEvent -= NoteWasMissed;
            }

            if (scoreController)
            {
                scoreController.multiplierDidChangeEvent -= MultiplierChanged;
            }

            if (comboController)
            {
                comboController.comboDidChangeEvent -= ComboChanged;
            }

            if (saberCollisionManager)
            {
                saberCollisionManager.sparkleEffectDidStartEvent -= SaberStartedCollision;
                saberCollisionManager.sparkleEffectDidEndEvent -= SaberEndedCollision;
            }

            if (gameEnergyCounter)
            {
                gameEnergyCounter.gameEnergyDidReach0Event -= LevelWasFailed;
            }

            if (relativeScoreCounter)
            {
                relativeScoreCounter.relativeScoreOrImmediateRankDidChangeEvent -= ScoreChangedEvent;
            }
        }

        private void NoteWasCut(NoteController noteController, in NoteCutInfo noteCutInfo)
        {
            if (!lastNoteTime.HasValue) return;

            if (noteCutInfo.allIsOK)
            {
                // Note was cut
                EventManager eventManager = GetEventManagerByType(noteCutInfo.saberType);
                eventManager?.OnSlice?.Invoke();
            }
            else
            {
                // Player has skill issue
                leftSaberEventManager?.OnComboBreak?.Invoke();
                rightSaberEventManager?.OnComboBreak?.Invoke();
            }

            if (Mathf.Approximately(noteController.noteData.time, lastNoteTime.Value))
            {
                lastNoteTime = 0;
                leftSaberEventManager?.OnLevelEnded?.Invoke();
                rightSaberEventManager?.OnLevelEnded?.Invoke();
            }
        }

        private void NoteWasMissed(NoteController noteController)
        {
            if (!lastNoteTime.HasValue) return;

            if (noteController.noteData.colorType != ColorType.None)
            {
                leftSaberEventManager?.OnComboBreak?.Invoke();
                rightSaberEventManager?.OnComboBreak?.Invoke();
            }

            if (Mathf.Approximately(noteController.noteData.time, lastNoteTime.Value))
            {
                lastNoteTime = 0;
                leftSaberEventManager?.OnLevelEnded?.Invoke();
                rightSaberEventManager?.OnLevelEnded?.Invoke();
            }
        }

        private void MultiplierChanged(int multiplier, float progress)
        {
            if (multiplier > 1 && progress < 0.1f)
            {
                leftSaberEventManager?.MultiplierUp?.Invoke();
                rightSaberEventManager?.MultiplierUp?.Invoke();
            }
        }

        private void ComboChanged(int combo)
        {
            leftSaberEventManager?.OnComboChanged?.Invoke(combo);
            rightSaberEventManager?.OnComboChanged?.Invoke(combo);
        }

        private void SaberStartedCollision(SaberType saberType)
        {
            EventManager eventManager = GetEventManagerByType(saberType);
            eventManager?.SaberStartColliding?.Invoke();
        }

        private void SaberEndedCollision(SaberType saberType)
        {
            EventManager eventManager = GetEventManagerByType(saberType);
            eventManager?.SaberStopColliding?.Invoke();
        }

        private void LevelWasFailed()
        {
            leftSaberEventManager?.OnLevelFail?.Invoke();
            rightSaberEventManager?.OnLevelFail?.Invoke();
        }

        private void ScoreChangedEvent()
        {
            float relativeScore = relativeScoreCounter.relativeScore;
            if (Math.Abs(previousScore - relativeScore) > 0f)
            {
                leftSaberEventManager?.OnAccuracyChanged?.Invoke(relativeScore);
                rightSaberEventManager?.OnAccuracyChanged?.Invoke(relativeScore);
                previousScore = relativeScore;
            }
        }

        private void LightEventCallback()
        {

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

        private float GetLastNoteTime(IReadonlyBeatmapData beatmapData)
        {
            float lastNoteTime = 0.0f;
            foreach (var noteData in beatmapData.GetBeatmapDataItems<NoteData>(0))
            {
                if (noteData.colorType == ColorType.None) continue;

                if (noteData.time > lastNoteTime)
                {
                    lastNoteTime = noteData.time;
                }
            }
            return lastNoteTime;
        }
        #endregion
    }
}

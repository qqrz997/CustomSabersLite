using CustomSaber;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomSabersLite.Components
{
    internal class CSLSaber : MonoBehaviour
    {
        public CSLSaberModelController Controller;

        private IEnumerable<Renderer> saberRenderers;

        private EventManager eventManager;

        private IReadonlyBeatmapData beatmapData;

        public void Setup(IReadonlyBeatmapData beatmapData, Transform parent)
        {
            this.beatmapData = beatmapData;

            transform.SetParent(parent);
            transform.position = parent.position;
            transform.rotation = parent.rotation;
        }

        public void Awake()
        {
            saberRenderers = gameObject.GetComponentsInChildren<Renderer>();
        }

        public void Start()
        {
            try
            {
                eventManager = gameObject.GetComponent<EventManager>();
            }
            catch
            {
                eventManager = null;
            }

            AddEvents();
        }

        public void SetColor(Color colour)
        {
            foreach (Renderer renderer in saberRenderers)
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

        #region EVENTS

        private BeatmapObjectManager beatmapObjectManager;
        private ScoreController scoreController;
        private ComboController comboController;
        private RelativeScoreAndImmediateRankCounter relativeScoreCounter;
        private ObstacleSaberSparkleEffectManager saberCollisionManager;
        private GameEnergyCounter gameEnergyCounter;
        private float? lastNoteTime;
        private float previousScore;
        public void AddEvents()
        {
            if (eventManager == null)
            {
                eventManager = gameObject.AddComponent<EventManager>();
            }

            if (eventManager?.OnLevelStart == null) return;

            Logger.Debug("Adding events");

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
                Logger.Error("Problem encountered when trying to get event objects");
                Logger.Error(ex.ToString());
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

            eventManager.OnLevelStart.Invoke();
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
                eventManager?.OnSlice?.Invoke();
            }
            else
            {
                // Player has skill issue
                eventManager?.OnComboBreak?.Invoke();
            }

            if (Mathf.Approximately(noteController.noteData.time, lastNoteTime.Value))
            {
                lastNoteTime = 0;
                eventManager?.OnLevelEnded?.Invoke();
            }
        }

        private void NoteWasMissed(NoteController noteController)
        {
            if (!lastNoteTime.HasValue) return;

            if (noteController.noteData.colorType != ColorType.None)
            {
                eventManager?.OnComboBreak?.Invoke();
            }

            if (Mathf.Approximately(noteController.noteData.time, lastNoteTime.Value))
            {
                lastNoteTime = 0;
                eventManager?.OnLevelEnded?.Invoke();
            }
        }

        private void MultiplierChanged(int multiplier, float progress)
        {
            if (multiplier > 1 && progress < 0.1f)
            {
                eventManager?.MultiplierUp?.Invoke();
            }
        }

        private void ComboChanged(int combo)
        {
            eventManager?.OnComboChanged?.Invoke(combo);
        }

        private void SaberStartedCollision(SaberType saberType)
        {
            eventManager?.SaberStartColliding?.Invoke();
        }

        private void SaberEndedCollision(SaberType saberType)
        {
            eventManager?.SaberStopColliding?.Invoke();
        }

        private void LevelWasFailed()
        {
            eventManager?.OnLevelFail?.Invoke();
        }

        private void ScoreChangedEvent()
        {
            float relativeScore = relativeScoreCounter.relativeScore;
            if (Math.Abs(previousScore - relativeScore) > 0f)
            {
                eventManager?.OnAccuracyChanged?.Invoke(relativeScore);
                previousScore = relativeScore;
            }
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

using CustomSaber;
using CustomSabersLite.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Components
{
    internal class EventManagerManager : IDisposable
    {
        private readonly BeatmapObjectManager beatmapObjectManager;
        private readonly GameEnergyCounter gameEnergyCounter;
        private readonly ObstacleSaberSparkleEffectManager obstacleCollisionManager;
        private readonly RelativeScoreAndImmediateRankCounter relativeScoreCounter;
        private readonly IScoreController scoreController;
        private readonly IComboController comboController;
        private readonly IReadonlyBeatmapData beatmapData;
        private readonly CSLConfig config;

        public EventManagerManager(BeatmapObjectManager beatmapObjectManager, GameEnergyCounter gameEnergyCounter, ObstacleSaberSparkleEffectManager obstacleCollisionManager, RelativeScoreAndImmediateRankCounter relativeScoreCounter, IScoreController scoreController, IComboController comboController, IReadonlyBeatmapData beatmapData, CSLConfig config)
        {
            this.beatmapObjectManager = beatmapObjectManager;
            this.gameEnergyCounter = gameEnergyCounter;
            this.obstacleCollisionManager = obstacleCollisionManager;
            this.relativeScoreCounter = relativeScoreCounter;
            this.scoreController = scoreController;
            this.comboController = comboController;
            this.beatmapData = beatmapData;
            this.config = config;
        }

        private EventManager eventManager;
        private float? lastNoteTime;
        private float previousScore;
        private SaberType saberType;

        public void InitializeEventManager(EventManager eventManager, SaberType saberType)
        {
            this.eventManager = eventManager;
            this.saberType = saberType;

            if (!config.EnableCustomEvents)
            {
                return;
            }

            AddEvents();
        }

        public void Dispose()
        {
            RemoveEvents();
        }

        public void AddEvents()
        {
            if (eventManager?.OnLevelStart == null)
            {
                return;
            }

            Logger.Debug("Adding events");

            lastNoteTime = GetLastNoteTime(beatmapData);

            scoreController.multiplierDidChangeEvent += MultiplierChanged;
            
            beatmapObjectManager.noteWasCutEvent += NoteWasCut;
            beatmapObjectManager.noteWasMissedEvent += NoteWasMissed;

            comboController.comboDidChangeEvent += ComboChanged;

            if (obstacleCollisionManager)
            {
                obstacleCollisionManager.sparkleEffectDidStartEvent += SaberStartedCollision;
                obstacleCollisionManager.sparkleEffectDidEndEvent += SaberEndedCollision;
            }

            gameEnergyCounter.gameEnergyDidReach0Event += LevelWasFailed;

            relativeScoreCounter.relativeScoreOrImmediateRankDidChangeEvent += ScoreChangedEvent;

            eventManager.OnLevelStart.Invoke();
        }

        private void RemoveEvents()
        {
            beatmapObjectManager.noteWasCutEvent -= NoteWasCut;
            beatmapObjectManager.noteWasMissedEvent -= NoteWasMissed;

            scoreController.multiplierDidChangeEvent -= MultiplierChanged;

            comboController.comboDidChangeEvent -= ComboChanged;

            if (obstacleCollisionManager)
            {
                obstacleCollisionManager.sparkleEffectDidStartEvent -= SaberStartedCollision;
                obstacleCollisionManager.sparkleEffectDidEndEvent -= SaberEndedCollision;
            }

            gameEnergyCounter.gameEnergyDidReach0Event -= LevelWasFailed;

            relativeScoreCounter.relativeScoreOrImmediateRankDidChangeEvent -= ScoreChangedEvent;
        }

        private void NoteWasCut(NoteController noteController, in NoteCutInfo noteCutInfo)
        {
            if (!lastNoteTime.HasValue) return;

            if (noteCutInfo.allIsOK)
            {
                // Note was cut
                if (noteCutInfo.saberType == saberType)
                {
                    eventManager?.OnSlice?.Invoke();
                }
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
    }
}

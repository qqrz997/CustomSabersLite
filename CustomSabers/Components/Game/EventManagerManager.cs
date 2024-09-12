using CustomSaber;
using CustomSabersLite.Configuration;
using System;
using UnityEngine;
using System.Linq;

namespace CustomSabersLite.Components.Game;

internal class EventManagerManager(BeatmapObjectManager beatmapObjectManager, GameEnergyCounter gameEnergyCounter, ObstacleSaberSparkleEffectManager obstacleCollisionManager, RelativeScoreAndImmediateRankCounter relativeScoreCounter, IScoreController scoreController, IComboController comboController, IReadonlyBeatmapData beatmapData, CSLConfig config) : IDisposable
{
    private readonly BeatmapObjectManager beatmapObjectManager = beatmapObjectManager;
    private readonly GameEnergyCounter gameEnergyCounter = gameEnergyCounter;
    private readonly ObstacleSaberSparkleEffectManager obstacleCollisionManager = obstacleCollisionManager;
    private readonly RelativeScoreAndImmediateRankCounter relativeScoreCounter = relativeScoreCounter;
    private readonly IScoreController scoreController = scoreController;
    private readonly IComboController comboController = comboController;
    private readonly IReadonlyBeatmapData beatmapData = beatmapData;
    private readonly CSLConfig config = config;

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

    public void Dispose()
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

    private void ComboChanged(int combo) => eventManager?.OnComboChanged?.Invoke(combo);

    private void SaberStartedCollision(SaberType saberType) => eventManager?.SaberStartColliding?.Invoke();

    private void SaberEndedCollision(SaberType saberType) => eventManager?.SaberStopColliding?.Invoke();

    private void LevelWasFailed() => eventManager?.OnLevelFail?.Invoke();

    private void ScoreChangedEvent()
    {
        var relativeScore = relativeScoreCounter.relativeScore;
        if (Math.Abs(previousScore - relativeScore) > 0f)
        {
            eventManager?.OnAccuracyChanged?.Invoke(relativeScore);
            previousScore = relativeScore;
        }
    }

    private float GetLastNoteTime(IReadonlyBeatmapData beatmapData) => beatmapData
        .GetBeatmapDataItems<NoteData>(0)
        .LastOrDefault(data => data.colorType != ColorType.None)?.time ?? 0.0f;
}

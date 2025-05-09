﻿using System;
using System.Linq;
using CustomSaber;
using CustomSabersLite.Utilities.Extensions;

namespace CustomSabersLite.Services;

internal class SaberEventService : IDisposable
{
    private readonly BeatmapObjectManager beatmapObjectManager;
    private readonly GameEnergyCounter gameEnergyCounter;
    private readonly ObstacleSaberSparkleEffectManager obstacleCollisionManager;
    private readonly RelativeScoreAndImmediateRankCounter relativeScoreCounter;
    private readonly IScoreController scoreController;
    private readonly IComboController comboController;
    private readonly IReadonlyBeatmapData beatmapData;

    public SaberEventService(BeatmapObjectManager beatmapObjectManager,
        GameEnergyCounter gameEnergyCounter,
        ObstacleSaberSparkleEffectManager obstacleCollisionManager,
        RelativeScoreAndImmediateRankCounter relativeScoreCounter,
        IScoreController scoreController,
        IComboController comboController,
        IReadonlyBeatmapData beatmapData)
    {
        this.beatmapObjectManager = beatmapObjectManager;
        this.gameEnergyCounter = gameEnergyCounter;
        this.obstacleCollisionManager = obstacleCollisionManager;
        this.relativeScoreCounter = relativeScoreCounter;
        this.scoreController = scoreController;
        this.comboController = comboController;
        this.beatmapData = beatmapData;
    }

    private EventManager? eventManager;
    private float? lastNoteTime;
    private float previousScore;
    private SaberType saberType;

    public void InitializeEventManager(EventManager? eventManager, SaberType saberType)
    {
        if (eventManager == null || eventManager.OnLevelStart == null)
        {
            return;
        }
        
        this.eventManager = eventManager;
        this.saberType = saberType;

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
        if (lastNoteTime == null || eventManager == null) return;

        if (!noteCutInfo.allIsOK)
        {
            // Player has skill issue
            eventManager.OnComboBreak?.Invoke();
        }
        else if (noteCutInfo.saberType == saberType)
        {
            // Note was cut
            eventManager.OnSlice?.Invoke();
        }

        if (noteController.noteData.time.Approximately(lastNoteTime.Value))
        {
            lastNoteTime = 0;
            eventManager.OnLevelEnded?.Invoke();
        }
    }

    private void NoteWasMissed(NoteController noteController)
    {
        if (lastNoteTime == null || eventManager == null) return;

        if (noteController.noteData.colorType != ColorType.None)
        {
            eventManager.OnComboBreak?.Invoke();
        }

        if (noteController.noteData.time.Approximately(lastNoteTime.Value))
        {
            lastNoteTime = 0;
            eventManager.OnLevelEnded?.Invoke();
        }
    }

    private void MultiplierChanged(int multiplier, float progress)
    {
        if (eventManager != null && multiplier > 1 && progress < 0.1f)
        {
            eventManager.MultiplierUp?.Invoke();
        }
    }

    private void ComboChanged(int combo)
    {
        if (eventManager != null) eventManager.OnComboChanged?.Invoke(combo);
    }

    private void SaberStartedCollision(SaberType saberType)
    {
        if (eventManager != null) eventManager.SaberStartColliding?.Invoke();
    }

    private void SaberEndedCollision(SaberType saberType)
    {
        if (eventManager != null) eventManager.SaberStopColliding?.Invoke();
    }

    private void LevelWasFailed()
    {
        if (eventManager != null) eventManager.OnLevelFail?.Invoke();
    }

    private void ScoreChangedEvent()
    {
        float relativeScore = relativeScoreCounter.relativeScore;
        if (Math.Abs(previousScore - relativeScore) > 0f)
        {
            if (eventManager != null) eventManager.OnAccuracyChanged?.Invoke(relativeScore);
            previousScore = relativeScore;
        }
    }

    private float GetLastNoteTime(IReadonlyBeatmapData beatmapData) => beatmapData
        .GetBeatmapDataItems<NoteData>(0)
        .LastOrDefault(data => data.colorType != ColorType.None)?.time ?? 0.0f;
}

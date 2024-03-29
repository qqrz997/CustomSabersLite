﻿using IPA.Utilities;
using System;
using Zenject;
using UnityEngine;
using CustomSabersLite.Utilities;
using CustomSabersLite.Configuration;

namespace CustomSabersLite.Components
{
    internal class ExtraSaberManager : IInitializable, IDisposable
    {
        private readonly CSLConfig config;
        private readonly CSLSaberSet saberSet;
        private readonly SaberManager saberManager;
        private readonly ScoreController scoreController;

        public ExtraSaberManager(ScoreController scoreController, CSLSaberSet saberSet, CSLConfig config, SaberManager saberManager)
        {
            this.scoreController = scoreController;
            this.saberSet = saberSet;
            this.config = config;
            this.saberManager = saberManager;
        }

        private BeatmapObjectManager beatmapObjectManager;

        private CSLSaber leftSaber;
        private CSLSaber rightSaber;

        private readonly string defaultSaberObjectName = "BasicSaberModel(Clone)";
        private Transform defaultLeftSaber => saberManager.leftSaber.transform.Find(defaultSaberObjectName);
        private Transform defaultRightSaber => saberManager.rightSaber.transform.Find(defaultSaberObjectName); 

        public void Initialize()
        {
            beatmapObjectManager = ReflectionUtil.GetField<BeatmapObjectManager, ScoreController>(scoreController, "_beatmapObjectManager");

            DateTime time = Utils.CanUseDateTimeNowSafely ? DateTime.Now : DateTime.UtcNow;

            if (!(time.Month == 4 && time.Day == 1))
            {
                return;
            }

            if (config.Fooled && !config.ForcefullyFoolish)
            {
                return;
            }
            else
            {
                Logger.Critical("Let the fun begin!");
                config.Fooled = true;
            }

            if (config.CurrentlySelectedSaber != "Default")
            {
                leftSaber = saberSet.LeftSaber;
                rightSaber = saberSet.RightSaber;

                AddCustomEvents();
            }
            else
            {
                AddDefaultEvents();
            }
        }

        public void Dispose()
        {
            RemoveCustomEvents();
            RemoveDefaultEvents();
        }

        private void AddCustomEvents() => beatmapObjectManager.noteWasCutEvent += CustomSaberDidCutNote;
        private void AddDefaultEvents() => beatmapObjectManager.noteWasCutEvent += DefaultSaberDidCutNote;

        private void RemoveCustomEvents() => beatmapObjectManager.noteWasCutEvent -= CustomSaberDidCutNote;
        private void RemoveDefaultEvents() => beatmapObjectManager.noteWasCutEvent += DefaultSaberDidCutNote;

        private void CustomSaberDidCutNote(NoteController noteController, in NoteCutInfo noteCutInfo)
        {
            switch (noteCutInfo.saberType)
            {
                case SaberType.SaberA:
                    ExtraSetupLeft(leftSaber.transform); break;
                case SaberType.SaberB:
                    ExtraSetupRight(rightSaber.transform); break;
            }
        }

        private void DefaultSaberDidCutNote(NoteController noteController, in NoteCutInfo noteCutInfo)
        {
            switch (noteCutInfo.saberType)
            {
                case SaberType.SaberA:
                    ExtraSetupLeft(defaultLeftSaber); break;
                case SaberType.SaberB:
                    ExtraSetupRight(defaultRightSaber); break;
            }
        }

        private bool didntSetupLeft = true;

        private void ExtraSetupLeft(Transform parent)
        {
            parent.localEulerAngles += new Vector3(0f, 180f, 0f);

            if (didntSetupLeft)
            {
                parent.localPosition = new Vector3(0f, 0f, 0.81f);
            }
            else
            {
                parent.localPosition = new Vector3(0f, 0f, 0.0f);
            }

            didntSetupLeft = !didntSetupLeft;
        }

        private bool didntSetupRight = true;

        private void ExtraSetupRight(Transform parent)
        {
            parent.localEulerAngles += new Vector3(0f, 180f, 0f);

            if (didntSetupRight)
            {
                parent.localPosition = new Vector3(0f, 0f, 0.83f);
            }
            else
            {
                parent.localPosition = new Vector3(0f, 0f, 0.0f);
            }

            didntSetupRight = !didntSetupRight;
        }
    }
}

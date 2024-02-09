using System;
using UnityEngine;
using UnityEngine.Events;

namespace CustomSaber
{
    [AddComponentMenu("Custom Sabers/Event Manager")]
    public class EventManager : MonoBehaviour
    {
        [Serializable]
        public class ComboChangedEvent : UnityEvent<int> { }

        [Serializable]
        public class AccuracyChangedEvent : UnityEvent<float> { }

        public UnityEvent OnSlice;

        public UnityEvent OnComboBreak;

        public UnityEvent MultiplierUp;

        public UnityEvent SaberStartColliding;

        public UnityEvent SaberStopColliding;

        public UnityEvent OnLevelStart;

        public UnityEvent OnLevelFail;

        public UnityEvent OnLevelEnded;

        public UnityEvent OnBlueLightOn;

        public UnityEvent OnRedLightOn;

        [HideInInspector]
        public ComboChangedEvent OnComboChanged = new ComboChangedEvent();

        [HideInInspector]
        public AccuracyChangedEvent OnAccuracyChanged = new AccuracyChangedEvent();
    }
}
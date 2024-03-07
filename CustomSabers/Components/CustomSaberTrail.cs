using UnityEngine;

namespace CustomSaber.Utilities
{
    internal class CustomSaberTrail : SaberTrail
    {
        protected bool _customTrailInited;

        private Transform _customTrailTopTransform;

        private Transform _customTrailBottomTransform;

        private readonly SaberMovementData _customTrailMovementData = new SaberMovementData();

        private Vector3 _customTrailTopPos;

        private Vector3 _customTrailBottomPos;

        public SaberMovementData customTrailMovementData => _customTrailMovementData;

        void Awake()
        {
            //i'm stupid and i don't know why i need this but i do so yer
        }

        public void Setup(Transform customTrailTopTransform, Transform customTrailBottomTransform)
        {
            //Custom saber trails don't all work well with the regular trail values so we have to use their settings (currently done by handler)
            //Extra settings may be needed
            _customTrailTopTransform = customTrailTopTransform;
            _customTrailBottomTransform = customTrailBottomTransform;
            gameObject.layer = 12;

            _customTrailInited = true;
        }

        void Update()
        {
            if (gameObject.activeInHierarchy)
            {
                _customTrailTopPos = _customTrailTopTransform.position;
                _customTrailBottomPos = _customTrailBottomTransform.position;
                _customTrailMovementData.AddNewData(_customTrailTopPos, _customTrailBottomPos, TimeHelper.time);
            }
        }
    }
}
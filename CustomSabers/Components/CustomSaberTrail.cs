using UnityEngine;

namespace CustomSaber.Utilities
{
    internal class CustomSaberTrail : SaberTrail
    {
        private Transform customTrailTopTransform;

        private Transform customTrailBottomTransform;

        private readonly SaberMovementData customTrailMovementData = new SaberMovementData();

        private Vector3 customTrailTopPos;

        private Vector3 customTrailBottomPos;

        public SaberMovementData CustomTrailMovementData => customTrailMovementData;

        void Awake()
        {
            // i'm stupid and i don't know why i need this but i do so yer
        }

        public void Setup(Transform topTransform, Transform bottomTransform)
        {
            // Custom saber trails don't all work well with the regular trail values so we have to use their settings (currently done by handler)
            // Extra settings may be needed
            customTrailTopTransform = topTransform;
            customTrailBottomTransform = bottomTransform;
            gameObject.layer = 12;
        }

        void Update()
        {
            if (gameObject.activeInHierarchy)
            {
                customTrailTopPos = customTrailTopTransform.position;
                customTrailBottomPos = customTrailBottomTransform.position;
                customTrailMovementData.AddNewData(customTrailTopPos, customTrailBottomPos, TimeHelper.time);
            }
        }
    }
}
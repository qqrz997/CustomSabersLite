using CustomSabersLite.Configuration;
using System.Collections;
using System.Linq;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Utilities
{
    internal class DefaultSaberSetter : MonoBehaviour
    {
        private CSLConfig config;
        private SaberManager saberManager;
        private TrailUtils trailUtils;

        [Inject]
        public void Construct(CSLConfig config, SaberManager saberManager, TrailUtils trailUtils)
        {
            this.config = config;
            this.saberManager = saberManager;
            this.trailUtils = trailUtils;
        }

        private void Start()
        {
            StartCoroutine(WaitForSabers());
        }

        private IEnumerator WaitForSabers()
        {
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<Saber>().Any());

            SaberTrail leftTrail = saberManager.leftSaber.gameObject.GetComponentInChildren<SaberTrail>();
            SaberTrail rightTrail = saberManager.rightSaber.gameObject.GetComponentInChildren<SaberTrail>();

            SetupTrail(leftTrail);
            SetupTrail(rightTrail);
        }

        private void SetupTrail(SaberTrail trail)
        {
            trailUtils.SetTrailDuration(trail);
            trailUtils.SetWhiteTrailDuration(trail);
        }
    }
}

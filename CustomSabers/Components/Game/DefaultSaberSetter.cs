using CustomSabersLite.Configuration;
using CustomSabersLite.Utilities;
using System.Collections;
using System.Linq;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Components.Game;

internal class DefaultSaberSetter(CSLConfig config, SaberManager saberManager, GameplayCoreSceneSetupData gameplayCoreData, ICoroutineStarter coroutineStarter) : IInitializable
{
    private readonly CSLConfig config = config;
    private readonly SaberManager saberManager = saberManager;
    private readonly GameplayCoreSceneSetupData gameplayCoreData = gameplayCoreData;
    private readonly ICoroutineStarter coroutineStarter = coroutineStarter;

    public void Initialize() => coroutineStarter.StartCoroutine(WaitForSaberModelController());

    private IEnumerator WaitForSaberModelController()
    {
        yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<SaberModelController>().Any());
        SetupSabers();
    }

    private void SetupSabers()
    {
        SetupSaber(saberManager.leftSaber);
        SetupSaber(saberManager.rightSaber);
    }

    private void SetupSaber(Saber saber)
    {
        var saberModelController = saber.GetComponentInChildren<SaberModelController>();
        var trail = saberModelController?.gameObject.GetComponent<SaberTrail>() ?? saber.GetComponentInChildren<SaberTrail>();
        if (!saberModelController || !trail)
        {
            return;
        }
    }
}

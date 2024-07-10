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

        if (config.EnableCustomColorScheme)
        {
            var color = saber.saberType == SaberType.SaberA ? config.LeftSaberColor : config.RightSaberColor;

            SetCustomSchemeColor(color, saberModelController);
            trail._color = color.ColorWithAlpha(gameplayCoreData.playerSpecificSettings.saberTrailIntensity);
        }
    }

    private static void SetCustomSchemeColor(Color color, SaberModelController saberModelController)
    {
        foreach (var setSaberGlowColor in saberModelController._setSaberGlowColors)
        {
            var materialPropertyBlock = setSaberGlowColor._materialPropertyBlock ?? new MaterialPropertyBlock();

            foreach (var propertyTintColorPair in setSaberGlowColor._propertyTintColorPairs)
            {
                materialPropertyBlock.SetColor(propertyTintColorPair.property, color * propertyTintColorPair.tintColor);
            }

            setSaberGlowColor._meshRenderer.SetPropertyBlock(materialPropertyBlock);
        }
    }
}

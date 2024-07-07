using CustomSabersLite.Components.Game;
using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
using CustomSabersLite.UI.Views.Saber_List;
using CustomSabersLite.Utilities;
using CustomSabersLite.Utilities.Extensions;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.UI.Managers;

internal class MenuSaberManager
{
    [Inject] private readonly InternalResourcesProvider internalResourcesProvider;
    [Inject] private readonly CSLConfig config;
    [Inject] private readonly DiContainer container;

    private MenuSaber leftSaber;
    private MenuSaber rightSaber;

    public void Init(Transform leftParent, Transform rightParent)
    {
        leftSaber = new(config, leftParent, internalResourcesProvider.SaberTrailRenderer);
        rightSaber = new(config, rightParent, internalResourcesProvider.SaberTrailRenderer);
    }

    public void ReplaceSabers(LiteSaber newLeftSaber, LiteSaber newRightSaber)
    {
        leftSaber.ReplaceSaber(newLeftSaber);
        rightSaber.ReplaceSaber(newRightSaber);
    }

    public void SetColor(Color left, Color right)
    {

    }

    public void SetActive(bool active)
    {

    }
}

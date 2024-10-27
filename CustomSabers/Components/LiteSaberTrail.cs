using CustomSabersLite.Models;
using CustomSabersLite.Utilities;
using UnityEngine;

namespace CustomSabersLite.Components;

internal class LiteSaberTrail : SaberTrail
{
    private readonly SaberMovementData customTrailMovementData = new();

    public int OverrideWidth { private get; set; } = 100;

    public bool UseWidthOverride { private get; set; }

    public CustomTrailData? InstanceTrailData { get; private set; }

    new void Awake() => _movementData = customTrailMovementData;

    public void Init(CustomTrailData trailData)
    {
        InstanceTrailData = trailData;
        gameObject.layer = 12;

        SetColor(trailData.Color);
    }

    void Update()
    {
        if (gameObject.activeInHierarchy && InstanceTrailData != null)
        {
            var topPosition = InstanceTrailData.TopPosition;
            var bottomPosition = !UseWidthOverride ? InstanceTrailData.BottomPosition
                : InstanceTrailData.GetOverrideWidthBottom(OverrideWidth);
            customTrailMovementData.AddNewData(topPosition, bottomPosition, TimeHelper.time);
        }
    }

    public void SetColor(Color color)
    {
        if (InstanceTrailData != null && InstanceTrailData.ColorType != CustomSaber.ColorType.CustomColor)
        {
            color *= InstanceTrailData.ColorMultiplier;
            _trailRenderer._meshRenderer.materials.ForEach(m => m.SetColor(MaterialProperties.Color, color));
            _color = color;
        }
    }
}
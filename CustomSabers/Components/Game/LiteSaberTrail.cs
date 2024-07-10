using CustomSabersLite.Data;
using CustomSabersLite.Utilities.Extensions;
using UnityEngine;

namespace CustomSabersLite.Components.Game;

internal class LiteSaberTrail : SaberTrail
{
    private readonly SaberMovementData customTrailMovementData = new();

    public int OverrideWidth { private get; set; } = 100;

    public bool UseWidthOverride { private get; set; }

    public CustomTrailData InstanceTrailData { get; private set; }

    void Awake() => _movementData = customTrailMovementData;

    public void Init(CustomTrailData trailData)
    {
        InstanceTrailData = trailData;
        gameObject.layer = 12;

        SetColorImpl(trailData.Color);
    }

    void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            var topPosition = InstanceTrailData.TopPosition;
            var bottomPosition = !UseWidthOverride ? InstanceTrailData.BottomPosition
                : InstanceTrailData.GetOverrideWidthBottom(OverrideWidth);
            customTrailMovementData.AddNewData(topPosition, bottomPosition, TimeHelper.time);
        }
    }

    public void SetColor(Color color)
    {
        if (InstanceTrailData.ColorType != CustomSaber.ColorType.CustomColor) SetColorImpl(color);
    }

    private void SetColorImpl(Color color)
    {
        color *= InstanceTrailData.ColorMultiplier;
        foreach (var material in _trailRenderer._meshRenderer.materials)
            material.SetColor(MaterialProperties.Color, color);
        _color = color;
    }
}
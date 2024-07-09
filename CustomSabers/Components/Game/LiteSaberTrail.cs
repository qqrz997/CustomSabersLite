using CustomSabersLite.Data;
using UnityEngine;

namespace CustomSabersLite.Components.Game;

internal class LiteSaberTrail : SaberTrail
{
    private CustomSaber.ColorType trailColorType;
    private Color colorMultiplier;
    private Transform customTrailTopTransform;
    private Transform customTrailBottomTransform;

    private readonly SaberMovementData customTrailMovementData = new();

    void Awake() => _movementData = customTrailMovementData;

    public void Init(CustomTrailData trailData)
    {
        trailColorType = trailData.ColorType;
        colorMultiplier = trailData.ColorMultiplier;
        customTrailTopTransform = trailData.Top;
        customTrailBottomTransform = trailData.Bottom;

        customTrailTopTransform.name = "Custom Top";
        customTrailBottomTransform.name = "Custom Bottom";
        gameObject.layer = 12;

        SetColorImpl(trailData.Color);
    }

    void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            customTrailMovementData.AddNewData(customTrailTopTransform.position, customTrailBottomTransform.position, TimeHelper.time);
        }
    }

    public void SetColor(Color color)
    {
        if (trailColorType != CustomSaber.ColorType.CustomColor) SetColorImpl(color);
    }

    private void SetColorImpl(Color color)
    {
        color *= colorMultiplier;
        foreach (var material in _trailRenderer._meshRenderer.materials)
            material.SetColor(MaterialProperties.Color, color);
        _color = color;
    }
}
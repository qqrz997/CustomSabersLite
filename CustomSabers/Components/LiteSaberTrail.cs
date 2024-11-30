using CustomSabersLite.Models;
using CustomSabersLite.Utilities;
using UnityEngine;

namespace CustomSabersLite.Components;

internal class LiteSaberTrail : SaberTrail
{
    private readonly SaberMovementData customTrailMovementData = new();

    public float OverrideWidth { private get; set; } = 1f;
    public bool UseWidthOverride { private get; set; }

    public CustomTrailData? InstanceTrailData { get; private set; }

    public void Init(CustomTrailData trailData)
    {
        InstanceTrailData = trailData;
        gameObject.layer = 12;
    }
    
    public void SetColor(Color color)
    {
        if (InstanceTrailData == null)
        {
            return;
        }
        
        _color = InstanceTrailData.GetTrailColor(color);
        
        foreach (var trailMaterial in _trailRenderer._meshRenderer.materials)
        {
            trailMaterial.SetColor(MaterialProperties.Color, _color);
        }
    }
    
    private new void Awake()
    {
        _movementData = customTrailMovementData;
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy || InstanceTrailData == null)
        {
            return;
        }

        customTrailMovementData.AddNewData(
            InstanceTrailData.TopPosition, 
            UseWidthOverride ? InstanceTrailData.GetOverrideWidthBottom(OverrideWidth) : InstanceTrailData.BottomPosition,
            TimeHelper.time);
    }
}
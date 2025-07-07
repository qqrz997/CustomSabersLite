using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Common;
using CustomSabersLite.Utilities.Extensions;
using UnityEngine;

namespace CustomSabersLite.Components;

internal class LiteSaberTrail : SaberTrail
{
    private readonly SaberMovementData customTrailMovementData = new();

    private Transform trailTop = null!;
    private Transform trailBottom = null!;
    private ITrailData trailData = null!;
    private bool didInit;
    
    public void Init(
        Transform trailTop,
        Transform trailBottom,
        ITrailData trailData)
    {
        this.trailTop = trailTop;
        this.trailBottom = trailBottom;
        this.trailData = trailData;
        _trailDuration = trailData.LengthSeconds;
        
        gameObject.layer = 12;
        didInit = true;
    }

    public float OverrideWidth { private get; set; } = 1f;
    public bool UseWidthOverride { private get; set; }
    
    public ITrailData TrailData => trailData;
    
    public void SetColor(Color color)
    {
        _color = (trailData.UseCustomColor ? trailData.CustomColor : color) * trailData.ColorMultiplier;
        
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
        if (!gameObject.activeInHierarchy || !didInit) return;

        var topPos = trailTop.position;
        var bottomPos = trailBottom.position;
        
        customTrailMovementData.AddNewData(
            topPos,
            UseWidthOverride ? GetOverrideWidthBottom(OverrideWidth) : bottomPos,
            TimeHelper.time);

        return;
        
        Vector3 GetOverrideWidthBottom(float trailWidth)
        {
            float distance = Vector3.Distance(topPos, bottomPos);
            return distance.Approximately(0) ? bottomPos 
                : Vector3.LerpUnclamped(topPos, bottomPos, trailWidth / distance);
        }
    }
}
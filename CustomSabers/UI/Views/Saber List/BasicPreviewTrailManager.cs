using CustomSabersLite.Components.Game;
using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.UI.Managers;

internal class BasicPreviewTrailManager
{
    [Inject] private readonly CSLConfig config;

    private readonly BasicPreviewTrail leftTrail = new("Basic Preview Trail");
    private readonly BasicPreviewTrail rightTrail = new("Basic Preview Trail");

    public void Init(Transform leftParent, Transform rightParent)
    {
        leftTrail.Init(leftParent);
        rightTrail.Init(rightParent);
    }

    public void SetTrails(LiteSaber leftSaber, LiteSaber rightSaber)
    {
        var leftTrailData = leftSaber.InstanceTrails.Length > 0 ? leftSaber.InstanceTrails[0] : CustomTrailData.Default;
        var rightTrailData = rightSaber.InstanceTrails.Length > 0 ? rightSaber.InstanceTrails[0] : CustomTrailData.Default;
        leftTrail.ReplaceTrail(leftTrailData);
        rightTrail.ReplaceTrail(rightTrailData);
    }

    public void UpdateTrails()
    {
        leftTrail.UpdateMesh(config);
        rightTrail.UpdateMesh(config);
    }

    public void SetColor(Color left, Color right)
    {
        leftTrail.UpdateColor(left);
        rightTrail.UpdateColor(right);
    }
}

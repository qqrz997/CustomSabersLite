using CustomSabersLite.Configuration;
using CustomSabersLite.Models;
using System.Linq;
using UnityEngine;

namespace CustomSabersLite.UI;

internal class BasicPreviewTrailManager(CSLConfig config)
{
    private readonly CSLConfig config = config;

    private readonly BasicPreviewTrail leftTrail = new("Basic Preview Trail");
    private readonly BasicPreviewTrail rightTrail = new("Basic Preview Trail");

    public void Init(Transform leftParent, Transform rightParent)
    {
        leftTrail.Init(leftParent);
        rightTrail.Init(rightParent);
    }

    public void SetTrails(ILiteSaber? leftSaber, ILiteSaber? rightSaber)
    {
        leftTrail.ReplaceTrail(GetPrimaryTrailData(leftSaber));
        rightTrail.ReplaceTrail(GetPrimaryTrailData(rightSaber));
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

    private CustomTrailData GetPrimaryTrailData(ILiteSaber? saber) =>
        saber == null ? CustomTrailData.Default
        : saber.TrailData.Length == 0 ? CustomTrailData.Default
        : saber.TrailData.First();
}

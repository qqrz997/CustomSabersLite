using CustomSabersLite.Configuration;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities;
using System.Linq;
using UnityEngine;

namespace CustomSabersLite.UI;

internal class BasicPreviewTrailManager(CSLConfig config, GameResourcesProvider gameResourcesProvider)
{
    private readonly BasicPreviewTrail leftTrail = new(config, gameResourcesProvider, SaberType.SaberA);
    private readonly BasicPreviewTrail rightTrail = new(config, gameResourcesProvider, SaberType.SaberB);

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
        leftTrail.UpdateMesh();
        rightTrail.UpdateMesh();
    }

    public void SetColor(Color left, Color right)
    {
        leftTrail.UpdateColor(left);
        rightTrail.UpdateColor(right);
    }

    private CustomTrailData? GetPrimaryTrailData(ILiteSaber? saber) =>
        saber?.TrailData.FirstOrDefault() ?? null;
}

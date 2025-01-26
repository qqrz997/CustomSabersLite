using System.Linq;
using CustomSabersLite.Configuration;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Services;
using UnityEngine;

namespace CustomSabersLite.Menu;

internal class BasicPreviewTrailManager(CslConfig config, GameResourcesProvider gameResourcesProvider)
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
        leftTrail.ReplaceTrail(leftSaber?.TrailData.FirstOrDefault());
        rightTrail.ReplaceTrail(rightSaber?.TrailData.FirstOrDefault());
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
}

using System.Linq;
using CustomSabersLite.Models;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Menu;

internal class StaticPreviewTrailManager
{
    private readonly StaticPreviewTrail leftTrail;
    private readonly StaticPreviewTrail rightTrail;

    public StaticPreviewTrailManager(
        [Inject(Id = SaberType.SaberA)] StaticPreviewTrail leftTrail,
        [Inject(Id = SaberType.SaberB)] StaticPreviewTrail rightTrail)
    {
        this.leftTrail = leftTrail;
        this.rightTrail = rightTrail;
    }

    public void Init(Transform leftParent, Transform rightParent)
    {
        leftTrail.Init(leftParent);
        rightTrail.Init(rightParent);
    }

    public void SetTrails(SaberInstanceSet saberInstanceSet)
    {
        leftTrail.ReplaceTrail(saberInstanceSet.LeftTrails.FirstOrDefault());
        rightTrail.ReplaceTrail(saberInstanceSet.RightTrails.FirstOrDefault());
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

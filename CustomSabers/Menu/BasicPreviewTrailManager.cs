using System.Linq;
using CustomSabersLite.Models;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Menu;

internal class BasicPreviewTrailManager
{
    private readonly BasicPreviewTrail leftTrail;
    private readonly BasicPreviewTrail rightTrail;

    public BasicPreviewTrailManager(
        [Inject(Id = SaberType.SaberA)] BasicPreviewTrail leftTrail,
        [Inject(Id = SaberType.SaberB)] BasicPreviewTrail rightTrail)
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

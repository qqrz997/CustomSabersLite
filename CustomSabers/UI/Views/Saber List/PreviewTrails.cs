using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
using UnityEngine;

namespace CustomSabersLite.UI.Managers;

internal class PreviewTrails
{
    private readonly GameObject leftTrail;
    private readonly GameObject rightTrail;

    private readonly MeshRenderer leftMeshRenderer;
    private readonly MeshRenderer rightMeshRenderer;

    private readonly MeshFilter leftMesh;
    private readonly MeshFilter rightMesh;

    private Color leftColor = Color.white;
    private Color rightColor = Color.white;

    private readonly Vector2[] uvs = [
            new(1, 0),
            new(0, 0),
            new(0, 1),
            new(1, 1) ];

    private readonly int[] tris = [
            2, 1, 0,
            0, 3, 2 ];

    private CustomTrailData? currentLeftTrail;
    private CustomTrailData? currentRightTrail;

    public PreviewTrails()
    {
        leftTrail = new("Preview Trail Left", typeof(MeshRenderer), typeof(MeshFilter));
        rightTrail = new("Preview Trail Right", typeof(MeshRenderer), typeof(MeshFilter));
        leftMeshRenderer = leftTrail.GetComponent<MeshRenderer>();
        rightMeshRenderer = rightTrail.GetComponent<MeshRenderer>();
        leftMesh = leftTrail.GetComponent<MeshFilter>();
        rightMesh = rightTrail.GetComponent<MeshFilter>();
        leftMesh.mesh = CreateTrailMesh();
        rightMesh.mesh = CreateTrailMesh();
        SetActive(false);
    }

    public void SetPosition(Vector3 leftPosition, Vector3 rightPosition, Quaternion leftRotation, Quaternion rightRotation)
    {
        leftTrail.transform.SetPositionAndRotation(leftPosition, leftRotation);
        rightTrail.transform.SetPositionAndRotation(rightPosition, rightRotation);
    }

    public void SetTrails(CustomTrailData? leftTrail, CustomTrailData? rightTrail)
    {
        currentLeftTrail = leftTrail;
        currentRightTrail = rightTrail;
    }

    public void UpdateTrails(CSLConfig config)
    {
        var maybeTrail = currentLeftTrail;
        if (maybeTrail == null)
        {
            Clear();
            return;
        }
        var trail = maybeTrail.Value;

        SwapMaterial(trail);

        var duration = !config.OverrideTrailDuration ? trail.Length
            : 0.4f * config.TrailDuration / 100f;

        Vector3 bottom;
        if (config.OverrideTrailWidth)
        {
            var trailTop = trail.Top.localPosition;
            var trailBottom = trail.Bottom.localPosition;
            var distance = Vector3.Distance(trailTop, trailBottom);
            var width = distance > 0 ? config.TrailWidth / 100f / distance : 1f;
            bottom = Vector3.LerpUnclamped(trailTop, trailBottom, width);
        }
        else
        {
            bottom = trail.Bottom.localPosition;
        }

        UpdateVertices(bottom, trail.Top.localPosition, duration * 1.4f);
    }

    private void UpdateVertices(Vector3 bottom, Vector3 top, float length)
    {
        leftMesh.mesh.vertices = [
            bottom,
            top,
            new(top.x, top.y + length, top.z),
            new(bottom.x, bottom.y + length, bottom.z),
        ];
        leftMesh.mesh.RecalculateBounds();

        rightMesh.mesh.vertices = [
            bottom,
            top,
            new(top.x, top.y + length, top.z),
            new(bottom.x, bottom.y + length, bottom.z),
        ];
        rightMesh.mesh.RecalculateBounds();

        UpdateColor(leftColor, rightColor);
    }

    private void Clear()
    {
        leftMesh.mesh.vertices = new Vector3[uvs.Length];
        rightMesh.mesh.vertices = new Vector3[uvs.Length];
    }

    private void SwapMaterial(CustomTrailData trailData)
    {
        if (trailData.Material)
        {
            leftMeshRenderer.material = new(trailData.Material);
            rightMeshRenderer.material = new(trailData.Material);
            currentLeftTrail = trailData;
        }
    }

    public void UpdateColor(Color left, Color right)
    {
        if (currentLeftTrail == null)
        {
            return;
        }
        var trail = currentLeftTrail.Value;
        if (trail.ColorType == CustomSaber.ColorType.CustomColor) return;

        leftColor = left;
        rightColor = right;

        foreach (var rendererMaterial in leftMeshRenderer.materials)
            rendererMaterial.SetColor(MaterialProperties.Color, left);
        foreach (var rendererMaterial in rightMeshRenderer.materials)
            rendererMaterial.SetColor(MaterialProperties.Color, right);

        leftMesh.mesh.colors = [ left, left, left, left ];
        rightMesh.mesh.colors = [ right, right, right, right ];
    }

    public void SetActive(bool active)
    {
        leftTrail.SetActive(active);
        rightTrail.SetActive(active);
    }

    private Mesh CreateTrailMesh() => new()
    {
        vertices = new Vector3[uvs.Length],
        uv = uvs,
        triangles = tris
    };
}

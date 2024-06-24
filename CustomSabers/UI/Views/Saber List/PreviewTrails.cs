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

    public void SetActive(bool active)
    {
        leftTrail.SetActive(active);
        rightTrail.SetActive(active);
    }

    public void UpdateTrails(CSLConfig config)
    {
        if (currentLeftTrail != null)
        {
            (var bottom, var top, var length) = CalculateTrailDimensions(currentLeftTrail.Value, config);
            leftMesh.mesh.vertices = CreateUpdatedVertices(bottom, top, length);
            leftMesh.mesh.RecalculateBounds();

            if (currentLeftTrail.Value.Material) leftMeshRenderer.material = currentLeftTrail.Value.Material;
        }
        else
        {
            leftMesh.mesh.vertices = new Vector3[uvs.Length];
        }

        if (currentRightTrail != null)
        {
            (var bottom, var top, var length) = CalculateTrailDimensions(currentRightTrail.Value, config);
            rightMesh.mesh.vertices = CreateUpdatedVertices(bottom, top, length);
            rightMesh.mesh.RecalculateBounds();

            if (currentRightTrail.Value.Material) rightMeshRenderer.material = currentRightTrail.Value.Material;
        }
        else
        {
            rightMesh.mesh.vertices = new Vector3[uvs.Length];
        }

        UpdateColor(leftColor, rightColor);
    }

    public void UpdateColor(Color left, Color right)
    {
        // todo - https://discord.com/channels/441805394323439646/443146108420620318/1254622303984291932
        if (currentLeftTrail != null)
        {
            if (currentLeftTrail.Value.ColorType == CustomSaber.ColorType.CustomColor)
            {
                leftColor = currentLeftTrail.Value.Color;
                leftMesh.mesh.colors = [leftColor, leftColor, leftColor, leftColor];
            }
            else
            {
                leftColor = left;
                foreach (var rendererMaterial in leftMeshRenderer.materials)
                    rendererMaterial.SetColor(MaterialProperties.Color, leftColor);
                leftMesh.mesh.colors = [leftColor, leftColor, leftColor, leftColor];
            }
        }

        if (currentRightTrail != null)
        {
            if (currentRightTrail.Value.ColorType == CustomSaber.ColorType.CustomColor)
            {
                rightColor = currentRightTrail.Value.Color;
                rightMesh.mesh.colors = [rightColor, rightColor, rightColor, rightColor];
            }
            else
            {
                rightColor = right;
                foreach (var rendererMaterial in rightMeshRenderer.materials)
                    rendererMaterial.SetColor(MaterialProperties.Color, rightColor);
                rightMesh.mesh.colors = [rightColor, rightColor, rightColor, rightColor];
            }
        }
    }

    private (Vector3 bottom, Vector3 top, float Length) CalculateTrailDimensions(CustomTrailData trailData, CSLConfig config)
    {
        var duration = !config.OverrideTrailDuration ? trailData.Length
            : 0.4f * config.TrailDuration / 100f;

        Vector3 bottom;
        if (config.OverrideTrailWidth)
        {
            var trailTop = trailData.Top.localPosition;
            var trailBottom = trailData.Bottom.localPosition;
            var distance = Vector3.Distance(trailTop, trailBottom);
            var width = distance > 0 ? config.TrailWidth / 100f / distance : 1f;
            bottom = Vector3.LerpUnclamped(trailTop, trailBottom, width);
        }
        else
        {
            bottom = trailData.Bottom.localPosition;
        }

        return (bottom, trailData.Top.localPosition, duration * 1.4f);
    }

    private Vector3[] CreateUpdatedVertices(Vector3 bottom, Vector3 top, float length) =>
    [
        bottom,
        top,
        new(top.x, top.y + length, top.z),
        new(bottom.x, bottom.y + length, bottom.z),
    ];

    private Mesh CreateTrailMesh() => new()
    {
        vertices = new Vector3[uvs.Length],
        uv = uvs,
        triangles = tris
    };
}

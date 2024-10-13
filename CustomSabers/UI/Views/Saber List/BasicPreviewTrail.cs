using CustomSabersLite.Configuration;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities;
using UnityEngine;

namespace CustomSabersLite.UI.Managers;

internal class BasicPreviewTrail
{
    private readonly GameObject gameObject;
    private readonly MeshRenderer meshRenderer;
    private readonly MeshFilter meshFilter;

    private readonly Mesh mesh;

    public BasicPreviewTrail(string name)
    {
        gameObject = new(name);
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        mesh = new Mesh();
    }

    private CustomTrailData currentTrailData = CustomTrailData.Default;

    private Vector3[] vertices = [];
    private int[] triangles = [];
    private Vector2[] uvs = [];
    private Color[] colors = [];

    public void Init(Transform parent)
    {
        gameObject.transform.SetParent(parent, false);
        meshFilter.mesh = mesh;
        mesh.MarkDynamic();
        vertices = new Vector3[4];
        triangles = [0, 3, 1, /**/ 0, 2, 3];
        uvs = [new(1, 0), new(0, 0), new(1, 1), new(0, 1)];
        colors = new Color[4];
    }

    public void ReplaceTrail(CustomTrailData trailData)
    {
        meshRenderer.material = trailData.Material;
        currentTrailData = trailData;
    }

    public void UpdateMesh(CSLConfig config)
    {
        var bottom = config.OverrideTrailWidth ? currentTrailData.GetOverrideWidthBottom(config.TrailWidth, true)
            : currentTrailData.BottomLocalPosition;
        var top = currentTrailData.TopLocalPosition;
        var length = config.OverrideTrailDuration ? config.TrailDuration / 250f
            : Mathf.Clamp(currentTrailData.Length, 0f, 0.4f);

        var bottomEnd = bottom with { y = bottom.y + length };
        var topEnd = top with { y = top.y + length };

        vertices[0] = bottom;
        vertices[1] = top;
        vertices[2] = bottomEnd;
        vertices[3] = topEnd;

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.colors = colors;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
    }

    public void UpdateColor(Color color)
    {
        var trailColor = currentTrailData.ColorType == CustomSaber.ColorType.CustomColor
            ? currentTrailData.Color * currentTrailData.ColorMultiplier
            : color * currentTrailData.ColorMultiplier;
        meshRenderer.materials.ForEach(m => m.SetColor(MaterialProperties.Color, trailColor));
        colors[0] = trailColor;
        colors[1] = trailColor;
        colors[2] = trailColor;
        colors[3] = trailColor;
        mesh.colors = colors;
    }
}

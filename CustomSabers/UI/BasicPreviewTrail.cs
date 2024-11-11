using CustomSabersLite.Configuration;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities;
using UnityEngine;
using TrailColorType = CustomSaber.ColorType;

namespace CustomSabersLite.UI;

internal class BasicPreviewTrail
{
    private readonly CSLConfig config;

    private readonly GameObject gameObject;
    private readonly MeshRenderer meshRenderer;
    private readonly MeshFilter meshFilter;

    private readonly CustomTrailData defaultTrailData;
    private readonly Material defaultTrailMaterial;
    private readonly Mesh mesh;

    private CustomTrailData customTrailData;

    public BasicPreviewTrail(CSLConfig config, GameResourcesProvider gameResourcesProvider, SaberType saberType)
    {
        this.config = config;
        gameObject = new GameObject("BasicPreviewTrail");
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        mesh = new Mesh();

        var defaultTop = new GameObject("DefaultTop").transform;
        var defaultBottom = new GameObject("DefaultBottom").transform;
        defaultTop.transform.SetParent(gameObject.transform, false);
        defaultBottom.transform.SetParent(gameObject.transform, false);
        defaultTop.localPosition = Vector3.forward;

        defaultTrailMaterial = gameResourcesProvider.SaberTrailRenderer._meshRenderer.material;
        
        customTrailData = defaultTrailData = new CustomTrailData(
            defaultTop, defaultBottom, defaultTrailMaterial,
            saberType == SaberType.SaberA ? TrailColorType.LeftSaber : TrailColorType.RightSaber, 
            Color.white, Color.white, TrailUtils.DefaultDuration);
    }

    private Vector3[] vertices = [];
    private int[] triangles = [];
    private Vector2[] uvs = [];
    private Color[] colors = [];

    private CustomTrailData? CurrentTrailData => config.TrailType switch
    {
        TrailType.Custom => customTrailData,
        TrailType.Vanilla => defaultTrailData,
        _ => null
    };

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

    public void ReplaceTrail(CustomTrailData? trailData)
    {
        customTrailData = trailData ?? defaultTrailData;
        meshRenderer.material = trailData?.Material ?? defaultTrailData.Material;
    }

    public void UpdateMesh()
    {
        if (CurrentTrailData is null)
        {
            gameObject.SetActive(false);
            return;
        }

        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }

        meshRenderer.material = CurrentTrailData.Material;

        var top = CurrentTrailData.TopLocalPosition;
        var bottom = config.OverrideTrailWidth ? CurrentTrailData.GetOverrideWidthBottom(config.TrailWidth, true) : CurrentTrailData.BottomLocalPosition;
        float length = config.OverrideTrailDuration ? (config.TrailDuration / 250f) : Mathf.Clamp(CurrentTrailData.Length, 0f, 0.4f);

        vertices[0] = bottom;
        vertices[1] = top;
        vertices[2] = bottom with { y = bottom.y + length };
        vertices[3] = top with { y = top.y + length };

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
    }

    public void UpdateColor(Color color)
    {
        if (CurrentTrailData is null)
        {
            return;
        }
        var trailColor = CurrentTrailData.ColorType == TrailColorType.CustomColor
            ? CurrentTrailData.Color * CurrentTrailData.ColorMultiplier
            : color * CurrentTrailData.ColorMultiplier;
        meshRenderer.materials.ForEach(m => m.SetColor(MaterialProperties.Color, trailColor));
        colors[0] = trailColor;
        colors[1] = trailColor;
        colors[2] = trailColor;
        colors[3] = trailColor;
        mesh.colors = colors;
    }
}

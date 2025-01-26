using CustomSabersLite.Configuration;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Common;
using CustomSabersLite.Utilities.Extensions;
using CustomSabersLite.Utilities.Services;
using UnityEngine;
using TrailColorType = CustomSaber.ColorType;

namespace CustomSabersLite.Menu;

internal class BasicPreviewTrail
{
    private readonly CslConfig config;

    private readonly GameObject gameObject;
    private readonly GameObject defaultTrailObject;
    private readonly MeshRenderer meshRenderer;
    private readonly MeshFilter meshFilter;

    private readonly CustomTrailData defaultTrailData;
    private readonly Mesh mesh;

    private CustomTrailData customTrailData;

    private Vector3[] vertices = [];
    private int[] triangles = [];
    private Vector2[] uvs = [];
    private Color[] colors = [];
    
    private Vector3 SaberPosition => gameObject.transform.position;
    
    public BasicPreviewTrail(CslConfig config, GameResourcesProvider gameResourcesProvider, SaberType saberType)
    {
        this.config = config;
        
        gameObject = new("BasicPreviewTrail");
        defaultTrailObject = new("DefaultTrail");
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshFilter = gameObject.AddComponent<MeshFilter>();
        mesh = new();

        defaultTrailObject.transform.SetParent(gameObject.transform, false);
        var defaultTop = new GameObject("Top").transform;
        var defaultBottom = new GameObject("Bottom").transform;
        defaultTop.transform.SetParent(defaultTrailObject.transform, false);
        defaultBottom.transform.SetParent(defaultTrailObject.transform, false);
        defaultTop.localPosition = Vector3.forward;

        customTrailData = defaultTrailData = new(
            defaultTop, defaultBottom, gameResourcesProvider.DefaultTrailMaterial,
            saberType == SaberType.SaberA ? TrailColorType.LeftSaber : TrailColorType.RightSaber, 
            Color.white, Color.white, TrailUtils.DefaultDuration);
    }

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
        var currentTrailData = GetCurrentTrailData();
        
        if (currentTrailData is null)
        {
            gameObject.SetActive(false);
            return;
        }

        if (!gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }

        meshRenderer.material = currentTrailData.Material;

        var top = currentTrailData.TopLocalPosition;
        var bot = currentTrailData.BottomLocalPosition;
        
        // Update default transforms scale to match the saber length
        defaultTrailObject.transform.localScale = config.OverrideSaberLength ? new(1f, 1f, config.SaberLength) : Vector3.one;

        float topDistance = Vector3.Distance(SaberPosition, currentTrailData.TopPosition);
        float bottomDistance = Vector3.Distance(SaberPosition, currentTrailData.BottomPosition);
        
        if (config.OverrideSaberLength)
        {
            top.z = topDistance;
            bot.z = bottomDistance;
        }
        
        if (config.OverrideTrailWidth)
        {
            float distance = Vector3.Distance(top, bot);
            float width = distance > 0 ? config.TrailWidth / distance : 1f;
            bot = Vector3.LerpUnclamped(top, bot, width);
        }

        float length = config.OverrideTrailDuration ? config.TrailDuration * 0.4f : currentTrailData.Length.Clamp(0f, 0.4f);
        
        vertices[0] = bot;
        vertices[1] = top;
        vertices[2] = bot with { y = bot.y + length };
        vertices[3] = top with { y = top.y + length };
        
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
    }

    public void UpdateColor(Color color)
    {
        var currentTrailData = GetCurrentTrailData();
        if (currentTrailData is null)
        {
            return;
        }
        var trailColor = currentTrailData.GetTrailColor(color);
        foreach (var material in meshRenderer.materials)
        {
            material.SetColor(MaterialProperties.Color, trailColor);
        }
        for (int i = 0; i < colors.Length; i++) colors[i] = trailColor;
        mesh.colors = colors;
    }

    private CustomTrailData? GetCurrentTrailData() => config.TrailType switch
    {
        TrailType.Custom => customTrailData,
        TrailType.Vanilla => defaultTrailData,
        _ => null
    };
}

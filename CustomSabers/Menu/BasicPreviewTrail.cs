using CustomSabersLite.Configuration;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Common;
using CustomSabersLite.Utilities.Extensions;
using UnityEngine;

namespace CustomSabersLite.Menu;

internal class BasicPreviewTrail
{
    private readonly PluginConfig config;

    private readonly GameObject gameObject;
    private readonly MeshRenderer meshRenderer;
    private readonly Mesh mesh = new();
    private readonly Vector3[] vertices = new Vector3[4];
    private readonly int[] triangles = [0, 3, 1, /**/ 0, 2, 3];
    private readonly Vector2[] uvs = [new(1, 0), new(0, 0), new(1, 1), new(0, 1)];
    private readonly Color[] colors = new Color[4];

    public BasicPreviewTrail(PluginConfig config)
    {
        this.config = config;
        
        gameObject = new("BasicPreviewTrail");
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        gameObject.AddComponent<MeshFilter>().mesh = mesh;
        mesh.MarkDynamic();
    }

    private ITrailData? trailData;
    private Color color;

    public void Init(Transform parent)
    {
        gameObject.transform.SetParent(parent, false);
    }

    public void ReplaceTrail(ITrailData? trailData)
    {
        this.trailData = trailData;

        if (trailData is null)
        {
            meshRenderer.enabled = false;
            return;
        }
        
        meshRenderer.enabled = true;
        meshRenderer.material = trailData.Material;
    }
    
    public void UpdateMesh()
    {
        if (trailData is null)
        {
            return;
        }

        var bot = trailData.TrailBottomOffset;
        var top = trailData.TrailTopOffset;

        if (config.OverrideSaberLength)
        {
            bot.z *= config.SaberLength;
            top.z *= config.SaberLength;
        }
        
        if (config.OverrideTrailWidth)
        {
            float distance = Vector3.Distance(top, bot);
            if (distance != 0) bot = Vector3.LerpUnclamped(top, bot, config.TrailWidth / distance);
        }
        
        float length = config.OverrideTrailDuration ? config.TrailDuration * 0.4f 
            : trailData.LengthSeconds.Clamp(0f, 0.4f);

        vertices[0] = bot;
        vertices[1] = top;
        
        bot.y += length;
        top.y += length;
        
        vertices[2] = bot;
        vertices[3] = top;
        
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        
        UpdateColor(color);
    }

    public void UpdateColor(Color color)
    {
        if (trailData is null)
        {
            return;
        }
        
        this.color = color;
        var trailColor = (trailData.UseCustomColor ? trailData.CustomColor : color) * trailData.ColorMultiplier;
        
        foreach (var material in meshRenderer.materials)
        {
            material.SetColor(MaterialProperties.Color, trailColor);
        }
        
        for (int i = 0; i < colors.Length; i++) colors[i] = trailColor;
        
        mesh.colors = colors;
    }
}

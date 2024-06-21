using CustomSabersLite.Data;
using UnityEngine;

namespace CustomSabersLite.UI.Managers;

internal class PreviewTrails
{
    private readonly GameObject leftTrailMesh;
    private readonly GameObject rightTrailMesh;

    private readonly MeshRenderer leftMeshRenderer;
    private readonly MeshRenderer rightMeshRenderer;

    private readonly MeshFilter leftMesh;
    private readonly MeshFilter rightMesh;

    private CustomTrailData currentTrail;

    public PreviewTrails()
    {
        leftTrailMesh = new("Left Preview Trail", typeof(MeshRenderer), typeof(MeshFilter));
        rightTrailMesh = new("Right Preview Trail", typeof(MeshRenderer), typeof(MeshFilter));
        leftMeshRenderer = leftTrailMesh.GetComponent<MeshRenderer>();
        rightMeshRenderer = rightTrailMesh.GetComponent<MeshRenderer>();
        leftMesh = leftTrailMesh.GetComponent<MeshFilter>();
        rightMesh = rightTrailMesh.GetComponent<MeshFilter>();

        (leftMesh.mesh, rightMesh.mesh) = (CreateTrailMesh(), CreateTrailMesh());
    }

    public void SetScale(Vector3 bottom, Vector3 top, float length = 0.5f)
    {
        leftMesh.mesh.vertices = [
            bottom,
            top,
            new(top.x, top.y + length, top.z),
            new(bottom.x, bottom.y + length, bottom.z),
        ];

        rightMesh.mesh.vertices = [
            bottom,
            top,
            new(top.x, top.y + length, top.z),
            new(bottom.x, bottom.y + length, bottom.z),
        ];
    }

    public void SwapMaterial(CustomTrailData trailData)
    {
        (var material, var texture) = (trailData.Material, trailData.Material.mainTexture);

        if (material)
        {
            leftMeshRenderer.material = new(material);
            rightMeshRenderer.material = new(material);
            currentTrail = trailData;
        }

        if (texture)
        {
            leftMeshRenderer.material.mainTexture = GameObject.Instantiate(texture);
            rightMeshRenderer.material.mainTexture = GameObject.Instantiate(texture);
        }
    }

    private static Mesh CreateTrailMesh()
    {
        var bottom = new Vector3(0, 0, 0);
        var top = new Vector3(0, 1, 0);
        var length = 2f;

        return new()
        {
            vertices = [
                bottom,
                top,
                new(top.x, top.y + length, top.z),
                new(bottom.x, bottom.y + length, bottom.z) ],

            uv = [
                new(1, 0),
                new(0, 0),
                new(0, 1),
                new(1, 1) ],

            triangles = [
                2, 1, 0,
                0, 3, 2 ]
        };
    }

    public void SetPosition(Vector3 leftPosition, Vector3 rightPosition, Quaternion rotation)
    {
        leftTrailMesh.transform.SetPositionAndRotation(leftPosition, rotation);
        rightTrailMesh.transform.SetPositionAndRotation(rightPosition, RightRotation);
    }

    public void SetColor(Color left, Color right)
    {
        if (currentTrail.ColorType == CustomSaber.ColorType.CustomColor) return;

        foreach (var rendererMaterial in leftMeshRenderer.materials)
            rendererMaterial.SetColor(MaterialProperties.Color, left);
        foreach (var rendererMaterial in rightMeshRenderer.materials)
            rendererMaterial.SetColor(MaterialProperties.Color, right);
    }

    public void SetActive(bool active)
    {
        leftTrailMesh.SetActive(active);
        rightTrailMesh.SetActive(active);
    }

    private Quaternion Flip(Quaternion original) =>
        Quaternion.Euler(original.x, original.y + 180f, original.z);

    private readonly Quaternion RightRotation = Quaternion.Euler(270f, 283.25f, 0f);
}

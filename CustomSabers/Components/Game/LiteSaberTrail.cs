using CustomSabersLite.Data;
using UnityEngine;

namespace CustomSabersLite.Components.Game;

internal class LiteSaberTrail : SaberTrail
{
    private Transform customTrailTopTransform;
    private Transform customTrailBottomTransform;

    private readonly SaberMovementData customTrailMovementData = new();

    void Awake() => _movementData = customTrailMovementData;

    public void Setup(Transform topTransform, Transform bottomTransform)
    {
        customTrailTopTransform = topTransform;
        customTrailBottomTransform = bottomTransform;
        customTrailTopTransform.name = "Custom Top";
        customTrailBottomTransform.name = "Custom Bottom";

        gameObject.layer = 12;
    }

    void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            customTrailMovementData.AddNewData(customTrailTopTransform.position, customTrailBottomTransform.position, TimeHelper.time);
        }
    }

    public void SetColor(Color color)
    {
        _color = color;

        foreach (var rendererMaterial in _trailRenderer._meshRenderer.materials)
        {
            rendererMaterial.SetColor(MaterialProperties.Color, color);
        }
    }
}
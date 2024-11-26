using CustomSaber;
using CustomSabersLite.Components;
using CustomSabersLite.Utilities;
using UnityEngine;

namespace CustomSabersLite.Models;

internal class DefaultSaber : ILiteSaber
{
    private readonly DefaultSaberColorer defaultSaberColorer;
    
    private DefaultSaber(GameObject defaultSaberPrefab)
    {
        GameObject = Object.Instantiate(defaultSaberPrefab);
        GameObject.transform.position = Vector3.zero;
        GameObject.GetComponentsInChildren<SetSaberGlowColor>().ForEach(x => x.enabled = false);
        GameObject.GetComponentsInChildren<SetSaberFakeGlowColor>().ForEach(x => x.enabled = false);
        GameObject.GetComponent<SaberTrail>().enabled = false;
        GameObject.SetActive(true);
        
        defaultSaberColorer = GameObject.AddComponent<DefaultSaberColorer>();
    }

    public static DefaultSaber Create(GameObject defaultSaberPrefab) => new(defaultSaberPrefab);

    public GameObject GameObject { get; }
    public EventManager? EventManager => null;
    public CustomTrailData[] TrailData { get; } = [];

    public void SetColor(Color color) => defaultSaberColorer.SetColor(color);
    public void SetParent(Transform parent) => GameObject.transform.SetParent(parent, false);
    
    public void SetLength(float length) =>
        GameObject.transform.localScale = GameObject.transform.localScale with { z = length };

    public void SetWidth(float width) =>
        GameObject.transform.localScale = GameObject.transform.localScale with { x = width, y = width };

    public void Destroy()
    {
        if (GameObject != null)
        {
            Object.Destroy(GameObject);
        }
    }
}

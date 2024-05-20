using CustomSaber;
using CustomSabersLite.Data;
using CustomSabersLite.Utilities.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace CustomSabersLite.Components
{
    internal class CSLSaber : MonoBehaviour
    {
        private readonly List<Material> colorableMaterials = new List<Material>();

        public EventManager EventManager;

        public void Setup(Transform parent, bool worldPositionStays = false)
        {
            transform.SetParent(parent, worldPositionStays);
            transform.position = parent.position;
            transform.rotation = parent.rotation;
        }

        void Awake()
        {
            GetColorableMaterialsFromSaber();
            EventManager = gameObject.TryGetComponentOrDefault<EventManager>();
        }

        void OnDestroy()
        {
            if (EventManager)
            {
                Destroy(EventManager);
            }
        }

        public void SetColor(Color color)
        {
            foreach (Material mat in colorableMaterials)
            {
                mat.SetColor(MaterialProperties.Color, color);
            }
        }

        private void GetColorableMaterialsFromSaber()
        {
            foreach (Renderer renderer in gameObject.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer is null) continue;

                Material[] materials = renderer.sharedMaterials;
                int materialCount = materials.Length;

                for (int i = 0; i < materialCount; i++) 
                {
                    Material material = materials[i];

                    if (material is null || !material.HasProperty(MaterialProperties.Color)) continue;

                    if (material.HasProperty(MaterialProperties.CustomColors))
                    {
                        if (material.GetFloat(MaterialProperties.CustomColors) > 0) AddColorableMaterial(renderer, materials, i);
                    }
                    else if (material.HasProperty(MaterialProperties.Glow))
                    {
                        if (material.GetFloat(MaterialProperties.Glow) > 0) AddColorableMaterial(renderer, materials, i);
                    }
                    else if (material.HasProperty(MaterialProperties.Bloom))
                    {
                        if (material.GetFloat(MaterialProperties.Bloom) > 0) AddColorableMaterial(renderer, materials, i);
                    }
                }
            }
        }

        private void AddColorableMaterial(Renderer renderer, Material[] materials, int index)
        {
            materials[index] = new Material(materials[index]);
            renderer.sharedMaterials = materials;
            colorableMaterials.Add(materials[index]);
        }
    }
}

using CustomSaber;
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
                mat.SetColor("_Color", color);
            }
        }

        private void GetColorableMaterialsFromSaber()
        {
            IEnumerable<Renderer> saberRenderers = gameObject.GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in saberRenderers)
            {
                if (renderer == null) continue;

                foreach (Material material in renderer.materials)
                {
                    if (material == null) continue;

                    if (material.HasProperty("_Color"))
                    {
                        if (material.HasProperty("_CustomColors"))
                        {
                            if (material.GetFloat("_CustomColors") > 0)
                            {
                                colorableMaterials.Add(material);
                            }
                        }
                        else if (material.HasProperty("_Glow")  && material.GetFloat("_Glow") > 0
                              || material.HasProperty("_Bloom") && material.GetFloat("_Bloom") > 0)
                        {
                            colorableMaterials.Add(material);
                        }
                    }
                }
            }
        }
    }
}

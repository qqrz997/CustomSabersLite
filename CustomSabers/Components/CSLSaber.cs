using CustomSaber;
using IPA.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomSabersLite.Components
{
    internal class CSLSaber : MonoBehaviour
    {
        private List<Material> colorableMaterials = new List<Material>();

        public EventManager EventManager;

        public void Setup(Transform parent)
        {
            transform.SetParent(parent);
            transform.position = parent.position;
            transform.rotation = parent.rotation;
            gameObject.SetActive(true);
        }

        public void Awake()
        {
            gameObject.SetActive(false);

            GetColorableMaterialsFromSaber();

            try
            {
                EventManager = gameObject.GetComponent<EventManager>();
            }
            catch
            {
                EventManager = gameObject.AddComponent<EventManager>();
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

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

        private IEnumerable<Renderer> saberRenderers;
        public EventManager EventManager;

        public void Setup(Transform parent)
        {
            transform.SetParent(parent);
            transform.position = parent.position;
            transform.rotation = parent.rotation;
        }

        public void Awake()
        {
            saberRenderers = gameObject.GetComponentsInChildren<Renderer>();
        }

        public void Start()
        {
            try
            {
                EventManager = gameObject.GetComponent<EventManager>();
            }
            catch
            {
                EventManager = null;
            }
        }

        }

        {
            foreach (Renderer renderer in saberRenderers)
            {
                if (renderer == null) continue;

                foreach (Material rendererMaterial in renderer.materials)
                {
                    if (rendererMaterial == null) continue;

                    if (rendererMaterial.HasProperty("_Color"))
                    {
                        if (rendererMaterial.HasProperty("_CustomColors"))
                        {
                            if (rendererMaterial.GetFloat("_CustomColors") > 0)
                            {
                                rendererMaterial.SetColor("_Color", colour);
                            }
                        }
                        else if (rendererMaterial.HasProperty("_Glow") && rendererMaterial.GetFloat("_Glow") > 0
                            || rendererMaterial.HasProperty("_Bloom") && rendererMaterial.GetFloat("_Bloom") > 0)
                        {
                            rendererMaterial.SetColor("_Color", colour);
                        }
                    }
                }
            }
        }
    }
}

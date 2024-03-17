using System.Collections.Generic;
using UnityEngine;

namespace CustomSabersLite.Components
{
    internal class CSLSaber : MonoBehaviour
    {
        public SaberType SaberType { get; private set; }

        public Vector3 customSaberTopPos { get; private set; }

        public Vector3 customSaberBottomPos { get; private set; }

        private Transform customSaberTopTransform;

        private Transform customSaberBottomTransform;

        private IEnumerable<Renderer> saberRenderers;

        public GameObject customSaberObject => gameObject;

        public void Init()
        {
            saberRenderers = customSaberObject.GetComponentsInChildren<Renderer>();
        }

        public void SetColor(Color colour)
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

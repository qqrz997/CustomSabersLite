using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomSabersLite.Utilities
{
    public static class MaterialHelpers
    {
        public static bool TryGetMainTexture(this Material material, out Texture texture)
        {
            return TryGetTexture(material, Shader.PropertyToID("_MainTex"), out texture);
        }

        public static bool TryGetTexture(this Material material, int propId, out Texture tex)
        {
            tex = null;
            if (!material.HasProperty(propId))
            {
                return false;
            }

            tex = material.GetTexture(propId);
            return tex != null;
        }
    }
}

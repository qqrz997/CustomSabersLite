using AssetBundleLoadingTools.Utilities;
using CustomSaber;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomSabersLite.Utilities
{
    internal class ShaderRepairUtils
    {
        public static async Task RepairSaberShadersAsync(GameObject sabersObject)
        {
            List<Material> materials = ShaderRepair.GetMaterialsFromGameObjectRenderers(sabersObject);
            foreach (Material trailMaterial in sabersObject.GetComponentsInChildren<CustomTrail>().Select(t => t.TrailMaterial))
            {
                if (!materials.Contains(trailMaterial))
                {
                    materials.Add(trailMaterial);
                }
            }
            await ShaderRepair.FixShadersOnMaterialsAsync(materials);
        }
    }
}

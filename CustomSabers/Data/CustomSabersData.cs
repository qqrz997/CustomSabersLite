using CustomSaber.Utilities;
using System.IO;
using UnityEngine;

namespace CustomSaber.Data
{
    public class CustomSaberData
    {
        public string FileName { get; }

        public AssetBundle AssetBundle { get; }

        public GameObject Sabers {  get; }

        public SaberDescriptor Descriptor { get; }

        public CustomSaberData(string fileName)
        {
            FileName = fileName;

            if (FileName != "DefaultSabers")
            {
                try
                {
                    AssetBundle = AssetBundle.LoadFromFile(Path.Combine(Plugin.CustomSaberAssetsPath, fileName));
                    Sabers = AssetBundle.LoadAsset<GameObject>("_CustomSaber");
                    Descriptor = Sabers.GetComponent<SaberDescriptor>();
                    Descriptor.CoverImage = Descriptor.CoverImage ?? CustomSaberUtils.GetNullCoverImage();
                }
                catch
                {
                    Plugin.Log.Warn($"Problem encountered when getting the AssetBundle for {fileName}");

                    Descriptor = new SaberDescriptor
                    {
                        SaberName = "Invalid Saber",
                        AuthorName = fileName
                    };

                    FileName = "DefaultSabers";
                }
            }
            else
            {
                Descriptor = new SaberDescriptor
                {
                    SaberName = "Default",
                    AuthorName = "Beat Games",
                    Description = "Default Sabers",
                    CoverImage = ImageLoading.LoadSpriteFromResources("CustomSaber.Resources.defaultsabers-image.png")
                };
            }
        }

        public CustomSaberData(GameObject leftSaber, GameObject rightSaber)
        {
            FileName = "DefaultSabers";

            Descriptor = new SaberDescriptor
            {
                SaberName = "Default",
                AuthorName = "Beat Games",
                Description = "Vanilla game sabers"
            };

            GameObject saberParent = new GameObject();
            if (saberParent)
            {
                leftSaber.transform.SetParent(saberParent.transform);
                rightSaber.transform.SetParent (saberParent.transform);
            }

            Sabers = saberParent;
        }

        public void Destroy()
        {
            if (AssetBundle != null)
            {
                AssetBundle.Unload(true);
            }
            else
            {
                Object.Destroy(Descriptor);
            }
        }
    }
}

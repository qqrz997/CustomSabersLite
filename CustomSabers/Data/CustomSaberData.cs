using UnityEngine;

namespace CustomSabersLite.Data
{
    internal class CustomSaberData
    {
        public string FilePath { get; set; }

        public GameObject SabersObject { get; set; }

        public SaberDescriptor Descriptor { get; set; }

        public bool MissingShaders;

        public CustomSaberData(string relativePath = null, GameObject sabersObject = null, SaberDescriptor descriptor = null)
        {
            FilePath = relativePath;
            SabersObject = sabersObject;
            Descriptor = descriptor;
        }

        public CustomSaberData ForDefaultSabers()
        {
            return new CustomSaberData()
            {
                FilePath = "Default",
                Descriptor = new SaberDescriptor
                {
                    SaberName = "Default",
                    AuthorName = "Beat Games"
                }
            };
        }

        public void Destroy()
        {
            Object.Destroy(Descriptor);
            Object.Destroy(SabersObject);
        }
    }
}

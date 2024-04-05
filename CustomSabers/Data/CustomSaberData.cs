using UnityEngine;

namespace CustomSabersLite.Data
{
    internal class CustomSaberData
    {
        public string FileName { get; set; }

        public GameObject SabersObject { get; set; }

        public SaberDescriptor Descriptor { get; set; }

        public bool MissingShaders;

        public CustomSaberData(string fileName)
        {
            FileName = fileName;

            if (FileName == "Default")
            {
                Descriptor = new SaberDescriptor
                {
                    SaberName = "Default",
                    AuthorName = "Beat Games",
                    Description = "Default Sabers",
                };
            }
        }

        public void Destroy()
        {
            Object.Destroy(SabersObject);
            Object.Destroy(Descriptor);
        }
    }
}

using UnityEngine;

namespace CustomSabersLite.Data
{
    internal class CustomSaberData
    {
        public string FilePath { get; private set; }
        public GameObject SabersObject { get; private set; }
        public SaberDescriptor Descriptor { get; private set; }
        public CustomSaberType Type { get; private set; }

        public bool MissingShaders; // not yet implemented

        public CustomSaberData(string relativePath, GameObject sabersObject, SaberDescriptor descriptor, CustomSaberType customSaberType)
        {
            FilePath = relativePath;
            SabersObject = sabersObject;
            Descriptor = descriptor;
            Type = customSaberType;
        }

        public static CustomSaberData ForDefaultSabers() =>
            new CustomSaberData("Default", null, new SaberDescriptor { SaberName = "Default", AuthorName = "Beat Games" }, CustomSaberType.Default);

        public void Destroy()
        {
            Object.Destroy(Descriptor);
            Object.Destroy(SabersObject);
        }
    }
}

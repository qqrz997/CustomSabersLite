using UnityEngine;

namespace CustomSabersLite.Data
{
    /// <summary>
    /// Class that declares the neccessary information to manage a custom saber instance
    /// </summary>
    internal class CustomSaberData
    {
        public string FilePath { get; private set; }
        public GameObject SaberPrefab { get; private set; }
        public SaberDescriptor Descriptor { get; private set; }
        public CustomSaberType Type { get; private set; }

        public bool MissingShaders; // not yet implemented

        public CustomSaberData(string relativePath, GameObject saberPrefab, SaberDescriptor descriptor, CustomSaberType customSaberType)
        {
            FilePath = relativePath;
            SaberPrefab = saberPrefab;
            Descriptor = descriptor;
            Type = customSaberType;
        }

        public static CustomSaberData ForDefaultSabers() =>
            new CustomSaberData(null, null, new SaberDescriptor { SaberName = "Default", AuthorName = "Beat Games" }, CustomSaberType.Default);

        public void Destroy()
        {
            Object.Destroy(Descriptor);
            Object.Destroy(SaberPrefab);
        }
    }
}

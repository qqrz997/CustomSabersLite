﻿using CustomSaber.Utilities;
using System.IO;
using UnityEngine;
using AssetBundleLoadingTools.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomSaber.Data
{
    public class CustomSaberData
    {
        public string FileName { get; set; }

        public AssetBundle AssetBundle { get; set; }

        public GameObject SabersObject { get; set; }

        public SaberDescriptor Descriptor { get; set; }

        public CustomSaberData(string fileName)
        {
            FileName = fileName;

            if (FileName == "DefaultSabers")
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

        public void Destroy()
        {
            if (AssetBundle != null)
            {
                AssetBundle.Unload(true);
            }
            else
            {
                UnityEngine.Object.Destroy(Descriptor);
            }
        }
    }
}
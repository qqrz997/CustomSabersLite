using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomSaber.Data
{
    public struct CustomSaberMetadata
    {
        public string SaberFileName;

        public string AuthorName;

        public byte[] CoverImage;

        public CustomSaberMetadata(string saberName,  string authorName, byte[] coverImage)
        {
            SaberFileName = saberName;
            AuthorName = authorName;
            CoverImage = coverImage;
        }
    }
}

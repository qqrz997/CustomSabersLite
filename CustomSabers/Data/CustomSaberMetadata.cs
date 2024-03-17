using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomSabersLite.Data
{
    public struct CustomSaberMetadata
    {
        public string SaberName;

        public string AuthorName;

        public string SaberFileName;

        public bool MissingShaders; // todo - flag

        public byte[] CoverImage;

        public CustomSaberMetadata(string saberName,  string authorName, string saberFileName = null, bool missingShaders = false, byte[] coverImage = null)
        {
            SaberName = saberName;
            AuthorName = authorName;
            SaberFileName = saberFileName;
            MissingShaders = missingShaders;
            CoverImage = coverImage;
        }
    }
}

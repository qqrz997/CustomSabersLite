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

        public bool MissingShaders; //todo - flag

        public bool InvalidChars; //todo - flag

        public byte[] CoverImage;

        public CustomSaberMetadata(string saberName,  string authorName, bool missingShaders = false, bool invalidChars = false, byte[] coverImage = null)
        {
            SaberFileName = saberName;
            AuthorName = authorName;
            MissingShaders = missingShaders;
            InvalidChars = invalidChars;
            CoverImage = coverImage;
        }
    }
}

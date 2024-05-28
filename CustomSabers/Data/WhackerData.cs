using UnityEngine;

namespace CustomSabersLite.Data
{
    internal struct WhackerObject
    {
        public string androidFileName;
        public string pcFileName;
        public WhackerDescriptor descriptor;
        public WhackerConfig config;
    }

    internal struct WhackerDescriptor
    {
        public string objectName;
        public string author;
        public string description;
        public string coverImage;
    }

    internal struct WhackerConfig
    {
        public bool hasTrail;
    }

    internal struct WhackerTrail
    {
        public int trailId;
        public ColorType colorType;
        public Color trailColor;
        public Color multiplierColor;
        public int length;
        public float whiteStep;
    }

    internal struct WhackerTrailTransform
    {
        public int trailId;
        public bool isTop;
    }
}

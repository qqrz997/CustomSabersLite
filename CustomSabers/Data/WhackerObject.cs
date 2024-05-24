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
}

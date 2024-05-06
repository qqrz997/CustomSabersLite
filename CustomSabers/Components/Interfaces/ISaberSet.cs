namespace CustomSabersLite.Components.Interfaces
{
    internal interface ISaberSet
    {
        /// <summary>
        /// Gets the instance of the selected saber by type
        /// </summary>
        CSLSaber CustomSaberForSaberType(SaberType saberType);
    }
}

using System;

namespace CustomSabersLite.Models;

internal interface ISaberPrefab : IDisposable
{
    public SaberInstanceSet Instantiate();
    public ITrailData[] GetTrailsForType(SaberType saberType);
}
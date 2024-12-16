using System;

namespace CustomSabersLite.Utilities.Services;

internal class UtcTimeService : ITimeService
{
    public DateTime GetUtcTime() => DateTime.UtcNow;
}

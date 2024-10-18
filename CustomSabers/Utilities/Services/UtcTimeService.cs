using System;

namespace CustomSabersLite.Utilities;

internal class UtcTimeService : ITimeService
{
    public DateTime GetUtcTime() => DateTime.UtcNow;
}

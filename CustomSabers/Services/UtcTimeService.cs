using System;

namespace CustomSabersLite.Services;

internal class UtcTimeService : ITimeService
{
    public DateTime GetUtcTime() => DateTime.UtcNow;
}

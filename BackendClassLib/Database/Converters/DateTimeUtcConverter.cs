using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BackendClassLib.Database.Converters;

public class DateTimeUtcConverter : ValueConverter<DateTime, DateTime>
{
    public DateTimeUtcConverter() : base(
        d => d.ToUniversalTime(),
        d => DateTime.SpecifyKind(d, DateTimeKind.Utc).ToLocalTime()
    )
    {

    }
}
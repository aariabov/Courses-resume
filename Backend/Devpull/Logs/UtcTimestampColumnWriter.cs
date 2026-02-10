using Serilog.Events;

using Serilog.Sinks.PostgreSQL;

namespace Devpull.Logs;

public class UtcTimestampColumnWriter : ColumnWriterBase
{
    public UtcTimestampColumnWriter()
        : base(NpgsqlTypes.NpgsqlDbType.TimestampTz) { }

    public override object GetValue(LogEvent logEvent, IFormatProvider? formatProvider = null)
    {
        return logEvent.Timestamp.UtcDateTime;
    }
}

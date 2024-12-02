using System.ComponentModel;
using System.Globalization;

namespace Qala.Cli.Utils;

public class CommaSeparatedGuidListConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string stringValue)
        {
            var guids = stringValue.Split(',', StringSplitOptions.RemoveEmptyEntries);
            return Array.ConvertAll(guids, Guid.Parse).ToList();
        }
        var result = base.ConvertFrom(context, culture, value) ?? throw new InvalidOperationException("Conversion resulted in a null value.");
        return result;
    }
}
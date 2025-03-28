using System.ComponentModel;
using System.Globalization;

namespace Qala.Cli.Utils;

public class CommaSeparatedStringListConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is string stringValue)
        {
            var strings = stringValue.Split(',', StringSplitOptions.RemoveEmptyEntries);
            return strings.ToList();
        }
        var result = base.ConvertFrom(context, culture, value) ?? throw new InvalidOperationException("Conversion resulted in a null value.");
        return result;
    }
}
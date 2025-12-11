using System.ComponentModel;

namespace Cocona.Command.Binder;

public  sealed class CoconaValueConverter : ICoconaValueConverter
{
    public object? ConvertTo(Type t, string? value)
    {
        if (value is null)
        {
            return null;
        }

        if (t == typeof(bool))
        {
            return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);
        }

        if (t == typeof(int))
        {
            return int.Parse(value);
        }

        if (t == typeof(string))
        {
            return value;
        }

        if (t == typeof(FileInfo))
        {
            return new FileInfo(value);
        }

        if (t == typeof(DirectoryInfo))
        {
            return new DirectoryInfo(value);
        }

        return TypeDescriptor.GetConverter(t).ConvertFrom(value);
    }
}

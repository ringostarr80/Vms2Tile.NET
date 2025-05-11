namespace DbReader;

public enum DataType
{
    Points,
    Lines,
    Polygons
}

public static class DataTypeExtensions
{
    public static DataType? TryFrom(string value)
    {
        if (Enum.TryParse<DataType>(value, true, out var result))
        {
            return result;
        }

        return null;
    }
}

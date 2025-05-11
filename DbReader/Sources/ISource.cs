namespace DbReader.Sources;

public interface ISource
{
    Task<List<byte>> GetRawDataAsync(int x, int y, int z, string key, string value = "", DataType type = DataType.Polygons);
}

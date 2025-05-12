using System.Data;
using DbReader.Exceptions;
using Microsoft.Data.Sqlite;

namespace DbReader.Sources;

public class SQLite : IDisposable, ISource
{
    private const string SINGLE_TILE_QUERY = @"SELECT x, y, z, data
        FROM tiles
        WHERE detail_zoom = $detail_zoom
            AND object_type = $object_type
            AND osm_key = $osm_key
            AND osm_value = $osm_value
            AND x = $x
            AND y = $y
            AND z = $z";
    private const string MULTI_TILE_QUERY = @"SELECT x, y, z, data
        FROM tiles
        WHERE detail_zoom = $detail_zoom
            AND object_type = $object_type
            AND osm_key = $osm_key
            AND osm_value = $osm_value
            AND x >= $x_min
            AND x < $x_max
            AND y >= $y_min
            AND y < $y_max
            AND z = $z";

    private SqliteConnection Connection { get; }

    public SQLite(string filename)
    {
        if (string.IsNullOrWhiteSpace(filename))
        {
            throw new ArgumentException("Filename cannot be null or empty.", nameof(filename));
        }
        if (!File.Exists(filename))
        {
            throw new SourceDbNotFoundException(filename, "The specified SQLite file does not exist.", 404);
        }

        var connectionString = new SqliteConnectionStringBuilder($"Data Source={filename}") {
            Mode = SqliteOpenMode.ReadOnly
        };
        this.Connection = new SqliteConnection(connectionString.ToString());
        this.Connection.Open();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.Connection.Dispose();
        }
    }

    private static int GetDetailZoom(int z, string value, DataType type)
    {
        if (type == DataType.Points) {
            return 14;
        }

        int[] detailZooms = [0, 0, 2, 2, 4, 4, 6, 6, 8, 8, 10, 10, 12, 12, 14];
        switch (value) {
            case "terrain":
            case "depth":
                detailZooms = [0, 0, 2, 2, 4, 4, 6, 6, 8, 8, 10, 10, 12, 12, 12];
                break;

            case "bathymetry":
            case "blue_marble":
            case "elevation":
                detailZooms = [0, 0, 2, 2, 4, 4, 6, 6, 8, 8, 10, 10, 10, 10, 10];
                break;

            default: // ignore
                break;
        }

        return detailZooms[Math.Max(Math.Min(z, detailZooms.Length - 1), 0)];
    }

    public async Task<List<byte>> GetRawDataAsync(int x, int y, int z, string key, string value = "", DataType type = DataType.Polygons)
    {
        switch (key) {
            case "land":
            case "terrain":
            case "blue_marble":
            case "elevation":
            case "bathymetry":
            case "depth":
                value = key;
                key = "locr";
                type = DataType.Polygons;
                break;
            default: // ignore
                break;
        }

        var detailZoom = GetDetailZoom(z, value, type);

        var maxTileZoom = 16;
        var data = new List<byte>();
        var numberOfTiles = 0;
        var tileWeight = 0UL;

        for (var queryZ = 0; queryZ <= maxTileZoom; queryZ++) {
            var command = this.Connection.CreateCommand();

            if (queryZ <= z) {
                command.CommandText = SINGLE_TILE_QUERY;
                command.Parameters.AddWithValue("$detail_zoom", detailZoom);
                command.Parameters.AddWithValue("$object_type", type.ToString());
                command.Parameters.AddWithValue("$osm_key", key);
                command.Parameters.AddWithValue("$osm_value", value);
                command.Parameters.AddWithValue("$x", x >> (z - queryZ));
                command.Parameters.AddWithValue("$y", y >> (z - queryZ));
                command.Parameters.AddWithValue("$z", queryZ);
            } else {
                var queryLeftX = x << (queryZ - z);
                var queryTopY = y << (queryZ - z);

                var queryRightX = queryLeftX + (1 << (queryZ - z));
                var queryBottomY = queryTopY + (1 << (queryZ - z));

                command.CommandText = MULTI_TILE_QUERY;
                command.Parameters.AddWithValue("$detail_zoom", detailZoom);
                command.Parameters.AddWithValue("$object_type", type.ToString());
                command.Parameters.AddWithValue("$osm_key", key);
                command.Parameters.AddWithValue("$osm_value", value);
                command.Parameters.AddWithValue("$x_min", queryLeftX);
                command.Parameters.AddWithValue("$x_max", queryRightX);
                command.Parameters.AddWithValue("$y_min", queryTopY);
                command.Parameters.AddWithValue("$y_max", queryBottomY);
                command.Parameters.AddWithValue("$z", queryZ);
            }

            using var reader = await command.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

            if (reader.HasRows) {
                tileWeight += (ulong)Math.Pow(4, maxTileZoom - queryZ);

                while (await reader.ReadAsync())
                {
                    var rowData = (byte[])reader["data"];
                    var dataLength = rowData.Length;

                    data.AddRange(BitConverter.GetBytes(reader.GetInt32(reader.GetOrdinal("x"))));
                    data.AddRange(BitConverter.GetBytes(reader.GetInt32(reader.GetOrdinal("y"))));
                    data.AddRange(BitConverter.GetBytes(reader.GetInt32(reader.GetOrdinal("z"))));
                    data.AddRange(BitConverter.GetBytes(detailZoom));
                    data.AddRange(BitConverter.GetBytes(dataLength));

                    if (dataLength > 0)
                    {
                        data.AddRange(rowData);
                    }

                    numberOfTiles++;
                }
            }

            if (tileWeight >= Math.Pow(4, maxTileZoom - z))
            {
                break;
            }
        }

        data.InsertRange(0, BitConverter.GetBytes(numberOfTiles));

        return data;
    }
}

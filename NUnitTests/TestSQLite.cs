using DbReader;
using DbReader.Exceptions;
using DbReader.Sources;

namespace NUnitTests;

public class TestSQLite
{
    private static string GetTestSqliteFilePath()
    {
        return Path.Combine(
            Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.ToString(),
            "data",
            "braunschweig.sqlite"
        );
    }

    [Test]
    public async Task TestGetDataBuildingPolygons()
    {
        var tileDb = new SQLite(GetTestSqliteFilePath());
        var tileData = await tileDb.GetRawDataAsync(
            x: 34686,
            y: 21566,
            z: 16,
            key: "building",
            value: "*",
            type: DataType.Polygons
        );

        Assert.That(tileData, Is.Not.Null);
        Assert.That(tileData, Is.TypeOf<List<byte>>());
        Assert.That(tileData, Has.Count.GreaterThanOrEqualTo(4));
    }

    [Test]
    public async Task TestGetDataCityPoints() {
        var tileDb = new SQLite(GetTestSqliteFilePath());
        var tileData = await tileDb.GetRawDataAsync(
            x: 34686,
            y: 21566,
            z: 16,
            key: "place",
            value: "city",
            type: DataType.Points
        );

        Assert.That(tileData, Is.Not.Null);
        Assert.That(tileData, Is.TypeOf<List<byte>>());
        Assert.That(tileData, Has.Count.GreaterThanOrEqualTo(4));
    }

    [Test]
    public async Task TestGetLandData() {
        var tileDb = new SQLite(GetTestSqliteFilePath());
        var tileData = await tileDb.GetRawDataAsync(x: 34686, y: 21566, z: 16, key: "land");

        Assert.That(tileData, Is.Not.Null);
        Assert.That(tileData, Is.TypeOf<List<byte>>());
        Assert.That(tileData, Has.Count.GreaterThanOrEqualTo(4));
    }

    [Test]
    public async Task TestGetTerrainData() {
        var tileDb = new SQLite(GetTestSqliteFilePath());
        var tileData = await tileDb.GetRawDataAsync(x: 34686, y: 21566, z: 16, "terrain");

        Assert.That(tileData, Is.Not.Null);
        Assert.That(tileData, Is.TypeOf<List<byte>>());
        Assert.That(tileData, Has.Count.GreaterThanOrEqualTo(4));
    }

    [Test]
    public async Task TestGetBlueMarbleData() {
        var tileDb = new SQLite(GetTestSqliteFilePath());
        var tileData = await tileDb.GetRawDataAsync(x: 34686, y: 21566, z: 16, "blue_marble");

        Assert.That(tileData, Is.Not.Null);
        Assert.That(tileData, Is.TypeOf<List<byte>>());
        Assert.That(tileData, Has.Count.GreaterThanOrEqualTo(4));
    }

    [Test]
    public async Task TestGetRawDataWhereZoomIsZero() {
        var tileDb = new SQLite(GetTestSqliteFilePath());
        var tileData = await tileDb.GetRawDataAsync(x: 0, y: 0, z: 0, key: "land");

        Assert.That(tileData, Is.Not.Null);
        Assert.That(tileData, Is.TypeOf<List<byte>>());
        Assert.That(tileData, Has.Count.GreaterThanOrEqualTo(4));
    }

    [Test]
    public async Task TestGetDataFromInternalMultiTileQuery() {
        var tileDb = new SQLite(GetTestSqliteFilePath());
        var tileData = await tileDb.GetRawDataAsync(x: 1083, y: 673, z: 12, key: "land");

        Assert.That(tileData, Is.Not.Null);
        Assert.That(tileData, Is.TypeOf<List<byte>>());
        Assert.That(tileData, Has.Count.GreaterThanOrEqualTo(4));
    }

    [Test]
    public void TestDbFileDoesNotExists() {
        Assert.Throws<SourceDbNotFoundException>(() => new SQLite("non_existing_file.sqlite"));
    }

    [Test]
    public async Task TestBehaviourWhenMaxTileZoomMinusZIsNegative() {
        var tileDb = new SQLite(GetTestSqliteFilePath());
        var tileData = await tileDb.GetRawDataAsync(
            x: 69372,
            y: 43129,
            z: 17,
            key: "highway",
            value: "pedestrian",
            type: DataType.Polygons
        );

        Assert.That(tileData, Is.Not.Null);
        Assert.That(tileData, Is.TypeOf<List<byte>>());
        Assert.That(tileData, Has.Count.GreaterThanOrEqualTo(4));
    }
}
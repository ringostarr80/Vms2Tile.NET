using DbReader;

namespace NUnitTests;

public class TestDataType
{
    [Test]
    public void TestFrom()
    {
        Assert.Multiple(() =>
        {
            Assert.That(DataTypeExtensions.TryFrom("Points"), Is.EqualTo(DataType.Points));
            Assert.That(DataTypeExtensions.TryFrom("Lines"), Is.EqualTo(DataType.Lines));
            Assert.That(DataTypeExtensions.TryFrom("Polygons"), Is.EqualTo(DataType.Polygons));

            Assert.That(DataTypeExtensions.TryFrom("points"), Is.EqualTo(DataType.Points));
            Assert.That(DataTypeExtensions.TryFrom("POINTS"), Is.EqualTo(DataType.Points));
            Assert.That(DataTypeExtensions.TryFrom("PoInTs"), Is.EqualTo(DataType.Points));

            Assert.That(DataTypeExtensions.TryFrom(""), Is.Null);
            Assert.That(DataTypeExtensions.TryFrom("foo"), Is.Null);
        });
    }
}

using DbReader.Exceptions;

namespace NUnitTests;

public class TestSourceDbNotFoundException
{
    [Test]
    public void TestException()
    {
        Assert.Multiple(() =>
        {
            var exception = new SourceDbNotFoundException("my_db.sql", "my_db.sql filename not found.", 404);
            Assert.That(exception.Database, Is.EqualTo("my_db.sql"));
            Assert.That(exception.Message, Is.EqualTo("my_db.sql filename not found."));
            Assert.That(exception.Code, Is.EqualTo(404));
            Assert.That(exception.InnerException, Is.Null);
        });
    }
}

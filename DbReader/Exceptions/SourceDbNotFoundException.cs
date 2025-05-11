namespace DbReader.Exceptions;

public class SourceDbNotFoundException(string database, string message, int code = 0, Exception? innerException = null) : Exception(message, innerException)
{
    public string Database { get; } = database;
    public int Code { get; } = code;
}

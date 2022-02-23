namespace Task2;

public interface ILogger
{
    void WriteMessage(string message);

    void WriteError(string message);

    void EntryLog(StreamWriter streamWriter);

    void Log(string logMessage, string fileName, bool isError, StreamWriter streamWriter);

    void Log(string logMessage, bool isError, StreamWriter streamWriter);
}


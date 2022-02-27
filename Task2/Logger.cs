namespace Task2.Logger;

public class Logger : ILogger
{
    public void WriteMessage(string message)
    {
        Console.WriteLine(message);
    }

    public void WriteError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"ERROR: {message}");
        Console.ResetColor();
    }

    public void EntryLog(StreamWriter streamWriter)
    {
        streamWriter.WriteLine($"\r--------------------------------------------------------------");
        streamWriter.WriteLine($"Synchronize entry: {DateTime.Now.ToLongDateString()}");
        streamWriter.WriteLine($"{DateTime.Now.ToLongTimeString()}");
        streamWriter.WriteLine($"--------------------------------------------------------------");
    }

    public void Log(string logMessage, bool isError, StreamWriter streamWriter, string fileName = "")
    {
        streamWriter.WriteLine($"\r\nLog Entry: {DateTime.Now.ToLongTimeString()}");
        streamWriter.WriteLine($"{logMessage}: {fileName}");
        if (isError)
        {
            WriteError($"{logMessage} {fileName}");
        }
        else
        {
            WriteMessage($"{logMessage} {fileName}");
        }
    }
}
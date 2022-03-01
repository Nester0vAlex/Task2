namespace Task2;

public static class Constants
{
    public const int OriginalDirectoryPathIndexInParameters = 0;
    public const int CopiedDirectoryPathIndexInParameters = 1;
    public const int LogDirectoryPathIndexInParameters = 2;
    public const int IntervalIndexInParameters = 3;
    public const int NumberOfArguments = 4;

    public const string ExitMessage = "Press any key to exit";

    public const string OperationTypeDeleteDirectory = "DeleteDirectory";
    public const string OperationTypeDeleteFile = "DeleteFile";
    public const string OperationTypeCopyFile = "CopyFile";

    public const string OperationFinished = "Directory has been synchronizated";
    public const string LogFileName = "Log.txt";
    public const string FileCopied = "File Copied";
    public const string FileDeleted = "File Deleted";
    public const string FileCreated = "File Created";
    public const string DirectoryCopied = "Directory Copied";
    public const string DirectoryDeleted = "Directory Deleted";
    public const string DirectoryCreated = "Directory Created";
    public const string OperationFailed = "Operation failed";
    public const ConsoleKey ButtonForCancel = ConsoleKey.Escape;

    public const string ErrorMessageInvalidInterval = "Invalid interval";
    public const string ErrorMessageInvalidNumberOfArguments = "Invalid number of arguments";
    public const string ErrorMessageOriginalDirectoryNotFound = "Original directory not found";
    public const string ErrorMessageOriginalAndCopiedDirectoriesAreSame = "Original and copied directories are same";
    public const string MessageWithSyntaxHint = $"Syntax order should be: [original_directory_path] " +
        $"[copied_directory_path] [log_file_path] [logging_interval_in_seconds]";
}


namespace Task2;

public enum ValidationStatus : byte
{
    Success,
    InvalidNumberOfArguments,
    InvalidInterval,
    OriginalDirectoryNotFound,
    OriginalAndCopiedDirectoriesAreSame
}

public class Engine
{
    private readonly ILogger logger;
    public StreamWriter _StreamWriter { get; private set; }
    public string OriginalDirectoryPath { get; private set; }
    public string CopiedDirectoryPath { get; private set; }
    public string LogPath { get; private set; }
    public int Interval { get; private set; }

    public Engine(ILogger logger)
    {
        this.logger = logger;
    }
    public void StartSynchronization()
    {
        SynchronizeDirectories(OriginalDirectoryPath, CopiedDirectoryPath);

        logger.Log(Constants.OperationFinished, CopiedDirectoryPath, isError: false, _StreamWriter);
    }
    void SynchronizeDirectories(string origDir, string copiedDir)
    {
        if (!Directory.Exists(copiedDir))
        {
            Directory.CreateDirectory(copiedDir);
            logger.Log(Constants.DirectoryCreated, Path.GetFileName(copiedDir), isError: false, _StreamWriter);
        }
        foreach (string file in Directory.GetFiles(origDir))
        {
            string fileName = Path.GetFileName(file);

            FileInfo originalFileInfo = new(origDir + "\\" + fileName);
            FileInfo copiedFileInfo = new(copiedDir + "\\" + fileName);
            DateTimeOffset originalFileTimeChanged = originalFileInfo.LastWriteTime;
            DateTimeOffset copiedFileTimeChanged = copiedFileInfo.LastWriteTime;

            if (!File.Exists(copiedDir + "\\" + fileName) || originalFileTimeChanged != copiedFileTimeChanged)
            {
                string fileToReplace = copiedDir + "\\" + fileName;
                tryOperation(file, fileToReplace, "CopyFile");
                logger.Log(Constants.FileCopied, fileName, isError: false, _StreamWriter);
            }
        }
        foreach (string file in Directory.GetFiles(copiedDir))
        {
            string fileName = Path.GetFileName(file);

            if (!File.Exists(origDir + "\\" + fileName))
            {
                tryOperation(file, secondFile: null, "DeleteFile");
                logger.Log(Constants.FileDeleted, fileName, isError: false, _StreamWriter);
            }
        }
        foreach (string file in Directory.GetDirectories(copiedDir))
        {
            string dirName = Path.GetFileName(file);

            if (!Directory.Exists(origDir + "\\" + dirName))
            {
                tryOperation(file, secondFile: null, Constants.OperationTypeDeleteDirectory);
                logger.Log(Constants.DirectoryDeleted, dirName, isError: false, _StreamWriter);
            }
        }
        foreach (string s in Directory.GetDirectories(origDir))
        {
            SynchronizeDirectories(s, copiedDir + "\\" + Path.GetFileName(s));
        }
    }
    void tryOperation(string firstFile, string secondFile, string operationType)
    {
        try
        {
            switch (operationType)
            {
                case Constants.OperationTypeDeleteFile:
                    File.Delete(firstFile);
                    break;

                case Constants.OperationTypeDeleteDirectory:
                    Directory.Delete(firstFile, recursive: true);
                    break;

                case Constants.OperationTypeCopyFile:
                    File.Copy(firstFile, secondFile, overwrite: true);
                    break;
            }
        }
        catch (Exception ex)
        {
            SafeExit(ex.Message, isError: true);
        }
    }
    public void waitForNextSynchronization(int interval) //(не)работающее закрытие кнопкой
    {
        Task taskForCancelling = Task.Factory.StartNew(() =>
        {
            if (Console.ReadKey(true).Key == Constants.ButtonForCancel)
            {
                _StreamWriter.Close();
                Environment.Exit(0);
            }
        }, TaskCreationOptions.AttachedToParent);
        Thread.Sleep(TimeSpan.FromSeconds(interval));
    }
    public ValidationStatus Validate(string[] args)
    {
        LogPath = args[Constants.LogPathIndexInParameters];
        _StreamWriter = File.AppendText(LogPath + "\\" + Constants.LogFileName);
        logger.EntryLog(_StreamWriter);

        if (args.Length != Constants.NumberOfArguments)
        {
            return ValidationStatus.InvalidNumberOfArguments;
        }

        OriginalDirectoryPath = args[Constants.OriginalDirectoryPathIndexInParameters];
        CopiedDirectoryPath = args[Constants.CopiedDirectoryPathIndexInParameters];

        int interval;
        int.TryParse(args[Constants.IntervalIndexInParameters], out interval);
        if (interval == 0)
        {
            return ValidationStatus.InvalidInterval;
        }
        else
        {
            Interval = interval;
        }

        if (!Directory.Exists(OriginalDirectoryPath))
        {
            return ValidationStatus.OriginalDirectoryNotFound;
        }

        if (OriginalDirectoryPath == CopiedDirectoryPath)
        {
            return ValidationStatus.OriginalAndCopiedDirectoriesAreSame;
        }

        if (!Directory.Exists(LogPath))
        {
            Directory.CreateDirectory(LogPath);
        }

        return ValidationStatus.Success;
    }
    public void ShowValidationErrorMessage(ValidationStatus status)
    {
        switch (status)
        {
            case ValidationStatus.InvalidNumberOfArguments:
                SafeExit(Constants.ErrorMessageInvalidNumberOfArguments, isError: true);
                break;

            case ValidationStatus.OriginalDirectoryNotFound:
                SafeExit(Constants.ErrorMessageOriginalDirectoryNotFound, isError: true);
                break;

            case ValidationStatus.OriginalAndCopiedDirectoriesAreSame:
                SafeExit(Constants.ErrorMessageOriginalAndCopiedDirectoriesAreSame, isError: true);
                break;

            case ValidationStatus.InvalidInterval:
                SafeExit(Constants.ErrorMessageInvalidInterval, isError: true);
                break;
        }
    }
    void SafeExit(string message, bool isError)
    {
        logger.Log(message, isError, _StreamWriter);
        _StreamWriter.Close();
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        Environment.Exit(-1);
    }
}


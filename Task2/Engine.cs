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
    private StreamWriter StreamWriter { get; set; }
    private string OriginalDirectoryPath { get; set; }
    private string CopiedDirectoryPath { get; set; }
    private string LogPath { get; set; }
    private int Interval { get; set; }

    public Engine(ILogger logger)
    {
        this.logger = logger;
    }
    public void StartSynchronization()
    {
        SynchronizeDirectories(OriginalDirectoryPath, CopiedDirectoryPath);

        logger.Log(Constants.OperationFinished, isError: false, StreamWriter, CopiedDirectoryPath);
    }
    void SynchronizeDirectories(string origDir, string copiedDir)
    {
        if (!Directory.Exists(copiedDir))
        {
            Directory.CreateDirectory(copiedDir);
            logger.Log(Constants.DirectoryCreated, isError: false, StreamWriter, Path.GetFileName(copiedDir));
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
                TryOperation(file, Constants.OperationTypeCopyFile, fileToReplace);
                logger.Log(Constants.FileCopied, isError: false, StreamWriter, fileName);
            }
        }
        foreach (string file in Directory.GetFiles(copiedDir))
        {
            string fileName = Path.GetFileName(file);

            if (!File.Exists(origDir + "\\" + fileName))
            {
                TryOperation(file, Constants.OperationTypeDeleteFile);
                logger.Log(Constants.FileDeleted, isError: false, StreamWriter, fileName);
            }
        }
        foreach (string file in Directory.GetDirectories(copiedDir))
        {
            string dirName = Path.GetFileName(file);

            if (!Directory.Exists(origDir + "\\" + dirName))
            {
                TryOperation(file, Constants.OperationTypeDeleteDirectory);
                logger.Log(Constants.DirectoryDeleted, isError: false, StreamWriter, dirName);
            }
        }
        foreach (string s in Directory.GetDirectories(origDir))
        {
            SynchronizeDirectories(s, copiedDir + "\\" + Path.GetFileName(s));
        }
    }
    void TryOperation(string firstFile, string operationType, string secondFile = null)
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
    public void WaitForNextSynchronization() //(не)работающее закрытие кнопкой
    {
        Task taskForCancelling = Task.Factory.StartNew(() =>
        {
            if (Console.ReadKey(true).Key == Constants.ButtonForCancel)
            {
                StreamWriter.Close();
                Environment.Exit(0);
            }
        }, TaskCreationOptions.AttachedToParent);
        Thread.Sleep(TimeSpan.FromSeconds(Interval));
    }
    public ValidationStatus Validate(string[] args)
    {
        LogPath = args[Constants.LogPathIndexInParameters];
        StreamWriter = File.AppendText(LogPath + "\\" + Constants.LogFileName);
        logger.EntryLog(StreamWriter);

        if (args.Length != Constants.NumberOfArguments)
        {
            return ValidationStatus.InvalidNumberOfArguments;
        }

        OriginalDirectoryPath = args[Constants.OriginalDirectoryPathIndexInParameters];
        CopiedDirectoryPath = args[Constants.CopiedDirectoryPathIndexInParameters];

        int.TryParse(args[Constants.IntervalIndexInParameters], out int interval);
        if (interval == 0)
        {
            return ValidationStatus.InvalidInterval;
        }
        else
        {
            this.Interval = interval;
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
        logger.Log(message, isError, StreamWriter);
        StreamWriter.Close();
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        Environment.Exit(-1);
    }
}
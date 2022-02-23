using Task2;
using Task2.Logger;

Logger logger = new();
Engine engine = new(logger);

ValidationStatus validationResult = engine.Validate(args);

if (validationResult == ValidationStatus.Success)
{
    Console.WriteLine($"Press {Constants.ButtonForCancel} to end synchronization process");

    while (true)
    {
        engine.StartSynchronization();

        engine.waitForNextSynchronization(engine.Interval);
    }
}
else
{
    engine.ShowValidationErrorMessage(validationResult);
}

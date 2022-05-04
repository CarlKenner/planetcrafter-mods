namespace Doublestop.Tpc;

public static class ConsoleUtil
{
    public static async ValueTask<ConsoleKeyInfo> ReadKeyAsync(bool intercept, int checkIntervalMs, CancellationToken cancel)
    {
        if (checkIntervalMs <= 0)
            throw new ArgumentOutOfRangeException(nameof(checkIntervalMs), "Check interval must be a positive value.");

        while (!Console.KeyAvailable)
            await Task.Delay(checkIntervalMs, cancel);

        return Console.ReadKey(intercept);
    }

    public static async ValueTask<bool> WaitForConfirmationAsync(bool intercept, int checkIntervalMs,
        CancellationToken cancel)
    {
        while (true)
        {
            var key = await ReadKeyAsync(intercept, checkIntervalMs, cancel);
            switch (key.Key)
            {
                case ConsoleKey.Y:
                    return true;

                case ConsoleKey.N:
                case ConsoleKey.Enter:
                case ConsoleKey.Escape:
                    return false;
            }
        }
    }
}
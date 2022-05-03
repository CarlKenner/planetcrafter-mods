namespace Doublestop.Tpc.Internal;

internal static class FileUtil
{
    #region Public Methods

    public static void CopyDirectoryTree(string sourceDir, string destinationDir, CancellationToken cancel)
    {
        var dir = new DirectoryInfo(sourceDir);
        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

        var dirs = dir.GetDirectories();
        var files = dir.GetFiles();
        if (!dirs.Any() && !files.Any())
            return;

        Directory.CreateDirectory(destinationDir);
        foreach (var file in files)
        {
            cancel.ThrowIfCancellationRequested();
            file.CopyTo(Path.Combine(destinationDir, file.Name));
        }
        foreach (var subDir in dirs)
        {
            cancel.ThrowIfCancellationRequested();
            var newDestinationDir = Path.Combine(destinationDir, subDir.Name);
            CopyDirectoryTree(subDir.FullName, newDestinationDir, cancel);
        }
    }

    public static int CountFiles(string dir, bool recurse = false)
    {
        return CountFiles(dir, null, recurse);
    }

    public static int CountFiles(string dir, string? pattern, bool recurse = false)
    {
        return new DirectoryInfo(dir).CountFiles(pattern, recurse);
    }

    public static int CountFiles(this DirectoryInfo directory, bool recurse = false)
    {
        return CountFiles(directory, null, recurse);
    }

    public static int CountFiles(this DirectoryInfo directory, string? pattern, bool recurse = false)
    {
        if (string.IsNullOrWhiteSpace(pattern))
            pattern = "*.*";
        return directory.Exists
            ? directory.EnumerateFiles(pattern, recurse ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                .Count()
            : 0;
    }

    #endregion
}
using System.Diagnostics;

namespace Doublestop.Tpc.Game;

/// <summary>
/// Represents the game's .NET assemblies, such as <c>Assembly-CSharp.dll</c>.
/// </summary>
[DebuggerDisplay("{Directory}")]
public sealed class ManagedFiles
{
    #region Fields

    public const string AssemblyCSharpName = "Assembly-CSharp.dll";
    public const string AssemblyCSharpFirstPassName = "Assembly-CSharp-firstpass.dll";

    #endregion

    #region Constructors

    /// <param name="managedDirectoryPath">Full path to the directory containing <c>Assembly-CSharp.dll</c>.</param>
    /// <exception cref="ArgumentException">Directory was null, empty, or all whitespace.</exception>
    public ManagedFiles(string managedDirectoryPath)
    {
        if (string.IsNullOrWhiteSpace(managedDirectoryPath))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(managedDirectoryPath));

        Directory = new DirectoryInfo(managedDirectoryPath);
    }

    #endregion

    #region Properties

    /// <summary>
    /// The directory containing the game's .NET assemblies, eg <c>Assembly-CSharp.dll</c>.
    /// </summary>
    public DirectoryInfo Directory { get; }

    /// <summary>
    /// Enumerates all <c>.dll</c> files contained at the top level of <see cref="Directory"/>.
    /// </summary>
    public IEnumerable<FileInfo> AssemblyFiles => Directory.EnumerateFiles("*.dll", SearchOption.TopDirectoryOnly);

    /// <summary>
    /// Enumerates all files (<c>*.*</c>) contained at the top level of <see cref="Directory"/>.
    /// </summary>
    public IEnumerable<FileInfo> AllFiles => Directory.EnumerateFiles("*.*", SearchOption.TopDirectoryOnly);

    #endregion
}
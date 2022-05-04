using System.Reflection;

namespace Doublestop.Tpc.Internal;

internal sealed record LoadedAssembly(Assembly Assembly, MetadataLoadContext Context) : IDisposable
{
    public Assembly Assembly { get; init; } = Assembly ?? 
                                              throw new ArgumentNullException(nameof(Assembly));

    public MetadataLoadContext Context { get; init; } = Context ?? 
                                                        throw new ArgumentNullException(nameof(Context));

    public void Dispose() => Context.Dispose();
}
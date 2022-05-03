using System.Runtime.Serialization;

namespace Doublestop.Tpc.Plugins.Installing;

[Serializable]
public class PluginAssemblyNotFoundException : InstallationException
{
    #region Constructors

    public PluginAssemblyNotFoundException()
    {
    }

    public PluginAssemblyNotFoundException(string message) : base(message)
    {
    }

    public PluginAssemblyNotFoundException(string message, Exception inner) : base(message, inner)
    {
    }

    protected PluginAssemblyNotFoundException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }

    #endregion
}
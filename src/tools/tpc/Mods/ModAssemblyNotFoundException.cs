using System.Runtime.Serialization;

namespace Doublestop.Tpc.Mods;

[Serializable]
public class ModAssemblyNotFoundException : InstallationException
{
    #region Constructors

    public ModAssemblyNotFoundException()
    {
    }

    public ModAssemblyNotFoundException(string message) : base(message)
    {
    }

    public ModAssemblyNotFoundException(string message, Exception inner) : base(message, inner)
    {
    }

    protected ModAssemblyNotFoundException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }

    #endregion
}
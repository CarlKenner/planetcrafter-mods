using System.Runtime.Serialization;

namespace Doublestop.Tpc.Mods;

[Serializable]
public class InstallationException : ApplicationException
{
    #region Constructors

    public InstallationException()
    {
    }

    public InstallationException(string message) : base(message)
    {
    }

    public InstallationException(string message, Exception inner) : base(message, inner)
    {
    }

    protected InstallationException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }

    #endregion
}
using System.Runtime.Serialization;

namespace Thangs.Tpc.Mods;

[Serializable]
public class TooManyModAssembiesFoundException : InstallationException
{
    #region Constructors

    public TooManyModAssembiesFoundException()
    {
    }

    public TooManyModAssembiesFoundException(string message) : base(message)
    {
    }

    public TooManyModAssembiesFoundException(string message, Exception inner) : base(message, inner)
    {
    }

    protected TooManyModAssembiesFoundException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }

    #endregion
}
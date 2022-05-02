using System.Runtime.Serialization;

namespace Doublestop.Tpc.Mods;

[Serializable]
public class TooManyModAssembliesFoundException : InstallationException
{
    #region Constructors

    public TooManyModAssembliesFoundException()
    {
    }

    public TooManyModAssembliesFoundException(string message) : base(message)
    {
    }

    public TooManyModAssembliesFoundException(string message, Exception inner) : base(message, inner)
    {
    }

    protected TooManyModAssembliesFoundException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }

    #endregion
}
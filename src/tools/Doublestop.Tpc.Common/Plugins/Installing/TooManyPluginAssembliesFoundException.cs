using System.Runtime.Serialization;

namespace Doublestop.Tpc.Plugins.Installing;

[Serializable]
public class TooManyPluginAssembliesFoundException : InstallationException
{
    #region Constructors

    public TooManyPluginAssembliesFoundException()
    {
    }

    public TooManyPluginAssembliesFoundException(string message) : base(message)
    {
    }

    public TooManyPluginAssembliesFoundException(string message, Exception inner) : base(message, inner)
    {
    }

    protected TooManyPluginAssembliesFoundException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }

    #endregion
}
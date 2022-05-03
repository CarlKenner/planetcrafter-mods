using System.Runtime.Serialization;

namespace Doublestop.Tpc.Commands;

internal enum ListOutputMode
{
    [EnumMember(Value = "table")]
    T = 0,

    [EnumMember(Value = "details")]
    D,

    [EnumMember(Value = "names")]
    N,

    [EnumMember(Value = "filenames")]
    F,

    [EnumMember(Value = "all")]
    A
}
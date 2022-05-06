using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using BepInEx;
using Doublestop.Tpc.Plugins.Metadata;

namespace Doublestop.Tpc.Internal;

public static class BepInAttributes
{
    #region Public Methods

    internal static bool TryReadPluginMetadata(Type type, [NotNullWhen(true)] out PluginMetadata? metadata)
    {
        metadata = ReadPluginMetadata(type);
        return metadata is not null;
    }

    internal static PluginMetadata? ReadPluginMetadata(Type type)
    {
        var customAttributes = CustomAttributeData.GetCustomAttributes(type);
        PluginInfoModel? info = null;
        var deps = new List<PluginDependencyModel>();

        foreach (var attribute in customAttributes)
        {
            if (info is null && attribute.TryReadPluginInfo(out info))
                continue;

            if (attribute.TryReadPluginDependency(out var dependency))
                deps.Add(dependency);
        }

        if (info is null)
            return null;

        return new PluginMetadata(Path.GetFileName(type.Assembly.Location), info, deps);
    }

    internal static string? ValueAsString(this CustomAttributeTypedArgument arg) 
        => Convert.ToString(arg.Value);

    internal static bool TryReadPluginInfo(this CustomAttributeData attribute,
        [NotNullWhen(true)] out PluginInfoModel? info)
    {
        info = ReadPluginInfo(attribute);
        return info is not null;
    }

    internal static PluginInfoModel? ReadPluginInfo(this CustomAttributeData attribute)
    {
        if (!IsAttribute<BepInPlugin>(attribute))
            // not a [BepInPlugin] attribute
            return null;

        // pull the args to make the code a bit more readable
        const int expectedArgCount = 3;
        var args = attribute.ConstructorArguments;

        if (args.Count != expectedArgCount || args.Any(ca => !IsStringType(ca.ArgumentType)))
            // expecting the (guid, name, version) constructor params
            return null;

        // guid (eg "dudes_freakishly_cool_mod")
        // guids are regex restricted to a-z, A-Z, 0-9, -, _
        var guid = args[0].ValueAsString();
        if (string.IsNullOrWhiteSpace(guid))
            // bad metadata? guid should never be empty.
            // we can (might) get away with having only the guid,
            // but we cannot move forward without it.
            return null;

        // name (eg "dude's freakishly cool mod")
        var name = args[1].ValueAsString() ?? "";
        
        // version (eg 1.2.3.4)
        var version = args[2].ValueAsString() ?? "";

        return new PluginInfoModel(new PluginGuid(guid), name, version);
    }

    internal static bool TryReadPluginDependency(this CustomAttributeData attribute,
        [NotNullWhen(true)] out PluginDependencyModel? dependency)
    {
        dependency = ReadPluginDependency(attribute);
        return dependency is not null;
    }

    internal static PluginDependencyModel? ReadPluginDependency(this CustomAttributeData attribute)
    {
        if (!IsAttribute<BepInDependency>(attribute) ||
            !attribute.ConstructorArguments.Any())
        {
            // not the deps attribute, or there was a problem reading metadata.
            // the ctor args list should never be empty.
            return null;
        }

        /*
        ctor 1: BepInDependency(string DependencyGUID, BepInDependency.DependencyFlags Flags = BepInDependency.DependencyFlags.HardDependency)
        ctor 2: BepInDependency(string DependencyGUID, string MinimumDependencyVersion)
         */

        // arg 1: dependency's guid
        // arg 2: string (min ver) or BepInDependency.DependencyFlags (hard=1, soft=2)
        //      - arg 2 may not be present, in which case it is implied to be "hard dependency"

        var guidArg = attribute.ConstructorArguments.First();

        // first arg: plugin guid
        var guid = guidArg.Value?.ToString();
        if (string.IsNullOrWhiteSpace(guid))
        {
            // bad metadata? guid should not be empty.
            return null;
        }

        // second arg: <null>, string, or deps flags
        var versionOrFlagsArg = attribute.ConstructorArguments.Count > 1
            ? attribute.ConstructorArguments[1]
            : (CustomAttributeTypedArgument?)null;

        // if second arg is null, we're done.
        // use the depinfo ctor that will automatically set the hard/soft flags
        if (versionOrFlagsArg?.Value is null)
            return new PluginDependencyModel(guid);

        var versionOrFlagsValue = versionOrFlagsArg.Value.Value?.ToString();

        // if second arg is a string, it's ctor #2, the min version overload
        if (IsStringType(versionOrFlagsArg.Value.ArgumentType))
            return new PluginDependencyModel(guid, versionOrFlagsValue);

        // arg 2 is dep flags, which probably is the enum value's string representation,
        // but might instead look like an integer. try both. enum values can't be digits-only,
        // so try the integer route first.
        if (int.TryParse(versionOrFlagsValue, out var flags))
            return new PluginDependencyModel(guid, flags);

        // try arg 2 as an enum
        if (Enum.TryParse<BepInDependency.DependencyFlags>(versionOrFlagsValue, out var depsFlags))
            return new PluginDependencyModel(guid, (int)depsFlags);

        // whatever arg 2 is, we couldn't handle it.
        // we'll assume ctor #1, default deps flags value.
        return new PluginDependencyModel(guid);
    }

    internal static bool IsStringType(Type argumentType) => string.Equals(argumentType.FullName, typeof(string).FullName);

    #endregion

    #region Private Methods

    static bool IsAttribute<T>(CustomAttributeData attribute) where T : Attribute =>
        IsAttribute(attribute, typeof(T).FullName ?? typeof(T).Name);

    static bool IsAttribute(CustomAttributeData attribute, string typeFullName) => string.Equals(
        attribute.AttributeType.FullName,
        typeFullName);

    #endregion
}
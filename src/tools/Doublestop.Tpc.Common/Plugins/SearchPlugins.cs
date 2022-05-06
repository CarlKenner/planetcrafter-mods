using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Doublestop.Tpc.Internal;
using Doublestop.Tpc.Plugins.Metadata;

namespace Doublestop.Tpc.Plugins;

/// <summary>
/// Idea for building plugin searches dynamically. Half baked.
/// </summary>
public sealed class SearchPlugins
{
    #region Fields

    public static readonly SearchPlugins Instance = new();

    #endregion

    #region Public Methods

    public IEnumerable<PluginMetadata> Search(
        IEnumerable<PluginAssembly> pluginAssemblies,
        IReadOnlyList<string> terms,
        bool useRegex,
        bool matchAll)
    {
        var plugins = pluginAssemblies
            .Where(GetAssemblyPredictes().JoinOr().Compile())
            .SelectMany(a => a.Plugins)
            .Where(GetPluginPredicates().JoinOr().Compile());

        return plugins;

        IEnumerable<Expression<Func<PluginAssembly, bool>>> GetAssemblyPredictes()
        {
            if (terms.Any())
                yield return Where.AssemblyNameMatches(terms, useRegex, matchAll);
        }

        IEnumerable<Expression<Func<PluginMetadata, bool>>> GetPluginPredicates()
        {
            if (terms.Any())
            {
                yield return Where.GuidMatches(terms, useRegex, matchAll);
                yield return Where.NameMatches(terms, useRegex, matchAll);
            }
        }
    }

    #endregion

    #region Nested Types

    static class Where
    {
        #region Public Methods

        internal static Expression<Func<PluginAssembly, bool>> AssemblyNameMatches(
            IEnumerable<string> patterns,
            bool useRegex,
            bool matchAll)
        {
            return MatchStrings<PluginAssembly>(a => a.Name, patterns, useRegex, matchAll);
        }

        internal static Expression<Func<PluginMetadata, bool>> GuidMatches(
            IEnumerable<string> terms,
            bool useRegex,
            bool matchAll)
        {
            return MatchStrings<PluginMetadata>(t => t.Guid.Value, terms, useRegex, matchAll);
        }

        internal static Expression<Func<PluginMetadata, bool>> NameMatches(
            IEnumerable<string> terms,
            bool useRegex,
            bool matchAll)
        {
            return MatchStrings<PluginMetadata>(t => t.Name, terms, useRegex, matchAll);
        }

        #endregion

        #region Private Methods

        static Expression<Func<T, bool>> MatchStrings<T>(
            Expression<Func<T, string>> prop,
            IEnumerable<string> terms,
            bool useRegex,
            bool matchAll)
        {
            return useRegex 
                ? RegexMatches(prop, terms.Select(s => new Regex(s, RegexOptions.IgnoreCase)), matchAll)
                : PatternMatches(prop, terms, new StringPatternMatcher(true), matchAll);
        }

        static Expression<Func<T, bool>> PatternMatches<T>(Expression<Func<T, string>> prop,
            IEnumerable<string> patterns,
            StringPatternMatcher matcher,
            bool matchAll)
        {
            return matchAll
                ? plugin => patterns.All(s => matcher.IsMatch(prop.Compile()(plugin), s))
                : plugin => patterns.Any(s => matcher.IsMatch(prop.Compile()(plugin), s));
        }

        static Expression<Func<T, bool>> RegexMatches<T>(Expression<Func<T, string>> prop,
            IEnumerable<Regex> patterns,
            bool matchAll)
        {
            return matchAll
                ? plugin => patterns.All(s => s.IsMatch(prop.Compile()(plugin)))
                : plugin => patterns.Any(s => s.IsMatch(prop.Compile()(plugin)));
        }

        #endregion
    }

    #endregion
}
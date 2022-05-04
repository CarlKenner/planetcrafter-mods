using Pluralize.NET;

namespace Doublestop.Tpc.Internal;

public static class Strings
{
    static readonly Pluralizer DefaultPluralizer = new();

    public static string Pluralize(int count, string singular, string plural) =>
        count == 1 ? singular : plural;

    public static string Pluralize(this string word, int count) => DefaultPluralizer.Format(word, count);
}


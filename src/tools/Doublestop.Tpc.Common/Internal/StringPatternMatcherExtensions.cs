namespace Doublestop.Tpc.Internal
{
    internal static class StringPatternMatcherExtensions
    {
        #region Public Methods

        public static bool MatchesPattern(this string str, string pattern)
        {
            return MatchesPattern(str, pattern, -1, -1);
        }

        public static bool MatchesPattern(this string str, string pattern, int stringLen, int patternLen)
        {
            return StringPatternMatcher.Instance.IsMatch(str, pattern, stringLen, patternLen);
        }

        #endregion
    }
}
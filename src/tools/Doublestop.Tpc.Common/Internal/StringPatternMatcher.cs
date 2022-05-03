namespace Doublestop.Tpc.Internal
{
    internal sealed class StringPatternMatcher
    {
        #region Fields

        public static readonly StringPatternMatcher Instance = new StringPatternMatcher();

        readonly Func<char, char, bool> _charComparer;

        #endregion

        #region Constructors

        public StringPatternMatcher(bool ignoreCase = false)
        {
            _charComparer = (a, b) =>
                a == b || ignoreCase && char.ToUpper(a) == char.ToUpper(b);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Matches input str with given wildcard pattern.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="pattern"></param>
        /// <param name="stringLen"></param>
        /// <param name="patternLen"></param>
        /// <returns></returns>
        public bool IsMatch(string str, string pattern, int stringLen = -1, int patternLen = -1)
        {
            if (stringLen < 0)
                stringLen = str.Length;
            if (patternLen < 0)
                patternLen = pattern.Length;

            // empty pattern can only match with
            // empty string
            if (patternLen == 0)
                return stringLen == 0;

            // lookup table for storing results of
            // subproblems
            bool[,] lookup = new bool[stringLen + 1, patternLen + 1];

            // initailze lookup table to false
            for (var i = 0; i < stringLen + 1; i++)
            for (var j = 0; j < patternLen + 1; j++)
                lookup[i, j] = false;

            // empty pattern can match with
            // empty string
            lookup[0, 0] = true;

            // Only '*' can match with empty string
            for (var j = 1; j <= patternLen; j++)
                if (pattern[j - 1] == '*')
                    lookup[0, j] = lookup[0, j - 1];

            // fill the table in bottom-up fashion
            for (var i = 1; i <= stringLen; i++)
            {
                for (var j = 1; j <= patternLen; j++)
                {
                    // Two cases if we see a '*'
                    // a) We ignore '*'' character and move
                    // to next character in the pattern,
                    //     i.e., '*' indicates an empty
                    //     sequence.
                    // b) '*' character matches with ith
                    //     character in input
                    if (pattern[j - 1] == '*')
                        lookup[i, j] = lookup[i, j - 1]
                                       || lookup[i - 1, j];

                    // Current characters are considered as
                    // matching in two cases
                    // (a) current character of pattern is '?'
                    // (b) characters actually match
                    else if (pattern[j - 1] == '?'
                             || _charComparer(str[i - 1], pattern[j - 1]))
                        lookup[i, j] = lookup[i - 1, j - 1];

                    // If characters don't match
                    else
                        lookup[i, j] = false;
                }
            }
            return lookup[stringLen, patternLen];
        }

        #endregion
    }
}
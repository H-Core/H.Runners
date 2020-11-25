using System;
using System.Collections.Generic;
using System.Linq;

namespace H.Runners
{
    public static class TextUtilities
    {
        public static int LevenshteinDistance(string string1, string string2)
        {
            if (string1 == null) throw new ArgumentNullException(nameof(string1));
            if (string2 == null) throw new ArgumentNullException(nameof(string2));

            var m = new int[string1.Length + 1, string2.Length + 1];

            for (var i = 0; i <= string1.Length; i++) { m[i, 0] = i; }
            for (var j = 0; j <= string2.Length; j++) { m[0, j] = j; }

            for (var i = 1; i <= string1.Length; i++)
            {
                for (var j = 1; j <= string2.Length; j++)
                {
                    var diff = string1[i - 1] == string2[j - 1] ? 0 : 1;

                    m[i, j] = Math.Min(Math.Min(m[i - 1, j] + 1,
                            m[i, j - 1] + 1),
                        m[i - 1, j - 1] + diff);
                }
            }

            return m[string1.Length, string2.Length];
        }

        public static int MinimalLevenshteinDistance(string string1, string string2)
        {
            var bigString = string1.Length > string2.Length ? string1 : string2;
            var smallString = string1.Length > string2.Length ? string2 : string1;

            var list = new List<int>();
            var count = bigString.Length - smallString.Length + 1;
            for (var i = 0; i < count; ++i)
            {
                var subString = bigString.Substring(i, smallString.Length);

                list.Add(LevenshteinDistance(subString, smallString));
            }

            return list.OrderBy(i => i).ToList().FirstOrDefault();
        }
    }
}

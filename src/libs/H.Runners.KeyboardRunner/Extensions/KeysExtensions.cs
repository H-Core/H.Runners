using System;
using System.Collections.Generic;
using System.Linq;
using H.Core;

namespace H.Runners.Extensions
{
    internal static class KeysExtensions
    {
        public static string ToSendKeys(this Keys keys)
        {
            keys = keys ?? throw new ArgumentNullException(nameof(keys));

            var modifiersDictionary = new Dictionary<Key, string>
            {
                { Key.Shift, "+" },
                { Key.LShift, "+" },
                { Key.RShift, "+" },
                { Key.Ctrl, "^" },
                { Key.LCtrl, "^" },
                { Key.RCtrl, "^" },
                { Key.Alt, "%" },
                { Key.LAlt, "%" },
                { Key.RAlt, "%" },
            };
            var modifiers = string.Concat(keys.Values
                .Where(key => modifiersDictionary.ContainsKey(key))
                .Distinct()
                .Select(key => modifiersDictionary[key]));
            var values = string.Concat(keys.Values
                .Where(key => !modifiersDictionary.ContainsKey(key))
                .Select(key => key switch
                {
                    >= Key.D1 and <= Key.D9 => $"{{{$"{key}".TrimStart('D')}}}",
                    _ => $"{{{key}}}",
                }));

            return $"{modifiers}({values})";
        }
    }
}

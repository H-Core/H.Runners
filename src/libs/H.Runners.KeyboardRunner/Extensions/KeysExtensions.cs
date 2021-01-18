using System;
using System.Linq;
using H.Core;

namespace H.Runners.Extensions
{
    internal static class KeysExtensions
    {
        public static string ToSendKeys(this Keys keys)
        {
            keys = keys ?? throw new ArgumentNullException(nameof(keys));

            var modifiers = string.Concat(keys.Values.Select(key => key switch
            {
                Key.Shift or Key.LShift or Key.RShift => "+",
                Key.Ctrl or Key.LCtrl or Key.RCtrl => "^",
                Key.Alt or Key.LAlt or Key.RAlt => "%",
                _ => string.Empty,
            }));
            var values = string.Concat(keys.Values
                .Select(key => key switch
                {
                    >= Key.D1 and <= Key.D9 => $"{{{$"{key}".TrimStart('D')}}}",
                    _ => $"{{{key}}}",
                }));

            return $"{modifiers}({values})";
        }
    }
}

using System.Collections.Generic;
using System.Linq;

namespace H.Runners
{
    public static class Transliterator
    {
        public enum TranslateType
        {
            Gost,
            Iso
        }

        private static readonly IReadOnlyDictionary<char, string> GostDictionary = new Dictionary<char, string>
        {
            ['Є'] = "EH", ['І'] = "I", ['і'] = "i", ['№'] = "#", ['є'] = "eh",
            ['А'] = "A", ['Б'] = "B", ['В'] = "V", ['Г'] = "G", ['Д'] = "D",
            ['Е'] = "E", ['Ё'] = "JO", ['Ж'] = "ZH", ['З'] = "Z", ['И'] = "I",
            ['Й'] = "JJ", ['К'] = "K", ['Л'] = "L", ['М'] = "M", ['Н'] = "N",
            ['О'] = "O", ['П'] = "P", ['Р'] = "R", ['С'] = "S", ['Т'] = "T",
            ['У'] = "U", ['Ф'] = "F", ['Х'] = "KH", ['Ц'] = "C", ['Ч'] = "CH",
            ['Ш'] = "SH", ['Щ'] = "SHH", ['Ъ'] = "'", ['ъ'] = "", ['Ы'] = "Y", ['Ь'] = "",
            ['Э'] = "EH", ['Ю'] = "YU", ['Я'] = "YA",
            ['«'] = "", ['»'] = "", ['—'] = "-"//, [' '] = "-"
        };
        
        private static readonly IReadOnlyDictionary<char, string> IsoDictionary = new Dictionary<char, string>
        {
            ['Є'] = "YE", ['І'] = "I", ['Ѓ'] = "G", ['і'] = "i", ['№'] = "#", ['є'] = "ye", ['ѓ'] = "g",
            ['А'] = "A", ['Б'] = "B", ['В'] = "V", ['Г'] = "G", ['Д'] = "D",
            ['Е'] = "E", ['Ё'] = "YO", ['Ж'] = "ZH", ['З'] = "Z", ['И'] = "I",
            ['Й'] = "J", ['К'] = "K", ['Л'] = "L", ['М'] = "M", ['Н'] = "N",
            ['О'] = "O", ['П'] = "P", ['Р'] = "R", ['С'] = "S", ['Т'] = "T",
            ['У'] = "U", ['Ф'] = "F", ['Х'] = "X", ['Ц'] = "C", ['Ч'] = "CH",
            ['Ш'] = "SH", ['Щ'] = "SHH", ['Ъ'] = "'", ['Ы'] = "Y", ['Ь'] = "",
            ['Э'] = "E", ['Ю'] = "YU", ['Я'] = "YA",
            ['«'] = "", ['»'] = "", ['—'] = "-"//, [' '] = "-"
        };

        public static string Convert(string text, TranslateType type)
        {
            var dictionary = type == TranslateType.Gost ? GostDictionary : IsoDictionary;

            return new string(text.Select(c =>
            {
                var str = c.ToString();
                var isLower = str.ToLower() == str;
                var key = dictionary.ContainsKey(c) ? c : str.ToUpper()[0];
                var newString = dictionary.ContainsKey(key) ? dictionary[key] : str;

                return isLower ? newString.ToLower() : newString;
            }).SelectMany(i => i).ToArray());
        }
    }
}

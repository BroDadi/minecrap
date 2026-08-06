using OpenTK.Mathematics;

namespace minecrap.gui
{
    internal static class FontData
    {
        private static string[] charset =
        {
            " ☺☻♥♦♣♠•◘○◙♂♀♪♫☼",
            "►◄↕‼¶§▬†↑↓→←∟↔▲▼",
            " !\"#$%&'()*+,-./",
            "0123456789:;<=>?",
            "@ABCDEFGHIJKLMNO",
            "PQRSTUVWXYZ[\\]^_",
            "`abcdefghijklmno",
            "pqrstuvwxyz{|}~⌂",
            "АБВГДЕЖЗИЙКЛМНОП",
            "РСТУФХЦЧШЩЪЫЬЭЮЯ",
            "абвгдежзийклмноп",
            "░▒▓│┤╡╢╖╕╣║╗╝╜╛┐",
            "└┴┬├─┼╞╟╚╔╩╦╠═╬╧",
            "╨╤╥╙╘╒╓╫╪┘┌█▄▌▐▀",
            "рстуфхцчшщъыьэюя",
            "ЁёЄєЇїЎў°∙·√№¤■"
        };

        public static readonly Dictionary<char, Vector2[]> coordsByChar = GetCharDict();

        private static Dictionary<char, Vector2[]> GetCharDict()
        {
            Dictionary<char, Vector2[]> result = new();
            
            for (int y = 0; y < charset.Length; y++)
            {
                for (int x = 0; x < charset[y].Length; x++)
                {
                    char c = charset[y][x];
                    if (!result.ContainsKey(c))
                    {
                        Vector2[] arr =
                        [
                            new Vector2(x / 16f, (16 - y) / 16f),
                            new Vector2((x + 1) / 16f, (16 - y) / 16f),
                            new Vector2((x + 1) / 16f, (16 - y - 1) / 16f),
                            new Vector2(x / 16f, (16 - y - 1) / 16f),
                        ];
                        result[c] = arr;
                    }
                }
            }
            return result;
        }
    }
}
namespace PacMan
{
    using System;
    using System.Collections.Generic;
    static class Numbers
    {
        public static string[] Zero = new string[5]
        {
            " ████ ",
            "█    █",
            "█    █",
            "█    █",
            " ████ "
        };
        public static string[] One = new string[5]
        {
            "  ██  ",
            " ███  ",
            "  ██  ",
            "  ██  ",
            " ████ "
        };
        public static string[] Two = new string[5]
        {
            " ████ ",
            "█    █",
            "   ██ ",
            " ██   ",
            "██████"
        };
        public static string[] Three = new string[5]
        {
            " ████ ",
            "█    █",
            "   ██ ",
            "█    █",
            " ████ "
        };
        public static string[] Four = new string[5]
        {
            "   ██ ",
            "  █ █ ",
            " █  █ ",
            "██████",
            "    █ "
        };
        public static string[] Five = new string[5]
        {
            "██████",
            "█     ",
            "█████ ",
            "     █",
            "█████ "
        };
        public static string[] Six = new string[5]
        {
            " ████ ",
            "█     ",
            "█████ ",
            "█    █",
            " ████ "
        };
        public static string[] Seven = new string[5]
        {
            "██████",
            "    ██",
            "    █ ",
            "   █  ",
            "  ██  "
        };
        public static string[] Eight = new string[5]
        {
            " ████ ",
            "█    █",
            " ████ ",
            "█    █",
            " ████ "
        };
        public static string[] Nine = new string[5]
        {
            " ████ ",
            "█    █",
            " █████",
            "     █",
            " ████ "
        };

        public static Dictionary<char, string[]> numberset = new Dictionary<char, string[]>()
        {
            { '0', Zero },
            { '1', One },
            { '2', Two },
            { '3', Three },
            { '4', Four },
            { '5', Five },
            { '6', Six },
            { '7', Seven },
            { '8', Eight },
            { '9', Nine },
        };

        public static void PrintDigit(string[] digit)
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine(digit[i]);
            }
        }
    }
}

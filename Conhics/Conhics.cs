using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;

namespace Conhics {
    public class Window {
        private static SafeFileHandle s_handle;
        private static Integration.CharInfo[] s_virtualWin;
        private static Integration.SmallRect s_rect;
        private static int s_width;
        private static int s_height;

        public static void Setup() {
            s_width = Console.WindowWidth;
            s_height = Console.WindowHeight;

            s_handle = Integration.CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

            if (s_handle.IsInvalid)
                throw new Exception("Could not start...");

            s_virtualWin = new Integration.CharInfo[s_width * s_height];
            s_rect = new Integration.SmallRect() { Left = 0, Top = 0, Right = Convert.ToInt16(s_width), Bottom = Convert.ToInt16(s_height) };

            Clear();
        }

        public static string Input(string displayText, int x, int y, bool enforceInput) {
            Console.SetCursorPosition(x, y);
            Console.Write(displayText + ": ");
            int offsetX = displayText.Length + 2;
            var data = "";
            var typeLength = 0;
            while (true) {
                var key = Console.ReadKey(true);

                switch (key.Key) {
                    case ConsoleKey.Spacebar:
                        if (x + offsetX + typeLength >= Console.WindowWidth - 1)
                            continue;
                        data += " ";
                        typeLength++;
                        break;
                    case ConsoleKey.Enter when enforceInput && data.Length <= 0:
                        break;
                    case ConsoleKey.Enter:
                        return data;
                    case ConsoleKey.Backspace when typeLength > 0:
                        data = data.Substring(0, data.Length - 1);
                        typeLength--;
                        Console.SetCursorPosition(x + offsetX + typeLength, y);
                        Console.Write(' ');
                        break;
                    case ConsoleKey.Escape:
                        break;
                }
                Console.SetCursorPosition(x + offsetX + typeLength, y);
                if (char.IsDigit(key.KeyChar) || char.IsLetter(key.KeyChar)) {

                    if (x + offsetX + typeLength >= Console.WindowWidth - 1)
                        continue;

                    data += key.KeyChar;
                    Console.Write(key.KeyChar.ToString());
                    typeLength++;
                }
            }
        }

        public static char GetPos(int x, int y) {
            return s_virtualWin[y * s_width + x].Char.UnicodeChar;
        }

        public static void Print(char character, int x, int y, short attribute = 7) {
            if (s_virtualWin[y * x + x].Char.UnicodeChar.Equals(character)) return;
            s_virtualWin[y * s_width + x].Attributes = attribute;
            s_virtualWin[y * s_width + x].Char.UnicodeChar = character;
        }

        public static void Print(string text, int x, int y, short attribute = 7) {
            if (x >= s_width)
                throw new Exception("Out of bounds");
            if (x + text.Length > s_width)
                text = text.Substring(0, text.Length - (x + text.Length - s_width));

            for (var i = 0; i < text.Length; i++)
                s_virtualWin[y * s_width + x + i].Char.UnicodeChar = text[i];
        }

        public static bool Flush() {
            return Integration.WriteConsoleOutputW(s_handle, s_virtualWin,
                new Integration.Coord() { X = Convert.ToInt16(s_width), Y = Convert.ToInt16(s_height) },
                new Integration.Coord() { X = 0, Y = 0 },
                ref s_rect);
        }

        public static void Clear() {
            for (var i = 0; i < s_virtualWin.Length; i++) {
                s_virtualWin[i].Attributes = 7;
                s_virtualWin[i].Char.UnicodeChar = ' ';
            }
        }
    }
}

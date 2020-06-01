using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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
        private static ConsoleKeyInfo? s_lastKey;
        private static bool s_activeEventCapture;

        public static void Setup(string title = "", int columns = 120, int rows = 30, short charWidth = 8, short charHeight = 16, bool activeEventCapture = true) {
            s_activeEventCapture = activeEventCapture;

            if (title.Length > 0)
                Console.Title = title;

            SetupWindowAndFontSize(columns, rows, charWidth, charHeight);

            Console.CursorVisible = false;

            s_width = Console.WindowWidth;
            s_height = Console.WindowHeight;

            s_handle = Integration.CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

            if (s_handle.IsInvalid)
                throw new Exception("Could not start...");

            s_virtualWin = new Integration.CharInfo[s_width * s_height];
            s_rect = new Integration.SmallRect() { Left = 0, Top = 0, Right = Convert.ToInt16(s_width), Bottom = Convert.ToInt16(s_height) };

            // Start event handler
            if (activeEventCapture)
                new Thread(EventCatcher).Start();

            Clear();
        }

        public static void SetupWindowAndFontSize(int columns, int rows, short charPixelWidth, short charPixelHeight) {
            // Setting the font and fontsize
            // Other values can be changed too

            // Instantiating CONSOLE_FONT_INFO_EX and setting its size (the function will fail otherwise)
            Integration.CONSOLE_FONT_INFO_EX ConsoleFontInfo = new Integration.CONSOLE_FONT_INFO_EX();
            ConsoleFontInfo.cbSize = (uint)Marshal.SizeOf(ConsoleFontInfo);

            // Optional, implementing this will keep the fontweight and fontsize from changing
            // See notes
            // GetCurrentConsoleFontEx(GetStdHandle(StdHandle.OutputHandle), false, ref ConsoleFontInfo);
            ConsoleFontInfo.FontFamily = 3000;
            ConsoleFontInfo.dwFontSize.X = (short)(charPixelWidth + 1);
            ConsoleFontInfo.dwFontSize.Y = charPixelHeight;

            if (Integration.SetCurrentConsoleFontEx(Integration.GetStdHandle((int) Integration.StdHandle.OutputHandle),
                false, ref ConsoleFontInfo)) {
                Console.SetWindowSize(columns, rows);
                Console.SetBufferSize(columns, rows);
                Console.SetWindowPosition(0, 0);
            }
        }

        private static void EventCatcher() {
            while (true) {
                s_lastKey = Console.ReadKey(true);
            }
        }

        public static ConsoleKeyInfo? GetLastKey(bool autoClear = true) {
            if (!s_activeEventCapture)
                throw new Exception("This feature has been disabled in setup");

            var outgoing = s_lastKey;
            if (autoClear)
                s_lastKey = null;
            return outgoing;
        }

        public static string Input(string displayText, int x, int y, bool enforceInput) {
            if (!s_activeEventCapture)
                throw new Exception("This feature has been disabled in setup");

            Console.CursorVisible = true;

            Console.SetCursorPosition(x, y);
            Console.Write(displayText + ": ");
            int offsetX = displayText.Length + 2;
            var data = "";
            var typeLength = 0;
            while (true) {
                var result = GetLastKey(false);
                if (result == null)
                    continue;
                var key = result.Value;
                // Manually reset last key
                s_lastKey = null;

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
                        Console.CursorVisible = false;
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

        public static void Print(char character, int x, int y, ConsoleColor color = ConsoleColor.White) {
            if (s_virtualWin[y * s_width + x].Char.UnicodeChar.Equals(character)) return;
            s_virtualWin[y * s_width + x].Attributes = (short)color;
            s_virtualWin[y * s_width + x].Char.UnicodeChar = character;
        }

        public static void Print(string text, int x, int y, ConsoleColor color = ConsoleColor.White) {
            if (x >= s_width)
                throw new Exception("Out of bounds");
            if (x + text.Length > s_width)
                text = text.Substring(0, text.Length - (x + text.Length - s_width));

            for (var i = 0; i < text.Length; i++) {
                s_virtualWin[y * s_width + x + i].Attributes = (short)color;
                s_virtualWin[y * s_width + x + i].Char.UnicodeChar = text[i];
            }
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

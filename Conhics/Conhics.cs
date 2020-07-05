using System;
using System.IO;
using System.Runtime.InteropServices;

using Microsoft.Win32.SafeHandles;

using Conhics.Input;

namespace Conhics
{
    public class Window {
        private static SafeFileHandle s_handle;
        private static Integration.CharInfo[] s_virtualWin;
        private static Integration.SmallRect s_rect;
        private static int s_width;
        private static int s_height;
        private static ConsoleKeyInfo? s_lastKey;
        private static bool s_activeEventCapture;
        private static bool s_taskRunning;

        public static void Setup(string title = "", int columns = 120, int rows = 30, short charWidth = 8, short charHeight = 16, bool activeEventCapture = true) {
            s_activeEventCapture = activeEventCapture;

            if (title.Length > 0)
                Console.Title = title;

            if (Console.LargestWindowWidth < columns || Console.LargestWindowHeight < rows)
                throw new Exception("Requested window size exceeds the screen capacity");

            SetupFontSize(charWidth, charHeight);
            SetConsoleDimension(columns, rows);
            Console.SetWindowPosition(0, 0);

            Console.CursorVisible = false;

            s_width = Console.WindowWidth;
            s_height = Console.WindowHeight;

            s_handle = Integration.CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

            if (s_handle.IsInvalid)
                throw new Exception("Could not start...");

            s_virtualWin = new Integration.CharInfo[s_width * s_height];
            s_rect = new Integration.SmallRect() { Left = 0, Top = 0, Right = Convert.ToInt16(s_width), Bottom = Convert.ToInt16(s_height) };

            // Start event handler
<<<<<<< Updated upstream
            if (activeEventCapture) {
                s_taskRunning = true;
                new Task(EventCatcher).Start();
            }
=======
            // if (activeEventCapture)
            //     new Thread(EventCatcher).Start();
            Keyboard.IsEnabled = activeEventCapture;
>>>>>>> Stashed changes

            Clear();
        }

        private static int UpdateConsoleSize(int width, int height) {

            s_virtualWin = new Integration.CharInfo[width * height];
            s_rect = new Integration.SmallRect() { Left = 0, Top = 0, Right = Convert.ToInt16(width), Bottom = Convert.ToInt16(height) };
            Console.CursorVisible = false;

            s_width = width;
            s_height = height;

            return SetConsoleDimension(width, height);
        }

        private static int SetConsoleDimension(int width, int height) {

            try {
                Console.SetWindowSize(width, height);
                Console.SetBufferSize(width, height);
                Console.SetWindowPosition(0, 0);
            }
            catch (IOException) {
                return 1;
            }
            catch (ArgumentOutOfRangeException) {
                return 1;
            }

            return 0;
        }

        public static void CleanUp() {
            s_taskRunning = false;
            Clear();
            Flush();
        }

        private static void SetupFontSize(short charPixelWidth, short charPixelHeight) {
            // Instantiating CONSOLE_FONT_INFO_EX and setting its size (the function will fail otherwise)
            Integration.CONSOLE_FONT_INFO_EX consoleFontInfo = new Integration.CONSOLE_FONT_INFO_EX();
            consoleFontInfo.cbSize = (uint)Marshal.SizeOf(consoleFontInfo);

            consoleFontInfo.FontFamily = 3000;
            consoleFontInfo.dwFontSize.X = (short)(charPixelWidth + 1);
            consoleFontInfo.dwFontSize.Y = charPixelHeight;

            if (!Integration.SetCurrentConsoleFontEx(Integration.GetStdHandle((int) Integration.StdHandle.OutputHandle),
                false, ref consoleFontInfo)) return;
        }

        private static bool SizeHasUpdated() {
            return Console.WindowWidth != s_width || Console.WindowHeight != s_height || 
                   Console.BufferWidth != s_width || Console.BufferHeight != s_height;
        }

        // ### Keyboard input is now retrieved from Conhics.Input.Keyboard.Input as a Conhics.Input.KeyboardInput struct. ###

        // private static void EventCatcher() {
        //     while (s_taskRunning) {
        //         if (Console.KeyAvailable)
        //             s_lastKey = Console.ReadKey(true);
        //     }
        // }

        // public static ConsoleKeyInfo? GetLastKey(bool autoClear = true) {
        //     if (!s_activeEventCapture)
        //         throw new Exception("This feature has been disabled in setup");
        //
        //     var outgoing = s_lastKey;
        //     if (autoClear)
        //         s_lastKey = null;
        //     return outgoing;
        // }

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
            if (x >= s_width ||
                y >= s_height ||
                x < 0 || y < 0)
                throw new Exception("Out of bounds");

            // Don't print a character if it already exists
            if (s_virtualWin[y * s_width + x].Char.UnicodeChar.Equals(character)) return;

            s_virtualWin[y * s_width + x].Attributes = (short)color;
            s_virtualWin[y * s_width + x].Char.UnicodeChar = character;
        }

        public static void Print(string text, int x, int y, ConsoleColor color = ConsoleColor.White) {
            if (x >= s_width ||
                y >= s_height ||
                x < 0 || y < 0)
                throw new Exception("Out of bounds");

            // Crop away the text that's out of the screen
            if (x + text.Length > s_width)
                text = text.Substring(0, text.Length - (x + text.Length - s_width));

            for (var i = 0; i < text.Length; i++) {
                s_virtualWin[y * s_width + x + i].Attributes = (short)color;
                s_virtualWin[y * s_width + x + i].Char.UnicodeChar = text[i];
            }
        }

        public static bool UpdateDimensions() {
            if (Console.WindowTop != 0)
                Console.SetWindowPosition(0, 0);

            return SizeHasUpdated() && UpdateConsoleSize(Console.WindowWidth, Console.WindowHeight) == 1;
        }

        public static bool Flush() {
            UpdateDimensions();

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

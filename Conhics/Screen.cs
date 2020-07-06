namespace Conhics {
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using Conhics.Input;
    using Microsoft.Win32.SafeHandles;

    /**
     * <summary>
     * This is the core class of conhics.
     * The screen class is responsible for
     * direct updates to the console enabling
     * for massive performance boosts.
     * </summary>
     */
    public class Screen {
        private static SafeFileHandle s_handle;
        private static Integration.CharInfo[] s_virtualWin;
        private static Integration.SmallRect s_rect;
        private static int s_width;
        private static int s_height;
        private static bool s_activeEventCapture;

        /**
         * <summary>
         * Instantiate conhics, this will
         * apply correct console and
         * character dimensions, start
         * event handler (optional) and
         * finally apply a title to the console.
         * </summary>
         */
        public static void Setup(string title = "", int columns = 120, int rows = 30, short charWidth = 8, short charHeight = 16, bool activeEventCapture = true) {
            s_activeEventCapture = activeEventCapture;

            if (title.Length > 0)
                Console.Title = title;

            // Calculate how much larger/smaller the characters are than default
            int charWidthPercentage = 8 / charWidth;
            int charHeightPercentage = 16 / charHeight;

            if (Console.LargestWindowWidth * charWidthPercentage < columns ||
                Console.LargestWindowHeight * charHeightPercentage < rows)
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
            Keyboard.IsEnabled = activeEventCapture;
            Mouse.IsEnabled = activeEventCapture;

            Clear();
        }

        /** <summary>
         * Cleanup ends all running tasks for conhics and clears
         * the screen.
         * </summary>
         */
        public static void CleanUp() {
            Clear();
            Flush();
        }

        /**
         * <summary>
         * The input method is a bare-bone method
         * for capturing a string input instead
         * of a single event.
         * </summary>
         */
        public static string Input(string displayText, int x, int y, bool enforceInput) {
            if (!s_activeEventCapture)
                throw new Exception("This feature has been disabled in setup");

            Console.CursorVisible = true;

            Console.SetCursorPosition(x + displayText.Length, y);
            Print(displayText, x, y);
            Flush();
            int offsetX = displayText.Length;
            var data = string.Empty;
            var typeLength = 0;
            while (true) {
                if (!Keyboard.Input.HasValue)
                    continue;

                var keyPress = Keyboard.Input.Value;

                if (!keyPress.KeyDown)
                    continue;

                switch (keyPress.ConsoleKey) {
                    case ConsoleKey.Spacebar:
                        if (x + offsetX + typeLength >= Console.WindowWidth - 1)
                            continue;
                        data += " ";
                        typeLength++;
                        break;
                    case ConsoleKey.Enter when enforceInput && data.Length <= 0:
                    case ConsoleKey.Enter:
                        Console.CursorVisible = false;
                        return data;
                    case ConsoleKey.Backspace when typeLength > 0:
                        data = data.Substring(0, data.Length - 1);
                        typeLength--;
                        Console.SetCursorPosition(x + offsetX + typeLength, y);
                        Print(' ', x + offsetX + data.Length, y);
                        Flush();
                        break;
                    case ConsoleKey.Escape:
                        break;
                }

                if (char.IsDigit(keyPress.Character) || char.IsLetter(keyPress.Character)) {
                    if (x + offsetX + typeLength >= WinBuf.BufferWidth - 1)
                        continue;

                    data += keyPress.Character;
                    Print(keyPress.Character.ToString(), x + offsetX + data.Length - 1, y);
                    Flush();
                    typeLength++;
                }

                Console.SetCursorPosition(x + offsetX + typeLength, y);
            }
        }

        /**
         * <summary>
         * The the character at the specified
         * coordinate.
         * </summary>
         */
        public static char GetPos(int x, int y) {
            return s_virtualWin[y * s_width + x].Char.UnicodeChar;
        }

        /**
         * <summary>
         * Output a character to a specific position
         * in the console.
         * </summary>
         */
        public static void Print(char character, int x, int y, ConsoleColor color = ConsoleColor.White) {
            if (x >= s_width ||
                y >= s_height ||
                x < 0 || y < 0)
                throw new Exception("Out of bounds");

            // Don't print a character if it already exists
            if (s_virtualWin[y * s_width + x].Char.UnicodeChar.Equals(character))
                return;

            s_virtualWin[y * s_width + x].Attributes = (short)color;
            s_virtualWin[y * s_width + x].Char.UnicodeChar = character;
        }

        /**
         * <summary>
         * Output a string to a specific position
         * in the console, the specified coordinate
         * names the beginning of the string that
         * will be printed.
         * </summary>
         */
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

        /**
         * <summary>
         * Updates the internal dimensions of
         * the console.
         * </summary>
         */
        public static bool UpdateDimensions() {
            if (Console.WindowTop != 0)
                Console.SetWindowPosition(0, 0);

            return SizeHasUpdated() && UpdateConsoleSize(Console.WindowWidth, Console.WindowHeight) == 1;
        }

        /**
         * <summary>
         * Flush compiles the virtual console produced
         * by print commands and outputs it onto the
         * real console.
         * </summary>
         */
        public static bool Flush() {
            UpdateDimensions();

            return Integration.WriteConsoleOutputW(s_handle, s_virtualWin,
                new Integration.Coord() { X = Convert.ToInt16(s_width), Y = Convert.ToInt16(s_height) },
                new Integration.Coord() { X = 0, Y = 0 },
                ref s_rect);
        }

        /**
         * <summary>
         * Clears the virtual console. Calling
         * this method does NOT clear the console
         * itself, only the virtual one. Thus it
         * has to be followed with a Flush() to
         * apply.
         * </summary>
         */
        public static void Clear() {
            for (var i = 0; i < s_virtualWin.Length; i++) {
                s_virtualWin[i].Attributes = 7;
                s_virtualWin[i].Char.UnicodeChar = ' ';
            }
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

        private static void SetupFontSize(short charPixelWidth, short charPixelHeight) {
            // Instantiating CONSOLE_FONT_INFO_EX and setting its size (the function will fail otherwise)
            Integration.CONSOLE_FONT_INFO_EX consoleFontInfo = default(Integration.CONSOLE_FONT_INFO_EX);
            consoleFontInfo.cbSize = (uint)Marshal.SizeOf(consoleFontInfo);

            consoleFontInfo.FontFamily = 3000;
            consoleFontInfo.dwFontSize.X = charPixelWidth;
            consoleFontInfo.dwFontSize.Y = charPixelHeight;

            Integration.SetCurrentConsoleFontEx(Integration.GetStdHandle((int)Integration.StdHandle.OutputHandle), false, ref consoleFontInfo);
        }

        private static bool SizeHasUpdated() {
            return Console.WindowWidth != s_width || Console.WindowHeight != s_height || Console.BufferWidth != s_width || Console.BufferHeight != s_height;
        }
    }
}

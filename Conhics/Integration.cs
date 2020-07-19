[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Congui")]

namespace Conhics {
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Runtime.InteropServices;

    using Microsoft.Win32.SafeHandles;

    internal class Integration {
        [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern SafeFileHandle CreateFile(
            string fileName,
            [MarshalAs(UnmanagedType.U4)] uint fileAccess,
            [MarshalAs(UnmanagedType.U4)] uint fileShare,
            IntPtr securityAttributes,
            [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
            [MarshalAs(UnmanagedType.U4)] int flags,
            IntPtr template);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteConsoleOutputW(
            SafeFileHandle hConsoleOutput,
            CharInfo[] lpBuffer,
            Coord dwBufferSize,
            Coord dwBufferCoord,
            ref SmallRect lpWriteRegion);

        [StructLayout(LayoutKind.Sequential)]
        public struct Coord {
            public short X;
            public short Y;

            public Coord(short X, short Y) {
                this.X = X;
                this.Y = Y;
            }
        };

        [StructLayout(LayoutKind.Explicit, CharSet=CharSet.Unicode)]
        public struct CharUnion {
            [FieldOffset(0)] public char UnicodeChar;
            [FieldOffset(0)] public byte AsciiChar;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct CharInfo {
            [FieldOffset(0)] public CharUnion Char;
            [FieldOffset(2)] public short Attributes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SmallRect {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct COORD {
            public short X;
            public short Y;

            public COORD(short X, short Y) {
                this.X = X;
                this.Y = Y;
            }
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct CONSOLE_FONT_INFO_EX {
            public uint cbSize;
            public uint nFont;
            public COORD dwFontSize;
            public int FontFamily;
            public int FontWeight;
            public fixed char FaceName[(int)LF_FACESIZE];
            const uint LF_FACESIZE = 32;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetCurrentConsoleFontEx(
            IntPtr consoleOutput,
            bool maximumWindow,
            ref CONSOLE_FONT_INFO_EX consoleCurrentFontEx);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(
            int nStdHandle
        );

        public enum StdHandle {
            InputHandle = -10,
            OutputHandle = -11
        }
        // End console output-specific members

        // Begin input-specific members
        public const int ENABLE_MOUSE_INPUT = 0x0010;
        public const int ENABLE_QUICK_EDIT_MODE = 0x0040;
        public const int ENABLE_EXTENDED_FLAGS = 0x0080;
        public const int KEY_EVENT = 1;
        public const int MOUSE_EVENT = 2;

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUT_RECORD {
            [FieldOffset(0)]
            public short EventType;
            [FieldOffset(4)]
            public KEY_EVENT_RECORD KeyEvent;
            [FieldOffset(4)]
            public MOUSE_EVENT_RECORD MouseEvent;
            [FieldOffset(4)]
            public WINDOW_BUFFER_SIZE_RECORD WindowBufferSizeEvent;
        }

        public struct MOUSE_EVENT_RECORD {
            public COORD dwMousePosition;
            public int dwButtonState;
            public int dwControlKeyState;
            public int dwEventFlags;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct KEY_EVENT_RECORD {
            [FieldOffset(0)]
            [MarshalAs(UnmanagedType.Bool)]
            public bool bKeyDown;
            [FieldOffset(4)]
            public ushort wRepeatCount;
            [FieldOffset(6)]
            public ushort wVirtualKeyCode;
            [FieldOffset(8)]
            public ushort wVirtualScanCode;
            [FieldOffset(10)]
            public char UnicodeChar;
            [FieldOffset(10)]
            public byte AsciiChar;
            [FieldOffset(12)]
            public int dwControlKeyState;
        };

        public struct WINDOW_BUFFER_SIZE_RECORD {
            public COORD dwSize;
        };
        
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleMode(IntPtr hConsoleHandle, ref int lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadConsoleInput(IntPtr hConsoleInput, ref INPUT_RECORD lpBuffer, uint nLength, ref uint lpNumberOfEventsRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, int dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetNumberOfConsoleInputEvents(IntPtr hConsoleInput, out uint lpcNumberOfEvents);
        // End console input-specific members

        // Begin console screen-specific members
        [StructLayout(LayoutKind.Sequential)]
        public struct SMALL_RECT {
            public short Left;
            public short Top;
            public short Right;
            public short Bottom;
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public struct COLORREF {
            public uint dwColor;

            // public COLORREF(Color color) {
            //     this.dwColor = color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
            // }

            public COLORREF(uint r, uint g, uint b) {
                this.dwColor = r + (g << 8) + (b << 16);
            }

            // public Color ToColor() {
            //     return Color.FromArgb(
            //         red: (int)(0x000000FFU & this.dwColor),
            //         green: (int)(0x0000FF00U & this.dwColor) >> 8,
            //         blue: (int)(0x00FF0000U & this.dwColor) >> 16);
            // }

            // public void SetColor(Color color) {
            //     this.dwColor = color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
            // }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CONSOLE_SCREEN_BUFFER_INFO_EX {
            public int cbSize;
            public COORD dwSize;
            public COORD dwCursorPosition;
            public ushort wAttributes;
            public SMALL_RECT srWindow;
            public COORD dwMaximumWindowSize;
            public ushort wPopupAttributes;
            public bool bFullscreenSupported;
            public COLORREF Black;
            public COLORREF DarkBlue;
            public COLORREF DarkGreen;
            public COLORREF DarkCyan;
            public COLORREF DarkRed;
            public COLORREF DarkMagenta;
            public COLORREF DarkYellow;
            public COLORREF Gray;
            public COLORREF DarkGray;
            public COLORREF Blue;
            public COLORREF Green;
            public COLORREF Cyan;
            public COLORREF Red;
            public COLORREF Magenta;
            public COLORREF Yellow;
            public COLORREF White;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleScreenBufferInfoEx(SafeFileHandle hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleScreenBufferInfoEx(SafeFileHandle hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO_EX csbe);


        public static void ManageNativeReturnValue<T>(T returnValue) {
            if (returnValue is bool returnVal) {
                // Manage Boolean return value
                if (!returnVal) {
                    throw new Win32Exception();
                }
            }
        }
    }
}
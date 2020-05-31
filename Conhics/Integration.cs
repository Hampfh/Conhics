using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

using Microsoft.Win32.SafeHandles;

namespace Conhics {
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
            InputHandle = -10,  // Used for mouse input
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
        
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleMode(IntPtr hConsoleHandle, ref int lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadConsoleInput(IntPtr hConsoleInput, ref INPUT_RECORD lpBuffer, uint nLength, ref uint lpNumberOfEventsRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, int dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetNumberOfConsoleInputEvents(IntPtr hConsoleInput, out uint lpcNumberOfEvents);
        


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
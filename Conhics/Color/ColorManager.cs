// <copyright file="ColorManager.cs" company="Hampfh and haholm">
// Copyright (c) Hampfh and haholm. All rights reserved.
// </copyright>

namespace Conhics.Color {
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Manages the 16 colors available for use with Conhics.
    /// </summary>
    public static class ColorManager {
        private static Integration.CONSOLE_SCREEN_BUFFER_INFO_EX consoleScreenBufferInfo;
        private static Dictionary<string, ConhicsColor> colorDictionary;

        static ColorManager() {
            colorDictionary = new Dictionary<string, ConhicsColor>();
            consoleScreenBufferInfo = default(Integration.CONSOLE_SCREEN_BUFFER_INFO_EX);
            consoleScreenBufferInfo.cbSize = Marshal.SizeOf(consoleScreenBufferInfo);
            Integration.GetConsoleScreenBufferInfoEx(Screen.s_handle, ref consoleScreenBufferInfo);
            foreach (ConsoleColor color in Enum.GetValues(typeof(ConsoleColor))) {
                string colorName = Enum.GetName(typeof(ConsoleColor), color);
                colorDictionary.Add(
                    key: colorName,
                    value: new ConhicsColor(color, colorName));
            }
        }

        public static void AddColor(byte r, byte g, byte b, string colorName, string removeColorWithName) {
            ConhicsColor conhicsColorToEdit = GetColor(removeColorWithName);
            colorDictionary.Remove(removeColorWithName);
            EditColor(conhicsColorToEdit, r, g, b, colorName);
            colorDictionary.Add(colorName, conhicsColorToEdit);
        }

        public static ConhicsColor GetColor(string colorName) {
            return colorDictionary[colorName];
        }

        public static IEnumerable<ConhicsColor> GetColors() {
            foreach (ConhicsColor conhicsColor in colorDictionary.Values) {
                yield return conhicsColor;
            }
        }

        private static void EditColor(ConhicsColor conhicsColorToEdit, byte r, byte g, byte b, string colorName) {
            ConsoleColor consoleColorToEdit = conhicsColorToEdit.ConsoleColor;
            conhicsColorToEdit.SetColor(r, g, b, colorName);
            string consoleColorToEditName = Enum.GetName(typeof(ConsoleColor), consoleColorToEdit);
            object consoleScreenBufferInfoBoxed = consoleScreenBufferInfo;
            typeof(Integration.CONSOLE_SCREEN_BUFFER_INFO_EX)
                .GetField(name: consoleColorToEditName)
                .SetValue(obj: consoleScreenBufferInfoBoxed, value: conhicsColorToEdit.ColorRef);
            consoleScreenBufferInfo = (Integration.CONSOLE_SCREEN_BUFFER_INFO_EX)consoleScreenBufferInfoBoxed;
            Integration.SetConsoleScreenBufferInfoEx(Screen.s_handle, ref consoleScreenBufferInfo);
        }
    }
}
// <copyright file="ConhicsColor.cs" company="Hampfh and haholm">
// Copyright (c) Hampfh and haholm. All rights reserved.
// </copyright>

namespace Conhics.Color {
    using System;

    public class ConhicsColor { // is this instantiable from outside the assembly?
        internal Integration.COLORREF ColorRef { get; private set; }

        internal ConsoleColor ConsoleColor { get; }

        public byte Red { get; private set; }

        public byte Green { get; private set; }

        public byte Blue { get; private set; }

        public string Name { get; private set; }

        internal ConhicsColor(ConsoleColor color, string name) {
            int colorInt = (int)color;
            int brightnessCoefficient = ((colorInt & 8) > 0) ? 2 : 1;
            int r = ((colorInt & 4) > 0) ? 64 * brightnessCoefficient : 0,
                g = ((colorInt & 2) > 0) ? 64 * brightnessCoefficient : 0,
                b = ((colorInt & 1) > 0) ? 64 * brightnessCoefficient : 0;
            this.ColorRef = new Integration.COLORREF(
                r: (uint)r,
                g: (uint)g,
                b: (uint)b);
            this.ConsoleColor = color;
            this.Red = (byte)(r * 255.0 / int.MaxValue);  // * int.MaxValue or uint.MaxValue...?
            this.Green = (byte)(g * 255.0 / int.MaxValue);
            this.Blue = (byte)(b * 255.0 / int.MaxValue);
            this.Name = name;
        }

        internal ConhicsColor(ConsoleColor color, string name, byte r, byte g, byte b) {
            this.SetColor(r, g, b, name);
            this.ConsoleColor = color;
        }

        internal void SetColor(byte r, byte g, byte b, string name) {
            this.ColorRef = new Integration.COLORREF(
                r: (uint)(r / 255.0 * uint.MaxValue),   // * uint.MaxValue or int.MaxValue...?
                g: (uint)(g / 255.0 * uint.MaxValue),
                b: (uint)(b / 255.0 * uint.MaxValue));
            this.Red = r;
            this.Green = g;
            this.Blue = b;
            this.Name = name;
        }
    }
}
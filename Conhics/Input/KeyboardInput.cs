// <copyright file="KeyboardInput.cs" company="Hampfh and haholm">
// Copyright (c) Hampfh and haholm. All rights reserved.
// </copyright>

namespace Conhics.Input {
    using System;

    /// <summary>
    /// Contains information regarding keyboard input.
    /// </summary>
    public struct KeyboardInput {
        /// <summary>
        /// Whether or not the key is pressed.
        /// </summary>
        public readonly bool KeyDown;

        /// <summary>
        /// The amount of times the key was repeated through being held down since the last <see cref="KeyboardInput"/> instance.
        /// </summary>
        public readonly ushort KeyRepeatCount;

        /// <summary>
        /// The virtual-key code of the key pressed. Virtual-key codes: <see cref="!:https://docs.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes"/>.
        /// </summary>
        public readonly ushort VirtualKeyCode;

        /// <summary>
        /// Equal to virtual key code but with a cast to console key.
        /// </summary>
        public readonly ConsoleKey ConsoleKey;

        /// <summary>
        /// The Unicode character of the key that was pressed.
        /// </summary>
        public readonly char Character;

        /// <summary>
        /// The current keyboard state.
        /// </summary>
        public readonly KeyboardStates State;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardInput"/> struct.
        /// </summary>
        /// <param name="keyEventRecord">A <see cref="Conhics.Integration.KEY_EVENT_RECORD"/> object defining keyboard event information.</param>
        internal KeyboardInput(Integration.KEY_EVENT_RECORD keyEventRecord) {
            this.KeyDown = keyEventRecord.bKeyDown;
            this.KeyRepeatCount = keyEventRecord.wRepeatCount;
            this.VirtualKeyCode = keyEventRecord.wVirtualKeyCode;
            this.ConsoleKey = (ConsoleKey)keyEventRecord.wVirtualKeyCode;
            this.Character = keyEventRecord.UnicodeChar;
            this.State = (KeyboardStates)keyEventRecord.dwControlKeyState;
        }
    }
}
// <copyright file="Mouse.cs" company="Hampfh and haholm">
// Copyright (c) Hampfh and haholm. All rights reserved.
// </copyright>

namespace Conhics.Input {
    using System;

    /// <summary>
    /// Contains functionality for mouse input.
    /// </summary>
    public static class Mouse {
        /// <summary>
        /// Gets or sets a value indicating whether mouse input is enabled - true, or disabled - false.
        /// </summary>
        /// <value>A value indicating whether mouse input is enabled - true, or disabled - false.</value>
        public static bool IsEnabled {
            get {
                return InputManager.IsMouseEnabled;
            }

            set {
                if (value) {
                    InputManager.InputHandle = Integration.GetStdHandle((int)Integration.StdHandle.InputHandle);
                    ConfigureConsoleMode(InputManager.InputHandle);
                }

                InputManager.IsMouseEnabled = value;
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="MouseInput"/> struct with the most recent mouse input.
        /// </summary>
        /// <value>An instance of the <see cref="MouseInput"/> struct with the most recent mouse input.</value>
        public static MouseInput Input { get; internal set; }

        private static void ConfigureConsoleMode(IntPtr inputHandle) {
            int consoleMode = 0;
            Integration.ManageNativeReturnValue(
                returnValue: Integration.GetConsoleMode(
                    hConsoleHandle: inputHandle,
                    lpMode: ref consoleMode));
            consoleMode |= Integration.ENABLE_MOUSE_INPUT;      // Enable mouse input
            consoleMode &= ~Integration.ENABLE_QUICK_EDIT_MODE; // Disable quick edit mode (ability to highlight text)
            consoleMode |= Integration.ENABLE_EXTENDED_FLAGS;   // Enable extended flags for quick edit to take effect
            Integration.ManageNativeReturnValue(
                returnValue: Integration.SetConsoleMode(
                    hConsoleHandle: inputHandle,
                    dwMode: consoleMode));
        }
    }
}
// <copyright file="Keyboard.cs" company="Hampfh and haholm">
// Copyright (c) Hampfh and haholm. All rights reserved.
// </copyright>

namespace Conhics.Input {
    /// <summary>
    /// Contains functionality for keyboard input.
    /// </summary>
    public class Keyboard {
        /// <summary>
        /// Gets or sets a value indicating whether keyboard input is enabled - true, or disabled - false.
        /// </summary>
        /// <value>A value indicating whether keyboard input is enabled - true, or disabled - false.</value>
        public static bool IsEnabled {
            get {
                return InputManager.IsKeyboardEnabled;
            }

            set {
                if (value) {
                    InputManager.InputHandle = Integration.GetStdHandle((int)Integration.StdHandle.InputHandle);
                }

                InputManager.IsKeyboardEnabled = value;
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="KeyboardInput"/> struct with the most recent keyboard input.
        /// </summary>
        /// <value>An instance of the <see cref="KeyboardInput"/> struct with the most recent keyboard input.</value>
        public static KeyboardInput Input { get; internal set; }
    }
}
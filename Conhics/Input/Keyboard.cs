// <copyright file="Keyboard.cs" company="Hampfh and haholm">
// Copyright (c) Hampfh and haholm. All rights reserved.
// </copyright>

namespace Conhics.Input {
    using System.Collections.Concurrent;

    /// <summary>
    /// Contains functionality for keyboard input.
    /// </summary>
    public class Keyboard {
        public static readonly ConcurrentQueue<KeyboardInput> inputQueue = new ConcurrentQueue<KeyboardInput>();

        /// <summary>
        /// Gets or sets a value indicating whether keyboard input is enabled - true, or disabled - false.
        /// </summary>
        /// <value>A value indicating whether keyboard input is enabled - true, or disabled - false.</value>
        public static bool IsEnabled {
            get => InputManager.IsKeyboardEnabled;

            set {
                if (value)
                    InputManager.InputHandle = Integration.GetStdHandle((int)Integration.StdHandle.InputHandle);

                InputManager.IsKeyboardEnabled = value;
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="KeyboardInput"/> struct with the most recent keyboard input.
        /// </summary>
        /// <value>An instance of the <see cref="KeyboardInput"/> struct with the most recent keyboard input.</value>
        public static KeyboardInput? Input {
            get {
                if (inputQueue.TryDequeue(out var firstInput))
                    return firstInput;

                return LastInput;
            }
        }

        public static KeyboardInput? LastInput { get; internal set; }

        /// <summary>
        /// Push the latest input to the end of the input queue.
        /// </summary>
        internal static void EnqueueInput(KeyboardInput input) {
            if (inputQueue.Count >= 50)
                inputQueue.TryDequeue(out _);
            inputQueue.Enqueue(input);
        }
    }
}
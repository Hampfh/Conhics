// <copyright file="KeyboardStates.cs" company="Hampfh and haholm">
// Copyright (c) Hampfh and haholm. All rights reserved.
// </copyright>

namespace Conhics.Input {
    /// <summary>
    /// Keyboard state values.
    /// </summary>
    [System.Flags]
    public enum KeyboardStates : int {
        /// <summary>
        /// Caps lock is enabled.
        /// </summary>
        CapsLock = 0x0080,

        /// <summary>
        /// The pressed key is an enhanced key.
        /// </summary>
        EnhancedKey = 0x0100,

        /// <summary>
        /// Left alt is pressed.
        /// </summary>
        LeftAlt = 0x0002,

        /// <summary>
        /// Left ctrl is pressed.
        /// </summary>
        LeftCtrl = 0x0008,

        /// <summary>
        /// Num lock is enabled.
        /// </summary>
        NumLock = 0x0020,

        /// <summary>
        /// Right alt is pressed.
        /// </summary>
        RightAlt = 0x0001,

        /// <summary>
        /// Right ctrl is pressed.
        /// </summary>
        RightCtrl = 0x0004,

        /// <summary>
        /// Scroll lock is enabled.
        /// </summary>
        ScrollLock = 0x0040,

        /// <summary>
        /// Shift is pressed.
        /// </summary>
        Shift = 0x0010,
    }
}
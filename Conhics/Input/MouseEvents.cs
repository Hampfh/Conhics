// <copyright file="MouseEvents.cs" company="Hampfh and haholm">
// Copyright (c) Hampfh and haholm. All rights reserved.
// </copyright>

namespace Conhics.Input {
    /// <summary>
    /// Mouse related event values.
    /// </summary>
    public enum MouseEvents {
        /// <summary>
        /// A mouse button is being pressed or released.
        /// </summary>
        PressedOrReleased = 0x0000,

        /// <summary>
        /// The mouse moved.
        /// </summary>
        Moved = 0x0001,

        /// <summary>
        /// A mouse button was double clicked.
        /// </summary>
        DoubleClicked = 0x0002,

        /// <summary>
        /// The mouse wheel was scrolled in the vertical direction.
        /// </summary>
        MouseWheeled = 0x0004,

        /// <summary>
        /// The mouse wheel was scrolled in the horizontal direction.
        /// </summary>
        MouseWheeledHorizontally = 0x0008,
    }
}
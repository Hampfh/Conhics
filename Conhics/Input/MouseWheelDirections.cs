// <copyright file="MouseWheelDirections.cs" company="Hampfh and haholm">
// Copyright (c) Hampfh and haholm. All rights reserved.
// </copyright>

namespace Conhics.Input {
    /// <summary>
    /// Mouse wheel scroll direction values.
    /// </summary>
    public enum MouseWheelDirections {
        /// <summary>
        /// For when the mouse is not scrolled in any direction.
        /// </summary>
        None,

        /// <summary>
        /// The scroll direction pointing away from the user.
        /// </summary>
        Forward,

        /// <summary>
        /// The scroll direction pointing towards the user.
        /// </summary>
        Backward,

        /// <summary>
        /// The scroll direction to the right relative to the user.
        /// </summary>
        Right,

        /// <summary>
        /// The scroll direction to the left relative to the user.
        /// </summary>
        Left,
    }
}
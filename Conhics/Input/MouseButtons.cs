// <copyright file="MouseButtons.cs" company="Hampfh and haholm">
// Copyright (c) Hampfh and haholm. All rights reserved.
// </copyright>

namespace Conhics.Input {
    /// <summary>
    /// Mouse button values.
    /// </summary>
    public enum MouseButtons {
        /// <summary>
        /// No mouse button.
        /// </summary>
        None = 0x0000,

        /// <summary>
        /// Left mouse button.
        /// </summary>
        Left = 0x0001,

        /// <summary>
        /// Right mouse button.
        /// </summary>
        Right = 0x0002,

        /// <summary>
        /// Second left mouse button.
        /// </summary>
        SecondLeft = 0x0004,

        /// <summary>¨
        /// Third left mouse button.
        /// </summary>
        ThirdLeft = 0x0008,

        /// <summary>
        /// Fourth left mouse button.
        /// </summary>
        FourthLeft = 0x0010,
    }
}
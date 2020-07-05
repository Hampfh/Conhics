// <copyright file="EventTypes.cs" company="Hampfh and haholm">
// Copyright (c) Hampfh and haholm. All rights reserved.
// </copyright>

namespace Conhics.Input {
    /// <summary>
    /// Event type values.
    /// </summary>
    internal enum EventTypes : short {
        /// <summary>
        /// Internal event, should be ignored.
        /// </summary>
        FocusEvent = 0x0010,

        /// <summary>
        /// Keyboard event.
        /// </summary>
        KeyEvent = 0x0001,

        /// <summary>
        /// Internal event, should be ignored.
        /// </summary>
        MenuEvent = 0x0008,

        /// <summary>
        /// Mouse event.
        /// </summary>
        MouseEvent = 0x0002,

        /// <summary>
        /// Window buffer size event, information about the new size of the console screen buffer.
        /// </summary>
        WindowBufferSizeEvent = 0x0004, // this is interesting...
    }
}
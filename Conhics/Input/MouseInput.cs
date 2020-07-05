// <copyright file="MouseInput.cs" company="Hampfh and haholm">
// Copyright (c) Hampfh and haholm. All rights reserved.
// </copyright>

namespace Conhics.Input {
    /// <summary>
    /// Contains information regarding mouse input.
    /// </summary>
    public struct MouseInput {
        /// <summary>
        /// The X position of the mouse from the left.
        /// </summary>
        public readonly short X;

        /// <summary>
        /// The Y position of the mouse from the top.
        /// </summary>
        public readonly short Y;

        /// <summary>
        /// The mouse button that was pressed.
        /// </summary>
        public readonly MouseButtons Button;

        /// <summary>
        /// The mouse event that occured.
        /// </summary>
        public readonly MouseEvents Event;

        /// <summary>
        /// The mouse scroll wheel direction.
        /// </summary>
        public readonly MouseWheelDirections MouseWheelDirection;

        /// <summary>
        /// Initializes a new instance of the <see cref="MouseInput"/> struct.
        /// </summary>
        /// <param name="mouseEventRecord">A Conhics.Integration.MOUSE_EVENT_RECORD object defining mouse event information.</param>
        internal MouseInput(Integration.MOUSE_EVENT_RECORD mouseEventRecord) {
            this.X = mouseEventRecord.dwMousePosition.X;
            this.Y = mouseEventRecord.dwMousePosition.Y;
            this.Button = (MouseButtons)(mouseEventRecord.dwButtonState & 0x00ff);
            this.Event = (MouseEvents)mouseEventRecord.dwEventFlags;
            this.MouseWheelDirection = MouseWheelDirections.None;
            int mouseWheelVelocity = mouseEventRecord.dwButtonState >> 16; // Extract the high word value, example: 0x00010004 >> 16 == 0x00000001
            bool positiveMouseWheelVelocity = mouseWheelVelocity > 0;
            if (mouseWheelVelocity == 0) {
                return;
            }
            else if (this.Event == MouseEvents.MouseWheeled) {
                this.MouseWheelDirection = positiveMouseWheelVelocity ? MouseWheelDirections.Forward : MouseWheelDirections.Backward;
            }
            else if (this.Event == MouseEvents.MouseWheeledHorizontally) {
                this.MouseWheelDirection = positiveMouseWheelVelocity ? MouseWheelDirections.Right : MouseWheelDirections.Left;
            }
        }

        // TODO: public MouseInput(...) { ... }
    }
}
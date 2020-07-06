// <copyright file="InputManager.cs" company="Hampfh and haholm">
// Copyright (c) Hampfh and haholm. All rights reserved.
// </copyright>

namespace Conhics.Input {
    using System;

    using Conhics;
    using Conhics.Events;

    internal static class InputManager {
        static InputManager() {
            EventManager.RegisterEvent(inputEventParameters);
        }

        private static EventParameters inputEventParameters = new EventParameters(
            condition: GetIsInputEnabled,
            subscribingMethod: UpdateInput);

        private static Integration.INPUT_RECORD s_inputRecord = default;
        private static uint s_numberOfInputEvents = 0;
        private static uint s_numberOfInputEventsRead = 0;

        internal static IntPtr InputHandle { get; set; }

        internal static bool IsMouseEnabled { get; set; }

        internal static bool IsKeyboardEnabled { get; set; }

        private static void UpdateInput() {
            if (s_numberOfInputEvents == 0) {
                Integration.ManageNativeReturnValue(
                    returnValue: Integration.GetNumberOfConsoleInputEvents(
                        hConsoleInput: InputHandle,
                        lpcNumberOfEvents: out s_numberOfInputEvents));
                if (!GetIsInputEnabled() || s_numberOfInputEvents == 0) {
                    return;
                }
            }

            Integration.ManageNativeReturnValue(
                returnValue: Integration.ReadConsoleInput(
                    hConsoleInput: InputHandle,
                    lpBuffer: ref s_inputRecord,
                    nLength: 1,
                    lpNumberOfEventsRead: ref s_numberOfInputEventsRead /* always equals 1 if nLength: 1 */));
            s_numberOfInputEvents -= s_numberOfInputEventsRead;
            switch ((EventTypes)s_inputRecord.EventType) {
                case EventTypes.KeyEvent:
                    Keyboard.Input = new KeyboardInput(s_inputRecord.KeyEvent);
                    break;
                case EventTypes.MouseEvent:
                    Mouse.Input = new MouseInput(s_inputRecord.MouseEvent);
                    break;
                case EventTypes.WindowBufferSizeEvent:
                    WinBuf.BufferWidth = s_inputRecord.WindowBufferSizeEvent.dwSize.X;
                    WinBuf.BufferHeight = s_inputRecord.WindowBufferSizeEvent.dwSize.Y;
                    break;
                default:
                    return;
            }
        }

        private static bool GetIsInputEnabled() {
            return IsMouseEnabled || IsKeyboardEnabled;
        }
    }
}
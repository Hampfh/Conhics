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

        private static Integration.INPUT_RECORD inputRecord = default;
        private static uint numberOfInputEvents = 0;
        private static uint numberOfInputEventsRead = 0;

        internal static IntPtr InputHandle { get; set; }

        internal static bool IsMouseEnabled { get; set; }

        internal static bool IsKeyboardEnabled { get; set; }

        private static void UpdateInput() {
            if (numberOfInputEvents == 0) {
                Integration.ManageNativeReturnValue(
                    returnValue: Integration.GetNumberOfConsoleInputEvents(
                        hConsoleInput: InputHandle,
                        lpcNumberOfEvents: out numberOfInputEvents));
                if (!GetIsInputEnabled() || numberOfInputEvents == 0) {
                    return;
                }
            }

            Integration.ManageNativeReturnValue(
                returnValue: Integration.ReadConsoleInput(
                    hConsoleInput: InputHandle,
                    lpBuffer: ref inputRecord,
                    nLength: 1,
                    lpNumberOfEventsRead: ref numberOfInputEventsRead /* always equals 1 if nLength: 1 */));
            numberOfInputEvents -= numberOfInputEventsRead;
            switch ((EventTypes)inputRecord.EventType) {
                case EventTypes.KeyEvent:
                    Keyboard.Input = new KeyboardInput(inputRecord.KeyEvent);
                    break;
                case EventTypes.MouseEvent:
                    Mouse.Input = new MouseInput(inputRecord.MouseEvent);
                    break;
                case EventTypes.WindowBufferSizeEvent:
                    WinBuf.BufferWidth = inputRecord.WindowBufferSizeEvent.dwSize.X;
                    WinBuf.BufferHeight = inputRecord.WindowBufferSizeEvent.dwSize.Y;
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
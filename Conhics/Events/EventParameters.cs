// <copyright file="EventParameters.cs" company="Hampfh and haholm">
// Copyright (c) Hampfh and haholm. All rights reserved.
// </copyright>

namespace Conhics.Events {
    using System;

    /// <summary>
    /// Holds objects to be executed relative to events.
    /// </summary>
    public class EventParameters {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventParameters"/> class.
        /// </summary>
        /// <param name="condition">A <see cref="Func{Boolean}"/> instance specifying an event condition that returns true if the condition is met, and otherwise returns false.</param>
        /// <param name="subscribingMethod">A <see cref="Action"/> instance specifying a method to be called when the event condition is met.</param>
        public EventParameters(Func<bool> condition, Action subscribingMethod) {
            this.Condition = condition;
            this.SubscribingMethod = subscribingMethod;
        }

        /// <summary>
        /// Gets a <see cref="Func{Boolean}"/> instance specifying an event condition that returns true if the condition is met, and otherwise returns false.
        /// </summary>
        /// <value>A <see cref="Func{Boolean}"/> instance specifying an event condition that returns true if the condition is met, and otherwise returns false.</value>
        public Func<bool> Condition { get; private set; }

        /// <summary>
        /// Gets a <see cref="Action"/> instance specifying a method to be called when the event condition is met.
        /// </summary>
        /// <value>A <see cref="Action"/> instance specifying a method to be called when the event condition is met.</value>
        public Action SubscribingMethod { get; private set; }
    }
}
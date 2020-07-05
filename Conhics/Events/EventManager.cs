// <copyright file="EventManager.cs" company="Hampfh and haholm">
// Copyright (c) Hampfh and haholm. All rights reserved.
// </copyright>

namespace Conhics.Events {
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Manages and handles events.
    /// </summary>
    public static class EventManager {

        /// TODO: rework? works fine for now...

        /// <summary>
        /// The maximum number of events that can be registered.
        /// </summary>
        public const int MaximumNumberOfEvents = 10;
        private const int PreferredEventsPerSecond = 120;

        private static ConcurrentDictionary<int, EventParameters> eventDictionary = new ConcurrentDictionary<int, EventParameters>();
        private static Task eventTask = new Task(EventLoop);
        private static Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// Gets a value indicating how many events have been registered.
        /// </summary>
        /// <value>A value indicating how many events have been registered.</value>
        public static int NumberOfEvents {
            get {
                return eventDictionary.Count;
            }
        }

        /// <summary>
        /// Register an event to be managed.
        /// </summary>
        /// <param name="eventParameters">A <see cref="EventParameters"/> object defining event information.</param>
        /// <returns>A <see cref="bool"/> value indicating if the event was registered.</returns>
        public static bool RegisterEvent(EventParameters eventParameters) {
            if (eventDictionary.Count == MaximumNumberOfEvents) {
                throw new OverflowException("Can not register another event. The maximum number of events possible to register was reached.");
            }

            if (!eventDictionary.TryAdd(eventParameters.GetHashCode(), eventParameters)) {
                return false;
            }

            if (eventTask.Status == TaskStatus.Created) {
                eventTask.Start();
            }
            else if (eventTask.IsCompleted) {
                eventTask = Task.Run(EventLoop);
            }

            return true;
        }

        /// <summary>
        /// Unregisters an event from the event manager. The <see cref="EventParameters"/> instance must be the same instance as the event was registered with.
        /// </summary>
        /// <param name="eventParameters">A <see cref="EventParameters"/> object defining event information.</param>
        /// <returns>A <see cref="bool"/> value indicating if the event was unregistered.</returns>
        public static bool UnregisterEvent(EventParameters eventParameters) {
            return eventDictionary.TryRemove(eventParameters.GetHashCode(), out eventParameters);
        }

        private static void EventLoop() {
            int maxProcessorCapacity = (int)Math.Ceiling(Environment.ProcessorCount / 2.0);
            var options = new ParallelOptions {
                MaxDegreeOfParallelism = NumberOfEvents > maxProcessorCapacity ? maxProcessorCapacity : NumberOfEvents,
            };
            while (true) {
                if (eventDictionary.Count == 0) {
                    break;
                }

                stopwatch.Restart();
                Parallel.ForEach(
                    source: eventDictionary.Values,
                    parallelOptions: options,
                    body: (eventParameters, loopState, something) => {
                        if (eventParameters.Condition()) {
                            eventParameters.SubscribingMethod();
                        }
                    });
                stopwatch.Stop();
                double preferredEventDuration = 1000 / PreferredEventsPerSecond;
                if (stopwatch.ElapsedMilliseconds < preferredEventDuration) {
                    Thread.Sleep((int)(preferredEventDuration - stopwatch.ElapsedMilliseconds));
                }
            }
        }
    }
}
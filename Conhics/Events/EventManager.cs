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
        // TODO: rework? works fine for now...

        /// <summary>
        /// The maximum number of events that can be registered.
        /// </summary>
        public const int MaximumNumberOfEvents = 10;
        private const int PreferredEventsPerSecond = 120;

        private static readonly ConcurrentDictionary<int, EventParameters> EventDictionary = new ConcurrentDictionary<int, EventParameters>();
        private static readonly Stopwatch Stopwatch = new Stopwatch();
        private static Task s_eventTask = new Task(EventLoop);

        /// <summary>
        /// Gets a value indicating how many events have been registered.
        /// </summary>
        /// <value>A value indicating how many events have been registered.</value>
        public static int NumberOfEvents => EventDictionary.Count;

        /// <summary>
        /// Register an event to be managed.
        /// </summary>
        /// <param name="eventParameters">A <see cref="EventParameters"/> object defining event information.</param>
        /// <returns>A <see cref="bool"/> value indicating if the event was registered.</returns>
        public static bool RegisterEvent(EventParameters eventParameters) {
            if (EventDictionary.Count == MaximumNumberOfEvents) {
                throw new OverflowException("Can not register another event. The maximum number of events possible to register was reached.");
            }

            if (!EventDictionary.TryAdd(eventParameters.GetHashCode(), eventParameters)) {
                return false;
            }

            if (s_eventTask.Status == TaskStatus.Created) {
                s_eventTask.Start();
            }
            else if (s_eventTask.IsCompleted) {
                s_eventTask = Task.Run(EventLoop);
            }

            return true;
        }

        /// <summary>
        /// Unregisters an event from the event manager. The <see cref="EventParameters"/> instance must be the same instance as the event was registered with.
        /// </summary>
        /// <param name="eventParameters">A <see cref="EventParameters"/> object defining event information.</param>
        /// <returns>A <see cref="bool"/> value indicating if the event was unregistered.</returns>
        public static bool UnregisterEvent(EventParameters eventParameters) {
            return EventDictionary.TryRemove(eventParameters.GetHashCode(), out eventParameters);
        }

        private static void EventLoop() {
            int maxProcessorCapacity = (int)Math.Ceiling(Environment.ProcessorCount / 2.0);
            var options = new ParallelOptions {
                MaxDegreeOfParallelism = NumberOfEvents > maxProcessorCapacity ? maxProcessorCapacity : NumberOfEvents,
            };
            while (true) {
                if (EventDictionary.Count == 0) {
                    break;
                }

                Stopwatch.Restart();
                Parallel.ForEach(
                    source: EventDictionary.Values,
                    parallelOptions: options,
                    body: (eventParameters, loopState, something) => {
                        if (eventParameters.Condition()) {
                            eventParameters.SubscribingMethod();
                        }
                    });
                Stopwatch.Stop();
                double preferredEventDuration = 1000 / (double)PreferredEventsPerSecond;
                if (Stopwatch.ElapsedMilliseconds < preferredEventDuration) {
                    Thread.Sleep((int)(preferredEventDuration - Stopwatch.ElapsedMilliseconds));
                }
            }
        }
    }
}
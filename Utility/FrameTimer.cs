using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using log4net;

namespace InfiniTK.Utility
{
    /// <summary>
    /// Used to limit frame rate, or simply provides an FPS counter and idle timer.
    /// </summary>
    public class FrameTimer
    {
        private static readonly ILog Log = LogManager.
            GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const int FramesPerSecond = 60;
        private const float FrameDelayIncrement = 0.1f;
        private const int FpsHistorySize = FramesPerSecond;

        private bool limitFrameRate = true;

        /// <summary>
        /// This property is used to enable/disable the frame rate limiter.
        /// </summary>
        public bool LimitFrameRate
        {
            get { return limitFrameRate; }
            set
            {
                limitFrameRate = value;
                if (limitFrameRate) return;
                if (fpsHistory.Count > 0) fpsHistory.Clear();
                frameDelay = 0;
            }
        }

        /// <summary>
        /// This property provides a constantly updated value.
        /// </summary>
        public double FPS { get; private set; }

        /// <summary>
        /// A new frame occurs every time the Idle method is called.
        /// This variable is used to measure frames-per-second.
        /// </summary>
        private int frameCounter;

        /// <summary>
        /// Used to maintain a steady frame rate.
        /// </summary>
        private double frameDelay;

        /// <summary>
        /// Recent history of FPS values to determine average.
        /// </summary>
        private readonly Queue<double> fpsHistory = new Queue<double>();

        /// <summary>
        /// This variable accumulates the number of milliseconds, and is used to determine
        /// when the frames-per-second reading is updated.
        /// </summary>
        private double frameCounterMillis;

        /// <summary>
        /// Used to measure the amount of time taken to render one frame..
        /// </summary>
        private readonly Stopwatch timeSinceFrameStart = new Stopwatch();

        /// <summary>
        /// Used to measure the amount of time since the last frame started, primarily to calculate FPS.
        /// </summary>
        private readonly Stopwatch timeSinceLastFrameStart = new Stopwatch();

        /// <summary>
        /// Starts a timer to record the length of time since last idle.
        /// </summary>
        public void Start()
        {
            timeSinceLastFrameStart.Start();
        }

        /// <summary>
        /// Compute time since last Idle start.
        /// </summary>
        public double ComputeTimeSinceLastFrameStart()
        {
            // Time since last Idle start.
            timeSinceLastFrameStart.Stop();
            var timeSinceLastIdle = timeSinceLastFrameStart.Elapsed.TotalMilliseconds;
            timeSinceLastFrameStart.Reset();
            timeSinceLastFrameStart.Start();

            // Calculate frames-per-second.
            Update(timeSinceLastIdle);

            // Start timer to time how long Idle method takes.
            timeSinceFrameStart.Start();

            // Time since last Idle is required for animation etc.
            return timeSinceLastIdle;
        }

        /// <summary>
        /// This method is used to compute the amount of time for Idle work.
        /// </summary>
        public double ComputeTimeSinceIdleStart()
        {
            timeSinceFrameStart.Stop();
            var timeSinceIdleStart = timeSinceFrameStart.Elapsed.TotalMilliseconds;

            // Log the FPS periodically (once a second).
            if (frameCounterMillis >= 1000)
            {
                Log.InfoFormat("FPS: {0}; avg: {1:F3}; delay: {2:F3}ms; idle: {3:F3}ms",
                    frameCounter, FPS, frameDelay, timeSinceIdleStart);

                frameCounterMillis -= 1000;
                frameCounter = 0;
            }

            frameCounter++;
            timeSinceFrameStart.Reset();
            return timeSinceIdleStart;
        }

        /// <summary>
        /// This method both calculates the frame rate and also optionally limits
        /// the frame rate. A weighted average for the frame rate is used.
        /// </summary>
        private void Update(double timeSinceLastIdle)
        {
            // Logging FPS every second. Accumulate time since last second elapsed.
            frameCounterMillis += timeSinceLastIdle;

            // Use an average value for FPS, when history queue has filled.
            if (fpsHistory.Count != FpsHistorySize)
            {
                // Add the latest FPS value to the queue.
                FPS = 1000 / timeSinceLastIdle;
                fpsHistory.Enqueue(FPS);
            }
            else
            {
                // Add latest FPS value to the queue and remove oldest.
                fpsHistory.Enqueue(1000 / timeSinceLastIdle);
                fpsHistory.Dequeue();

                // Calculate a weighted average for the FPS.
                var weight = 1.0;
                var sumWeight = 0.0;
                var sumFPS = 0.0;
                foreach (var fps in fpsHistory)
                {
                    sumFPS += fps * weight;
                    sumWeight += weight;
                    weight *= 2;
                }

                FPS = (sumFPS / FpsHistorySize) / (sumWeight / FpsHistorySize);
            }

            // The rest of this method is to limit frame rate.
            if (!LimitFrameRate) return;

            // Calculate new value for frame delay.
            if (FPS > FramesPerSecond) frameDelay += FrameDelayIncrement;
            else if (FPS < FramesPerSecond) frameDelay -= FrameDelayIncrement * 2;
            if (frameDelay < 0) frameDelay = 0;

            // Only start to delay when we have enough history for an average.
            if (frameDelay > 0 && fpsHistory.Count == FpsHistorySize)
                Thread.Sleep((int) frameDelay);
        }
    }
}

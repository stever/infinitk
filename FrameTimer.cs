using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using log4net;

namespace InfiniTK
{
    /// <summary>
    /// Used to limit frame rate, or simply provides an FPS counter and idle timer.
    /// </summary>
    public class FrameTimer
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #region Constants
        private const int FRAMES_PER_SECOND = 60; // LCD frame rate locked to 60 FPS. See also https://en.wikipedia.org/wiki/Frame_rate.
        private const float FRAME_DELAY_INCREMENT = 0.1f;
        private const int FPS_HISTORY_SIZE = FRAMES_PER_SECOND;
        #endregion

        #region LimitFrameRate property

        private bool _limitFrameRate = true;

        /// <summary>
        /// This property is used to enable/disable the frame rate limiter.
        /// </summary>
        public bool LimitFrameRate
        {
            get { return _limitFrameRate; }
            set
            {
                _limitFrameRate = value;
                if (_limitFrameRate) return;
                if (_fpsHistory.Count > 0) _fpsHistory.Clear();
                _frameDelay = 0;
            }
        }

        #endregion

        /// <summary>
        /// This property provides a constantly updated value.
        /// </summary>
        public double FPS { get; private set; }

        /// <summary>
        /// A new frame occurs every time the Idle method is called.
        /// This variable is used to measure frames-per-second.
        /// </summary>
        private int _frameCounter;

        /// <summary>
        /// Used to maintain a steady frame rate.
        /// </summary>
        private double _frameDelay;

        /// <summary>
        /// Recent history of FPS values to determine average.
        /// </summary>
        private readonly Queue<double> _fpsHistory = new Queue<double>();

        /// <summary>
        /// This variable accumulates the number of milliseconds, and is used to determine
        /// when the frames-per-second reading is updated.
        /// </summary>
        private double _frameCounterMillis;

        /// <summary>
        /// Used to measure the amount of time taken to render one frame..
        /// </summary>
        private readonly Stopwatch _timeSinceFrameStart = new Stopwatch();

        /// <summary>
        /// Used to measure the amount of time since the last frame started, primarily to calculate FPS.
        /// </summary>
        private readonly Stopwatch _timeSinceLastFrameStart = new Stopwatch();

        /// <summary>
        /// Starts a timer to record the length of time since last idle.
        /// </summary>
        public void Start()
        {
            _timeSinceLastFrameStart.Start();
        }

        /// <summary>
        /// Compute time since last Idle start.
        /// </summary>
        public double ComputeTimeSinceLastFrameStart()
        {
            // Time since last Idle start.
            _timeSinceLastFrameStart.Stop();
            double timeSinceLastIdle = _timeSinceLastFrameStart.Elapsed.TotalMilliseconds;
            _timeSinceLastFrameStart.Reset();
            _timeSinceLastFrameStart.Start();

            // Calculate frames-per-second.
            Update(timeSinceLastIdle);
            
            // Start timer to time how long Idle method takes.
            _timeSinceFrameStart.Start();

            // Time since last Idle is required for animation etc.
            return timeSinceLastIdle;
        }

        /// <summary>
        /// This method is used to compute the amount of time for Idle work.
        /// </summary>
        public double ComputeTimeSinceIdleStart()
        {
            _timeSinceFrameStart.Stop();
            double timeSinceIdleStart = _timeSinceFrameStart.Elapsed.TotalMilliseconds;

            // Log the FPS periodically (once a second).
            if (_frameCounterMillis >= 1000)
            {
                Log.InfoFormat("FPS: {0}; avg: {1}; delay: {2}ms; idle: {3}ms", 
                    _frameCounter, 
                    FPS.ToString("F3"),
                    _frameDelay.ToString("F3"),
                    timeSinceIdleStart.ToString("F3"));
                
                _frameCounterMillis -= 1000;
                _frameCounter = 0;
            }

            _frameCounter++;
            _timeSinceFrameStart.Reset();
            return timeSinceIdleStart;
        }

        /// <summary>
        /// This method both calculates the frame rate and also optionally limits
        /// the frame rate. A weighted average for the frame rate is used.
        /// </summary>
        private void Update(double timeSinceLastIdle)
        {
            // Logging FPS every second. Accumulate time since last second elapsed.
            _frameCounterMillis += timeSinceLastIdle;
            
            // Use an average value for FPS, when history queue has filled.
            if (_fpsHistory.Count != FPS_HISTORY_SIZE)
            {
                // Add the latest FPS value to the queue.
                FPS = 1000 / timeSinceLastIdle;
                _fpsHistory.Enqueue(FPS);
            }
            else
            {
                // Add latest FPS value to the queue and remove oldest.
                _fpsHistory.Enqueue(1000 / timeSinceLastIdle);
                _fpsHistory.Dequeue();

                // Calculate a weighted average for the FPS.
                double weight = 1;
                double sumWeight = 0;
                double sumFPS = 0;
                foreach (double fps in _fpsHistory)
                {
                    sumFPS += fps * weight;
                    sumWeight += weight;
                    weight *= 2;
                }
                FPS = (sumFPS / FPS_HISTORY_SIZE) 
                    / (sumWeight / FPS_HISTORY_SIZE);
            }

            // The rest of this method is to limit frame rate.
            if (!LimitFrameRate) return;

            // Calculate new value for frame delay.
            if (FPS > FRAMES_PER_SECOND) _frameDelay += FRAME_DELAY_INCREMENT;
            else if (FPS < FRAMES_PER_SECOND) _frameDelay -= (FRAME_DELAY_INCREMENT * 2);
            if (_frameDelay < 0) _frameDelay = 0;

            // Only start to delay when we have enough history for an average.
            if (_frameDelay > 0 && _fpsHistory.Count == FPS_HISTORY_SIZE)
                Thread.Sleep((int) _frameDelay);
        }
    }
}

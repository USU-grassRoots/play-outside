using System;
using System.Diagnostics;
using System.Threading;

namespace OutsideEngine.Timer
{
    /// <summary>
    /// Game timer, used to keep track of the in-game time elapsed.
    /// </summary>
    public class GameTimer
    {
        private readonly double _secondsPerCount;
        private double _deltaTime;

        private long _baseTime;
        private long _pausedTime;
        private long _stopTime;
        private long _prevTime;
        private long _currTime;

        private bool _stopped;

        /// <summary>
        /// Create a Game Timer. Timepoint 0.0 is the calling of the constructor
        /// </summary>
        public GameTimer()
        {
            _secondsPerCount = 0.0;
            _deltaTime = -1.0;
            _baseTime = 0;
            _pausedTime = 0;
            _prevTime = 0;
            _currTime = 0;
            _stopped = false;

            var countsPerSec = Stopwatch.Frequency;
            _secondsPerCount = 1.0 / countsPerSec;

        }

        /// <summary>
        /// How much total time has elapsed since the construction of this timer
        /// </summary>
        public float TotalTime
        {
            get
            {
                if (_stopped)
                {
                    return (float)(((_stopTime - _pausedTime) - _baseTime) * _secondsPerCount);
                }
                else
                {
                    return (float)(((_currTime - _pausedTime) - _baseTime) * _secondsPerCount);
                }
            }
        }

        /// <summary>
        /// How much has time changed on this update since the lastone
        /// </summary>
        public float DeltaTime
        {
            get { return (float)_deltaTime; }
        }

        /// <summary>
        /// How long the current frame has lasted
        /// </summary>
        public float FrameTime { get; set; }

        /// <summary>
        /// Reset the timer
        /// </summary>
        public void Reset()
        {
            var curTime = Stopwatch.GetTimestamp();
            _baseTime = curTime;
            _prevTime = curTime;
            _stopTime = 0;
            _stopped = false;
        }

        /// <summary>
        /// Start the timer
        /// </summary>
        public void Start()
        {
            var startTime = Stopwatch.GetTimestamp();
            if (_stopped)
            {
                _pausedTime += (startTime - _stopTime);
                _prevTime = startTime;
                _stopTime = 0;
                _stopped = false;
            }
        }

        /// <summary>
        /// Stop the timer (pause)
        /// </summary>
        public void Stop()
        {
            if (!_stopped)
            {
                var curTime = Stopwatch.GetTimestamp();
                _stopTime = curTime;
                _stopped = true;
            }
        }

        /// <summary>
        /// Update the timer - call once per event.
        /// Example: In rendering, call once per frame
        /// </summary>
        public void Tick()
        {
            if (_stopped)
            {
                _deltaTime = 0.0;
                return;
            }
            //while (_deltaTime < FrameTime) {
            var curTime = Stopwatch.GetTimestamp();
            _currTime = curTime;

            _deltaTime = (_currTime - _prevTime) * _secondsPerCount;
            //Thread.Sleep(0);
            //}
            _prevTime = _currTime;
            if (_deltaTime < 0.0)
            {
                _deltaTime = 0.0;
            }
        }
    }
}

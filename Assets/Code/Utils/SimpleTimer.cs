using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace TheDungeon.Utils
{

    public struct SimpleTimer
    {
        private float _delay;
        private float _timer;
        private float _duration;
        private float _oneOverDuration;
        private bool _isPlaying;
        private bool _loop;
        private bool _wasOnStartInvoked;

        private List<TimedEvent> _timedEvents;

        /// <summary>
        /// Called when you start the timer.
        /// </summary>
        public event Action OnStart;
        /// <summary>
        /// Called every time you update the timer and it's still running. Parameter provides a normalized time.
        /// </summary>
        public event Action<float> OnUpdate;
        /// <summary>
        /// Called when timer completed it's execution. In case of loop enabled, this is never called.
        /// </summary>
        public event Action OnComplete;
        /// <summary>
        /// Called at the end of a timer cycle when timer has loop enabled.
        /// </summary>
        public event Action OnLoopComplete;

        /// <summary>
        /// Starts the timer. You have to manually subscribe to events beforehand.
        /// </summary>
        /// <param name="duration">Duration of a timer.</param>
        /// <param name="loop">If true, timer will loop indefinitely.</param>
        public void Start(float duration, bool loop = false)
        {
            Start(duration, 0, loop);
        }

        /// <summary>
        /// Starts the timer. You have to manually subscribe to events beforehand.
        /// </summary>
        /// <param name="duration">Duration of a timer.</param>
        /// <param name="delay">Delay before timer starts.</param>
        /// <param name="loop">If true, timer will loop indefinitely.</param>
        public void Start(float duration, float delay, bool loop = false)
        {
            Assert.IsTrue(OnUpdate != null || OnLoopComplete != null || OnComplete != null, "None of the callbacks are assigned!");
            Debug.Assert(duration > 0, "Duration has to be greater than zero.");
            Debug.Assert(delay >= 0, "Delay can't be negative.");

            this._loop = loop;
            this._duration = duration;
            this._delay = delay;

            _timer = 0;
            _wasOnStartInvoked = false;

            if (_timedEvents != null)
                ResetTimedEvents();

            _oneOverDuration = 1 / duration;
            _isPlaying = true;
        }

        public bool IsPlaying()
        {
            return _isPlaying;
        }

        public void Stop()
        {
            _isPlaying = false;
        }

        public float GetTime()
        {
            return _timer;
        }

        /// <summary>
        ///  Add event to trigger at provided normalized time.
        /// </summary>
        /// <param name="normalizedTime">Normalized time in range (0, 1].</param>
        /// <param name="action">Callback.</param>
        public void AddTimedEvent(float normalizedTime, Action action)
        {
            Debug.Assert(normalizedTime >= 0 && normalizedTime < 1f, "Normalized time must be in range [0, 1). 0 is used when you have delay and need a callback on first update, 1 is not allowed because it is same as OnComplete or OnLoopComplete. ");

            if (_timedEvents == null)
                _timedEvents = new List<TimedEvent>();

            _timedEvents.Add(new TimedEvent(action, normalizedTime));
        }

        public void ClearTimedEvents()
        {
            _timedEvents = null;
        }

        public void Update(float deltaTime)
        {
            if (!_isPlaying)
                return;

            if (_delay > 0)
            {
                _delay -= deltaTime;
                return;
            }

            if (!_wasOnStartInvoked && OnStart != null)
            {
                _wasOnStartInvoked = true;
                OnStart?.Invoke();
            }

            if (_timer >= _duration)
            {
                if (_loop)
                {
                    OnUpdate?.Invoke(1f);
                    TryToInvokePotentialTimedEvents();
                    OnLoopComplete?.Invoke();
                    _timer -= _duration;
                }
                else
                {
                    _isPlaying = false;
                    OnUpdate?.Invoke(1f);
                    TryToInvokePotentialTimedEvents();
                    OnComplete?.Invoke();
                }
            }
            else
            {
                OnUpdate?.Invoke(_timer * _oneOverDuration);
                TryToInvokePotentialTimedEvents();
            }

            _timer += deltaTime;
        }

        private void TryToInvokePotentialTimedEvents()
        {
            if (_timedEvents != null)
            {
                for (var i = 0; i < _timedEvents.Count; i++)
                {
                    _timedEvents[i].TryInvoke(_timer * _oneOverDuration);
                }
            }
        }

        private void ResetTimedEvents()
        {
            for (var i = 0; i < _timedEvents.Count; i++)
            {
                _timedEvents[i].Reset();
            }
        }

        private class TimedEvent
        {
            private readonly Action _action;
            private readonly float _time;
            private bool _wasInvoked;

            public TimedEvent(Action action, float time)
            {
                _action = action;
                _time = time;
            }

            public void TryInvoke(float normalizedTime)
            {
                if (_wasInvoked)
                    return;

                if (normalizedTime >= _time)
                {
                    _wasInvoked = true;
                    _action.Invoke();
                }
            }

            public void Reset()
            {
                _wasInvoked = false;
            }
        }
    }

}

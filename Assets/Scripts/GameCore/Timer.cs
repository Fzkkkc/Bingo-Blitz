using System;
using Services;
using TMPro;
using UnityEngine;

namespace GameCore
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private float _timerDuration = 30f;
        [SerializeField] private float _timerGameDuration = 140f;
        
        private float _timeRemaining;
        private float _timeGameRemaining;
        private bool isRunning;
        private bool isRunningGame;

        public Action OnTimerStopped;
        public Action OnGameTimerStopped;
        public void Init()
        {
            ResetTimer();
            ResetGameTimer();
        }
        
        private void Update()
        {
            TimerRun();
        }

        private void TimerRun()
        {
            if (isRunning)
            {
                _timeRemaining -= Time.deltaTime;
                if (_timeRemaining <= 0)
                {
                    _timeRemaining = 0;
                    TimerEnd();
                }

                UpdateTimerText();
            }

            if (isRunningGame)
            {
                _timeGameRemaining -= Time.deltaTime;
                if (_timeGameRemaining <= 0)
                {
                    _timeGameRemaining = 0;
                    TimerGameEnd();
                }

                UpdateTimerText();
            }
        }

        public void StartTimer()
        {
            ResetTimer();
            isRunning = true;
        }

        public void StartGameTimer()
        {
            ResetGameTimer();
            isRunningGame = true;
        }
        
        private void TimerEnd()
        {
            OnTimerStopped?.Invoke();
            isRunning = false;
        }
        
        private void TimerGameEnd()
        {
            OnGameTimerStopped?.Invoke();
            isRunningGame = false;
        }

        public void StopTimer()
        {
            isRunning = false;
        }

        public void ResumeTimer()
        {
            isRunning = true;
        }

        public void ResetTimer()
        {
            _timeRemaining = _timerDuration;
            UpdateTimerText();
        }

        public void ResetGameTimer()
        {
            _timeGameRemaining = _timerGameDuration;
            UpdateTimerText();
        }
        
        private void UpdateTimerText()
        {
            var minutes = Mathf.FloorToInt(_timeRemaining / 60);
            var seconds = Mathf.FloorToInt(_timeRemaining % 60);
            _timerText.text = string.Format("{0:00} SEC", seconds);
        }
    }
}
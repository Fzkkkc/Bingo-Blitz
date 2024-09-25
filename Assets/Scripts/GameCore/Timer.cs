﻿using System;
using Services;
using TMPro;
using UnityEngine;

namespace GameCore
{
    public class Timer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private float _timerDuration = 30f;
        
        
        private float _timeRemaining;
        private bool isRunning;

        public Action OnTimerStopped;
        
        public void Init()
        {
            ResetTimer();
        }

        public float GetRemainingTime()
        {
            return _timeRemaining;
        }
        
        private void Update()
        {
            TimerRun();
        }

        private void TimerRun()
        {
            if (!isRunning) return;
            _timeRemaining -= Time.deltaTime;
            if (_timeRemaining <= 0)
            {
                _timeRemaining = 0;
                TimerEnd();
            }

            UpdateTimerText();
        }

        public void StartTimer()
        {
            ResetTimer();
            isRunning = true;
        }

        private void TimerEnd()
        {
            OnTimerStopped?.Invoke();
            isRunning = false;
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

        private void UpdateTimerText()
        {
            var minutes = Mathf.FloorToInt(_timeRemaining / 60);
            var seconds = Mathf.FloorToInt(_timeRemaining % 60);
            _timerText.text = string.Format("{0:00} SEC", seconds);
        }
    }
}
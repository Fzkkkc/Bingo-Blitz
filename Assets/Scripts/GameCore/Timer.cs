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
        [SerializeField] private TextMeshProUGUI _bingoCountText;

        private float _timeRemaining;
        private float _timeGameRemaining;
        private bool isRunning;
        private bool _gotBingo;
        private bool isRunningGame;

        public Action OnTimerStopped;
        public Action OnGameTimerStopped;

        public void Init()
        {
            ResetTimer();
            ResetGameTimer();
            GameInstance.GameState.PlayerBingoController.OnPlayerGotBingo += DecreaseGameTimer;
            GameInstance.UINavigation.OnGameStarted += ResetBingoCount;
            UpdateBingoCountText();  
        }

        private void OnDestroy()
        {
            GameInstance.GameState.PlayerBingoController.OnPlayerGotBingo -= DecreaseGameTimer;
            GameInstance.UINavigation.OnGameStarted -= ResetBingoCount;
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

                if(!_gotBingo)
                    UpdateGameTimerText();
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
            UpdateBingoCountText(0);  // Когда время игры закончилось, бинго всегда 0
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
            UpdateGameTimerText();
        }

        private void UpdateTimerText()
        {
            var minutes = Mathf.FloorToInt(_timeRemaining / 60);
            var seconds = Mathf.FloorToInt(_timeRemaining % 60);
            _timerText.text = string.Format("{0:00} SEC", seconds);
        }

        private void UpdateGameTimerText()
        {
            UpdateBingoCountText();  // Обновляем количество бинго при каждом обновлении времени
        }

        private void DecreaseGameTimer()
        {
            if (_timeGameRemaining > 5 && GameInstance.GameState.GameRunning)
            {
                _gotBingo = true;
                _timeGameRemaining = 5;
                UpdateBingoCountText(1);  
            }
        }

        private void UpdateBingoCountText(int? bingoCountOverride = null)
        {
            int bingoCount;

            if (bingoCountOverride.HasValue)
            {
                bingoCount = bingoCountOverride.Value;
            }
            else
            {
                bingoCount = Mathf.Max(0, Mathf.FloorToInt(_timeGameRemaining / 10));
            }

            _bingoCountText.text = bingoCount.ToString();
        }

        private void ResetBingoCount()
        {
            _gotBingo = false;
            _timeGameRemaining = _timerGameDuration;
            UpdateBingoCountText();
        }
    }
}

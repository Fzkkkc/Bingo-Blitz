﻿using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class UINavigation : MonoBehaviour
    {
        public List<CanvasGroup> GamePopups;
        public List<CanvasGroup> MainMenuPopups;
        [SerializeField] private Animator _transitionAnimator;

        public CanvasGroup LoadingMenu;
        public CanvasGroup MainMenu;
        public CanvasGroup GameMenu;

        public Action OnActivePopupChanged;
        public Action OnGameStarted;
        public Action OnGameRestarted;
        public Action OnGameWindowClosed;

        public bool _isInLoad;
        public bool _isInMenu = false;
        public bool _toMain;
        private bool gameClose;

        private bool _lastWin;
        
        [SerializeField] private DailyBonus _dailyBonus;

        public RewardedFramesCounters RewardedFramesCounters;

        private bool _firstOpened = true;
        
        public void Init()
        {
            ResetPopups();
        }

        private void ResetGamePopups()
        {
            foreach (var popup in GamePopups)
            {
                popup.alpha = 0f;
                popup.blocksRaycasts = false;
                popup.interactable = false;
            }
        }

        private void ResetMenuPopups()
        {
            foreach (var popup in MainMenuPopups)
            {
                popup.alpha = 0f;
                popup.blocksRaycasts = false;
                popup.interactable = false;
            }
        }

        private void ResetPopups()
        {
            OpenGroup(LoadingMenu);

            CloseGroup(GameMenu);
            CloseGroup(MainMenu);

            ResetGamePopups();
            ResetMenuPopups();
        }

        public void OpenMainMenu()
        {
            _isInLoad = false;
            StartCoroutine(OpenMenuPopup(0));
        }

        public void CloseGameUI()
        {
            _lastWin = false;

            gameClose = true;
            StartCoroutine(OpenMenuPopup(0, true, true));
        }

        public void BackToMainMenu()
        {
            StartCoroutine(OpenMenuPopup(0));
        }

        public void OpenDailyBonusUI()
        {
            StartCoroutine(AnimateScale(MainMenuPopups[1], true));
        }
        
        public void OpenBankUI()
        {
            StartCoroutine(AnimateScale(MainMenuPopups[2], true));
        }
        
        public void CloseBankUI()
        {
            StartCoroutine(AnimateScale(MainMenuPopups[2], false));
        }

        public void CloseDailyBonusUI()
        {
            StartCoroutine(AnimateScale(MainMenuPopups[1], false));
        }

        public void OpenCalendarBonusUI()
        {
            StartCoroutine(AnimateScale(MainMenuPopups[0], true));
        }

        public void CloseCalendarBonusUI()
        {
            StartCoroutine(AnimateScale(MainMenuPopups[0], false));
        }
        
        public void OpenGameMenu(int index)
        {
            GameInstance.MapRoadNavigation.SetCurrentLevelIndex(index);
            StartCoroutine(OpenGamePopup());
        }
        
        public IEnumerator OpenMenuPopup(int index, bool toMenu = true, bool needAward = false)
        {
            if (_isInMenu) yield break;
            
            TransitionAnimation();
            _isInMenu = true;
            GameInstance.GameState.GameRunning = false;
            yield return new WaitForSeconds(0.5f);
            GameInstance.FXController.PlayMenuBackgroundParticle();
            if (toMenu)
                GameInstance.MusicSystem.ChangeMusicClip();
            
            GameInstance.FXController.StopFireworksParticle();

            if (needAward)
            {
                GameInstance.MoneyManager.AddCoinsCurrency(GameInstance.GameState.CoinsCount);
                GameInstance.MoneyManager.AddDiamondsCurrency(GameInstance.GameState.DiamondsCount);
            }
            
            if (gameClose)
            {
                gameClose = false;
                ResetGamePopups();
                OnGameWindowClosed?.Invoke();
                GameInstance.FXController.StopGameBackgroundParticle();
            }

            SelectMenuPopup(index);
            ResetMenuPopups();
            if (!_firstOpened) yield break;
            _firstOpened = false;

            _dailyBonus.Init();
        }

        private IEnumerator OpenGamePopup(bool isRestart = false)
        {
            _isInMenu = false;
            TransitionAnimation();
            yield return new WaitForSeconds(0.5f);
            GameInstance.MusicSystem.ChangeMusicClip(false);
            GameInstance.FXController.StopFireworksParticle();
            GameInstance.FXController.PlayGameBackgroundParticle();
            GameInstance.FXController.StopMenuBackgroundParticle();
            ResetGamePopups();
            OpenGroup(GameMenu);

            SelectGamePopup(0);

            if (isRestart)
                OnGameRestarted.Invoke();
            else
                OnGameStarted?.Invoke();
        }
        
        public IEnumerator AnimateScale(CanvasGroup canvasGroup, bool show, float duration = 0.7f)
        {
            yield return new WaitForSeconds(0.2f);
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = show;
            canvasGroup.blocksRaycasts = show;

            var endScale = show ? new Vector3(1f, 1f, 1f) : Vector3.zero;
            var midScale = new Vector3(1.2f, 1.2f, 1.2f);

            var elapsedTime = 0f;

            var initialScale = show ? Vector3.zero : new Vector3(1f, 1f, 1f);
            canvasGroup.transform.localScale = initialScale;

            while (elapsedTime < duration / 2)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.transform.localScale = Vector3.Lerp(initialScale, midScale, elapsedTime / (duration / 2));
                yield return null;
            }

            elapsedTime = 0f;

            while (elapsedTime < duration / 2)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.transform.localScale = Vector3.Lerp(midScale, endScale, elapsedTime / (duration / 2));
                yield return null;
            }

            canvasGroup.transform.localScale = endScale;
            
        }

        public IEnumerator AnimateScaleAndMove(RectTransform rect, bool show, Vector3 targetPosition,
            float duration = 0.5f)
        {
            var endScale = show ? new Vector3(1f, 1f, 1f) : Vector3.zero;
            var midScale = new Vector3(1.4f, 1.4f, 1.4f);

            var initialPosition = rect.transform.localPosition;
            var endPosition = targetPosition;

            var elapsedTime = 0f;

            var initialScale = show ? Vector3.one : new Vector3(1f, 1f, 1f);
            rect.transform.localScale = initialScale;

            while (elapsedTime < duration / 2)
            {
                elapsedTime += Time.deltaTime;

                rect.transform.localScale = Vector3.Lerp(initialScale, midScale, elapsedTime / (duration / 2));

                rect.transform.localPosition = Vector3.Lerp(initialPosition, (initialPosition + endPosition) / 2,
                    elapsedTime / (duration / 2));

                yield return null;
            }

            elapsedTime = 0f;

            while (elapsedTime < duration / 2)
            {
                elapsedTime += Time.deltaTime;

                rect.transform.localScale = Vector3.Lerp(midScale, endScale, elapsedTime / (duration / 2));

                rect.transform.localPosition = Vector3.Lerp((initialPosition + endPosition) / 2, endPosition,
                    elapsedTime / (duration / 2));

                yield return null;
            }

            rect.transform.localScale = endScale;
            rect.transform.localPosition = endPosition;
        }


        public void TransitionAnimation()
        {
            _transitionAnimator.SetTrigger("Transition");
        }

        public void OpenGroup(CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }

        public void CloseGroup(CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }

        private void SelectMenuPopup(int selectedIndex)
        {
            OpenGroup(MainMenu);
            CloseGroup(GameMenu);

            for (var i = 0; i < MainMenuPopups.Count; i++)
                if (i == selectedIndex)
                {
                    MainMenuPopups[i].alpha = 1f;
                    MainMenuPopups[i].blocksRaycasts = true;
                    MainMenuPopups[i].interactable = true;
                }
                else
                {
                    MainMenuPopups[i].alpha = 0f;
                    MainMenuPopups[i].blocksRaycasts = false;
                    MainMenuPopups[i].interactable = false;
                }

            foreach (var popup in GamePopups)
            {
                popup.alpha = 0f;
                popup.blocksRaycasts = false;
                popup.interactable = false;
            }

            OnActivePopupChanged?.Invoke();
        }

        private void SelectGamePopup(int selectedIndex)
        {
            OpenGroup(GameMenu);
            CloseGroup(MainMenu);

            for (var i = 0; i < GamePopups.Count; i++)
                if (i == selectedIndex)
                {
                    GamePopups[i].alpha = 1f;
                    GamePopups[i].blocksRaycasts = true;
                    GamePopups[i].interactable = true;
                }
                else
                {
                    GamePopups[i].alpha = 0f;
                    GamePopups[i].blocksRaycasts = false;
                    GamePopups[i].interactable = false;
                }

            foreach (var popup in MainMenuPopups)
            {
                popup.alpha = 0f;
                popup.blocksRaycasts = false;
                popup.interactable = false;
            }

            OnActivePopupChanged?.Invoke();
        }
    }
}
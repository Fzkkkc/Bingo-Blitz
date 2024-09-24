using System;
using System.Collections;
using System.Collections.Generic;
using GameCore;
using Services;
using TMPro;
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

        public bool _isInLoad = false;
        public bool _toMain = false;
        private bool gameClose = false;
        
        private bool _lastWin = false;

        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _watchAdsButton;
        [SerializeField] private Image _frameMainImage;
        [SerializeField] private Sprite _frameMainLose;
        [SerializeField] private Sprite _frameMainWin;

        [SerializeField] private RatingGame _ratingGame;
        [SerializeField] private DailyBonus _dailyBonus;

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
            StartCoroutine(OpenMenuPopup(-1));
        }

        public void CloseGameUI()
        {
            _lastWin = false;
            
            gameClose = true;
            StartCoroutine(OpenMenuPopup(-1, true, true));
        }
        
        public void BackToMainMenu()
        {
            StartCoroutine(OpenMenuPopup(0));
        }
        
        public void OpenDailyBonusUI()
        {
            StartCoroutine(AnimateScale(MainMenuPopups[1], true));
        }
        
        public void CloseDailyBonusUI()
        {
            StartCoroutine(AnimateScale(MainMenuPopups[1], false));
        }
        
        public void OpenPotionShopUI()
        {
            StartCoroutine(OpenMenuPopup(2, false));
        }
        
        public void OpenBackgroundShopUI()
        {
            StartCoroutine(OpenMenuPopup(3, false));
        }

        public void OpenGameMenu()
        {
            StartCoroutine(OpenGamePopup());
        }
        
        public void RestartGame()
        {
            StartCoroutine(OpenGamePopup(true));
        }
        
        public void OpenGameOverPopup(bool isWin)
        {
            GameInstance.Timer.StopTimer();
            ResetGamePopups();
            StartCoroutine(FadeCanvasGroup(GamePopups[0], true));
            if (isWin)
            {
                GameInstance.FXController.PlayFireworksParticle();
                _lastWin = true;
                GameInstance.Audio.Play(GameInstance.Audio.WinGameEndSound);
                _watchAdsButton.interactable = false;
                _restartButton.gameObject.SetActive(true);
                _frameMainImage.sprite = _frameMainWin;
                GameInstance.Timer.TimerEndText.color = new Color(255f,255f,255f,255f);
            }
            else
            {
                _watchAdsButton.interactable = true;
                _restartButton.gameObject.SetActive(false);
                GameInstance.Audio.Play(GameInstance.Audio.LoseGameEndSound);
                _frameMainImage.sprite = _frameMainLose;
                GameInstance.Timer.TimerEndText.color = new Color(255f,255f,255f,0f);
            }
        }
        
        public void ContinueGame()
        {
            ResetGamePopups();
            GameInstance.GameState.ContinueGame();
        }
        
        public IEnumerator OpenMenuPopup(int index, bool toMenu = true, bool needAward = false)
        {
            TransitionAnimation();
            yield return new WaitForSeconds(0.5f);
            GameInstance.FXController.PlayMenuBackgroundParticle();
            _ratingGame.DisplayResults();
            if (toMenu)
            {
                GameInstance.MusicSystem.ChangeMusicClip();
            }
            else
            {
                GameInstance.MusicSystem.ChangeMusicClip(false);
            }
            GameInstance.FXController.StopFireworksParticle();
            
            if (needAward)
            {
                GameInstance.MoneyManager.AddCoinsCurrency(GameInstance.GameState.CurrentAwardCount);
                GameInstance.GameState.CurrentAwardCount = 0;
            }
            
            if (gameClose)
            {
                GameInstance.FXController.StateBoilingFX(false);
                gameClose = false;
                ResetGamePopups();
                OnGameWindowClosed?.Invoke();
                GameInstance.FXController.StopGameBackgroundParticle();
            }
            
            SelectMenuPopup(index);
        }
        
        private IEnumerator OpenGamePopup(bool isRestart = false)
        {
            TransitionAnimation();
            yield return new WaitForSeconds(0.5f);
            //GameInstance.MusicSystem.ChangeMusicClip(false);
            GameInstance.FXController.StopFireworksParticle();
            GameInstance.FXController.PlayGameBackgroundParticle();
            GameInstance.FXController.StopMenuBackgroundParticle();
            ResetGamePopups();
            OpenGroup(GameMenu);
            
            if (isRestart)
            {
                OnGameRestarted.Invoke();
            }
            else
            {
                OnGameStarted?.Invoke();
            }
        }
        
        public IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, bool show, float duration = 0.5f)
        {
            canvasGroup.interactable = show;
            canvasGroup.blocksRaycasts = show;

            var startAlpha = canvasGroup.alpha;
            var elapsedTime = 0f;

            var finishValue = show ? 1f : 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(startAlpha, finishValue, elapsedTime / duration);
                yield return null;
            }

            canvasGroup.alpha = finishValue;
        }

        public IEnumerator AnimateScale(CanvasGroup canvasGroup, bool show, float duration = 0.7f)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = show;
            canvasGroup.blocksRaycasts = show;

            RectTransform rectTransform = canvasGroup.GetComponent<RectTransform>();
            
            Vector3 startScale = rectTransform.localScale;
            Vector3 endScale = show ? new Vector3(1f, 1f, 1f) : Vector3.zero;
            Vector3 midScale = new Vector3(1.2f, 1.2f, 1.2f);

            float elapsedTime = 0f;

            // Устанавливаем начальный scale: либо 0 при показе, либо 1 при скрытии
            Vector3 initialScale = show ? Vector3.zero : new Vector3(1f, 1f, 1f);
            rectTransform.localScale = initialScale;

            // Анимация до 1.2
            while (elapsedTime < duration / 2)
            {
                elapsedTime += Time.deltaTime;
                rectTransform.localScale = Vector3.Lerp(initialScale, midScale, elapsedTime / (duration / 2));
                yield return null;
            }

            elapsedTime = 0f;

            // Анимация от 1.2 до конечного значения
            while (elapsedTime < duration / 2)
            {
                elapsedTime += Time.deltaTime;
                rectTransform.localScale = Vector3.Lerp(midScale, endScale, elapsedTime / (duration / 2));
                yield return null;
            }

            rectTransform.localScale = endScale;  // Устанавливаем финальный scale
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
            {
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
            }
            
            foreach (var popup in GamePopups)
            {
                popup.alpha = 0f;
                popup.blocksRaycasts = false;
                popup.interactable = false;
            }
            
            OnActivePopupChanged?.Invoke();

            if (!_firstOpened) return;
            _firstOpened = false;
                
            _dailyBonus.Init();
        }
        
        private void SelectGamePopup(int selectedIndex)
        {
            OpenGroup(GameMenu);
            CloseGroup(MainMenu);
            
            for (var i = 0; i < GamePopups.Count; i++)
            {
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
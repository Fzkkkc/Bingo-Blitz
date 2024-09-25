using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore
{
    public class GameState : MonoBehaviour
    {
        [Header("Animations")]
        [SerializeField] private Animator _wonComboAnimator;
        [SerializeField] private Animator _readyAnimator;
        
        [Header("UI")]
        [SerializeField] private List<Button> _starButtons;
        [SerializeField] private CanvasGroup _framesGroup;
        [SerializeField] private Button _oneFieldButton;
        [SerializeField] private Button _twoFieldButton;
        
        [Header("Classes")]
        public Timer Timer;
        public ImagesAnimation ImagesAnimation;
        
        [Header("Variables")] 
        [SerializeField] private int _selectedFieldType = 0;
        
        private bool _boughtOneCard = false;
        private bool _boughtTwoCard = false;
        
        private int StarCount => GameInstance.MapRoadNavigation
            ._levelsData[GameInstance.MapRoadNavigation._currentMainLevel].starCount;
        
        public void Init()
        {
            GameInstance.UINavigation.OnGameStarted += StartComboAnim;
            GameInstance.UINavigation.OnGameStarted += UpdateStarButtons;
            GameInstance.UINavigation.OnGameStarted += OpenPickCard;
            GameInstance.UINavigation.OnGameWindowClosed += ResetGame;
            _oneFieldButton.onClick.AddListener(SelectOneFieldType);
            _twoFieldButton.onClick.AddListener(SelectTwoFieldType);
            Timer.Init();
            Timer.OnTimerStopped += OpenGameField;
        }

        private void OnDestroy()
        {
            GameInstance.UINavigation.OnGameStarted -= StartComboAnim;
            GameInstance.UINavigation.OnGameWindowClosed -= ResetGame;
            GameInstance.UINavigation.OnGameStarted -= OpenPickCard;
            GameInstance.UINavigation.OnGameStarted -= UpdateStarButtons;
            Timer.OnTimerStopped -= OpenGameField;
        }

        private void StartComboAnim()
        {
            _wonComboAnimator.SetTrigger("PlayCombo");
        }

        private void ResetGame()
        {
            _selectedFieldType = 0;
            _wonComboAnimator.SetTrigger("StopCombo");
            _twoFieldButton.interactable = true;
            _oneFieldButton.interactable = true;
            _boughtOneCard = false;
            _boughtTwoCard = false;
        }
        
        private void UpdateStarButtons()
        {
            for (int i = 0; i < _starButtons.Count; i++)
            {
                _starButtons[i].interactable = (i < StarCount);
            }
        }

        private void OpenPickCard()
        {
            StartCoroutine(OpenCardPopup());
        }

        private IEnumerator OpenCardPopup()
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(GameInstance.UINavigation.AnimateScale(GameInstance.UINavigation.GamePopups[4], true));
            Timer.StartTimer();
        }

        private void OpenGameField()
        {
            if (_selectedFieldType == 0)
            {
                GameInstance.UINavigation.CloseGameUI();
            }
            else
            {
                StartCoroutine(OpenGameFieldAnimation());
            }
        }

        private IEnumerator OpenGameFieldAnimation()
        {
            StartCoroutine(GameInstance.UINavigation.AnimateScale(GameInstance.UINavigation.GamePopups[4], false));
            
            yield return new WaitForSeconds(0.7f);
            
            _readyAnimator.SetTrigger("Ready");
            StartCoroutine(ShowFieldAnimation());
        }

        private IEnumerator ShowFieldAnimation()
        {
            yield return new WaitForSeconds(3.5f);
            StartCoroutine(GameInstance.UINavigation.AnimateScale(_framesGroup, false));
            StartCoroutine(GameInstance.UINavigation.AnimateScale(GameInstance.UINavigation.GamePopups[2], true));
            ImagesAnimation.StartAnimation();
        }

        private void SelectOneFieldType()
        {
            if (_boughtOneCard)
            {
                _selectedFieldType = 1;
                _oneFieldButton.interactable = false;
                _twoFieldButton.interactable = true;
            }
            
            if (!GameInstance.MoneyManager.HasEnoughCoinsCurrency(80) || _boughtOneCard) return;
            GameInstance.MoneyManager.SpendCoinsCurrency(80);
            _selectedFieldType = 1;
            _oneFieldButton.interactable = false;
            _twoFieldButton.interactable = true;
            _boughtOneCard = true;
        }

        private void SelectTwoFieldType()
        {
            if (_boughtTwoCard)
            {
                _selectedFieldType = 2;
                _twoFieldButton.interactable = false;
                _oneFieldButton.interactable = true;
            }
            
            if (!GameInstance.MoneyManager.HasEnoughCoinsCurrency(160) || _boughtTwoCard) return;
            GameInstance.MoneyManager.SpendCoinsCurrency(160);
            _selectedFieldType = 2;
            _twoFieldButton.interactable = false;
            _oneFieldButton.interactable = true;
            _boughtTwoCard = true;
        }
    }
}
﻿using System.Collections;
using System.Collections.Generic;
using Services;
using TMPro;
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
        [SerializeField] private CanvasGroup _topImageCanvasGroup;
        [SerializeField] private CanvasGroup _winPosesFrameCanvasGroup;
        [SerializeField] private CanvasGroup _playerImagesCanvasGroup;
        [SerializeField] private CanvasGroup _numbersUsedCanvasGroup;
        [SerializeField] private Button _oneFieldButton;
        [SerializeField] private Button _twoFieldButton;
        [SerializeField] private Button _skipButton;
        [SerializeField] private RectTransform _firstField;
        [SerializeField] private RectTransform _secondField;
        [SerializeField] private RectTransform _offer1;
        [SerializeField] private RectTransform _offer2;
        [SerializeField] private RectTransform _bingoImage1;
        [SerializeField] private RectTransform _bingoImage2;
        [SerializeField] private TextMeshProUGUI _xpCountText;
        [SerializeField] private TextMeshProUGUI _coinsCountText;
        [SerializeField] private TextMeshProUGUI _diamondsCountText;
        
        [Header("Classes")]
        public Timer Timer;
        public ImagesAnimation ImagesAnimation;
        public PlayerBingoController PlayerBingoController;
        public BingoMainController BingoMainController;
        public PlayerBankSaves PlayerBankSaves;
        
        [Header("Variables")] 
        [SerializeField] private int _selectedFieldType = 0;
        public bool GameRunning = false;
        
        private bool _boughtOneCard = false;
        private bool _boughtTwoCard = false;
        
        [HideInInspector] public int XpCount;
        [HideInInspector] public ulong CoinsCount;
        [HideInInspector] public ulong DiamondsCount;
        
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
            _skipButton.onClick.AddListener(OpenGameOverResultPopup);
            Timer.Init();
            Timer.OnTimerStopped += OpenGameField;
            Timer.OnGameTimerStopped += OpenGameOverPopup;
        }

        private void OnDestroy()
        {
            GameInstance.UINavigation.OnGameStarted -= StartComboAnim;
            GameInstance.UINavigation.OnGameWindowClosed -= ResetGame;
            GameInstance.UINavigation.OnGameStarted -= OpenPickCard;
            GameInstance.UINavigation.OnGameStarted -= UpdateStarButtons;
            Timer.OnTimerStopped -= OpenGameField;
            Timer.OnGameTimerStopped -= OpenGameOverPopup;
        }

        private void StartComboAnim()
        {
            _wonComboAnimator.SetTrigger("PlayCombo");
        }

        private void ResetGame()
        {
            GameRunning = false;
            _selectedFieldType = 0;
            _wonComboAnimator.SetTrigger("StopCombo");
            _twoFieldButton.interactable = true;
            _oneFieldButton.interactable = true;
            _boughtOneCard = false;
            _boughtTwoCard = false;
            _framesGroup.transform.localScale = new Vector3(1, 1, 1);
            ImagesAnimation.ResetImagesScale();
            _offer1.localScale = Vector3.zero;
            _offer2.localScale = Vector3.zero;
            _bingoImage2.localScale = Vector3.zero;
            _bingoImage1.localScale = Vector3.zero;
            ResetCounters();
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
            StartCoroutine(GameInstance.UINavigation.AnimateScale(_winPosesFrameCanvasGroup, true));
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

        public void PlayBingoAnimation(int index)
        {
            if (index == 1)
            {
                StartCoroutine(GameInstance.UINavigation.AnimateScaleAndMove(_bingoImage1, true,
                    new Vector3(0f, _bingoImage1.transform.localScale.y, _bingoImage1.transform.localScale.z)));
            }
            else if (index == 2)
            {
                StartCoroutine(GameInstance.UINavigation.AnimateScaleAndMove(_bingoImage2, true,
                    new Vector3(0f, _bingoImage2.transform.localScale.y, _bingoImage2.transform.localScale.z)));
            }
            
        }
        
        private void OpenGameOverPopup()
        {
            if(!GameRunning) return;
            GameInstance.FXController.PlayFireworksParticle();
            GameRunning = false;
            GameInstance.Audio.Play(GameInstance.Audio.GameOverSound);
            StartCoroutine(GameInstance.UINavigation.AnimateScale(_topImageCanvasGroup, false));
            StartCoroutine(GameInstance.UINavigation.AnimateScale(_winPosesFrameCanvasGroup, false));
            StartCoroutine(GameInstance.UINavigation.AnimateScale(_numbersUsedCanvasGroup, false));
            StartCoroutine(GameInstance.UINavigation.AnimateScale(_playerImagesCanvasGroup, false));
            
            GameInstance.UINavigation.OpenGroup(GameInstance.UINavigation.GamePopups[3]);
            
            if (_selectedFieldType == 1)
            {
                StartCoroutine(GameInstance.UINavigation.AnimateScaleAndMove(_firstField, true,
                    new Vector3(-379, _firstField.transform.localScale.y, _firstField.transform.localScale.z)));
                
                StartCoroutine(GameInstance.UINavigation.AnimateScaleAndMove(_offer1, true,
                    new Vector3(351f, _offer1.transform.localScale.y, _offer1.transform.localScale.z)));
            }
            else if (_selectedFieldType == 2)
            {
                StartCoroutine(GameInstance.UINavigation.AnimateScaleAndMove(_firstField, true,
                    new Vector3(-636f, _firstField.transform.localScale.y, _firstField.transform.localScale.z)));

                StartCoroutine(GameInstance.UINavigation.AnimateScaleAndMove(_secondField, true,
                    new Vector3(5f, _secondField.transform.localScale.y, _secondField.transform.localScale.z)));
                
                StartCoroutine(GameInstance.UINavigation.AnimateScaleAndMove(_offer1, true,
                    new Vector3(647f, _offer1.transform.localScale.y, _offer1.transform.localScale.z)));
            }
        }

        private void OpenGameOverResultPopup()
        {
            if (_selectedFieldType == 1)
            {
                StartCoroutine(GameInstance.UINavigation.AnimateScaleAndMove(_offer2, true,
                    new Vector3(351f, _offer2.transform.localScale.y, _offer2.transform.localScale.z)));
                
                StartCoroutine(GameInstance.UINavigation.AnimateScaleAndMove(_offer1, false,
                    new Vector3(351f, _offer1.transform.localScale.y, _offer1.transform.localScale.z)));
            }
            else if (_selectedFieldType == 2)
            {
                StartCoroutine(GameInstance.UINavigation.AnimateScaleAndMove(_offer2, true,
                    new Vector3(647f, _offer2.transform.localScale.y, _offer2.transform.localScale.z)));
                
                StartCoroutine(GameInstance.UINavigation.AnimateScaleAndMove(_offer1, false,
                    new Vector3(647f, _offer1.transform.localScale.y, _offer1.transform.localScale.z)));
            }

            switch (PlayerBingoController.GetBingoCount())
            {
                case 0:
                    CoinsCount = 80; 
                    DiamondsCount = 0;
                    XpCount = 25;
                    GameInstance.MoneyManager.ChangeWPValue(XpCount);
                    break;
                case 1:
                    CoinsCount = 300;  
                    DiamondsCount = 2;
                    XpCount = 40;
                    GameInstance.MapRoadNavigation.IncreaseSubLevel();
                    GameInstance.MoneyManager.ChangeWPValue(XpCount);
                    break;
                case 2:
                    CoinsCount = 450;
                    DiamondsCount = 4;
                    XpCount = 80;
                    GameInstance.MapRoadNavigation.IncreaseSubLevel();
                    GameInstance.MoneyManager.ChangeWPValue(XpCount);
                    break;
            }
            
            UpdateCountersText();
            PlayerBankSaves.AddCoins((int) CoinsCount);
        }
        
        private IEnumerator OpenGameFieldAnimation()
        {
            StartCoroutine(GameInstance.UINavigation.AnimateScale(GameInstance.UINavigation.GamePopups[4], false));
            
            yield return new WaitForSeconds(0.7f);
            
            _readyAnimator.SetTrigger("Ready");
            PlayReadySounds();
            StartCoroutine(ShowFieldAnimation());
        }

        private IEnumerator ShowFieldAnimation()
        {
            SetFieldPositions();
            PlayerBingoController.FillPlayerBoard(GetFieldCount());
            yield return new WaitForSeconds(3.5f);
            StartCoroutine(GameInstance.UINavigation.AnimateScale(_topImageCanvasGroup, true));
            StartCoroutine(GameInstance.UINavigation.AnimateScale(_framesGroup, false));
            StartCoroutine(GameInstance.UINavigation.AnimateScale(GameInstance.UINavigation.GamePopups[2], true));
            StartCoroutine(GameInstance.UINavigation.AnimateScale(_numbersUsedCanvasGroup, true));
            StartCoroutine(GameInstance.UINavigation.AnimateScale(_playerImagesCanvasGroup, true));
            ImagesAnimation.StartAnimation();
            GameRunning = true;
            BingoMainController.StartSpawnBalls();
            Timer.StartGameTimer();
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
        
        private void SetFieldPositions()
        {
            switch (_selectedFieldType)
            {
                case 1:
                    _secondField.gameObject.SetActive(false);
                    _firstField.localPosition = new Vector3(-16.8850002f,-123.919998f,0f);
                    break;
                case 2:
                    _secondField.gameObject.SetActive(true);
                    _firstField.localPosition = new Vector3(-381.390015f, -123.919998f, 0f);
                    _secondField.localPosition = new Vector3(347.609924f, -123.919998f, 0f);
                    break;
            }
        }

        private void PlayReadySounds()
        {
            StartCoroutine(ReadySound());
        }

        private IEnumerator ReadySound()
        { 
            yield return new WaitForSeconds(1.1f);
            GameInstance.Audio.Play(GameInstance.Audio.ReadySound);
            yield return new WaitForSeconds(1.5f);
            GameInstance.Audio.Play(GameInstance.Audio.GoSound);
        }
        
        public int GetFieldCount()
        {
            return _selectedFieldType;
        }

        private void UpdateCountersText()
        {
            _xpCountText.text = $"x{XpCount}";
            _coinsCountText.text = $"x{CoinsCount}";
            _diamondsCountText.text = $"x{DiamondsCount}";
        }

        private void ResetCounters()
        {
            CoinsCount = 0;
            DiamondsCount = 0;
            XpCount = 0;
        }
    }
}
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
        
        [Header("Classes")]
        public Timer Timer;
        public ImagesAnimation ImagesAnimation;
        public PlayerBingoController PlayerBingoController;
        public BingoMainController BingoMainController;
        
        [Header("Variables")] 
        [SerializeField] private int _selectedFieldType = 0;
        public bool GameRunning = false;
        
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

        private void OpenGameOverPopup()
        {
            GameRunning = false;
            
            StartCoroutine(GameInstance.UINavigation.AnimateScale(_topImageCanvasGroup, false));
            StartCoroutine(GameInstance.UINavigation.AnimateScale(_winPosesFrameCanvasGroup, false));
            StartCoroutine(GameInstance.UINavigation.AnimateScale(_numbersUsedCanvasGroup, false));
            StartCoroutine(GameInstance.UINavigation.AnimateScale(_playerImagesCanvasGroup, false));
            
            GameInstance.UINavigation.OpenGroup(GameInstance.UINavigation.GamePopups[3]);
            
            if (_selectedFieldType == 1)
            {
                StartCoroutine(GameInstance.UINavigation.AnimateScaleAndMove(_firstField, true,
                    new Vector3(/*-636f*/-379, _firstField.transform.localScale.y, _firstField.transform.localScale.z)));
                
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

        public int GetFieldCount()
        {
            return _selectedFieldType;
        }
    }
}
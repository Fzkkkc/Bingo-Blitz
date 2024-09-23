using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GameCore
{
    public class GameState : MonoBehaviour
    {
        public enum Potion
        {
            YellowPotion,
            PurplePotion,
            GreenTallPotion,
            GreenSmallPotion,
            BlackPotion,
            RainbowPotion,
            Mint
        }

        [SerializeField] private List<Sprite> _manBigSprites;
        [SerializeField] private List<Sprite> _orderSprites;
        [SerializeField] private List<Sprite> _bottleSprites;
        [SerializeField] private List<Button> _potionButtons;
        [SerializeField] private Image _manImage;
        [SerializeField] private Image _orderImage;
        [SerializeField] private Image _bottleImage;
        [SerializeField] private Animator _gameAnimator;
        private int _manIndex;
        private int _orderIndex;

        private List<Potion> _selectedPotions = new List<Potion>();
        private List<Potion> _requiredPotions;
        private int _customerCounter = 0;
        public ulong CurrentAwardCount;

        private float _gameStartTime;
        private bool _gameEnded = false;

        public void Init()
        {
            SetupPotionButtons();
            GameInstance.UINavigation.OnGameStarted += ResetGame;
            GameInstance.UINavigation.OnGameRestarted += RestartGame;
            GameInstance.UINavigation.OnGameWindowClosed += ResetAnimator;
        }

        private void OnDestroy()
        {
            GameInstance.UINavigation.OnGameStarted -= ResetGame;
            GameInstance.UINavigation.OnGameRestarted -= RestartGame;
            GameInstance.UINavigation.OnGameWindowClosed -= ResetAnimator;
        }

        public void SelectNextMan()
        {
            _manIndex = Random.Range(0, _manBigSprites.Count);
            _orderIndex = Random.Range(0, _orderSprites.Count);

            _manImage.sprite = _manBigSprites[_manIndex];
            _orderImage.sprite = _orderSprites[_orderIndex];
            _gameAnimator.SetTrigger("NextMan");
            GameInstance.FXController.StateBoilingFX(true);
            _selectedPotions.Clear();
            _requiredPotions = GetRequiredPotions(_orderIndex);
        }

        public void ShowOrder()
        {
            _gameAnimator.SetTrigger("ShowOrder");
            GameInstance.FXController.StateBoilingFX(false);
        }
        
        public void CloseOrder()
        {
            _gameAnimator.SetTrigger("CloseOrder");
            GameInstance.FXController.StateBoilingFX(true);
        }
        
        public void ShowBook()
        {
            _gameAnimator.SetTrigger("ShowBook");
            GameInstance.FXController.StateBoilingFX(false);
        }
        
        public void CloseBook()
        {
            _gameAnimator.SetTrigger("CloseBook");
            GameInstance.FXController.StateBoilingFX(true);
        }
        
        public void RightPotion()
        {
            _gameAnimator.SetTrigger("RightPotion");
            GameInstance.FXController.StateBoilingFX(false);
            GameInstance.FXController.PlayDonePotionFX();
            StartCoroutine(CheckWin());
        }

        public void AddPotion(Potion potion, Button button, Color color)
        {
            if (!_requiredPotions.Contains(potion))
            {
                GameInstance.UINavigation.OpenGameOverPopup(false);
                GameInstance.FXController.StateBoilingFX(false);
                return;
            }

            if (!_selectedPotions.Contains(potion))
            {
                _selectedPotions.Add(potion);
                button.interactable = false;
                
                
                GameInstance.FXController.PlayChanPotionFX(color);
                
                GameInstance.FXController.ChangeBoilingFXColor(color);
                
                if (CheckCombination())
                {
                    RightPotion();
                }
            }
        }

        public void ContinueGame()
        {
            if (GameInstance.Timer.GetRemainingTime() == 0)
            {
                GameInstance.Timer.ResetTimer();
            }
            GameInstance.Timer.ResumeTimer();
            ResetAnimator();
            SelectNextMan();
            ResetButtons();
        }

        private void UpdateBottleImage()
        {
            _bottleImage.sprite = _bottleSprites[_customerCounter];
        }
        
        private IEnumerator CheckWin()
        {
            yield return new WaitForSeconds(4f);
            
            _customerCounter++;
            UpdateBottleImage();
            GameInstance.FXController.PlayCoinsFX();
            CurrentAwardCount += 25;
            GameInstance.Audio.Play(GameInstance.Audio.CoinSound);
            if (_customerCounter == 5)
            {
                _gameEnded = true;
                GameInstance.UINavigation.OpenGameOverPopup(true);
                SaveGameResults();
            }
            else
            {
                SelectNextMan();
                ResetButtons();
            }
        }

        private void SaveGameResults()
        {
            if (_gameEnded)
            {
                float gameDuration = Time.time - _gameStartTime;
                int minutes = Mathf.FloorToInt(gameDuration / 60);
                int seconds = Mathf.FloorToInt(gameDuration % 60);
                int moneyEarned = (int)CurrentAwardCount;

                SaveResult(minutes, seconds, moneyEarned);
            }
        }

        private void SaveResult(int minutes, int seconds, int moneyEarned)
        {
            string timeFormatted = $"{minutes:D2}:{seconds:D2}";

            var savedResults = LoadResults();

            var newResult = new GameResult(timeFormatted, moneyEarned);

            savedResults.Add(newResult);

            savedResults.Sort();

            if (savedResults.Count > 9)
            {
                savedResults.RemoveAt(savedResults.Count - 1);
            }

            SaveResults(savedResults);
        }


        private List<GameResult> LoadResults()
        {
            var results = new List<GameResult>();
            int count = PlayerPrefs.GetInt("ResultCount", 0);
            for (int i = 0; i < count; i++)
            {
                string time = PlayerPrefs.GetString($"Result_{i}_Time");
                int money = PlayerPrefs.GetInt($"Result_{i}_Money");
                results.Add(new GameResult(time, money));
            }
            return results;
        }

        private void SaveResults(List<GameResult> results)
        {
            PlayerPrefs.SetInt("ResultCount", results.Count);
            for (int i = 0; i < results.Count; i++)
            {
                PlayerPrefs.SetString($"Result_{i}_Time", results[i].Time);
                PlayerPrefs.SetInt($"Result_{i}_Money", results[i].Money);
            }
        }

        private void SetupPotionButtons()
        {
            if (_potionButtons.Count == 7) 
            {
                _potionButtons[0].onClick.AddListener(() => AddPotion(Potion.YellowPotion, _potionButtons[0], Color.yellow));
                _potionButtons[1].onClick.AddListener(() => AddPotion(Potion.PurplePotion,_potionButtons[1], Color.magenta));
                _potionButtons[2].onClick.AddListener(() => AddPotion(Potion.GreenTallPotion, _potionButtons[2], Color.green));
                _potionButtons[3].onClick.AddListener(() => AddPotion(Potion.GreenSmallPotion, _potionButtons[3], Color.green));
                _potionButtons[4].onClick.AddListener(() => AddPotion(Potion.BlackPotion, _potionButtons[4], Color.black));
                _potionButtons[5].onClick.AddListener(() => AddPotion(Potion.RainbowPotion, _potionButtons[5], Color.blue));
                _potionButtons[6].onClick.AddListener(() => AddPotion(Potion.Mint, _potionButtons[6], Color.green));
            }
        }

        public bool CheckCombination()
        {
            if (_requiredPotions.Count != _selectedPotions.Count)
            {
                return false;
            }

            foreach (Potion potion in _requiredPotions)
            {
                if (!_selectedPotions.Contains(potion))
                {
                    return false;
                }
            }

            return true;
        }

        private List<Potion> GetRequiredPotions(int orderIndex)
        {
            switch (orderIndex)
            {
                case 0: return new List<Potion> { Potion.BlackPotion, Potion.RainbowPotion };
                case 1: return new List<Potion> { Potion.BlackPotion, Potion.Mint };
                case 2: return new List<Potion> { Potion.Mint, Potion.YellowPotion };
                case 3: return new List<Potion> { Potion.PurplePotion, Potion.YellowPotion };
                case 4: return new List<Potion> { Potion.PurplePotion, Potion.Mint };
                case 5: return new List<Potion> { Potion.GreenSmallPotion, Potion.RainbowPotion };
                case 6: return new List<Potion> { Potion.BlackPotion, Potion.Mint, Potion.YellowPotion };
                case 7: return new List<Potion> { Potion.RainbowPotion, Potion.Mint, Potion.GreenTallPotion };
                case 8: return new List<Potion> { Potion.PurplePotion, Potion.Mint, Potion.GreenSmallPotion };
                case 9: return new List<Potion> { Potion.PurplePotion, Potion.GreenTallPotion, Potion.YellowPotion };
                default: return new List<Potion>();
            }
        }

        private void ResetGame()
        {
            ResetButtons();
            CurrentAwardCount = 0;
            _customerCounter = 0;
            _gameStartTime = Time.time; 
            UpdateBottleImage();
            SelectNextMan();
        }

        private void RestartGame()
        {
            ResetButtons();
            _customerCounter = 0;
            _gameStartTime = Time.time; 
            UpdateBottleImage();
            SelectNextMan();
        }

        private void ResetButtons()
        {
            foreach (var button in _potionButtons)
            {
                button.interactable = true;
            }
        }

        private void ResetAnimator()
        {
            _gameAnimator.SetTrigger("ToIdle");
        }
    }
}

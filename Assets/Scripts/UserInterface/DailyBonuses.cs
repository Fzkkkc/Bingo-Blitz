using System;
using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class DailyBonus : MonoBehaviour
    {
        public List<Button> _dailyButtons;
        public Button _grabButton;

        private int _currentDay = 0;
        private int _maxDays = 7;
        private DateTime _lastClaimDate;

        public void Init()
        {
            LoadCurrentDayAndDate();
            CheckIfBonusCanBeClaimed();
            UpdateClaimedBonuses();
            _grabButton.onClick.AddListener(GrabBonus);
        }

        private void GrabBonus()
        {
            if (_currentDay >= _maxDays)
            {
                _currentDay = 0;
                UpdateClaimedBonuses();
            }

            switch (_currentDay)
            {
                case 0: 
                    GameInstance.MoneyManager.AddCoinsCurrency(50); 
                    break;
                case 1: 
                    GameInstance.MoneyManager.AddDiamondsCurrency(4); 
                    break;
                case 2: 
                    GameInstance.MoneyManager.AddCoinsCurrency(60); 
                    break;
                case 3: 
                    GameInstance.MoneyManager.ChangeWPValue(50); 
                    break;
                case 4: 
                    GameInstance.MoneyManager.AddCoinsCurrency(70); 
                    break;
                case 5: 
                    GameInstance.MoneyManager.AddCoinsCurrency(80); 
                    break;
                case 6: 
                    GameInstance.MoneyManager.ChangeWPValue(40); 
                    GameInstance.MoneyManager.AddDiamondsCurrency(2);
                    GameInstance.MoneyManager.AddCoinsCurrency(80);
                    break;
            }
            
            _currentDay++;
            _lastClaimDate = DateTime.Today;
            SaveCurrentDayAndDate();
            _grabButton.interactable = false;
            UpdateClaimedBonuses();
            GameInstance.UINavigation.CloseDailyBonusUI();
        }
        
        private void ShowChildImage(Button parentButton)
        {
            parentButton.gameObject.SetActive(true);
        }

        private void UpdateClaimedBonuses()
        {
            for (int i = 0; i < _currentDay; i++)
            {
                ShowChildImage(_dailyButtons[i]);
            }
        }

        private void CheckIfBonusCanBeClaimed()
        {
            var today = DateTime.Today;

            if (_lastClaimDate != today)
            {
                _grabButton.interactable = true;
                GameInstance.UINavigation.OpenDailyBonusUI();
            }
            else
            {
                _grabButton.interactable = false;
            }
        }

        private void SaveCurrentDayAndDate()
        {
            PlayerPrefs.SetInt("DailyBonusDay", _currentDay);
            PlayerPrefs.SetString("LastClaimDate", _lastClaimDate.ToString("yyyy-MM-dd"));
            PlayerPrefs.Save();
        }

        private void LoadCurrentDayAndDate()
        {
            _currentDay = PlayerPrefs.GetInt("DailyBonusDay", 0);
            string savedDate = PlayerPrefs.GetString("LastClaimDate", "");
            if (DateTime.TryParse(savedDate, out DateTime parsedDate))
            {
                _lastClaimDate = parsedDate;
            }
            else
            {
                _lastClaimDate = DateTime.MinValue;
                Debug.Log($"_lastClaimDate {_lastClaimDate}");
            }
        }
    }
}

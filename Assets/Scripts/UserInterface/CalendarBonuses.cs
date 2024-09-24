using System;
using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class CalendarBonuses : MonoBehaviour
    {
        public List<Image> _dailyBonusImages;
        public Button _grabButton;
        public Sprite coinSprite;
        public Sprite diamondSprite;
        public Sprite wpSprite;

        private int _currentDay = 0;
        private int _maxDays = 30;
        private DateTime _lastClaimDate;
        private List<int> _rewardsForDays = new List<int>();

        private const string CurrentDayKey = "CalendarBonusDay";
        private const string LastClaimDateKey = "LastCalendarClaimDate";
        private const string RewardsForDaysKey = "CalendarBonusRewards";

        public void Start()
        {
            LoadCurrentDayAndDate();
            LoadRewardsForDays();
            CheckIfBonusCanBeClaimed();
            DeactivateImages();
            UpdateClaimedBonuses();
            _grabButton.onClick.AddListener(GrabBonus);
        }

        private void GrabBonus()
        {
            if (_currentDay >= _maxDays)
            {
                _currentDay = 0;
                _rewardsForDays.Clear();
            }

            int reward = UnityEngine.Random.Range(1, 4); 
            _rewardsForDays.Add(reward);
            GiveReward(reward);

            SetRewardSprite(_dailyBonusImages[_currentDay], reward);

            _currentDay++;
            _lastClaimDate = DateTime.Today;

            SaveCurrentDayAndDate();
            SaveRewardsForDays();

            _grabButton.interactable = false;
            GameInstance.UINavigation.CloseCalendarBonusUI();
        }

        private void GiveReward(int rewardType)
        {
            switch (rewardType)
            {
                case 1: // WP
                    GameInstance.MoneyManager.ChangeWPValue(10);
                    break;
                case 2: // Coins
                    GameInstance.MoneyManager.AddCoinsCurrency(10);
                    break;
                case 3: // Diamonds
                    GameInstance.MoneyManager.AddDiamondsCurrency(5);
                    break;
            }
        }

        private void SetRewardSprite(Image bonusImage, int rewardType)
        {
            bonusImage.enabled = true;
            switch (rewardType)
            {
                case 1:
                    bonusImage.sprite = wpSprite;
                    break;
                case 2:
                    bonusImage.sprite = coinSprite;
                    break;
                case 3:
                    bonusImage.sprite = diamondSprite;
                    break;
            }
            bonusImage.gameObject.SetActive(true);
        }

        private void UpdateClaimedBonuses()
        {
            for (int i = 0; i < _currentDay; i++)
            {
                int rewardType = _rewardsForDays[i];
                SetRewardSprite(_dailyBonusImages[i], rewardType);
            }
        }

        private void CheckIfBonusCanBeClaimed()
        {
            var today = DateTime.Today;

            _grabButton.interactable = _lastClaimDate != today;
        }

        private void SaveCurrentDayAndDate()
        {
            PlayerPrefs.SetInt(CurrentDayKey, _currentDay);
            PlayerPrefs.SetString(LastClaimDateKey, _lastClaimDate.ToString("yyyy-MM-dd"));
            PlayerPrefs.Save();
        }

        private void LoadCurrentDayAndDate()
        {
            _currentDay = PlayerPrefs.GetInt(CurrentDayKey, 0);
            string savedDate = PlayerPrefs.GetString(LastClaimDateKey, "");
            if (DateTime.TryParse(savedDate, out DateTime parsedDate))
            {
                _lastClaimDate = parsedDate;
            }
            else
            {
                _lastClaimDate = DateTime.MinValue;
            }
        }

        private void SaveRewardsForDays()
        {
            string rewardsString = string.Join(",", _rewardsForDays);
            PlayerPrefs.SetString(RewardsForDaysKey, rewardsString);
            PlayerPrefs.Save();
        }

        private void LoadRewardsForDays()
        {
            string savedRewards = PlayerPrefs.GetString(RewardsForDaysKey, "");
            if (!string.IsNullOrEmpty(savedRewards))
            {
                string[] rewardsArray = savedRewards.Split(',');
                foreach (string reward in rewardsArray)
                {
                    if (int.TryParse(reward, out int rewardValue))
                    {
                        _rewardsForDays.Add(rewardValue);
                    }
                }
            }
        }

        private void DeactivateImages()
        {
            foreach (var image in _dailyBonusImages)
            {
                image.enabled = false;
            }
        }
    }
}

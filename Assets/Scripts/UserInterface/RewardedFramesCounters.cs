using System;
using Services;
using TMPro;
using UnityEngine;

namespace UserInterface
{
    public class RewardedFramesCounters : MonoBehaviour
    {
        private int _coinAdCounter;
        private int _diamondAdCounter;

        [SerializeField] private TextMeshProUGUI _coinCounterText;
        [SerializeField] private TextMeshProUGUI _diamondCounterText;
        
        private const string CoinAdCounterKey = "CoinAdCounter";
        private const string DiamondAdCounterKey = "DiamondAdCounter";
        private const string LastResetDateKey = "LastAdCounterResetDate";
        
        public void Start()
        {
            CheckAndResetCounters();
            LoadCounters();
            UpdateCountersText();
        }
        
        public void IncreaseAdCounter(string adType)
        {
            if (adType == "coin")
            {
                _coinAdCounter++;
                PlayerPrefs.SetInt(CoinAdCounterKey, _coinAdCounter);
                GameInstance.MoneyManager.AddCoinsCurrency(5);
            }
            else if (adType == "diamond")
            {
                _diamondAdCounter++;
                PlayerPrefs.SetInt(DiamondAdCounterKey, _diamondAdCounter);
                GameInstance.MoneyManager.AddDiamondsCurrency(2);
            }

            UpdateCountersText();
            PlayerPrefs.Save();
        }
        
        private void LoadCounters()
        {
            _coinAdCounter = PlayerPrefs.GetInt(CoinAdCounterKey, 0);
            _diamondAdCounter = PlayerPrefs.GetInt(DiamondAdCounterKey, 0);
        }
        
        private void CheckAndResetCounters()
        {
            string lastResetDateString = PlayerPrefs.GetString(LastResetDateKey, "");
            DateTime lastResetDate;

            if (!DateTime.TryParse(lastResetDateString, out lastResetDate) || lastResetDate.Date != DateTime.Today)
            {
                ResetCounters();
                PlayerPrefs.SetString(LastResetDateKey, DateTime.Today.ToString("yyyy-MM-dd"));
                PlayerPrefs.Save();
            }
        }

        private void ResetCounters()
        {
            _coinAdCounter = 0;
            _diamondAdCounter = 0;

            PlayerPrefs.SetInt(CoinAdCounterKey, _coinAdCounter);
            PlayerPrefs.SetInt(DiamondAdCounterKey, _diamondAdCounter);
        }
        
        private void UpdateCountersText()
        {
            _diamondCounterText.text = $"{_diamondAdCounter}/3";
            _coinCounterText.text = $"{_coinAdCounter}/2";
        }
    }
}
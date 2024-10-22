using System;
using UnityEngine;

namespace Services
{
    public class MoneyManager : MonoBehaviour
    {
        private ulong _coins;
        public Action<ulong> OnCoinsCurrencyChange;
        public Action OnCoinsCurrencyValueChange;
        
        private ulong _diamonds;
        public Action<ulong> OnDiamondsCurrencyChange;
        public Action OnDiamondsCurrencyValueChange;
        
        private int _wp;
        private int _level;
        private const int ExperiencePerLevel = 200;
        
        public Action<int> OnLevelCountChange;
        public Action<int> OnWPCountChange;
        
        private ulong PrefsMoney
        {
            get => ulong.Parse(PlayerPrefs.GetString("PREFS_Money", "1500"));
            set => PlayerPrefs.SetString("PREFS_Money", value.ToString());
        }
        
        private ulong PrefsDiamonds
        {
            get => ulong.Parse(PlayerPrefs.GetString("PREFS_Diamonds", "0"));
            set => PlayerPrefs.SetString("PREFS_Diamonds", value.ToString());
        }
        
        private int PrefsWP
        {
            get => int.Parse(PlayerPrefs.GetString("PREFS_WP", "0"));
            set => PlayerPrefs.SetString("PREFS_WP", value.ToString());
        }
        
        private int PrefsLevel
        {
            get => int.Parse(PlayerPrefs.GetString("PREFS_Lvl", "0")); 
            set => PlayerPrefs.SetString("PREFS_Lvl", value.ToString());
        }
        
        public void Init(ulong startMoney)
        {
            _coins = PrefsMoney;
            _diamonds = PrefsDiamonds;
            _wp = PrefsWP;
            _level = PrefsLevel;
            /*AddCoinsCurrency(2000);
            AddDiamondsCurrency(20);*/
            //PlayerPrefs.DeleteAll();
        }
        
        public void ChangeWPValue(int count)
        {
            _wp += count;
            PrefsWP = _wp; 
            CheckLevelUp();
            OnWPCountChange?.Invoke(_wp);
        }

        private void CheckLevelUp()
        {
            if (_wp >= ExperiencePerLevel)
            {
                ChangeLevelValue(1);
                ResetWPValue(); 
            }
        }

        private void ChangeLevelValue(int count)
        {
            _level += count;
            PrefsLevel = _level; 
            OnLevelCountChange?.Invoke(_level);
        }

        private void ResetWPValue()
        {
            _wp = 0;
            PrefsWP = _wp; 
            OnWPCountChange?.Invoke(_wp);
        }
        
        public ulong GetCoinsCurrency()
        {
            return _coins;
        }
        
        public ulong GetDiamondsCurrency()
        {
            return _diamonds;
        }
        
        public int GetLevelCount()
        {
            return _level;
        }
        
        public int GetWpCount()
        {
            return _wp;
        }
        
        public void AddCoinsCurrency(ulong count)
        {
            PrefsMoney = _coins = (_coins + count);
            OnCoinsCurrencyChange?.Invoke(_coins);
            OnCoinsCurrencyValueChange?.Invoke();
        }
        
        public void AddDiamondsCurrency(ulong count)
        {
            PrefsDiamonds = _diamonds = (_diamonds + count);
            OnDiamondsCurrencyChange?.Invoke(_diamonds);
            OnDiamondsCurrencyValueChange?.Invoke();
        }
        
        public void SpendCoinsCurrency(ulong count)
        {
            ulong result = 0UL;
            if (_coins >= count)
                result = _coins - count;
            PrefsMoney = _coins = result;
            OnCoinsCurrencyChange?.Invoke(_coins);
            OnCoinsCurrencyValueChange?.Invoke();
        }
        
        public void SpendDiamondsCurrency(ulong count)
        {
            ulong result = 0UL;
            if (_diamonds >= count)
                result = _diamonds - count;
            PrefsDiamonds = _diamonds = result;
            OnDiamondsCurrencyChange?.Invoke(_diamonds);
            OnDiamondsCurrencyValueChange?.Invoke();
        }
        
        public bool HasEnoughCoinsCurrency(ulong amount)
        {
            return _coins >= amount;
        }
        
        public bool HasEnoughDiamondsCurrency(ulong amount)
        {
            return _diamonds >= amount;
        }
    }
}
using System;
using UnityEngine;

namespace Services
{
    public class MoneyManager : MonoBehaviour
    {
        private ulong _coins;
        public Action<ulong> OnCoinsCurrencyChange;
        public Action OnCoinsCurrencyValueChange;
        
        private ulong PrefsMoney
        {
            get => ulong.Parse(PlayerPrefs.GetString("PREFS_Money", "0"));
            set => PlayerPrefs.SetString("PREFS_Money", value.ToString());
        }
        public void Init(ulong startMoney)
        {
            _coins = PrefsMoney;
            //AddCoinsCurrency(startMoney);
            //PlayerPrefs.DeleteAll();
        }
        
        public ulong GetCoinsCurrency()
        {
            return _coins;
        }
        
        public void AddCoinsCurrency(ulong count)
        {
            PrefsMoney = _coins = (_coins + count);
            OnCoinsCurrencyChange?.Invoke(_coins);
            OnCoinsCurrencyValueChange?.Invoke();
            
            if(count != 0)
                GameInstance.Audio.Play(GameInstance.Audio.CoinSound);
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
        
        public bool HasEnoughCoinsCurrency(ulong amount)
        {
            return _coins >= amount;
        }
    }
}
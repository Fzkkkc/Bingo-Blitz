using System.Collections.Generic;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore
{
    public class PotionShop : MonoBehaviour
    {
        [SerializeField] private Image _chanImage; 
        [SerializeField] private Image _bookImage; 
        [SerializeField] private List<Sprite> _chanSprites; 
        [SerializeField] private List<Sprite> _bookSprites; 

        [SerializeField] private List<Button> _chanButtons;
        [SerializeField] private List<Button> _bookButtons;

        [SerializeField] private Sprite _boughtButtonSprite; 

        private int _currentChanIndex;
        private int _currentBookIndex;

        private const string ChanKey = "ChanIndex"; 
        private const string BookKey = "BookIndex";

        private void Start()
        {
            _currentChanIndex = PlayerPrefs.GetInt(ChanKey, -1); 
            _currentBookIndex = PlayerPrefs.GetInt(BookKey, -1);

            UpdateUI();
        }

        public void PurchaseChan(int index)
        {
            if (PlayerPrefs.GetInt("ChanPurchased_" + index, 0) == 1)
            {
                EquipChan(index);
            }
            else
            {
                if(!GameInstance.MoneyManager.HasEnoughCoinsCurrency(600)) return;
                GameInstance.MoneyManager.SpendCoinsCurrency(600);
                PlayerPrefs.SetInt("ChanPurchased_" + index, 1);
                EquipChan(index);
            }
        }

        private void EquipChan(int index)
        {
            _currentChanIndex = index;
            PlayerPrefs.SetInt(ChanKey, _currentChanIndex);

            if (_currentChanIndex >= 0 && _currentChanIndex < _chanSprites.Count)
            {
                _chanImage.sprite = _chanSprites[_currentChanIndex];
            }
            
            UpdateUI();
        }

        public void PurchaseBook(int index)
        {
            if (PlayerPrefs.GetInt("BookPurchased_" + index, 0) == 1)
            {
                EquipBook(index);
            }
            else
            {
                if(!GameInstance.MoneyManager.HasEnoughCoinsCurrency(600)) return;
                GameInstance.MoneyManager.SpendCoinsCurrency(600);
                PlayerPrefs.SetInt("BookPurchased_" + index, 1);
                EquipBook(index);
            }
        }

        private void EquipBook(int index)
        {
            _currentBookIndex = index;
            PlayerPrefs.SetInt(BookKey, _currentBookIndex);

            if (_currentBookIndex >= 0 && _currentBookIndex < _bookSprites.Count)
            {
                _bookImage.sprite = _bookSprites[_currentBookIndex];
            }

            UpdateUI();
        }

        private void UpdateUI()
        {
            for (int i = 0; i < _chanButtons.Count; i++)
            {
                bool isPurchased = PlayerPrefs.GetInt("ChanPurchased_" + i, 0) == 1;

                _chanButtons[i].interactable = !isPurchased || _currentChanIndex != i;

                if (isPurchased)
                {
                    _chanButtons[i].image.sprite = _boughtButtonSprite;
                    _chanButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = (_currentChanIndex == i) ? "Equipped" : "Equip";
                }
                else
                {
                    _chanButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = "600";
                }
            }

            for (int i = 0; i < _bookButtons.Count; i++)
            {
                bool isPurchased = PlayerPrefs.GetInt("BookPurchased_" + i, 0) == 1;

                _bookButtons[i].interactable = !isPurchased || _currentBookIndex != i;

                if (isPurchased)
                {
                    _bookButtons[i].image.sprite = _boughtButtonSprite;
                    _bookButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = (_currentBookIndex == i) ? "Equipped" : "Equip";
                }
                else
                {
                    _bookButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = "600";
                }
            }
        }
    }
}

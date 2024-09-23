using System.Collections.Generic;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore
{
    public class BackgroundShop : MonoBehaviour
    {
        [SerializeField] private Image _menuBg;
        [SerializeField] private Image _gameBg;
        [SerializeField] private List<Sprite> _bgSprites;

        [SerializeField] private Button _leftButton;
        [SerializeField] private Button _rightButton;
        [SerializeField] private Button _purchaseButton; 
        [SerializeField] private Image _lockImage;
        [SerializeField] private Sprite _boughtButtonSprite;
        [SerializeField] private Sprite _defaultButtonSprite; 

        private int _currentBgIndex;
        private int _equippedBgIndex; 
        private const string BgKey = "BgIndex"; 

        private void Start()
        {
            if (PlayerPrefs.GetInt("BgPurchased_0", 0) == 0)
            {
                PlayerPrefs.SetInt("BgPurchased_0", 1);
            }

            _equippedBgIndex = PlayerPrefs.GetInt(BgKey, 0);
            _currentBgIndex = _equippedBgIndex;
            UpdateBackground();
            UpdateUI();

            _leftButton.onClick.AddListener(OnLeftButtonClick);
            _rightButton.onClick.AddListener(OnRightButtonClick);
            _purchaseButton.onClick.AddListener(OnPurchaseButtonClick);
        }

        private void UpdateBackground()
        {
            if (_bgSprites.Count > 0)
            {
                if (_currentBgIndex >= 0 && _currentBgIndex < _bgSprites.Count)
                {
                    _menuBg.sprite = _bgSprites[_currentBgIndex];
                    _gameBg.sprite = _bgSprites[_currentBgIndex];
                }
            }
        }

        private void OnLeftButtonClick()
        {
            _currentBgIndex = (_currentBgIndex - 1 + _bgSprites.Count) % _bgSprites.Count;
            UpdateBackground();
            UpdateUI();
        }

        private void OnRightButtonClick()
        {
            _currentBgIndex = (_currentBgIndex + 1) % _bgSprites.Count;
            UpdateBackground();
            UpdateUI();
        }

        private void OnPurchaseButtonClick()
        {
            if (PlayerPrefs.GetInt("BgPurchased_" + _currentBgIndex, 0) == 1)
            {
                EquipBackground();
            }
            else
            {
                if(!GameInstance.MoneyManager.HasEnoughCoinsCurrency(1000)) return;

                GameInstance.MoneyManager.SpendCoinsCurrency(1000);
                PlayerPrefs.SetInt("BgPurchased_" + _currentBgIndex, 1);
                EquipBackground();
            }
        }

        private void EquipBackground()
        {
            _equippedBgIndex = _currentBgIndex;
            PlayerPrefs.SetInt(BgKey, _equippedBgIndex);
            UpdateBackground();
            UpdateUI();
        }

        public void RestoreOriginalBackground()
        {
            _currentBgIndex = _equippedBgIndex;
            UpdateBackground();
            UpdateUI();
        }

        private void UpdateUI()
        {
            var isPurchased = PlayerPrefs.GetInt("BgPurchased_" + _currentBgIndex, 0) == 1;

            _lockImage.gameObject.SetActive(!isPurchased);
            _purchaseButton.image.sprite = isPurchased ? _boughtButtonSprite : _defaultButtonSprite;

            var buttonText = _purchaseButton.GetComponentInChildren<TextMeshProUGUI>();
            if (isPurchased)
            {
                if (_equippedBgIndex == _currentBgIndex)
                {
                    buttonText.text = "Equipped";
                    _purchaseButton.interactable = false; 
                }
                else
                {
                    buttonText.text = "Equip";
                    _purchaseButton.interactable = true; 
                }
            }
            else
            {
                buttonText.text = "1000";
                _purchaseButton.interactable = true; 
            }
        }
    }
}

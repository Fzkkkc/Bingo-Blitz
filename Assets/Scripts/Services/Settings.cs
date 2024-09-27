using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_IOS
using UnityEngine.iOS;
#endif

namespace Services
{
    public class Settings : MonoBehaviour
    {
        [SerializeField] private List<Button> _settingsButtons;

        [SerializeField] private string _policyString;
        [SerializeField] private string _appURLString;

        [SerializeField] private Button _policyButton;
        [SerializeField] private Button _shareButton;
        [SerializeField] private Button _rateButton;
        [SerializeField] private Button _soundButton;
        [SerializeField] private Button _mainButton;

        [SerializeField] private Sprite _volumeOffSprite;
        private Sprite _volumeOnSprite;
        
        [SerializeField] private Image _settingsImage;
        [SerializeField] private Sprite _settingsOpenSprite;
        private Sprite _settingsCloseSprite;
        
        [SerializeField] private float _animationDuration = 1f;  

        private bool _isOpen = false;
        private bool _isOn = false;

        private void OnValidate()
        {
            _settingsImage ??= GetComponent<Image>();
        }

        private void Start()
        {
            _policyButton.onClick.AddListener(PolicyView);
            _shareButton.onClick.AddListener(ShareApp);
            _rateButton.onClick.AddListener(RateApp);
            _soundButton.onClick.AddListener(ChangeSoundVolume);
            _mainButton.onClick.AddListener(MainButtonState);

            _volumeOnSprite = _soundButton.image.sprite;
            _settingsCloseSprite = _settingsImage.sprite;
            foreach (var button in _settingsButtons)
            {
                button.image.raycastTarget = false;
            }

            ChangeSoundVolume();
        }

        private void ChangeSoundVolume()
        {
            if (_isOn)
            {
                _isOn = false;
                _soundButton.image.sprite = _volumeOffSprite;
                GameInstance.Audio.Volume = 0f;
                //_bgMusic.volume = 0f;
            }
            else
            {
                _isOn = true;
                _soundButton.image.sprite = _volumeOnSprite;
                GameInstance.Audio.Volume = 0.8f;
                //_bgMusic.volume = 0.4f;
            }
            
            GameInstance.MusicSystem.source.volume = GameInstance.Audio.Volume - 0.3f;
        }
        
        private void ShareApp()
        {
            if(_appURLString != "")
                Application.OpenURL(_appURLString);
        }

        private void PolicyView()
        {
            if(_policyString != "")
                Application.OpenURL(_policyString);
        }

        private void RateApp()
        {
#if UNITY_IOS
            Device.RequestStoreReview();
#endif
        }

        private void MainButtonState()
        {
            StartCoroutine(_isOpen ? HideSettingsButtons() : ShowSettingsButtons());
        }
        
        private IEnumerator ShowSettingsButtons()
        {
            _isOpen = true;
            _mainButton.interactable = false;
            float stepDelay = _animationDuration / _settingsButtons.Count; 

            for (int i = 0; i < _settingsButtons.Count; i++)
            {
                Button button = _settingsButtons[i];
                Image buttonImage = button.GetComponent<Image>();
                RectTransform rectTransform = button.GetComponent<RectTransform>();

                StartCoroutine(AnimateButton(button, buttonImage, rectTransform, -108f * (i + 1), true));

                yield return new WaitForSeconds(stepDelay);
            }

            foreach (var button in _settingsButtons)
            {
                button.image.raycastTarget = true;
            }

            _settingsImage.sprite = _settingsOpenSprite;
            _mainButton.interactable = true;
        }

        public IEnumerator HideSettingsButtons()
        {
            _isOpen = false;
            _mainButton.interactable = false;
            float stepDelay = _animationDuration / _settingsButtons.Count; 

            for (int i = 0; i < _settingsButtons.Count; i++)
            {
                Button button = _settingsButtons[i];
                Image buttonImage = button.GetComponent<Image>();
                RectTransform rectTransform = button.GetComponent<RectTransform>();

                StartCoroutine(AnimateButton(button, buttonImage, rectTransform, 0f, false));

                yield return new WaitForSeconds(stepDelay);
            }

            foreach (var button in _settingsButtons)
            {
                button.image.raycastTarget = false;
            }
            _settingsImage.sprite = _settingsCloseSprite;
            _mainButton.interactable = true;
        }

        private IEnumerator AnimateButton(Button button, Image buttonImage, RectTransform rectTransform, float targetY, bool isShowing)
        {
            float elapsedTime = 0f;
            Vector2 startPosition = rectTransform.anchoredPosition;
            Vector2 targetPosition = new Vector2(startPosition.x, targetY);

            float startAlpha = isShowing ? 0f : 1f;
            float targetAlpha = isShowing ? 1f : 0f;

            while (elapsedTime < _animationDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / _animationDuration);

                rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);
                buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, Mathf.Lerp(startAlpha, targetAlpha, t));

                yield return null;
            }

            rectTransform.anchoredPosition = targetPosition;
            buttonImage.color = new Color(buttonImage.color.r, buttonImage.color.g, buttonImage.color.b, targetAlpha);
        }
        
    }
}

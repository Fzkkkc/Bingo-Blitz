using System.Collections;
using Services;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class UiSliderXp : MonoBehaviour
    {
        [SerializeField] private Slider _statSlider;
        
        private void OnValidate()
        {
            _statSlider ??= GetComponent<Slider>();
        }

        private void Start()
        {
            GameInstance.MoneyManager.OnWPCountChange += OnValueChanged;
            OnValueChanged(GameInstance.MoneyManager.GetWpCount());
        }
        
        private void OnDestroy()
        {
            GameInstance.MoneyManager.OnWPCountChange -= OnValueChanged;
        }
        
        private void OnValueChanged(int amount)
        {
            StartCoroutine(AnimateSliderChange(amount));
        }
        
        private IEnumerator AnimateSliderChange(int newAmount)
        {
            float currentAmount = _statSlider.value; 
            float elapsedTime = 0f;
            float duration = 0.7f;

            float maxValue = _statSlider.maxValue;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);

                float newValue = Mathf.Lerp(currentAmount, (float)newAmount, t);
                _statSlider.value = newValue;


                yield return null;
            }

            _statSlider.value = newAmount;
        }
    }
}
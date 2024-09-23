using Services;
using TMPro;
using UnityEngine;
using System.Collections;

namespace UserInterface
{
    public class UITextMoney : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _currencyText;

        private void OnValidate()
        {
            _currencyText ??= GetComponentInChildren<TextMeshProUGUI>();
        }

        protected void Start()
        {
            GameInstance.MoneyManager.OnCoinsCurrencyChange += OnMoneyChanged;
            OnMoneyChanged(GameInstance.MoneyManager.GetCoinsCurrency());
        }
        
        private void OnDestroy()
        {
            GameInstance.MoneyManager.OnCoinsCurrencyChange -= OnMoneyChanged;
        }
        
        private void OnMoneyChanged(ulong money) 
        {
            StartCoroutine(AnimateTextChange(money));
        }

        private IEnumerator AnimateTextChange(ulong newAmount)
        {
            ulong currentAmount;
            if (ulong.TryParse(_currencyText.text, out currentAmount))
            {
                float elapsedTime = 0f;
                while (elapsedTime < 0.7f)
                {
                    elapsedTime += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsedTime / 0.7f);
                    ulong intermediateAmount = (ulong)Mathf.Lerp(currentAmount, (float)newAmount, t);
                    _currencyText.SetText(intermediateAmount.ToString());
                    yield return null;
                }
                _currencyText.SetText(newAmount.ToString());
            }
            else
            {
                _currencyText.SetText(newAmount.ToString());
            }
        }
    }
}
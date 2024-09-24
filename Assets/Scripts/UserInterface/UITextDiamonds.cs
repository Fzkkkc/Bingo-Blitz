using System.Collections;
using Services;
using TMPro;
using UnityEngine;

namespace UserInterface
{
    public class UITextDiamonds : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _currencyText;

        private void OnValidate()
        {
            _currencyText ??= GetComponentInChildren<TextMeshProUGUI>();
        }

        protected void Start()
        {
            GameInstance.MoneyManager.OnDiamondsCurrencyChange += OnDiamondsChanged;
            OnDiamondsChanged(GameInstance.MoneyManager.GetDiamondsCurrency());
        }
        
        private void OnDestroy()
        {
            GameInstance.MoneyManager.OnDiamondsCurrencyChange -= OnDiamondsChanged;
        }
        
        private void OnDiamondsChanged(ulong diamonds) 
        {
            StartCoroutine(AnimateTextChange(diamonds));
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
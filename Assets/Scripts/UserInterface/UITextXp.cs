using Services;
using TMPro;
using UnityEngine;
using System.Collections;

namespace UserInterface
{
    public class UITextXp : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _xpText;

        private void OnValidate()
        {
            _xpText ??= GetComponent<TextMeshProUGUI>();
        }

        protected void Start()
        {
            GameInstance.MoneyManager.OnWPCountChange += OnXpChanged;
            OnXpChanged(GameInstance.MoneyManager.GetWpCount());
        }
        
        private void OnDestroy()
        {
            GameInstance.MoneyManager.OnWPCountChange -= OnXpChanged;
        }
        
        private void OnXpChanged(int xp) 
        {
            StartCoroutine(AnimateTextChange(xp));
        }

        private IEnumerator AnimateTextChange(int newAmount)
        {
            ulong currentAmount;
            if (ulong.TryParse(_xpText.text, out currentAmount))
            {
                float elapsedTime = 0f;
                while (elapsedTime < 1f)
                {
                    elapsedTime += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsedTime / 1f);
                    ulong intermediateAmount = (ulong)Mathf.Lerp(currentAmount, (float)newAmount, t);
                    _xpText.SetText(intermediateAmount.ToString());
                    yield return null;
                }
                _xpText.SetText(newAmount.ToString());
            }
            else
            {
                _xpText.SetText(newAmount.ToString());
            }
        }
    }
}
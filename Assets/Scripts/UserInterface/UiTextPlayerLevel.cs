using Services;
using TMPro;
using UnityEngine;

namespace UserInterface
{
    public class UiTextPlayerLevel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _levelText;

        private void OnValidate()
        {
            _levelText ??= GetComponentInChildren<TextMeshProUGUI>();
        }

        protected void Start()
        {
            GameInstance.MoneyManager.OnLevelCountChange += UpdateLevelText;
            UpdateLevelText(GameInstance.MoneyManager.GetLevelCount());
        }
        
        private void OnDestroy()
        {
            GameInstance.MoneyManager.OnLevelCountChange -= UpdateLevelText;
        }
        
        private void UpdateLevelText(int newAmount)
        {
            _levelText.SetText($"{newAmount + 1}");
        }
    }
}
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore
{
    public class PlayerBankSaves : MonoBehaviour
    {
        private const int MAX_SAVES = 8;
        private int[] bankSaves = new int[MAX_SAVES];
        [SerializeField] private TextMeshProUGUI[] bankSavesTexts;
        [SerializeField] private ScrollRect _scrollView; 

        void Start()
        {
            LoadBankSaves();
            UpdateBankSavesTexts();
        }

        public void AddCoins(int coinsCount)
        {
            for (int i = MAX_SAVES - 1; i > 0; i--)
            {
                bankSaves[i] = bankSaves[i - 1];
            }

            bankSaves[0] = coinsCount; 

            SaveBankSaves();
            UpdateBankSavesTexts();
        }


        private void SaveBankSaves()
        {
            for (int i = 0; i < MAX_SAVES; i++)
            {
                PlayerPrefs.SetInt("BankSave" + i, bankSaves[i]);
            }
            PlayerPrefs.Save();
        }

        private void LoadBankSaves()
        {
            for (int i = 0; i < MAX_SAVES; i++)
            {
                bankSaves[i] = PlayerPrefs.GetInt("BankSave" + i, 0);
            }
        }

        private void UpdateBankSavesTexts()
        {
            int filledCount = 0;

            for (int i = 0; i < MAX_SAVES; i++)
            {
                if (bankSaves[i] > 0) 
                {
                    bankSavesTexts[i].text = "+" + bankSaves[i].ToString();
                    bankSavesTexts[i].gameObject.transform.parent.gameObject.SetActive(true);
                    filledCount++; 
                }
                else 
                {
                    bankSavesTexts[i].text = ""; 
                    bankSavesTexts[i].gameObject.transform.parent.gameObject.SetActive(false);
                }
            }

            _scrollView.vertical = filledCount >= 6;
        }
    }
}

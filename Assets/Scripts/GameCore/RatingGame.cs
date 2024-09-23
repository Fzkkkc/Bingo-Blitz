using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GameCore
{
    public class RatingGame : MonoBehaviour
    {
        [SerializeField] private List<TextMeshProUGUI> _timeTexts;
        [SerializeField] private List<TextMeshProUGUI> _moneyTexts;

        private void Start()
        {
            DisplayResults();
        }

        public void DisplayResults()
        {
            var results = LoadResults();

            for (int i = 0; i < _timeTexts.Count; i++)
            {
                if (i < results.Count)
                {
                    _timeTexts[i].text = results[i].Time;
                    _moneyTexts[i].text = results[i].Money.ToString();
                }
                else
                {
                    _timeTexts[i].text = "N/A";
                    _moneyTexts[i].text = "N/A";
                }
            }
        }


        private List<GameResult> LoadResults()
        {
            var results = new List<GameResult>();
            int count = PlayerPrefs.GetInt("ResultCount", 0);
            for (int i = 0; i < count; i++)
            {
                string time = PlayerPrefs.GetString($"Result_{i}_Time");
                int money = PlayerPrefs.GetInt($"Result_{i}_Money");
                results.Add(new GameResult(time, money));
            }
            return results;
        }
    }
}
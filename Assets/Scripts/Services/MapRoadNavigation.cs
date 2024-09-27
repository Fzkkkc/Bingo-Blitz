using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Services
{
    [Serializable]
    public class LevelData
    {
        public int mainLevel;
        public int subLevelProgress; 
        public int starCount;
        public List<bool> subLevels;
    }

    public class MapRoadNavigation : MonoBehaviour
    {
        [SerializeField] private List<Button> _levelButtons;
        [SerializeField] private List<TextMeshProUGUI> _textLevel; 
        public List<LevelData> _levelsData; 

        public int _currentMainLevel;
        private readonly int _maxMainLevels = 5;
        private readonly int _maxSubLevels = 9;

        private int PrefsMainLevel
        {
            get => PlayerPrefs.GetInt("PREFS_MainLevel", 0);
            set => PlayerPrefs.SetInt("PREFS_MainLevel", value);
        }

        public Action<int, int> OnCurrentLevelValueChanged;

        public void Init()
        {
            LoadLevelsData();
            _currentMainLevel = PrefsMainLevel;
            OpenButtonsLevel();
        }

        private void LoadLevelsData()
        {
            _levelsData = new List<LevelData>();

            for (var i = 0; i < _maxMainLevels; i++)
            {
                var levelData = new LevelData
                {
                    mainLevel = i,
                    subLevelProgress = PlayerPrefs.GetInt($"MainLevel_{i}_SubProgress", 0),
                    starCount = PlayerPrefs.GetInt($"MainLevel_{i}_Stars", 0),
                    subLevels = new List<bool>()
                };

                for (var j = 0; j < _maxSubLevels; j++)
                    levelData.subLevels.Add(PlayerPrefs.GetInt($"MainLevel_{i}_SubLevel_{j}", 0) == 1);

                _levelsData.Add(levelData);
            }
        }

        private void SaveLevelData(int mainLevel)
        {
            PlayerPrefs.SetInt($"MainLevel_{mainLevel}_SubProgress", _levelsData[mainLevel].subLevelProgress);
            PlayerPrefs.SetInt($"MainLevel_{mainLevel}_Stars", _levelsData[mainLevel].starCount);

            for (var i = 0; i < _maxSubLevels; i++)
                PlayerPrefs.SetInt($"MainLevel_{mainLevel}_SubLevel_{i}", _levelsData[mainLevel].subLevels[i] ? 1 : 0);

            PlayerPrefs.Save();
        }

        public void IncreaseSubLevel()
        {
            var currentLevelData = _levelsData[_currentMainLevel];

            if (currentLevelData.subLevelProgress < _maxSubLevels)
            {
                currentLevelData.subLevelProgress++;
                currentLevelData.subLevels[currentLevelData.subLevelProgress - 1] = true;

                if (currentLevelData.subLevelProgress % 3 == 0) currentLevelData.starCount++;
            }


            SaveLevelData(_currentMainLevel);
            OpenButtonsLevel();
        }

        public void IncreaseMainLevel()
        {
            if (_currentMainLevel < _maxMainLevels - 1) 
                _currentMainLevel++;

            SaveLevelData(_currentMainLevel); 
        }

        public void SetCurrentLevelIndex(int mainLevelIndex)
        {
            _currentMainLevel = mainLevelIndex;

            OnCurrentLevelValueChanged?.Invoke(_currentMainLevel, _levelsData[_currentMainLevel].subLevelProgress);
            OpenButtonsLevel();
        }

        public int GetMaxMainLevelIndex()
        {
            return _currentMainLevel;
        }

        private void OpenButtonsLevel()
        {
            UpdateAllButtonStates();

            for (var i = 0; i < _levelButtons.Count; i++)
            {
                var levelData = _levelsData[i];
                int playerLevel = GameInstance.MoneyManager.GetLevelCount();

                if (i == 0 || 
                    (i == 1 && playerLevel >= 10) || 
                    (i == 2 && playerLevel >= 15) || 
                    (i == 3 && playerLevel >= 20) || 
                    (i == 4 && playerLevel >= 25))
                {
                    _levelButtons[i].interactable = true; 
                    _textLevel[i].text = $"{levelData.subLevelProgress}/{_maxSubLevels}"; 
                }
                else
                {
                    _levelButtons[i].interactable = false;
                    _textLevel[i].text = "0/9"; 
                }
            }
        }

        private void UpdateAllButtonStates()
        {
            for (var i = 0; i < _levelButtons.Count; i++)
            {
                var starCount = _levelsData[i].starCount;

                var childButtons = _levelButtons[i].GetComponentsInChildren<Button>(true);

                for (var j = 0; j < childButtons.Length; j++)
                {
                    if (!_levelButtons[i].interactable)
                        childButtons[j].interactable = false;
                    else
                        childButtons[j].interactable = starCount > 0 && j <= starCount;
                }
            }
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GameCore
{
    [Serializable]
    public class BingoColumn
    {
        public List<Button> buttons = new List<Button>();
    }

    public class PlayerBingoController : MonoBehaviour
    {
        [Header("Bingo Columns")] 
        [SerializeField]
        private List<BingoColumn> _bingoColumns;

        [Header("Second Bingo Field")] 
        [SerializeField]
        private List<BingoColumn> _secondBingoColumns;

        // Диапазоны для каждого столбца
        private readonly int[] _columnRanges = { 1, 16, 31, 46, 61 };

        // Листы для хранения дочерних элементов кнопок для двух полей
        private List<TextMeshProUGUI> _firstFieldButtonTexts;
        private List<Image> _firstFieldButtonImages;

        private List<TextMeshProUGUI> _secondFieldButtonTexts;
        private List<Image> _secondFieldButtonImages;

        // Списки использованных кнопок для каждого поля
        private List<Button> _usedButtonsField1 = new List<Button>();
        private List<Button> _usedButtonsField2 = new List<Button>();

        // Выигрышные комбинации
        private List<List<string>> _winningCombinations;

        private int _playerBingoCount = 0;
        
        public Action OnPlayerGotBingo;
        public Action OnPlayerGotSecondBingo;
        
        private void Start()
        {
            _firstFieldButtonTexts = new List<TextMeshProUGUI>();
            _firstFieldButtonImages = new List<Image>();

            _secondFieldButtonTexts = new List<TextMeshProUGUI>();
            _secondFieldButtonImages = new List<Image>();

            InitializeField(_bingoColumns, _firstFieldButtonTexts, _firstFieldButtonImages);
            InitializeField(_secondBingoColumns, _secondFieldButtonTexts, _secondFieldButtonImages);
            InitializeWinningCombinations();
        }

        private void InitializeWinningCombinations()
        {
            _winningCombinations = new List<List<string>>
            {
                new List<string> { "Image", "Image (4)", "Image (24)", "Image (22)" },
                new List<string> { "Image", "Image (1)", "Image (2)", "Image (3)", "Image (4)" },
                new List<string> { "Image (5)", "Image (6)", "Image (7)", "Image (8)", "Image (9)" },
                new List<string> { "Image (10)", "Image (11)", "Image (12)", "Image (13)", "Image (14)" },
                new List<string> { "Image (15)", "Image (20)", "Image (16)", "Image (21)", "Image (17)" },
                new List<string> { "Image (22)", "Image (18)", "Image (23)", "Image (19)", "Image (24)" },
                new List<string> { "Image", "Image (5)", "Image (10)", "Image (15)", "Image (22)" },
                new List<string> { "Image (1)", "Image (6)", "Image (11)", "Image (20)", "Image (18)" },
                new List<string> { "Image (2)", "Image (7)", "Image (12)", "Image (16)", "Image (23)" },
                new List<string> { "Image (3)", "Image (8)", "Image (13)", "Image (21)", "Image (19)" },
                new List<string> { "Image (4)", "Image (9)", "Image (14)", "Image (17)", "Image (24)" },
                new List<string> { "Image", "Image (6)", "Image (12)", "Image (21)", "Image (24)" },
                new List<string> { "Image (4)", "Image (8)", "Image (12)", "Image (20)", "Image (22)" }
            };
        }

        private void InitializeField(List<BingoColumn> bingoColumns, List<TextMeshProUGUI> buttonTexts,
            List<Image> buttonImages)
        {
            foreach (var column in bingoColumns)
            {
                foreach (var button in column.buttons)
                {
                    var text = button.GetComponentInChildren<TextMeshProUGUI>();
                    buttonTexts.Add(text);

                    // Получаем дочерний Image для анимации
                    var childImage = button.GetComponentsInChildren<Image>();

                    // Ищем Image, который не является самим Image кнопки
                    foreach (var img in childImage)
                    {
                        if (img != button.GetComponent<Image>())
                        {
                            buttonImages.Add(img);
                            break; // Добавляем только первый найденный дочерний Image
                        }
                    }

                    // Добавление обработчика нажатий на кнопку
                    int index = buttonTexts.Count - 1; // Получаем индекс текста кнопки
                    button.onClick.AddListener(() => OnButtonClick(text, buttonImages[index], button)); // Передаем дочерний Image по индексу
                }
            }
        }

        public void FillPlayerBoard(int index)
        {
            switch (index)
            {
                case 1:
                    FillBingoBoard(_bingoColumns, _firstFieldButtonTexts, _firstFieldButtonImages);
                    break;
                case 2:
                    FillBingoBoard(_bingoColumns, _firstFieldButtonTexts, _firstFieldButtonImages);
                    FillBingoBoard(_secondBingoColumns, _secondFieldButtonTexts, _secondFieldButtonImages);
                    break;
            }
        }

        private void FillBingoBoard(List<BingoColumn> bingoColumns, List<TextMeshProUGUI> buttonTexts,
            List<Image> buttonImages)
        {
            for (var i = 0; i < bingoColumns.Count; i++)
            {
                var uniqueNumbers = GenerateUniqueNumbersForColumn(_columnRanges[i], _columnRanges[i] + 14);

                for (var j = 0; j < bingoColumns[i].buttons.Count; j++)
                {
                    var number = uniqueNumbers[j];

                    buttonTexts[i * 5 + j].text = number.ToString();
                    buttonImages[i * 5 + j].color = new Color(1, 1, 1, 1);
                }
            }

            foreach (var image in buttonImages)
            {
                image.transform.localScale = Vector3.zero; // Сбрасываем масштаб
            }
            
            _usedButtonsField1.Clear();
            _usedButtonsField2.Clear();
            _playerBingoCount = 0;
        }

        private List<int> GenerateUniqueNumbersForColumn(int min, int max)
        {
            var numbers = new List<int>();

            while (numbers.Count < 5)
            {
                var randomNumber = Random.Range(min, max + 1);

                if (!numbers.Contains(randomNumber)) numbers.Add(randomNumber);
            }

            return numbers;
        }

        private void OnButtonClick(TextMeshProUGUI buttonText, Image buttonImage, Button button)
        {
            // Получаем число с кнопки
            if (int.TryParse(buttonText.text, out int number))
            {
                // Проверяем, есть ли это число в списке использованных чисел из BingoMainController
                if (GameInstance.GameState.BingoMainController.IsNumberUsed(number))
                {
                    // Определяем к какому полю принадлежит кнопка
                    if (IsButtonInField(button, _bingoColumns))
                    {
                        ProcessButtonClick(buttonText, buttonImage, button, _usedButtonsField1);
                        CheckWinningCombinations(_usedButtonsField1);
                    }
                    else if (IsButtonInField(button, _secondBingoColumns))
                    {
                        ProcessButtonClick(buttonText, buttonImage, button, _usedButtonsField2);
                        CheckWinningCombinations(_usedButtonsField2);
                    }
                }
            }
        }

        private bool IsButtonInField(Button button, List<BingoColumn> bingoColumns)
        {
            foreach (var column in bingoColumns)
            {
                if (column.buttons.Contains(button))
                {
                    return true;
                }
            }
            return false;
        }

        private void ProcessButtonClick(TextMeshProUGUI buttonText, Image buttonImage, Button button, List<Button> usedButtons)
        {
            // Добавляем кнопку в список использованных кнопок
            if (!usedButtons.Contains(button))
            {
                usedButtons.Add(button);
                // Запускаем анимацию
                StartCoroutine(AnimateButton(buttonImage));

                // Очищаем текст кнопки
                buttonText.text = "";
            }
        }

        private void CheckWinningCombinations(List<Button> usedButtons)
        {
            // Собираем имена активированных кнопок
            HashSet<string> activatedButtonNames = new HashSet<string>();

            foreach (var usedButton in usedButtons)
            {
                activatedButtonNames.Add(usedButton.name); // Используем имя кнопки
            }

            foreach (var winningCombination in _winningCombinations)
            {
                bool isWinningCombination = true;

                foreach (var name in winningCombination)
                {
                    if (!activatedButtonNames.Contains(name))
                    {
                        isWinningCombination = false;
                        break;
                    }
                }

                if (isWinningCombination)
                {
                    if (_playerBingoCount == 0)
                    {
                        _playerBingoCount++;
                        OnPlayerGotBingo?.Invoke();
                    }
                    else if (_playerBingoCount == 1)
                    {
                        _playerBingoCount++;
                        OnPlayerGotSecondBingo?.Invoke();
                    }
                    Debug.Log("ПОБЕДА");
                    Debug.Log("Выигрышная комбинация: " + string.Join(", ", winningCombination));
                    // Обработка выигрышного сценария здесь
                    break; // Выход после нахождения выигрышной комбинации
                }
            }
        }

        private IEnumerator AnimateButton(Image buttonImage)
        {
            buttonImage.transform.localScale = Vector3.zero; // Сбрасываем масштаб перед началом анимации

            float animationDuration = 0.22f;
            float scaleUp = 1.5f;
            float elapsedTime = 0f;

            // Увеличение до 1.4
            while (elapsedTime < animationDuration)
            {
                float scale = Mathf.Lerp(0f, scaleUp, elapsedTime / animationDuration);
                buttonImage.transform.localScale = new Vector3(scale, scale, scale);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            buttonImage.transform.localScale = new Vector3(scaleUp, scaleUp, scaleUp);

            elapsedTime = 0f;

            // Уменьшение до 1
            while (elapsedTime < animationDuration)
            {
                float scale = Mathf.Lerp(scaleUp, 1f, elapsedTime / animationDuration);
                buttonImage.transform.localScale = new Vector3(scale, scale, scale);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            buttonImage.transform.localScale = Vector3.one; // Убедитесь, что scale вернулся к 1
        }

        public int GetBingoCount()
        {
            return _playerBingoCount;
        }
    }
}

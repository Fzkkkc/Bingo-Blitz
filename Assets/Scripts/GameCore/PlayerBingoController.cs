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
        private List<BingoColumn> _secondBingoColumns; // Второй список из 5 столбцов

        // Диапазоны для каждого столбца
        private readonly int[] _columnRanges = { 1, 16, 31, 46, 61 };

        // Листы для хранения дочерних элементов кнопок для двух полей
        private List<TextMeshProUGUI> _firstFieldButtonTexts;
        private List<Image> _firstFieldButtonImages;

        private List<TextMeshProUGUI> _secondFieldButtonTexts;
        private List<Image> _secondFieldButtonImages;

        // Список использованных кнопок
        private List<Button> _usedButtons = new List<Button>();

        private void Start()
        {
            _firstFieldButtonTexts = new List<TextMeshProUGUI>();
            _firstFieldButtonImages = new List<Image>();

            _secondFieldButtonTexts = new List<TextMeshProUGUI>();
            _secondFieldButtonImages = new List<Image>();

            InitializeField(_bingoColumns, _firstFieldButtonTexts, _firstFieldButtonImages);
            InitializeField(_secondBingoColumns, _secondFieldButtonTexts, _secondFieldButtonImages);
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
                    // Используем индекс кнопки, чтобы найти соответствующий дочерний Image из списка
                    int index = buttonTexts.Count - 1; // Получаем индекс текста кнопки
                    button.onClick.AddListener(() => OnButtonClick(text, buttonImages[index])); // Передаем дочерний Image по индексу
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

        private void OnButtonClick(TextMeshProUGUI buttonText, Image buttonImage)
        {
            // Получаем число с кнопки
            if (int.TryParse(buttonText.text, out int number))
            {
                // Проверяем, есть ли это число в списке использованных чисел из BingoMainController
                if (GameInstance.GameState.BingoMainController.IsNumberUsed(number))
                {
                    // Добавляем кнопку в список использованных кнопок
                    if (!_usedButtons.Contains(buttonText.GetComponentInParent<Button>()))
                    {
                        _usedButtons.Add(buttonText.GetComponentInParent<Button>());
                        Debug.Log($"Кнопка с номером {number} добавлена в использованные кнопки.");
                        
                        // Запускаем анимацию
                        StartCoroutine(AnimateButton(buttonImage));

                        // Очищаем текст кнопки
                        buttonText.text = "";
                    }
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
    }
}

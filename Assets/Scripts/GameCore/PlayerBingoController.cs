using System;
using System.Collections.Generic;
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
        [Header("Bingo Columns")] [SerializeField]
        private List<BingoColumn> _bingoColumns;

        [Header("Second Bingo Field")] [SerializeField]
        private List<BingoColumn> _secondBingoColumns; // Второй список из 5 столбцов

        // Диапазоны для каждого столбца
        private readonly int[] _columnRanges = {1, 16, 31, 46, 61};

        // Листы для хранения дочерних элементов кнопок для двух полей
        private List<TextMeshProUGUI> _firstFieldButtonTexts;
        private List<Image> _firstFieldButtonImages;

        private List<TextMeshProUGUI> _secondFieldButtonTexts;
        private List<Image> _secondFieldButtonImages;

        private void Start()
        {
            _firstFieldButtonTexts = new List<TextMeshProUGUI>();
            _firstFieldButtonImages = new List<Image>();

            _secondFieldButtonTexts = new List<TextMeshProUGUI>();
            _secondFieldButtonImages = new List<Image>();

            InitializeField(_bingoColumns, _firstFieldButtonTexts, _firstFieldButtonImages);

            InitializeField(_secondBingoColumns, _secondFieldButtonTexts, _secondFieldButtonImages);

            FillBingoBoard(_bingoColumns, _firstFieldButtonTexts, _firstFieldButtonImages);
            FillBingoBoard(_secondBingoColumns, _secondFieldButtonTexts, _secondFieldButtonImages);
        }

        private void InitializeField(List<BingoColumn> bingoColumns, List<TextMeshProUGUI> buttonTexts,
            List<Image> buttonImages)
        {
            foreach (var column in bingoColumns)
            foreach (var button in column.buttons)
            {
                var text = button.GetComponentInChildren<TextMeshProUGUI>();
                buttonTexts.Add(text);

                var image = button.transform.GetComponentInChildren<Image>();

                if (image != button.GetComponent<Image>())
                {
                    buttonImages.Add(image);
                }
                else
                {
                    var childImage = button.GetComponentsInChildren<Image>();
                    foreach (var img in childImage)
                        if (img != button.GetComponent<Image>())
                        {
                            buttonImages.Add(img);
                            break;
                        }
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

                    buttonImages[i * 5 + j].color = new Color(1, 1, 1, 0);
                }
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
    }
}
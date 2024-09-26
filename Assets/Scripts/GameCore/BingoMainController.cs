using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameCore
{
    public class BingoMainController : MonoBehaviour
    {
        [Header("Spawn Points")]
        [SerializeField] private RectTransform _spawnPoint;
        [SerializeField] private RectTransform _bigPoint;
        [SerializeField] private RectTransform _normalPoint;

        [Header("Bingo Ball Settings")]
        [SerializeField] private List<Image> _bingoBalls; // 9 balls
        [SerializeField] private List<Sprite> _bingoSprites; // 5 bingo sprites
        private List<TextMeshProUGUI> _bingoBallTexts; // List for TextMeshProUGUI components

        [Header("Used Numbers Animation")]
        [SerializeField] private List<Image> _usedNumbers; // Список для картинок с анимацией

        public int currentBallIndex = 0; // Current index of the ball to spawn
        public int movesCount = 0; // Counts the number of moves made
        private const int MaxMoves = 11; // Total moves before resetting
        public float ballOffset = 145f; // Offset for moving balls after NormalPoint

        private List<int> usedNumbers = new List<int>(); // Список использованных номеров

        private void Start()
        {
            _bingoBallTexts = new List<TextMeshProUGUI>();

            foreach (var ball in _bingoBalls)
            {
                TextMeshProUGUI textComponent = ball.GetComponentInChildren<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    _bingoBallTexts.Add(textComponent);
                }
            }
        }

        public void StartSpawnBalls()
        {
            StartCoroutine(SpawnBalls());
        }

        private IEnumerator SpawnBalls()
        {
            ResetBalls();
            while (GameInstance.GameState.GameRunning)
            {
                yield return new WaitForSeconds(2f);

                if (movesCount < MaxMoves)
                {
                    if (usedNumbers.Count < 75)
                    {
                        SpawnBall();
                    }
                    else
                    {
                        Debug.Log("Все номера использованы. Остановка корутины.");
                        yield break; // Останавливаем корутину, когда все номера использованы
                    }
                }
            }
        }

        private void SpawnBall()
        {
            // Получаем текущий шар
            Image currentBall = _bingoBalls[currentBallIndex];
            TextMeshProUGUI currentText = _bingoBallTexts[currentBallIndex];

            // Генерируем уникальное случайное число и назначаем спрайт
            int randomNumber = GetUniqueRandomNumber();
            AssignSpriteToBall(currentBall, currentText, randomNumber);

            // Если шарик на стартовой позиции, перемещаем его в большую точку
            if (currentBall.transform.localPosition == _spawnPoint.localPosition)
            {
                StartCoroutine(MoveBall(currentBall, _bigPoint.localPosition, 1.2f));
            }

            // Начинаем движение всех предыдущих шариков
            MovePreviousBalls();

            // Увеличиваем счетчик перемещений
            movesCount++;

            // Если достигли MaxMoves, сбрасываем все шары
            if (movesCount >= MaxMoves)
            {
                movesCount = 0;
            }

            // Обновляем индекс для следующего шарика
            currentBallIndex = (currentBallIndex + 1) % _bingoBalls.Count;
        }

        private void MovePreviousBalls()
        {
            for (int i = 0; i < _bingoBalls.Count; i++)
            {
                Image ball = _bingoBalls[i];

                // Если шарик не находится на стартовой позиции и у него есть значение, то продолжаем его движение
                if (ball.transform.localPosition != _spawnPoint.localPosition && !string.IsNullOrEmpty(_bingoBallTexts[i].text))
                {
                    Vector3 targetPosition = ball.transform.localPosition + new Vector3(ballOffset, 0f, 0f);

                    // Проверяем, достиг ли шарик позиции 728f
                    if (ball.transform.localPosition.x >= 728f)
                    {
                        // Если шарик достиг 728f, сбрасываем его позицию и назначаем новое значение
                        ball.transform.localPosition = _spawnPoint.localPosition;
                        //int randomNumber = GetUniqueRandomNumber();
                        //AssignSpriteToBall(ball, _bingoBallTexts[i], randomNumber);
                        StartCoroutine(MoveBall(ball, _bigPoint.localPosition, 1.2f));
                    }
                    else
                    {
                        StartCoroutine(MoveBall(ball, targetPosition, 1f));
                    }
                }
            }
        }

        private IEnumerator MoveBall(Image ball, Vector3 targetPosition, float targetScale)
        {
            Vector3 startPosition = ball.transform.localPosition;
            Vector3 startScale = ball.transform.localScale;

            float duration = 1f; // Продолжительность движения
            float time = 0;

            while (time < duration)
            {
                float t = time / duration;
                ball.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
                ball.transform.localScale = Vector3.Lerp(startScale, Vector3.one * targetScale, t);
                time += Time.deltaTime;
                yield return null;
            }

            // Обеспечиваем установку конечной позиции и масштаба
            ball.transform.localPosition = targetPosition;
            ball.transform.localScale = Vector3.one * targetScale;
        }

        private void AssignSpriteToBall(Image ball, TextMeshProUGUI text, int number)
        {
            int spriteIndex;
            Color textColor;

            if (number <= 15)
            {
                spriteIndex = 0; // First sprite
                textColor = new Color(1f, 0.647f, 0f); // Orange
            }
            else if (number <= 30)
            {
                spriteIndex = 1; // Second sprite
                textColor = Color.yellow; // Yellow
            }
            else if (number <= 45)
            {
                spriteIndex = 2; // Third sprite
                textColor = new Color(0.5f, 0f, 0.5f); // Purple
            }
            else if (number <= 60)
            {
                spriteIndex = 3; // Fourth sprite
                textColor = new Color(0f, 0.5f, 0f); // Dark Green
            }
            else
            {
                spriteIndex = 4; // Fifth sprite
                textColor = new Color(0f, 0.75f, 1f); // Light Blue
            }

            ball.sprite = _bingoSprites[spriteIndex];
            text.text = number.ToString(); // Обновляем текст с новым числом
            text.color = textColor; // Обновляем цвет текста
            ball.gameObject.SetActive(true); // Активируем шар

            // Запускаем анимацию появления
            StartCoroutine(AnimateUsedNumberAppearance(ball));
            StartCoroutine(AnimateUsedNumberAppearance(_usedNumbers[number - 1]));
        }

        private IEnumerator AnimateUsedNumberAppearance(Image ball)
        {
            float duration = 0.7f;
            float time = 0f;

            // Начинаем с масштаба 0
            Vector3 startScale = Vector3.zero;
            Vector3 endScale = Vector3.one * 1.2f; // Сначала увеличиваем до 1.2

            // Анимация увеличения масштаба до 1.2
            while (time < duration / 2)
            {
                float t = time / (duration / 2);
                ball.transform.localScale = Vector3.Lerp(startScale, endScale, t);
                time += Time.deltaTime;
                yield return null;
            }

            // Анимация уменьшения масштаба до 1.0
            time = 0f;
            startScale = Vector3.one * 1.2f;
            endScale = Vector3.one;

            while (time < duration / 2)
            {
                float t = time / (duration / 2);
                ball.transform.localScale = Vector3.Lerp(startScale, endScale, t);
                time += Time.deltaTime;
                yield return null;
            }

            ball.transform.localScale = Vector3.one; // Устанавливаем конечный масштаб 1.0
        }

        private int GetUniqueRandomNumber()
        {
            int randomNumber;

            // Генерируем уникальное случайное число
            do
            {
                randomNumber = Random.Range(1, 76);
            }
            while (usedNumbers.Contains(randomNumber));

            // Добавляем число в список использованных
            usedNumbers.Add(randomNumber);

            return randomNumber;
        }

        public void ResetBalls()
        {
            for (int i = 0; i < _bingoBalls.Count; i++)
            {
                Image ball = _bingoBalls[i];
                TextMeshProUGUI text = _bingoBallTexts[i];

                ball.transform.localPosition = _spawnPoint.localPosition; // Сброс позиции
                ball.transform.localScale = Vector3.one; // Сброс масштаба
                text.text = ""; // Очищаем текст
            }
            currentBallIndex = 0; // Сбрасываем индекс текущего шара
            usedNumbers.Clear(); // Очищаем список использованных номеров

            // Сбрасываем масштаб для всех изображений в _usedNumbers
            foreach (var ball in _usedNumbers)
            {
                ball.transform.localScale = Vector3.zero;
            }
        }
    }
}

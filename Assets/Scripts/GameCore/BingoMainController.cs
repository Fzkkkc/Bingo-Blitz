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

        private int currentBallIndex = 0; // Current index of the ball to spawn
        private int movesCount = 0; // Counts the number of moves made
        private const int MaxMoves = 10; // Total moves before resetting
        private float ballOffset = 135f; // Offset for moving balls after NormalPoint

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
                    yield return StartCoroutine(SpawnBall());
                }
            }
        }

        private IEnumerator SpawnBall()
        {
            // Получаем текущий шар
            Image currentBall = _bingoBalls[currentBallIndex];
            TextMeshProUGUI currentText = _bingoBallTexts[currentBallIndex];

            // Генерируем случайное число и назначаем спрайт
            int randomNumber = Random.Range(1, 76);
            AssignSpriteToBall(currentBall, currentText, randomNumber);

            // Двигаем текущий шарик к _bigPoint
            yield return StartCoroutine(MoveBall(currentBall, _bigPoint.localPosition, 1.2f));

            // Если это не первый шарик, начинаем двигать предыдущий шарик
            if (currentBallIndex > 0)
            {
                Image previousBall = _bingoBalls[currentBallIndex - 1];
                // Перемещаем предыдущий шарик к _normalPoint
                yield return StartCoroutine(MoveBall(previousBall, _normalPoint.localPosition, 1f));

                // После этого сдвигаем предыдущие шары вправо на ballOffset
                MovePreviousBalls();
            }

            // Увеличиваем счетчик перемещений
            movesCount++;

            // Если достигли MaxMoves, сбрасываем все шары
            if (movesCount >= MaxMoves)
            {
                ResetBalls();
                movesCount = 0;
            }

            // Обновляем индекс для следующего шарика
            currentBallIndex = (currentBallIndex + 1) % _bingoBalls.Count;
        }

        private void MovePreviousBalls()
        {
            // Сдвигаем все предыдущие шары вдоль оси X на ballOffset
            for (int i = 0; i < currentBallIndex; i++)
            {
                Image ball = _bingoBalls[i];
                Vector3 targetPosition = ball.transform.localPosition + new Vector3(ballOffset, 0f, 0f);
                StartCoroutine(MoveBall(ball, targetPosition, 1f));
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
                ball.gameObject.SetActive(false); // Деактивируем шар
            }
            currentBallIndex = 0; // Сбрасываем индекс текущего шара
        }
    }
}

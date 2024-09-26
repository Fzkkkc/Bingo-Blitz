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
        [SerializeField] private List<Image> _bingoBalls; 
        [SerializeField] private List<Sprite> _bingoSprites; 
        private List<TextMeshProUGUI> _bingoBallTexts; 

        [Header("Used Numbers Animation")]
        [SerializeField] private List<Image> _usedNumbers;

        public int currentBallIndex = 0; 
        public int movesCount = 0; 
        private const int MaxMoves = 11; 
        public float ballOffset = 145f; 

        private List<int> usedNumbers = new List<int>();

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

            for (int i = 0; i < 30; i++)
            {
                usedNumbers.Add(i);
            }
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
                }
            }
        }

        private void SpawnBall()
        {
            GameInstance.GameState.ImagesAnimation.SwapAndAnimateImages();
            Image currentBall = _bingoBalls[currentBallIndex];
            TextMeshProUGUI currentText = _bingoBallTexts[currentBallIndex];

            int randomNumber = GetUniqueRandomNumber();
            AssignSpriteToBall(currentBall, currentText, randomNumber);

            if (currentBall.transform.localPosition == _spawnPoint.localPosition)
            {
                StartCoroutine(MoveBall(currentBall, _bigPoint.localPosition, 1.2f));
            }

            MovePreviousBalls();

            movesCount++;

            if (movesCount >= MaxMoves)
            {
                movesCount = 0;
            }

            currentBallIndex = (currentBallIndex + 1) % _bingoBalls.Count;
        }

        private void MovePreviousBalls()
        {
            for (int i = 0; i < _bingoBalls.Count; i++)
            {
                Image ball = _bingoBalls[i];

                if (ball.transform.localPosition != _spawnPoint.localPosition && !string.IsNullOrEmpty(_bingoBallTexts[i].text))
                {
                    Vector3 targetPosition = ball.transform.localPosition + new Vector3(ballOffset, 0f, 0f);

                    if (ball.transform.localPosition.x >= 728f)
                    {
                        ball.transform.localPosition = _spawnPoint.localPosition;
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

            float duration = 0.55f; 
            float time = 0;

            while (time < duration)
            {
                float t = time / duration;
                ball.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
                ball.transform.localScale = Vector3.Lerp(startScale, Vector3.one * targetScale, t);
                time += Time.deltaTime;
                yield return null;
            }

            ball.transform.localPosition = targetPosition;
            ball.transform.localScale = Vector3.one * targetScale;
        }

        private void AssignSpriteToBall(Image ball, TextMeshProUGUI text, int number)
        {
            int spriteIndex;
            Color textColor;

            if (number <= 15)
            {
                spriteIndex = 0; 
                textColor = new Color(1f, 0.647f, 0f); 
            }
            else if (number <= 30)
            {
                spriteIndex = 1; 
                textColor = Color.yellow; 
            }
            else if (number <= 45)
            {
                spriteIndex = 2; 
                textColor = new Color(0.5f, 0f, 0.5f);
            }
            else if (number <= 60)
            {
                spriteIndex = 3; 
                textColor = new Color(0f, 0.5f, 0f); 
            }
            else
            {
                spriteIndex = 4; 
                textColor = new Color(0f, 0.75f, 1f); 
            }

            ball.sprite = _bingoSprites[spriteIndex];
            text.text = number.ToString(); 
            text.color = textColor; 

            StartCoroutine(AnimateUsedNumberAppearance(ball));
            StartCoroutine(AnimateUsedNumberAppearance(_usedNumbers[number - 1]));
        }

        private IEnumerator AnimateUsedNumberAppearance(Image ball)
        {
            float duration = 0.3f;
            float time = 0f;

            Vector3 startScale = Vector3.zero;
            Vector3 endScale = Vector3.one * 1.2f;

            while (time < duration / 2)
            {
                float t = time / (duration / 2);
                ball.transform.localScale = Vector3.Lerp(startScale, endScale, t);
                time += Time.deltaTime;
                yield return null;
            }

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

            ball.transform.localScale = Vector3.one;
        }

        private int GetUniqueRandomNumber()
        {
            int randomNumber;

            do
            {
                randomNumber = Random.Range(1, 76);
            }
            while (usedNumbers.Contains(randomNumber));

            usedNumbers.Add(randomNumber);

            return randomNumber;
        }

        public void ResetBalls()
        {
            for (int i = 0; i < _bingoBalls.Count; i++)
            {
                Image ball = _bingoBalls[i];
                TextMeshProUGUI text = _bingoBallTexts[i];

                ball.transform.localPosition = _spawnPoint.localPosition; 
                ball.transform.localScale = Vector3.one; 
                text.text = ""; 
            }
            currentBallIndex = 0; 
            usedNumbers.Clear();

            foreach (var ball in _usedNumbers)
            {
                ball.transform.localScale = Vector3.zero;
            }
        }
        
        public bool IsNumberUsed(int number)
        {
            return usedNumbers.Contains(number);
        }
    }
}

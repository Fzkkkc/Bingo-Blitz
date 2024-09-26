using System;
using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;

namespace GameCore
{
    public class ImagesAnimation : MonoBehaviour
    {
        [SerializeField] private List<Image> _imageList;
        
        private const float scaleUp = 1.2f;
        private const float scaleNormal = 1f;
        private const float animationDuration = 0.7f;

        private bool _isInitialized = false;

        private void Start()
        {
            GameInstance.UINavigation.OnGameWindowClosed += ResetImagesScale;
        }

        private void OnDestroy()
        {
            GameInstance.UINavigation.OnGameWindowClosed -= ResetImagesScale;
        }
        
        public void ResetImagesScale()
        {
            StopAllCoroutines();
            foreach (var image in _imageList)
            {
                image.transform.localScale = Vector3.zero;
            }

            _isInitialized = false;
        }
        
        public void StartAnimation()
        {
            StartCoroutine(AnimateImages());
        }
        
        private IEnumerator AnimateImages()
        {
            yield return new WaitForSeconds(1f);
            
            for (int i = 0; i < _imageList.Count; i += 2)
            {
                if (i < _imageList.Count) StartCoroutine(AnimateImage(_imageList[i]));
                if (i + 1 < _imageList.Count) StartCoroutine(AnimateImage(_imageList[i + 1]));

                yield return new WaitForSeconds(1f);
            }

            _isInitialized = true;
        }

        private IEnumerator AnimateImage(Image image)
        {
            image.transform.localScale = Vector3.zero;

            // Анимация увеличения масштаба до 1.2 за половину времени
            float elapsedTime = 0f;
            while (elapsedTime < animationDuration / 2)
            {
                float scale = Mathf.Lerp(0f, scaleUp, elapsedTime / (animationDuration / 2));
                image.transform.localScale = new Vector3(scale, scale, scale);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            image.transform.localScale = new Vector3(scaleUp, scaleUp, scaleUp);

            elapsedTime = 0f;
            while (elapsedTime < animationDuration / 2)
            {
                float scale = Mathf.Lerp(scaleUp, scaleNormal, elapsedTime / (animationDuration / 2));
                image.transform.localScale = new Vector3(scale, scale, scale);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            image.transform.localScale = new Vector3(scaleNormal, scaleNormal, scaleNormal);
        }
        
        public void SwapAndAnimateImages()
        {
            if(!_isInitialized) return;
            
            // Шанс 50% на выполнение замены и анимации
            if (Random.value < 0.4f)
            {
                // Список нечетных индексов
                List<int> oddIndices = new List<int>();
                for (int i = 0; i < _imageList.Count; i++)
                {
                    if (i % 2 == 0) // Нечетные индексы
                    {
                        oddIndices.Add(i);
                    }
                }

                if (oddIndices.Count < 2) return; // Если меньше двух нечетных индексов, ничего не делаем

                // Выбор двух различных случайных нечетных индексов
                int index1 = oddIndices[Random.Range(0, oddIndices.Count)];
                int index2;

                do
                {
                    index2 = oddIndices[Random.Range(0, oddIndices.Count)];
                } 
                while (index1 == index2); // Убедиться, что индексы разные

                // Обмен спрайтами между двумя выбранными изображениями
                Sprite tempSprite = _imageList[index1].sprite;
                _imageList[index1].sprite = _imageList[index2].sprite;
                _imageList[index2].sprite = tempSprite;

                // Запуск анимации для обеих изображений
                StartCoroutine(AnimateImage(_imageList[index1]));
                StartCoroutine(AnimateImage(_imageList[index2]));
            }
        }
    }
}
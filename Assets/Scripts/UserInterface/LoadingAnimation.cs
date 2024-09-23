using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Services;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class LoadingAnimation : MonoBehaviour
    {
        [SerializeField] private List<Image> _loadImages;
        [SerializeField] private float _fadeDuration = 0.5f;  
        [SerializeField] private float _scaleDuration = 0.5f; 
        [SerializeField] private float _maxScale = 140f;      
        [SerializeField] private float _minScale = 120f;  

        private void Start()
        {
            StartCoroutine(AnimateImages());
        }

        private IEnumerator AnimateImages()
        {
            GameInstance.UINavigation._isInLoad = true;
            foreach (var image in _loadImages)
            {
                yield return StartCoroutine(AnimateImage(image));
            }

            GameInstance.UINavigation.OpenMainMenu();
        }

        private IEnumerator AnimateImage(Image image)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
            image.rectTransform.sizeDelta = new Vector2(_minScale, _minScale);

            var elapsedTime = 0f;
            while (elapsedTime < _fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(0, 1, elapsedTime / _fadeDuration);
                image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
                yield return null;
            }

            elapsedTime = 0f;
            while (elapsedTime < _scaleDuration)
            {
                elapsedTime += Time.deltaTime;
                float scale = Mathf.Lerp(_minScale, _maxScale, elapsedTime / _scaleDuration);
                image.rectTransform.sizeDelta = new Vector2(scale, scale);
                yield return null;
            }

            elapsedTime = 0f;
            while (elapsedTime < _scaleDuration)
            {
                elapsedTime += Time.deltaTime;
                float scale = Mathf.Lerp(_maxScale, _minScale, elapsedTime / _scaleDuration);
                image.rectTransform.sizeDelta = new Vector2(scale, scale);
                yield return null;
            }
        }
    }
}

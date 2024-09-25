using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Image = UnityEngine.UI.Image;

namespace GameCore
{
    public class ImagesAnimation : MonoBehaviour
    {
        [SerializeField] private List<Image> _imageList;
        
        private const float scaleUp = 1.2f;
        private const float scaleNormal = 1f;
        private const float animationDuration = 0.7f;

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
    }
}
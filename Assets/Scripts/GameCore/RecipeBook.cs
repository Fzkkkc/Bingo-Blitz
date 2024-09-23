using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore
{
    public class RecipeBook : MonoBehaviour
    {
        [SerializeField] private Image _bookImage;          
        [SerializeField] private List<Sprite> _bookSprites; 
        
        [SerializeField] private Button _leftButton;        
        [SerializeField] private Button _rightButton;       
        private int _currentPageIndex = 0;                  

        private void Start()
        {
            _leftButton.onClick.AddListener(FlipLeft);
            _rightButton.onClick.AddListener(FlipRight);
            //UpdateBookImage();
        }
        
        private void OnDestroy()
        {
            _leftButton.onClick.RemoveListener(FlipLeft);
            _rightButton.onClick.RemoveListener(FlipRight);
        }


        private void FlipLeft()
        {
            if (_currentPageIndex == 0)
            {
                _currentPageIndex = _bookSprites.Count - 1;
            }
            else
            {
                _currentPageIndex--;
            }
            UpdateBookImage();
        }

        private void FlipRight()
        {
            if (_currentPageIndex == _bookSprites.Count - 1)
            {
                _currentPageIndex = 0;
            }
            else
            {
                _currentPageIndex++;
            }
            UpdateBookImage();
        }

        private void UpdateBookImage()
        {
            if (_bookImage != null && _bookSprites.Count > 0)
            {
                _bookImage.sprite = _bookSprites[_currentPageIndex];
                GameInstance.Audio.Play(GameInstance.Audio.BookSound);
            }
        }
    }
}

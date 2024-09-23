using UnityEngine;
using UnityEngine.EventSystems;

namespace UserInterface
{
    public class DragBgMenu : MonoBehaviour, IDragHandler
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private float minY = -468f;
        [SerializeField] private float maxY = 468f;

        private void OnValidate()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 newPosition = _rectTransform.anchoredPosition + new Vector2(0, eventData.delta.y);

            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

            _rectTransform.anchoredPosition = newPosition;
        }
    }
}
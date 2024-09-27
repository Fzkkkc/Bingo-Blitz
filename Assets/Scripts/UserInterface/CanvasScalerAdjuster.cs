using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class CanvasScalerAdjuster : MonoBehaviour
    {
        [SerializeField] private CanvasScaler _canvasScaler;

        private void Start()
        {
            SetMatchWidthOrHeight();
        }

        private void Update()
        {
            SetMatchWidthOrHeight();
        }

        private void SetMatchWidthOrHeight()
        {
            if (IsPhone() || Screen.width < 1350)
            {
                _canvasScaler.matchWidthOrHeight = 1f;
            }
            else if (IsTablet() || Screen.width >= 1350)
            {
                _canvasScaler.matchWidthOrHeight = 0f;
            }
        }

        private bool IsPhone()
        {
            return (Application.platform == RuntimePlatform.Android && !IsTablet()) ||
                   (Application.platform == RuntimePlatform.IPhonePlayer && !IsTablet());
        }

        private bool IsTablet()
        {
            return (Application.platform == RuntimePlatform.Android && Screen.width >= 1350) ||
                   (Application.platform == RuntimePlatform.IPhonePlayer && IsIpad());
        }

        private bool IsIpad()
        {
            return SystemInfo.deviceModel.StartsWith("iPad");
        }
    }
}
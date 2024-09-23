using UnityEngine;

namespace UserInterface
{
    public class RotateWithAccelerometer : MonoBehaviour
    {
        [SerializeField] private RectTransform _target; 
        [SerializeField] private float _minRotation = -10f;
        [SerializeField] private float _maxRotation = 10f;

        private void OnValidate()
        {
            _target ??= GetComponent<RectTransform>();
        }

        private void Update()
        {
            var accelerometerX = Input.acceleration.x;

            var rotationZ = Mathf.Lerp(_minRotation, _maxRotation, (accelerometerX + 1) / 2);

            rotationZ = Mathf.Clamp(rotationZ, _minRotation, _maxRotation);

            _target.localEulerAngles = new Vector3(0, 0, rotationZ);
        }
    }
}
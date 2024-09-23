using Services;
using UnityEngine;
using UnityEngine.UI;

namespace UserInterface
{
    public class ButtonFX : MonoBehaviour
    {

        [SerializeField] private Button _fxButton;

        private void OnValidate()
        {
            _fxButton ??= GetComponent<Button>();
        }

        private void Start()
        {
            _fxButton.onClick.AddListener(PlayButtonFX);
        }

        private void OnDestroy()
        {
            _fxButton.onClick.AddListener(PlayButtonFX);
        }

        private void PlayButtonFX()
        {
            GameInstance.Audio.Play(GameInstance.Audio.TapSound);
            GameInstance.FXController.PlayTapParticle(_fxButton.transform.position);
        }
    }
}
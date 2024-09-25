using System.Collections;
using System.Collections.Generic;
using Services;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore
{
    public class GameState : MonoBehaviour
    {
        [SerializeField] private Animator _wonComboAnimator;
        [SerializeField] private List<Button> _starButtons;

        public Timer Timer;
        
        private int StarCount => GameInstance.MapRoadNavigation
            ._levelsData[GameInstance.MapRoadNavigation._currentMainLevel].starCount;
        
        public void Init()
        {
            GameInstance.UINavigation.OnGameStarted += StartComboAnim;
            GameInstance.UINavigation.OnGameStarted += UpdateStarButtons;
            GameInstance.UINavigation.OnGameStarted += OpenPickCard;
            GameInstance.UINavigation.OnGameWindowClosed += StopComboAnim;
            Timer.Init();
            Timer.OnTimerStopped += OpenGameField;
        }

        private void OnDestroy()
        {
            GameInstance.UINavigation.OnGameStarted -= StartComboAnim;
            GameInstance.UINavigation.OnGameWindowClosed -= StopComboAnim;
            GameInstance.UINavigation.OnGameStarted -= OpenPickCard;
            GameInstance.UINavigation.OnGameStarted -= UpdateStarButtons;
            Timer.OnTimerStopped -= OpenGameField;
        }

        private void StartComboAnim()
        {
            _wonComboAnimator.SetTrigger("PlayCombo");
        }

        private void StopComboAnim()
        {
            _wonComboAnimator.SetTrigger("StopCombo");
        }
        
        private void UpdateStarButtons()
        {
            for (int i = 0; i < _starButtons.Count; i++)
            {
                _starButtons[i].interactable = (i < StarCount);
            }
        }

        private void OpenPickCard()
        {
            StartCoroutine(OpenCardPopup());
        }

        private IEnumerator OpenCardPopup()
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(GameInstance.UINavigation.AnimateScale(GameInstance.UINavigation.GamePopups[4], true));
        }

        private void OpenGameField()
        {
            
        }
    }
}
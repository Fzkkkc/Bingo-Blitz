using GameCore;
using UnityEngine;
using UserInterface;

namespace Services
{
    public class GameInstance : Singleton<GameInstance>
    {
        [SerializeField] private UINavigation _uiNavigation;
        [SerializeField] private FXController _fxController;
        [SerializeField] private AudioSystem _audio;
        [SerializeField] private MusicSystem _music;
        [SerializeField] private GameState _gameState;
        [SerializeField] private MoneyManager _moneyManager;
        [SerializeField] private Timer _timer;
        
        public static UINavigation UINavigation => Default._uiNavigation;
        public static FXController FXController => Default._fxController;
        public static AudioSystem Audio => Default._audio;
        public static GameState GameState => Default._gameState;
        public static MusicSystem MusicSystem => Default._music;
        public static MoneyManager MoneyManager => Default._moneyManager;
        public static Timer Timer => Default._timer;

        protected override void Awake()
        { 
            //PlayerPrefs.DeleteAll();
            base.Awake();
            _uiNavigation.Init();
            _fxController.Init();
            _audio.Init();
            _music.Init();
            _gameState.Init();
            _timer.Init();
            _moneyManager.Init(0);
        }
    }
}
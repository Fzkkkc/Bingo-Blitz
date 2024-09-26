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
        [SerializeField] private MoneyManager _moneyManager;
        [SerializeField] private MapRoadNavigation _mapRoadNavigation;
        [SerializeField] private GameState _gameState;
        
        public static UINavigation UINavigation => Default._uiNavigation;
        public static FXController FXController => Default._fxController;
        public static AudioSystem Audio => Default._audio;
        public static MusicSystem MusicSystem => Default._music;
        public static MoneyManager MoneyManager => Default._moneyManager;
        public static MapRoadNavigation MapRoadNavigation => Default._mapRoadNavigation;
        public static GameState GameState => Default._gameState;

        public int _frameRate = 60;
        
        protected override void Awake()
        { 
            //PlayerPrefs.DeleteAll();
            base.Awake();
            _uiNavigation.Init();
            _fxController.Init();
            _audio.Init();
            _music.Init();
            _mapRoadNavigation.Init();
            _gameState.Init();
            _moneyManager.Init(0);
            QualitySettings.vSyncCount = 0;
            QualitySettings.SetQualityLevel(1);
            if (_frameRate != Application.targetFrameRate)
                Application.targetFrameRate = _frameRate;
        }
    }
}
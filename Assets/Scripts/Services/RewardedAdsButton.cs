using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

namespace Services
{
    public class RewardedAdsButton : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
    {
        [SerializeField] private Button _showAdButton;
        [SerializeField] private Button _showAdButton2;
        [SerializeField] private Button _showAdButton3;
        [SerializeField] private Button _showAdButton4;
        [SerializeField] private string _androidAdUnitId = "Rewarded_Android";
        [SerializeField] private string _androidAdUnitId2 = "Rewarded_Android2";
        [SerializeField] private string _androidAdUnitId3 = "Rewarded_Android3";
        [SerializeField] private string _androidAdUnitId4 = "Rewarded_Android4";
        [SerializeField] private string _iOSAdUnitId = "Rewarded_iOS";
        [SerializeField] private string _iOSAdUnitId2 = "Rewarded_iOS2";
        [SerializeField] private string _iOSAdUnitId3 = "Rewarded_iOS3";
        [SerializeField] private string _iOSAdUnitId4 = "Rewarded_iOS4";
        private string _adUnitId; // This will remain null for unsupported platforms
        private string _adUnitId2;
        private string _adUnitId3;
        private string _adUnitId4;

        private void Awake()
        {
            // Get the Ad Unit ID for the current platform:
#if UNITY_IOS
            _adUnitId = _iOSAdUnitId;
            _adUnitId2 = _iOSAdUnitId2;
            _adUnitId3 = _iOSAdUnitId3;
            _adUnitId4 = _iOSAdUnitId4;
#elif UNITY_ANDROID
            _adUnitId = _androidAdUnitId;
            _adUnitId2 = _androidAdUnitId2;
            _adUnitId3 = _androidAdUnitId3;
            _adUnitId4 = _androidAdUnitId4;
#endif

            //Disable the button until the ad is ready to show:
            _showAdButton.interactable = false;
            _showAdButton2.interactable = false;
            _showAdButton3.interactable = false;
            _showAdButton4.interactable = false;
        }

        // Load content to the Ad Unit:
        public void LoadAd()
        {
            // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
            Advertisement.Load(_adUnitId, this);

            Advertisement.Load(_adUnitId2, this);

            Advertisement.Load(_adUnitId3, this);

            Advertisement.Load(_adUnitId4, this);
            
            _showAdButton.onClick.AddListener(ShowAd);
            _showAdButton2.onClick.AddListener(ShowAd2);
            _showAdButton3.onClick.AddListener(ShowAd3);
            _showAdButton4.onClick.AddListener(ShowAd4);
        }

        // If the ad successfully loads, add a listener to the button and enable it:
        public void OnUnityAdsAdLoaded(string adUnitId)
        {

            //// Button 1 ////
            if (adUnitId.Equals(_adUnitId))
                // Configure the button to call the ShowAd() method when clicked:

                // Enable the button for users to click:
                _showAdButton.interactable = true;


            /// ad 2 ////         
            if (adUnitId.Equals(_adUnitId2))
                // Configure the button to call the ShowAd() method when clicked:

                // Enable the button for users to click:
                //_showAdButton2.interactable = true;

                
           
            _showAdButton2.interactable = true;

            /// ad 3 ////
            ///
            /// 

            //if (adUnitId.Equals(_adUnitId3))

            // Configure the button to call the ShowAd() method when clicked:

            // Enable the button for users to click:
            _showAdButton3.interactable = true;
            _showAdButton4.interactable = true;
            
            
            if (PlayerPrefs.GetInt("DiamondAdCounter", 0) == 3)
            {
                _showAdButton2.interactable = false;
            }
            if (PlayerPrefs.GetInt("CoinAdCounter", 0) == 2)
            {
                _showAdButton.interactable = false;
            } 
        }

        public void OnUnityAdsAdLoaded2(string adUnitId2)
        {
        }

        // Implement a method to execute when the user clicks the button:
        public void ShowAd()
        {
            // Disable the button:
            _showAdButton.interactable = false;
            // Then show the ad:
            Advertisement.Show(_adUnitId, this);
        }

        public void ShowAd2()
        {
            // Disable the button:
            _showAdButton2.interactable = false;
            // Then show the ad:
            Advertisement.Show(_adUnitId2, this);
        }

        public void ShowAd3()
        {
            // Disable the button:
            _showAdButton3.interactable = false;
            // Then show the ad:
            Advertisement.Show(_adUnitId3, this);
        }
        
        public void ShowAd4()
        {
            // Disable the button:
            _showAdButton4.interactable = false;
            // Then show the ad:
            Advertisement.Show(_adUnitId4, this);
        }

        // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
        public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
        {
            if (adUnitId.Equals(_adUnitId) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
            {
                // Grant a reward.
                //Wheel Ad
                GameInstance.UINavigation.RewardedFramesCounters.IncreaseAdCounter("coin");

                // Load another ad:
                Advertisement.Load(_adUnitId, this);
                
                if (PlayerPrefs.GetInt("DiamondAdCounter", 0) == 3)
                {
                    _showAdButton2.interactable = false;
                }
                if (PlayerPrefs.GetInt("CoinAdCounter", 0) == 2)
                {
                    _showAdButton.interactable = false;
                } 
            }

            if (adUnitId.Equals(_adUnitId2) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
            {
                // Grant a reward.
                //Daily Ad

                GameInstance.UINavigation.RewardedFramesCounters.IncreaseAdCounter("diamond");
                // Load another ad:
                Advertisement.Load(_adUnitId2, this);
                
                if (PlayerPrefs.GetInt("DiamondAdCounter", 0) == 3)
                {
                    _showAdButton2.interactable = false;
                }
                if (PlayerPrefs.GetInt("CoinAdCounter", 0) == 2)
                {
                    _showAdButton.interactable = false;
                } 
            }

            if (adUnitId.Equals(_adUnitId3) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
            {
                // Grant a reward.
                //Weekly Ad
                GameInstance.MoneyManager.AddCoinsCurrency(3);
                // Load another ad:
                Advertisement.Load(_adUnitId3, this);
            }
            
            if (adUnitId.Equals(_adUnitId4) && showCompletionState.Equals(UnityAdsShowCompletionState.COMPLETED))
            {
                // Grant a reward.
                //Weekly Ad

                GameInstance.MoneyManager.AddDiamondsCurrency(1);
                    // Load another ad:
                Advertisement.Load(_adUnitId4, this);
            }
        }

        // Implement Load and Show Listener error callbacks:
        public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
        {
            // Use the error details to determine whether to try to load another ad.
        }

        public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
        {
            // Use the error details to determine whether to try to load another ad.
        }

        public void OnUnityAdsShowStart(string adUnitId)
        {
        }

        public void OnUnityAdsShowClick(string adUnitId)
        {
        }

        private void OnDestroy()
        {
            // Clean up the button listeners:
            _showAdButton.onClick.RemoveAllListeners();
            _showAdButton2.onClick.RemoveAllListeners();
            _showAdButton3.onClick.RemoveAllListeners();
            _showAdButton4.onClick.RemoveAllListeners();
        }
    }
}
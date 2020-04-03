using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;


//form unity tutorial
[RequireComponent(typeof(Button))]
public class AdScript : MonoBehaviour, IUnityAdsListener
{

    #if UNITY_IOS
    private string gameId = "3510681";
    #elif UNITY_ANDROID
    private string gameId = "3510680";
    #endif

    Button myButton;
    public string myPlacementId = "rewardedVideo";
    private GameObject netInfo;

    void Start()
    {
        netInfo = GameObject.FindGameObjectWithTag("NetInfo");
        netInfo.SetActive(false);
        myButton = GetComponent<Button>();

        // Set interactivity to be dependent on the Placement’s status:
        myButton.interactable = Advertisement.IsReady(myPlacementId);

        // Map the ShowRewardedVideo function to the button’s click listener:
        if (myButton) myButton.onClick.AddListener(ShowRewardedVideo);

        // Initialize the Ads listener and service:
        Advertisement.AddListener(this);
        Advertisement.Initialize(gameId, true);

        //tylko raz wyswietlamy reklame
        if (PlayerPrefs.GetInt("ADcounter", 0) == 1)
        {
            this.gameObject.SetActive(false);
        }
        //czy mamy internet?
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            this.gameObject.SetActive(false);
            netInfo.SetActive(true);
        }
    }

    // Implement a function for showing a rewarded video ad:
    void ShowRewardedVideo()
    {
        Advertisement.Show(myPlacementId);
    }

    // Implement IUnityAdsListener interface methods:
    public void OnUnityAdsReady(string placementId)
    {
        // If the ready Placement is rewarded, activate the button: 
        if (placementId == myPlacementId && myButton != null)
        {
            myButton.interactable = true;
        }
    }

    public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
    {
        // Define conditional logic for each ad completion status:
        if (showResult == ShowResult.Finished)
        {
            // Reward the user for watching the ad to completion.
            PlayerPrefs.SetInt("ADdisplayed", 1);
            //tylko raz mozemy wyswietlic reklame
            PlayerPrefs.SetInt("ADcounter", 1);
        }
        else if (showResult == ShowResult.Skipped)
        {
            // Do not reward the user for skipping the ad.
        }
        else if (showResult == ShowResult.Failed)
        {
            Debug.LogWarning("The ad did not finish due to an error.");
        }
    }

    public void OnUnityAdsDidError(string message)
    {
        // Log the error.
    }

    public void OnUnityAdsDidStart(string placementId)
    {
        // Optional actions to take when the end-users triggers an ad.
    }
}
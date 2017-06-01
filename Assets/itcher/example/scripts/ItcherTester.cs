
using System;
using UnityEngine;
using UnityEngine.UI;

public class ItcherTester : MonoBehaviour {

    private Itcher itcher;
    private ItcherUser launcherUser;

    public InputField itchApiKeyInputField;
    public InputField userEmailInputField;
    public GamesList gamesList;

    public Text DownloadKeyText;
    public Text DownlaodKeyDateText;

    public Text PurchasesText;

    public Text UserNameText;
    public Text DisplayNameText;
    public Text UserIdText;
    public Text GamerText;
    public Text DeveloperText;
    public Text PressText;    

	void Start () {
        itcher = GetComponent<Itcher>();
        if (Environment.GetEnvironmentVariable("ITCHIO_API_KEY") != null)
        {
            itcher.GetMe(onMeComplete);
            userEmailInputField.text = "";
            userEmailInputField.placeholder.GetComponent<Text>().text = "A user has been provided by the launcher.";
            userEmailInputField.readOnly = true;
        }
    }
	
    void SetUser(ItcherUser user)
    {
        UserNameText.text       = "User Name: " + user.username;
        DisplayNameText.text    = "Display Name: " + user.display_name;
        UserIdText.text         = "User ID: " + user.id.ToString();
        GamerText.text          = "Is Gamer: " + user.gamer.ToString();
        DeveloperText.text      = "Is Developer: " + user.developer.ToString();
        PressText.text          = "Is Press: " + user.press_user.ToString();
    }

    void SetDownloadKey(ItcherDownloadKey downloadKey)
    {
        DownloadKeyText.text = "Download Key: " + downloadKey.key;
        DownlaodKeyDateText.text = "Issued Date: " + downloadKey.created_at;
        SetUser(downloadKey.owner);
    }

    void SetPurchases(ItcherPurchase [] purchases)
    {
        PurchasesText.text = "";
        foreach (ItcherPurchase p in purchases)
        {
            PurchasesText.text += p.id + ": " + p.price + "\n";
        }
    }

    private void onMeComplete(ItcherMeRequestResult result)
    {
        if (result.successfull)
        {
            SetUser(result.response.user);
            if (Environment.GetEnvironmentVariable("ITCHIO_API_KEY") != null)
            {
                launcherUser = result.response.user;
            }
        }
    }

    public void GetMyGames()
    {
        if (itchApiKeyInputField.text!=string.Empty)
        { 
            itcher.apiKey = itchApiKeyInputField.text;
            itcher.GetMyGames(onGetMyGamesComplete);
        }
    }

    private void onGetMyGamesComplete(ItcherMyGamesResult result)
    {
        if(result.successfull)
        {
            gamesList.SetGames(result.response.games);
        }
    }

    public void GetOwnershipVerification()
    {
        if (itchApiKeyInputField.text != string.Empty)
        {
            itcher.apiKey = itchApiKeyInputField.text;
            if (launcherUser != null)
            {
                itcher.GetDownloadKeysWithUserId(launcherUser.id, gamesList.Game.id, onGetDownloadKeyComplete);
                itcher.GetPurchasesWithUserId(launcherUser.id, gamesList.Game.id, onGetPurchasesComplete);
            }
            else if (userEmailInputField.text != string.Empty)
            {
                itcher.GetDownloadKeysWithEmail(userEmailInputField.text, gamesList.Game.id, onGetDownloadKeyComplete);
                itcher.GetPurchasesWithEmail(userEmailInputField.text, gamesList.Game.id, onGetPurchasesComplete);
            }
        }
    }

    private void onGetDownloadKeyComplete(ItcherDownloadKeysResult result)
    {
        if (result.successfull)
        {
            SetDownloadKey(result.response.download_key);
        }
    }

    private void onGetPurchasesComplete(ItcherPurchasesResult result)
    {
        if (result.successfull)
        {
            SetPurchases(result.response.purchases);
        }
    }
}

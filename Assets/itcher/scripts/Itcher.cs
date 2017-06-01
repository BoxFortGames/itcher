using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Itcher : MonoBehaviour
{
    private const string MeCallURL = "https://itch.io/api/1/jwt/me";
    private const string MyGamesCallURL = "https://itch.io/api/1/{key}/my-games";
    private const string DownloadKeysCallURL = "https://itch.io/api/1/{key}/game/{gameid}/download_keys";
    private const string PurchasesCallURL = "https://itch.io/api/1/{key}/game/{gameid}/purchases";

    /// <summary>
    /// This is required to call the GetMyGames, GetDownloadKeys, and GetPurchases methods.
    /// These keys give access to many parts of your account so you should treat them like a password.
    /// You should never include this key in the build that you are distributing to your end user.
    /// </summary>
    [Tooltip(@"This is required to call the GetMyGames, GetDownloadKeys, and GetPurchases methods.
These keys give access to many parts of your account so you should treat them like a password.
You should never include this key in the build that you are distributing to your end user.")]
    public string apiKey;

    /// <summary>
    /// ONLY WORKS FROM LAUNCHER: 
    /// Requests the "ME" object from the Serverside API so that we can get a validated
    /// user profile
    /// </summary>
    /// <param name="apiKey"> The temporary Api Key provided by the itch.io launcher. </param>
    /// <param name="onComplete"> A callback to let you know if the call 
    /// succeded and returns a the ItcherMeRequestResult that contains an ItcherUser </param>
    public void GetMe(Action<ItcherMeRequestResult> onComplete)
    {
        string key = Environment.GetEnvironmentVariable("ITCHIO_API_KEY");
        if (key != null)
        {
            StartCoroutine(GetMeInternal(key, onComplete));
        }
        else if(onComplete != null)
        {
            ItcherMeRequestResult result = new ItcherMeRequestResult();
            result.response = new ItcherMeResponse();
            result.response.errors = new string [] { "This call requires that your app is launced from the itch.io launcher.", "ITCHIO_API_KEY Eviroment Variable was not set." };
            onComplete(result);
        }
    }

    private IEnumerator GetMeInternal(string key, Action<ItcherMeRequestResult> onComplete)
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        headers.Add("Authorization", key);
        WWW www = new WWW(MeCallURL, new byte[] { 0 }, headers);
        yield return www;
        Debug.Log("GetMe");
        Debug.Log(www.text);
        ItcherMeResponse response;
        try
        {
            response = JsonUtility.FromJson<ItcherMeResponse>(www.text);
        }
        catch
        {
            response = null;
        }
        ItcherMeRequestResult irr = new ItcherMeRequestResult();
        irr.successfull = response != null && (response.errors == null || response.errors.Length == 0);
        irr.response = response;
        
        if (onComplete != null)
        {
            onComplete(irr);
        }
    }

    /// <summary>
    /// Request a list of all the games you've uploaded or have edit access to.
    /// </summary>
    /// <param name="onComplete"> A callback to let you know if the call 
    /// succeded and returns a the ItcherMyGamesResult object that contain an array
    /// of ItcherGame objects. </param>
    public void GetMyGames(Action<ItcherMyGamesResult> onComplete)
    {
        
        if (apiKey != null && apiKey != string.Empty)
        {
            StartCoroutine(GetMyGamesInternal(onComplete));
        }
        else if (onComplete != null)
        {
            ItcherMyGamesResult result = new ItcherMyGamesResult();
            result.response = new ItcherMyGamesResponse();
            result.response.errors = new string[] { "Itcher requires that an API key assigned before making this call." };
            onComplete(result);
        }
    }

    private IEnumerator GetMyGamesInternal(Action<ItcherMyGamesResult> onComplete)
    {
        WWW www = new WWW(MyGamesCallURL.Replace("{key}", apiKey));
        yield return www;
        Debug.Log("GetMyGames");
        Debug.Log(www.text);
        ItcherMyGamesResponse response;
        try
        {
            response = JsonUtility.FromJson<ItcherMyGamesResponse>(www.text);
        }
        catch
        {
            response = null;
        }
        ItcherMyGamesResult irr = new ItcherMyGamesResult();
        irr.successfull = response != null && (response.errors == null || response.errors.Length == 0);
        irr.response = response;

        if (onComplete != null)
        {
            onComplete(irr);
        }
    }

    /// <summary>
    /// Used the UserId to check if a download key exists for game and returns it. 
    /// Does not work for users that have downloaded the game for free.
    /// </summary>
    /// <param name="userId"> The itch.io user id you wish to check for. Us GetMe to get a valid userId </param>
    /// <param name="gameId"> The itch.io game id for your game. Can be obtained via the GetMyGames call. </param>
    /// <param name="onComplete">A callback to let you know if the call succeded and returns a the 
    /// ItcherDownloadKeysResult object that contain an ItcherDownloadKey. </param>
    public void GetDownloadKeysWithUserId(int userId, int gameId, Action<ItcherDownloadKeysResult> onComplete)
    {

        if (apiKey != null && apiKey != string.Empty)
        {
            string url = DownloadKeysCallURL.Replace("{key}", apiKey);
            url = url.Replace("{gameid}", gameId.ToString());
            url += "?user_id=" + userId;

            StartCoroutine(GetDownloadKeysIdInternal(url, onComplete));
        }
        else if (onComplete != null)
        {
            ItcherDownloadKeysResult result = new ItcherDownloadKeysResult();
            result.response = new ItcherDownloadKeysResponse();
            result.response.errors = new string[] { "Itcher requires that an API key assigned before making this call." };
            onComplete(result);
        }
    }

    /// <summary>
    /// Used an email address to check if a download key exists for game and returns it.
    /// Does not work for users that have downloaded the game for free.
    /// </summary>
    /// <param name="email"> The email address that you wish to check for. </param>
    /// <param name="gameId">The itch.io game id for your game. Can be obtained via the GetMyGames call.</param>
    /// <param name="onComplete">A callback to let you know if the call succeded and returns a the 
    /// ItcherDownloadKeysResult object that contain an ItcherDownloadKey. </param>
    public void GetDownloadKeysWithEmail(string email, int gameId, Action<ItcherDownloadKeysResult> onComplete)
    {

        if (apiKey != null && apiKey != string.Empty)
        {
            string url = DownloadKeysCallURL.Replace("{key}", apiKey);
            url = url.Replace("{gameid}", gameId.ToString());
            url += "?email=" + email;

            StartCoroutine(GetDownloadKeysIdInternal(url, onComplete));
        }
        else if (onComplete != null)
        {
            ItcherDownloadKeysResult result = new ItcherDownloadKeysResult();
            result.response = new ItcherDownloadKeysResponse();
            result.response.errors = new string[] { "Itcher requires that an API key assigned before making this call." };
            onComplete(result);
        }
    }

    /// <summary>
    /// Use a download key to check if a download key exists for game and returns it.
    /// Does not work for users that have downloaded the game for free.
    /// </summary>
    /// <param name="downloadKey">The download key that you wish to check the validity of.</param>
    /// <param name="gameId">The itch.io game id for your game. Can be obtained via the GetMyGames call.</param>
    /// <param name="onComplete">A callback to let you know if the call succeded and returns a the 
    /// ItcherDownloadKeysResult object that contain an ItcherDownloadKey. </param>
    public void GetDownloadKeysWithDownloadKey(string downloadKey, int gameId, Action<ItcherDownloadKeysResult> onComplete)
    {

        if (apiKey != null && apiKey != string.Empty)
        {
            string url = DownloadKeysCallURL.Replace("{key}", apiKey);
            url = url.Replace("{gameid}", gameId.ToString());
            url += "?download_key=" + downloadKey;

            StartCoroutine(GetDownloadKeysIdInternal(url, onComplete));
        }
        else if (onComplete != null)
        {
            ItcherDownloadKeysResult result = new ItcherDownloadKeysResult();
            result.response = new ItcherDownloadKeysResponse();
            result.response.errors = new string[] { "Itcher requires that an API key assigned before making this call." };
            onComplete(result);
        }
    }

    private IEnumerator GetDownloadKeysIdInternal(string prebuiltUrl, Action<ItcherDownloadKeysResult> onComplete)
    {

        WWW www = new WWW(prebuiltUrl);
        yield return www;
        Debug.Log("GetDownloadKeys");
        Debug.Log(www.text);
        ItcherDownloadKeysResponse response;
        try
        {
            response = JsonUtility.FromJson<ItcherDownloadKeysResponse>(www.text);
        }
        catch
        {
            response = null;
        }
        ItcherDownloadKeysResult irr = new ItcherDownloadKeysResult();
        irr.successfull = response != null && (response.errors == null || response.errors.Length == 0);
        irr.response = response;

        if (onComplete != null)
        {
            onComplete(irr);
        }
    }

    /// <summary>
    /// Use a userId to request a list of purchases related to your game that the user has made.
    /// Does not work for users that have downloaded the game for free.
    /// </summary>
    /// <param name="userId">The itch.io user id you wish to check for. Us GetMe to get a valid userId </param>
    /// <param name="gameId">The itch.io game id for your game. Can be obtained via the GetMyGames call.</param>
    /// <param name="onComplete">A callback to let you know if the call succeded and returns a the 
    /// ItcherPurchasesResult object that contain an array of ItcherPurchase objects. </param>
    public void GetPurchasesWithUserId(int userId, int gameId, Action<ItcherPurchasesResult> onComplete)
    {

        if (apiKey != null && apiKey != string.Empty)
        {
            string url = PurchasesCallURL.Replace("{key}", apiKey);
            url = url.Replace("{gameid}", gameId.ToString());
            url += "?user_id=" + userId;
            StartCoroutine(GetPurchasesInternal(url, onComplete));
        }
        else if (onComplete != null)
        {
            ItcherPurchasesResult result = new ItcherPurchasesResult();
            result.response = new ItcherPurchasesResponse();
            result.response.errors = new string[] { "Itcher requires that an API key assigned before making this call."};
            onComplete(result);
        }
    }

    /// <summary>
    /// Use an email to request a list of purchases related to your game that the user has made.
    /// Does not work for users that have downloaded the game for free.
    /// </summary>
    /// <param name="email">The email address that you wish to check for. </param>
    /// <param name="gameId">The itch.io game id for your game. Can be obtained via the GetMyGames call.</param>
    /// <param name="onComplete">A callback to let you know if the call succeded and returns a the 
    /// ItcherPurchasesResult object that contain an array of ItcherPurchase objects. </param>
    public void GetPurchasesWithEmail(string email, int gameId, Action<ItcherPurchasesResult> onComplete)
    {

        if (apiKey != null && apiKey != string.Empty)
        {
            string url = PurchasesCallURL.Replace("{key}", apiKey);
            url = url.Replace("{gameid}", gameId.ToString());
            url += "?email=" + email;
            StartCoroutine(GetPurchasesInternal(url, onComplete));
        }
        else if (onComplete != null)
        {
            ItcherPurchasesResult result = new ItcherPurchasesResult();
            result.response = new ItcherPurchasesResponse();
            result.response.errors = new string[] { "Itcher requires that an API key assigned before making this call." };
            onComplete(result);
        }
    }

    private IEnumerator GetPurchasesInternal(string prebuiltUrl, Action<ItcherPurchasesResult> onComplete)
    {

        WWW www = new WWW(prebuiltUrl);
        yield return www;
        Debug.Log("GetPurchases");
        Debug.Log(www.text);
        ItcherPurchasesResponse response;
        try
        {
            response = JsonUtility.FromJson<ItcherPurchasesResponse>(www.text);
        }
        catch
        {
            response = null;
        }
        ItcherPurchasesResult irr = new ItcherPurchasesResult();
        irr.successfull = response != null && (response.errors == null || response.errors.Length == 0);
        irr.response = response;

        if (onComplete != null)
        {
            onComplete(irr);
        }
    }
}

// Response Classes to assist with json parsing.

[Serializable]
public class ItcherMeResponse 
{
    public string[] errors;
    public ItcherUser user;

}

[Serializable]
public class ItcherMyGamesResponse
{
    public string[] errors;
    public ItcherGame [] games;
}

[Serializable]
public class ItcherDownloadKeysResponse
{
    public string[] errors;
    public ItcherDownloadKey download_key;
}

[Serializable]
public class ItcherPurchasesResponse
{
    public string[] errors;
    public ItcherPurchase[] purchases;
}

[Serializable]
public class ItcherUser
{
    public int id;
    public string username;
    public string display_name;
    public bool gamer;
    public bool developer;
    public bool press_user;
    public string url;
    public string cover_url;
}

[Serializable]
public class ItcherGame
{
    public int id;
    public string title;
    public string type;
    public string short_text;
    public string url;
    public string cover_url;
    public string created_at;

    public int min_price;

    public int views_count;
    public int downloads_count;
    public int purchase_count;

    public bool p_android;
    public bool p_linux;
    public bool p_osx;
    public bool p_windows;
    public bool published;
    public string published_at;

    public ItcherEarning[] earnings;
}

[Serializable]
public class ItcherEarning
{
    public string currency;
    public string ammount_formatted;
    public int ammount;
}

[Serializable]
public class ItcherDownloadKey
{
    public int id;
    public int game_id;
    public string key;
    public string created_at;
    public int downloads;
    public ItcherUser owner;
}

[Serializable]
public class ItcherPurchase
{
    public int id;
    public int game_id;
    public int sale_rate;
    public string price;
    public string currency;
    public string purchase_type;
    public string status;
    public string email;
    public string source;
    public bool donation;
    public string created_at;
}

// Request Results

[Serializable]
public class ItcherRequestResult
{
    public bool successfull;
}

[Serializable]
public class ItcherMeRequestResult : ItcherRequestResult
{
    public ItcherMeResponse response;
}

[Serializable]
public class ItcherMyGamesResult : ItcherRequestResult
{
    public ItcherMyGamesResponse response;
}

[Serializable]
public class ItcherDownloadKeysResult : ItcherRequestResult
{
    public ItcherDownloadKeysResponse response;
}

[Serializable]
public class ItcherPurchasesResult : ItcherRequestResult
{
    public ItcherPurchasesResponse response;
}
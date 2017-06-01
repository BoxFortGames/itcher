# itcher
Unity Wrapper for the itch.io serverside API

# licence
See LICENCE file for licence details.

# usage
```C#
public class Example
{
  int userId;
  string username;
  string displayname;

  public void GetLauncerProfile()
  {
    itcher.apiKey = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
    itcher.GetMe(onMeComplete);
  }

  private void onMeComplete(ItcherMeRequestResult result)
  {
    if (result.successfull)
    {
      userId = result.response.user.id;
      username = result.response.user.username;
      displayname = result.response.user.display_name;
      itcher.GetDownloadKeysWithUserId(userId, myGameId, onGetDownloadKeyComplete);
      itcher.GetPurchasesWithUserId(userId, myGameId, onGetPurchasesComplete);
    }
  }
}
```

# itcher
Unity Wrapper for the itch.io serverside API

# licence
See LICENCE file for licence details.

# launcher integration
In order to use the user provide by the launcher you need to provide a .itch.toml file in the base folder of the zip file you upload that includes an action with the scope set to profile:me

Here is an example .itch.toml file:
```toml
[[actions]]
name = "play"
path = "mygame.exe"
sandbox = true
scope = "profile:me"
```
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

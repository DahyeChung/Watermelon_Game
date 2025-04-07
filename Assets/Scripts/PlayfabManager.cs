using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using UnityEngine;

public class PlayfabManager : MonoBehaviour
{
    private void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
            PlayFabSettings.staticSettings.TitleId = "5C96C"; // from playfab

        LogIn();
    }
    void LogIn()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }
    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, you made your first successful API call!");
        Debug.Log("PlayFab ID: " + result.PlayFabId);
        Debug.Log("Session Ticket: " + result.SessionTicket);
    }
    private void OnLoginFailure(PlayFabError error)
    {
        Debug.Log("Error while logging in: " + error.ErrorMessage);
        Debug.LogError(error.GenerateErrorReport());
    }

    public void SendLeaderBoard(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "GameScore", // from playfab
                    Value = score
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderboardUpdate, OnError);
    }
    void OnLeaderboardUpdate(UpdatePlayerStatisticsResult result)
    {
        Debug.Log("Successfully updated player score.");
    }

    void OnError(PlayFabError error)
    {
        Debug.Log("Error while updating leaderboard: " + error.ErrorMessage);
        Debug.LogError(error.GenerateErrorReport());
    }
}

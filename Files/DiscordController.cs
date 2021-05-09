using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
using System;

public class DiscordController : MonoBehaviour
{
    [Header("Game Presence")]
    [Tooltip("The id of your games discord application. You can create this in the Discord dev portal -> new application -> General info -> application id")]
    public long application_id;
    public string details; //ex. survival or deathmatch
    public string state; //ex. local co-op or in game
    [Tooltip("Key for your game's discord application's image, which should be the game's icon. found in Rich Presence -> Art Assets")]
    public string largeImageKey;
    [Tooltip("Appears when mouse hovers over the image. Should put the game name here.")]
    public string largeImageText;
    public string smallImageKey;
    [Tooltip("Appears when mouse hovers over the image. It should be what the image is. Ex: level 2")]
    public string smallImageText;

    //private stuffs
    public int partySize;
    public int maxPartySize;
    private string[] testText = new string[3] {"yay", "testing", "very cool"};

    //Technical stuff (don't change)
    
    public Discord.Discord discord;
    private long milliseconds = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds; //used for time playing/ ellapsed

    private void Start() {
        OpeningSettings();
        //InvokeRepeating("RandomValues", 1f, 4f);
    }

    void RandomValues(){
        SetPartySize(UnityEngine.Random.Range(0, 6), 5);

        int i = UnityEngine.Random.Range(0, 3);
        int y = UnityEngine.Random.Range(0, 3);
        SetStatusCommon(testText[i], testText[y]);
    }

    void OpeningSettings()
    {
        discord = new Discord.Discord(application_id, (System.UInt64)Discord.CreateFlags.Default);

        if(discord == null){
            Debug.Log("Discord not detected");
            gameObject.SetActive(false);
            return;
        }
        else{
            setActivityDefault();
        }

    }

    void setActivityDefault(){
        var activityManager = discord.GetActivityManager();
        var activity = new Discord.Activity {
            Details = details,
            State = state,
            Timestamps =
            {
                Start = milliseconds,
            },
            Assets =
            {
                LargeImage = largeImageKey, // Larger Image Asset Key
                LargeText = largeImageText, // Large Image Tooltip
                SmallImage = smallImageKey, // Small Image Asset Key
                SmallText = smallImageText, // Small Image Tooltip
            },
            Party =
            {
                Id = "12345678", //later
                Size = {
                    CurrentSize = partySize,
                    MaxSize = maxPartySize,
                },
            },
        };
        activityManager.UpdateActivity(activity, (res) => {
            if(res == Discord.Result.Ok){
                Debug.Log("Discord status set");
            }
            else{
                Debug.LogError("Failed to set discord status");
            }
        });
    }

    void Update()
    {
        if(discord == null){
            OpeningSettings(); //this will try again, then disable the script
            return;        
        }
        
        discord.RunCallbacks();
    }

    public void SetStatusCommon(string gameMode, string status){
        details = gameMode;
        state = status;
        setActivityDefault();
    }

    public void SetPartySize(int current, int max){ //this can be: Alive & Max, Joined & Total slots, etc...
        partySize = current;
        maxPartySize = max;
        setActivityDefault();
    }

    public void SetStatusSmallImage(string key, string text){ //if this doesn't work, check the key in the dev portal (new images may not work for an hour or 2 after being uploaded to the portal, just how the portal works D:)
        smallImageKey = key;
        smallImageText = text;
        setActivityDefault();
    }

    public void SetStatusLargeImage(string key, string text){ //this won't be used much
        largeImageKey = key;
        largeImageText = text;
        setActivityDefault();
    }

    private void OnApplicationQuit() {
        ClearStatus(application_id);
    }

    private void ClearStatus(long id){
        var activityManager = discord.GetActivityManager();

        activityManager.ClearActivity((result) =>
        {
            if (result == Discord.Result.Ok)
            {
                Debug.Log("Success!");
            }
            else
            {
                Debug.Log("Failed");
            }
        });
    }
}

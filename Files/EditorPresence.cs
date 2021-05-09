using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;
using System;

public class EditorPresence : MonoBehaviour
{
    [Header("General")]
    public string gameName;

    [Header("Editor Presence")]
    [Tooltip("Your will display what you are working on, Ex. Editing combat system or Designing levels")]
    public string workingOn;

    //Technical stuff (don't change)
    public Discord.Discord discord;
    private long editor_id = 800822797530038272; //This is my discord aplication containing the pictures used in the presence. DO NOT CHANGE... unless you have a plan
    private long milliseconds = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds; //used for time playing/ ellapsed

    private void Start() {
        OpeningSettings();
    }

    //the porblem is the discord portal appliction has gone bad, make a new one
    void OpeningSettings()
    {
        discord = new Discord.Discord(editor_id, (System.UInt64)Discord.CreateFlags.Default);

        if(discord == null){
            Debug.Log("Discord not detected");
            gameObject.SetActive(false);
            return;
        }
        else{
            setActivityDefault();
            StartCoroutine(ReloadStatusCheck());
        }

    }

    void setActivityDefault(){ //updating activity (i know the function name sucks, I'm not sure how it ended up like this...)
        var activityManager = discord.GetActivityManager();
        var activity = new Discord.Activity {
            Details = "Project: " + gameName,
            State = workingOn,
            Timestamps =
            {
                Start = milliseconds,
            },
            Assets =
            {
                LargeImage = "unity-tab", // Larger Image Asset Key
                LargeText = "Unity Game Engine", // Large Image Tooltip
                SmallImage = "vscode", // Small Image Asset Key
                SmallText = "Using Visual Studio Code", // Small Image Tooltip
            },
            Party =
            {
                Id = "12345678", //later
                Size = {
                    CurrentSize = 0,
                    MaxSize = 0,
                },
            },
        };
        activityManager.UpdateActivity(activity, (res) => {
            if(res == Discord.Result.Ok){
                Debug.Log("Discord status set (Unity Editor)");
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

    IEnumerator ReloadStatusCheck(){
        int check = 1;

        while(check == 1){
            yield return new WaitForSeconds(5);
            setActivityDefault();
            Debug.Log("reloaded");

        }    
    }

    private void OnApplicationQuit() {
        ClearStatus(editor_id);
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

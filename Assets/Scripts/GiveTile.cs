using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class GiveTile : MonoBehaviour
{
    public int maxOfflineTime = 6;
    public TextMeshProUGUI numTiles;
    public TextMeshProUGUI tilesIn;

    public static int tilesPerTime;

    double currentTime;
    int lastLogOff;
    double currentTimeNotRound;
    
    public static int tiles;


    // Start is called before the first frame update
    void Start()
    {

        lastLogOff = PlayerPrefs.GetInt("LastLogOff");
        tilesPerTime = 1;
        currentTime = Redis.ServerTime();
        tiles = PlayerPrefs.GetInt("NumOfTiles");

        if (lastLogOff == 0)
            lastLogOff = (int)currentTime;

        int timesTiles  = (int)((currentTime - lastLogOff) / 300);
        if (timesTiles > 5)
            timesTiles = 5;
        tiles += tilesPerTime * timesTiles;
        PlayerPrefs.SetInt("NumOfTiles", tiles);
        numTiles.text = $"Unplaced: {tiles}";

        currentTimeNotRound = currentTime;
        currentTime = currentTime % 300;
        numTiles.text = $"Unplaced: {tiles}";

    }

    bool noRepeat = true;

    public int timeOffset;
    
    private void Update()
    {
        currentTime += Time.unscaledDeltaTime;
        double fiveMinTimer = 300 - (int)(currentTime-timeOffset) % 300;
        int min = (int)fiveMinTimer / 60;
        int sec = (int)fiveMinTimer % 60;
        if(sec > 9)
            tilesIn.text = $"{tilesPerTime} Tiles In {min}:{sec}";
        else
            tilesIn.text = $"{tilesPerTime} Tiles In {min}:0{sec}";

        if ((fiveMinTimer == 0 || fiveMinTimer == 300) && noRepeat)
        {
            noRepeat = false;
            tiles += tilesPerTime;
            PlayerPrefs.SetInt("NumOfTiles", tiles);
            numTiles.text = $"Unplaced: {tiles}";
        }
        else if (fiveMinTimer == 299)
            noRepeat = true;
    }

    bool focused = false;

    private void OnApplicationFocus(bool focus)
    {
    }

    private void OnApplicationPause(bool pause)
    {
        currentTime = Redis.ServerTime();
        PlayerPrefs.SetInt("LastLogOff", (int)currentTime);
    }
}

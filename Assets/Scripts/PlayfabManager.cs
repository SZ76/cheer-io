using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using Newtonsoft.Json;
using System;
using TMPro;
using UnityEngine.UI;
using StackExchange.Redis;

public class PlayfabManager : MonoBehaviour
{

    public bool setData = false;
    public bool getData = false;
    // Start is called before the first frame update
    void Start()
    {
        //print(StringToInt("2P"));
        //Login();

        //localhost,abortConnect=false,connectTimeout=30000,responseTimeout=30000
        
    }

    int[] mapSize = new int[] { 2048, 1000 };
    int[] divider = new int[] { 16, 8 };

    private void Update()
    {

        if (setData)
        {
            setData = false;
            setBlankMap();
            
        }
        if (getData)
        {
            getData = false;
            GetData();
        }
    }


    void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = SystemInfo.deviceUniqueIdentifier,
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnSuccess, OnError);
    }


    public static void SaveData(string s, object obj)
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>()
            {
                {s, JsonConvert.SerializeObject(obj) }
            },
            Permission = UserDataPermission.Public

        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSent, OnError);
    }


    public void GetData()
    {
        setData = false;
        GetUserDataRequest request = new GetUserDataRequest()
        {
            PlayFabId = "7B4A994A420C8878"
        };
        PlayFabClientAPI.GetUserData(request, OnDataRecieved, OnError);
        
    }
    public GameObject tile;
    public static Color[] colors = new Color[] { Color.black, Color.red, Color.yellow, Color.green, Color.blue, Color.cyan, Color.magenta, Color.gray};

    public SpriteRenderer map;
    public static short[,] mapTiles = new short[2048, 1000];
    void OnDataRecieved(GetUserDataResult result)
    {
        Debug.Log("Data Recieved");
        float startTime = Time.time;
        if (result.Data != null && result.Data.ContainsKey("l1"))
        {
            //List<short> xVal = JsonConvert.DeserializeObject<List<short>>(result.Data["x"].Value);
            //List<short> yVal = JsonConvert.DeserializeObject<List<short>>(result.Data["y"].Value);
            for (int n = 0; n < divider[0] * divider[1]; n++)
            {
                List<string> lVal = JsonConvert.DeserializeObject<List<string>>(result.Data["l" + n].Value);

                for (int i = 0; i < lVal.Count; i += 1)
                {
                    for (int ofE = 0; ofE < 8; ofE++)
                    {
                        //each item in lVal is 8 tiles. i iterates trhough the items, ofE iterates through the 8 tiles per item

                        int x = (i * 8 + ofE) % (mapSize[0] / divider[0]) + ((n / (divider[0] / 2)) * (mapSize[0] / divider[0]));
                        int y = ((i * 8 + ofE) / (mapSize[0] / divider[0])) + ((n % (divider[1])) * (mapSize[1] / divider[1]));

                        int dataLoaded = 0;
                        Color c;
                        if (lVal[i] == null || lVal[i] == "" || lVal[i][ofE * 2] == '$')
                            c = new Color(27 / 255f, 84 / 255f, 173 / 255f);
                        else {
                            dataLoaded = (StringToInt(lVal[i][ofE * 2] + "" + lVal[i][ofE * 2 + 1]));
                            c = colors[(dataLoaded - 1000) / 100];
                        }
                        mapTiles[x,y] = (short)dataLoaded;
                        map.sprite.texture.SetPixel(x, y, c);
                    }
                }
            }
            map.sprite.texture.Apply();
            print("completed download in" + (startTime - Time.time) + " seconds");
        }
    }
    public static void OnDataSent(UpdateUserDataResult a)
    {
        Debug.Log(a);
    }
    void OnSuccess(LoginResult result)
    {
        Debug.Log("Successful login/account created");
        //GetData();
        //setBlankMap();
    }

    public static void OnError(PlayFabError error)
    {
        Debug.Log("error while logging in/creating account");
        Debug.Log(error.GenerateErrorReport());
    }

    public static string IntToString(int value)
    {
        string result = string.Empty;
        int targetBase = 126 - 40;
        do
        {
            result = (char)(40+(value % targetBase)) + result;
            value = value / targetBase;
        }
        while (value > 0);
        return result;
    }

    public static int StringToInt(string number)
    {
        int result = 0;
        foreach (char digit in number)
            result = result * (126 - 40) + (digit-40);
        return result;
    }

    public void setBlankMap()
    {
        for (int n = 0; n < 64; n+=1)
        {
            int tileSize = (mapSize[0] / divider[0] * mapSize[1] / divider[1]) / 8;
            string[] data = new string[tileSize];
            for(int i = 0; i < tileSize ; i++)
            {
                string newData = "";
                for(int ofE = 0; ofE < 8; ofE++)
                {
                    int x = (i * 8 + ofE) % (mapSize[0] / divider[0]) + ((n / (divider[0]/2)) * (mapSize[0] / divider[0]));
                    int y = ((i * 8 + ofE) / (mapSize[0] / divider[0])) + ((n % (divider[1])) * (mapSize[1] / divider[1]));
                    if (map.sprite.texture.GetPixel(x, y) != (Color.white))
                    {
                            
                        newData += IntToString(1000);
                    }
                    else
                    {
                        newData += "$$";
                    }
                }
                if (newData.Equals("$$$$$$$$$$$$$$$$"))
                    newData = "";

                data[i] = newData;
            }
            SaveData("l" + n, data);
        }
    }

    public static void StartCloudHelloWorld(string data, string name)
    {


        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "saveData", // Arbitrary function name (must exist in your uploaded cloud.js file)
            FunctionParameter = new { inputValue =  data, key = name }, // The parameter provided to your function
            GeneratePlayStreamEvent = true, // Optional - Shows this event in PlayStream
        }, OnCloudHelloWorld, OnErrorShared);
    }

    static void OnCloudHelloWorld(ExecuteCloudScriptResult result)
    {
    }

    static void OnErrorShared(PlayFabError error)
    {

    }
}
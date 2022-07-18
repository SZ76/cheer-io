using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StackExchange.Redis;
using TMPro;
using System;

public class Redis : MonoBehaviour
{
    // Start is called before the first frame update

    public static IDatabase db;
    void Awake()
    {
        var conn = ConnectionMultiplexer.Connect(
            "redis-19898.c9.us-east-1-2.ec2.cloud.redislabs.com:19898, " +//server ip
            "abortConnect=false," +//needs to be false to connect
            " connectTimeout=30000," +//time to connect
            " responseTimeout=30000," +
            " SyncTimeout=10000," +//time to recieve map
            " password=j5vjyibR5ayh2Q1sBya24gZL7xzCI5Tc"//password??
        );
        db = conn.GetDatabase();

        colors = colorsPublic;
    }

    private void Start()
    {
    }

    // Update is called once per frame
    public SpriteRenderer map;
    public bool runIt = false;
    public bool getData = false;

    public Color[] colorsPublic;
    public static Color[] colors; 
    //new Color(22/255f,40/255f,128/255f), new Color(22/255f,128/255f,57/255f), Color.blue, Color.red, Color.cyan, Color.magenta, Color.gray

void Update()
    {
        if (runIt)
        {
            
            runIt = false;
            byte[] mapByte = new byte[2048 * 2 * 1000];
            byte[] mapID = new byte[2048 * 1000];
            for(int x = 0; x < 2048; x++)
            {
                for (int y = 0; y < 1000; y++)
                {
                    if(map.sprite.texture.GetPixel(x, y) != Color.white)//check if the map is land
                    {
                        mapByte[(x + 2048 * y) * 2] = 1;
                        mapID[x + 2048 * y] = 1;
                    }
                }
            }
            RedisValue mapVal = mapByte;
            db.StringSet("map", mapVal);
            RedisValue mapIDR = mapID;
            db.StringSet("mapID", mapIDR);
            print("done");
        }

        if (getData)
        {
            getData = false;
            GetMap();
        }
    }

    public static byte[] mapByte;
    public static byte[] mapID;
    public static int numOfTiles = 0;
    public static GameObject[] mapNum = new GameObject[2048 * 1000];
    public GameObject number;
    void GetMap()
    {
        if (db.IsConnected("map"))
        {
            print("Getting map");
            RedisValue val = db.StringGet("map");
            byte[] mapByte = (RedisValue)val;
            Redis.mapByte = mapByte;
            Redis.mapID = db.StringGet("mapID");
            for (int x = 0; x < 2048; x++)
            {
                for (int y = 0; y < 1000; y++)
                {

                    byte lvl = mapByte[(x + 2048 * y) * 2 + 1];
                    if (lvl > 0)
                    {
                        GameObject num = Instantiate(number, new Vector3(x, y, 0), Quaternion.Euler(0, 0, 0));
                        num.transform.SetParent( number.transform.parent);
                        num.GetComponent<TextMeshProUGUI>().SetText(lvl.ToString());
                        mapNum[x + y * 2048] = num;
                        int coord = PlayerPrefs.GetInt(x + " " + y, 0);
                        if (coord == mapID[x + 2048 * y])
                            numOfTiles += 1;
                        else if (coord != 0)
                            PlayerPrefs.DeleteKey(x + " " + y);

                    }
                    byte gotColor = mapByte[(x + 2048 * y) * 2];
                    
                    if (gotColor != 0)
                    {
                        /*Vector3 hsv;
                        Color.RGBToHSV(colors[gotColor], out hsv.x, out hsv.y, out hsv.z);

                        byte vSubtract = mapByte[(x + 2048 * y) * 2 + 1];

                        vSubtract = (byte)((float)vSubtract / 100f * hsv.z);//sets scale from 0 to 100 - darkens based on level
                        if (vSubtract > hsv.z)
                            vSubtract = (byte)hsv.z;//sets to black if greater than hsv.z (v) so it doesnt go negative

                        Color newColor = Color.HSVToRGB(hsv.x, hsv.y, hsv.z - vSubtract);
                        if(vSubtract > 0) print(hsv.z + " " + vSubtract);*/
                        map.sprite.texture.SetPixel(x, y, colors[gotColor]);
                    }
                }
            }
            GiveTile.tilesPerTime = (int)(numOfTiles / 5) + 1;
            map.sprite.texture.Apply();
        }
    }

    public static void SetPixel(int x, int y, byte a, byte b, byte id)
    {
        if (db.IsConnected("map"))
        {
            RedisValue val = new byte[] { a, b };
            db.StringSetRange("map", (x + 2048 * y) * 2,val);
            RedisValue i = new byte[] { id };
            db.StringSetRange("mapID", (x + 2048 * y), i);
            SetPixelClient(x, y, id);
        }
    }

    public static void SetPixelClient(int x, int y, byte id)
    {
        int numOfTiles = PlayerPrefs.GetInt("numOfTiles") + 1;
        PlayerPrefs.SetInt("numOfTiles", numOfTiles);
        PlayerPrefs.SetInt(x + " " + y, id);
        mapID[x + 2048 * y] = id;

    }

    public static int ServerTime()
    {
        return (int)((TimeSpan)db.KeyIdleTime("Time")).TotalSeconds;
    }
}

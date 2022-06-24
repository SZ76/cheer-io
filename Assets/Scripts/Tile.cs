using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Newtonsoft.Json;
using TMPro;
using FancyScrollView.Example02;

public class Tile : MonoBehaviour {

    private void Start()
    {
        color = Redis.colors[PlayerPrefs.GetInt("Color")];
    }

    public SpriteRenderer map;
    public GameObject outline;
    public GameObject outline2;
    public GameObject buttonGo;
    public GameObject buttonCancel;
    public GameObject scroll;
    public GameObject hotbar;
    Color color;

    Vector2 camPos;
    public static int stage = 0;
    int[] rounded;

    GameObject newScroll;
    public GameObject canvas;
    public static byte scrollNum = 1;
    public static byte startScrollNum = 1;

    Color waterColor;

    public GameObject startTile;

    private void Update()
    {
        if(stage == 0)
        {
            buttonGo.SetActive(false);
            buttonCancel.SetActive(false);
            outline.SetActive(false);
            outline2.SetActive(false);
            scroll.SetActive(false);
            hotbar.SetActive(false);
        }else if(stage == 1)
        {
            buttonGo.SetActive(false);
            buttonCancel.SetActive(true);
            outline.SetActive(true);
            outline2.SetActive(false);
            hotbar.SetActive(false);
        }else if (stage == 2)
        {
            buttonGo.SetActive(true);
            buttonCancel.SetActive(true);
            outline.SetActive(true);
            outline2.SetActive(true);
            hotbar.SetActive(true);
        }


        if (Input.GetMouseButtonDown(0))
        {
            //SaveTileToServer(new int[] { 447, 623 }, 2, 100, 2);
            camPos = Camera.main.transform.position;
        }

        if (Input.GetMouseButtonUp(0))
        {

            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int[] roundedPos = new int[] { (int)Mathf.Round(pos.x), (int)Mathf.Round(pos.y) };

            if (camPos == (Vector2)Camera.main.transform.position)
            {
                
                if (TileOnLand(roundedPos[0], roundedPos[1]))//checks if tile is on land of current spot
                {
                    if (PlayerPrefs.GetInt("GameStarted") != 0)
                    {
                        if (stage == 0 &&//first click
                            Redis.mapByte[(roundedPos[0] + roundedPos[1] * 2048) * 2 + 1] > 1 &&//checks that tile can be split
                            IsPossesion(roundedPos[0], roundedPos[1]))//checks possesion
                        {
                            rounded = new int[] { (int)Mathf.Round(pos.x), (int)Mathf.Round(pos.y) };
                            outline.transform.position = new Vector2(rounded[0], rounded[1]);
                            stage = 1;
                        }
                        else if ((stage == 1 || stage == 2) &&
                            IsConnected(roundedPos[0], roundedPos[1]))
                        {
                            

                            rounded = new int[] { (int)Mathf.Round(pos.x), (int)Mathf.Round(pos.y) };
                            int x = (int)outline.transform.position.x;//xpos of old tile
                            int y = (int)outline.transform.position.y;//ypos of old tile
                            scrollNum = Redis.mapByte[(x + y * 2048) * 2 + 1];
                            startScrollNum = (byte)(1 + Redis.mapByte[(rounded[0] + rounded[1] * 2048) * 2 + 1]);

                            if (scrollNum > startScrollNum) {
                                Destroy(canvas.GetComponent<Example02>());
                                Destroy(newScroll);
                                StartCoroutine(SecondStage(pos));
                            }
                        }
                    }
                    else if(Redis.mapByte[(roundedPos[0] + roundedPos[1] * 2048) * 2] == 1)
                    {
                        startTile.transform.position = new Vector2(roundedPos[0], roundedPos[1]);
                    }
                }
            }
        }
    }

    IEnumerator SecondStage(Vector2 pos)
    {
        yield return new WaitForSeconds(.05f);
        
        //creates scroll bar to select number
        newScroll = Instantiate(scroll);//creates new scroll
        newScroll.transform.SetParent(canvas.transform);//sets canvas as parent so its visible
        newScroll.transform.localPosition = new Vector2(0, 25);//sets local position to correct position
        newScroll.SetActive(true);//activates it just incase???
        newScroll.transform.localScale = new Vector3(1, 1, 1);
        canvas.AddComponent<Example02>();//adds scroll component to canvas

        outline2.transform.position = new Vector2(rounded[0], rounded[1]);
        stage = 2;
    }

    [PunRPC]
    void sendData(int[] data, Vector3 color, byte a, byte b, byte id)
    {
        int index = data[0] + data[1] * 2048;
        map.sprite.texture.SetPixel(data[0], data[1], new Color(color.x, color.y, color.z));
        Redis.mapByte[index * 2] = a;
        Redis.mapByte[index * 2 + 1] = b;
        Destroy(Redis.mapNum[index]);
        GameObject obj = Instantiate(number, new Vector3(data[0], data[1], 0), Quaternion.Euler(0, 0, 0));
        obj.transform.SetParent(number.transform.parent);
        obj.GetComponent<TextMeshProUGUI>().SetText(b.ToString());
        Redis.mapNum[index] = obj;
        Redis.mapID[index] = id;
        map.sprite.texture.Apply();
    }

    bool TileOnLand(int x, int y)
    {
        waterColor = Redis.colors[0];
        return map.sprite.texture.GetPixel(x, y) != waterColor;
    }

    void SaveTileToServer(int[] data, byte a, byte b, byte id)
    {
        Redis.SetPixel(data[0], data[1], a, b, id);
    }

    public byte reduce = 3;

    public void ClickGoButton()
    {
        Destroy(canvas.GetComponent<Example02>());
        Destroy(newScroll);
        map.sprite.texture.SetPixel(rounded[0], rounded[1], color);
        map.sprite.texture.Apply();
        PhotonView photonView = PhotonView.Get(this);

        byte a = (byte)System.Array.IndexOf(Redis.colors, color);
        byte b = (byte)(reduce - Redis.mapByte[(rounded[0] + rounded[1] * 2048) * 2 + 1]);

        //removes one from old tile
        int x = (int)outline.transform.position.x;//xpos of old tile
        int y = (int)outline.transform.position.y;//ypos of old tile
        byte b1 = Redis.mapByte[(x + y * 2048) * 2];//map color old tile
        byte b2 = (byte)(Redis.mapByte[(x + y * 2048) * 2 + 1] - reduce);//lvl of old tile
        byte id = (byte)(Redis.mapID[x + y * 2048]);//tile id doesnt change ownership so ID stays same
        SaveTileToServer(new int[] { x, y }, b1, b2, id);//sends old tile update to server
        photonView.RPC("sendData", RpcTarget.All,
            new int[] { x, y }, new Vector3(color.r, color.g, color.b), b1, b2, id);//sends old tile to players connected to game

        //sends tile to players currently online
        byte id2 = (byte)(Redis.mapID[rounded[0] + rounded[1] * 2048] + 1);
        photonView.RPC("sendData", RpcTarget.All,
            rounded, new Vector3(color.r, color.g, color.b), a, b, id2);

        //save tile to server
        SaveTileToServer(rounded, a, b, id2);

        stage = 0;//restarts tile selection stage
    }

    public void ClickCancelButton()
    {
        Destroy(canvas.GetComponent<Example02>());
        Destroy(newScroll);
        stage = 0;
    }

    public GameObject number;

    public bool IsConnected(int x, int y)
    {
        int u = PlayerPrefs.GetInt((x) + " " + (y + 1));
        int d = PlayerPrefs.GetInt((x) + " " + (y - 1));
        int l = PlayerPrefs.GetInt((x - 1) + " " + (y));
        int r = PlayerPrefs.GetInt((x + 1) + " " + (y));
        bool up = u == Redis.mapID[x + (y + 1) * 2048];
        bool down = d == Redis.mapID[x + (y - 1) * 2048];
        bool left = l == Redis.mapID[(x-1) + (y) * 2048];
        bool right = r == Redis.mapID[(x+1) + (y) * 2048];
        if (u < 2 && d < 2 && l < 2 && r < 2)
            return false;

        return up || down || left || right;
    }

    public bool IsPossesion(int x, int y)
    {
        print(PlayerPrefs.GetInt(x + " " + y) + " " + Redis.mapID[x + y * 2048]);
        return PlayerPrefs.GetInt(x + " " + y) == Redis.mapID[x + y * 2048];
    }

    public void SetStartTile()
    {
        PhotonView photonView = PhotonView.Get(this);
        int colorInt = PlayerPrefs.GetInt("Color");
        color = Redis.colors[colorInt];

        int x = (int)Mathf.Round(startTile.transform.position.x);
        int y = (int)Mathf.Round(startTile.transform.position.y);
        int[] pos = new int[] { x, y };

        photonView.RPC("sendData", RpcTarget.All,
            pos, new Vector3(color.r, color.g, color.b), (byte)colorInt, (byte)10, (byte)2);

        map.sprite.texture.SetPixel(x, y, color);
        map.sprite.texture.Apply();

        //save tile to server
        SaveTileToServer(pos, (byte)colorInt, (byte)10, (byte)2);
    }


}

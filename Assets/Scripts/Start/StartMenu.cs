using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{

    public static int color;
    public void SetColor(ColorSelect x)
    {
        PlayerPrefs.SetInt("Color", x.colorID);
        color = x.colorID;
    }

    private void Start()
    {
        //PlayerPrefs.DeleteAll();
        if(PlayerPrefs.GetInt("GameStarted", 0) != 0)
        {
            Destroy(this.gameObject);
        }
        else
        {
            back.SetActive(true);
        }
    }

    public GameObject back;
    public GameObject startText;
    public GameObject startTile;
    public GameObject confirmButton;

    public void CloseUI()
    {
        back.SetActive(false);
        startText.SetActive(true);
        startTile.SetActive(true);
        startTile.GetComponent<SpriteRenderer>().color = Redis.colors[color];
        confirmButton.SetActive(true);
    }

    public void EndSetup()
    {
        PlayerPrefs.SetInt("GameStarted", 1);

        Destroy(startTile);
        Destroy(this.gameObject);
    }
}

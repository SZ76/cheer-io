using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSelect : MonoBehaviour
{
    public int colorID;

    private void Update()
    {
        if(StartMenu.color == colorID)
        {
            this.transform.localScale = new Vector3(1.15f, 1.15f, 1f);
        }
        else
        {
            this.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}

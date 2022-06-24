using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outline : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame

    void Update()
    {
        float time = Time.time*1000;
        if (time % 1000 > 750)
        {
            this.transform.rotation = Quaternion.Euler(0, 0, 90);
        }else if (time % 1000 > 500)
        {
            this.transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (time % 1000 > 250)
        {
            this.transform.rotation = Quaternion.Euler(0, 0, 270);
        }
        else if (time % 1000 > 0)
        {
            this.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}

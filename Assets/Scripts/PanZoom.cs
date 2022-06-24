using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanZoom : MonoBehaviour
{
    Vector3 touchStart;
    Vector3 touchStartScreen;
    public float zoomOutMin = 30;
    public float zoomOutMax = 600;

    public GameObject grid;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            touchStartScreen = Input.mousePosition;
        }
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
            Vector2 avg = (touchZero.position + touchOne.position) / 2;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            zoom(difference * 1f);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 direction = touchStart - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if ((Tile.stage > 1 && touchStartScreen.y > Screen.height / 5) || Tile.stage == 0)
            {
                Camera.main.transform.position += direction;
            }
        }
        zoom(Input.GetAxis("Mouse ScrollWheel" ) * 100);

        if(Camera.main.orthographicSize > 50)
        {
            grid.SetActive(false);
        }
        else
        {
            grid.SetActive(true);
        }
    }

    void zoom(float increment)
    {
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increment, zoomOutMin, zoomOutMax);
    }
}

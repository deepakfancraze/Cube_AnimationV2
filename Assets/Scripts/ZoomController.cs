using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomController : MonoBehaviour
{
    [SerializeField] IntroAnimController introAnimController;

    public float zoomOutMin = 20;
    public float zoomOutMax = 35f;


    public float screenMinScaleValue = 1.7f;
    public float screenMaxScaleValue = 3.3f;

    public float mouseZoomSpeed = 10f;
    public float touchZoomSpeed = 0.01f;
    public bool isvideoStoped = true;
    public VideoInfo videoInfo;
    public float percennt;
    public bool IsVideoStoped
    {
        get { return isvideoStoped; }
        set { isvideoStoped = value; }
    }

    void Update()
    {
        if (Input.touchSupported)
        {
            if (Input.touchCount == 2)
            {
                Touch firstTouch = Input.GetTouch(0);
                Touch secondTouch = Input.GetTouch(1);

                Vector2 firstTouchPreviousPos = firstTouch.position - firstTouch.deltaPosition;
                Vector2 secondTouchPreviousPos = secondTouch.position - secondTouch.deltaPosition;

                float prevMagnitude = (firstTouchPreviousPos - secondTouchPreviousPos).magnitude;
                float currentMagnitude = (firstTouch.position - secondTouch.position).magnitude;

                float diff = currentMagnitude - prevMagnitude;
                Zoom(diff * touchZoomSpeed * Time.deltaTime);
            }
        }

        if (introAnimController.canSwipe)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            Zoom(scroll * mouseZoomSpeed * Time.deltaTime);
        }
    

        if (Camera.main.fieldOfView < zoomOutMin)
        {
            Camera.main.fieldOfView = 30f;
        }
        else if (Camera.main.fieldOfView > zoomOutMax)
        {
            Camera.main.fieldOfView = 100f;
        }


    }

    private void Zoom(float increament)
    {
        //Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - increament, zoomOutMin, zoomOutMax);
        Camera.main.fieldOfView -= increament;
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, zoomOutMin, zoomOutMax);
        //percennt = (Camera.main.fieldOfView - zoomOutMin) / ( zoomOutMax- zoomOutMin)*100;
        percennt = Mathf.InverseLerp(zoomOutMin, zoomOutMax, Camera.main.fieldOfView);
        //VideoScreenController.screenScaleValue = Mathf.Lerp(screenMinScaleValue, screenMaxScaleValue, percennt);
    }
}

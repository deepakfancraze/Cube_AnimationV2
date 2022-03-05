using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TouchController : MonoBehaviour
{
    //public bool front, back, left, right, up, down;
    //[SerializeField] private LayerMask targetLayer;
    //[SerializeField] private float rotationRate = 3.0f;
    //[SerializeField] private bool xRotation;
    //[SerializeField] private bool invertX;
    //[SerializeField] private bool invertY;
    //[SerializeField] private bool touchAnywhere;
    //private Camera cam;
    //private float previousX;
    //private float previousY;
    //private bool rotating = false;

    //private void Awake()
    //{
    //    cam = Camera.main;
    //}

    //private void Update()
    //{
    //    if (!touchAnywhere)
    //    {
    //        if (!rotating)
    //        {
    //            RaycastHit hit;
    //            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
    //            if (!Physics.Raycast(ray, out hit, 1000, targetLayer))
    //            {
    //                return;
    //            }
    //        }
    //    }

    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        rotating = true;
    //        previousX = Input.mousePosition.x;
    //        previousY = Input.mousePosition.y;
    //    }

    //    if (Input.GetMouseButton(0))
    //    {
    //        float xAngleValue = -(Input.mousePosition.y - previousY) * rotationRate;
    //        float yAngleValue = -(Input.mousePosition.x - previousX) * rotationRate;
    //        if (!yRotation)
    //            xAngleValue = 0;

    //        if (!xRotation)
    //            yAngleValue = 0;

    //        if (invertX)
    //            yAngleValue *= -1;

    //        if (invertY)
    //            xAngleValue *= -1;
    //        transform.Rotate(xAngleValue, yAngleValue, 0, Space.World);

    //        previousX = Input.mousePosition.x;
    //        previousY = Input.mousePosition.y;
    //    }
    //    if (Input.GetMouseButtonUp(0))
    //        rotating = false;

    //}


    [SerializeField] private bool yRotation;
    [SerializeField] IntroAnimController introAnimController;

    public float mouseSpeedMultiplier = 8;
    public float TouchSpeedMultiplier = 2;
    public float smoothSpeed = 0.04f;
    public float mouseX;
    public float mouseY;



    private float previousX;
    private float previousY;
    private bool rotating = false;

    void OnMouseDrag()
    {

        //mouseX += Input.GetAxis("Mouse X") * mouseSpeedMultiplier;
        //mouseY += Input.GetAxis("Mouse Y") * mouseSpeedMultiplier;
        //if (!yRotation)
        //    mouseY = 0;
    }
    private void Update()
    {
        if (!introAnimController.canSwipe)
        {
            if (Input.touchCount > 1)
                return;
            Debug.Log("Pc__________");
            if (Input.GetMouseButton(0))
            {
                mouseX += Input.GetAxis("Mouse X") * mouseSpeedMultiplier;
                mouseY += Input.GetAxis("Mouse Y") * mouseSpeedMultiplier;
            }

            if (Input.touchCount > 0)
            {
                mouseX += Input.touches[0].deltaPosition.x * Time.deltaTime * TouchSpeedMultiplier;
                mouseY += Input.touches[0].deltaPosition.y * Time.deltaTime * TouchSpeedMultiplier;
            }
            if (!yRotation)
                mouseY = 0;

            Quaternion x = Quaternion.AngleAxis(mouseY, new Vector3(1, 0, 0));
            Quaternion y = Quaternion.AngleAxis(-mouseX, new Vector3(0, 1, 0));
            Quaternion q = x * y;
            transform.rotation = Quaternion.Lerp(transform.rotation, q, smoothSpeed * Time.deltaTime);
        }
    }

}

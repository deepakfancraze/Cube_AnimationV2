using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeMechanic : MonoBehaviour
{
    [SerializeField] Transform cube;
    Vector2 lastKnownPos;
    Vector2 mouseCurrentPos;
    Vector2 cubeLastAngles;
    [SerializeField] [Range(0.01f, 2)] float lerpFactor = 1;
    bool rotateToScreen = false;
    bool rotateRight = true;

    Vector3 lerp;
    Vector3 lerp_target;


    Vector2 mouseEndPos;
    [SerializeField] [Range(0, 1)] float sensitivity;

    void Update()
    {
        if (rotateToScreen)
        {
            Debug.Log("Rotate To Screen");
            RotateToScreen(rotateRight);
            if (cube.eulerAngles.y % 90 < 5)
            {
                rotateToScreen = false;
                lastKnownPos = Input.mousePosition;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                lastKnownPos = Input.mousePosition;
            }
            if (Input.GetMouseButton(0))
            {
                //Debug.Log("Touching ");
                mouseCurrentPos = Input.mousePosition;
                //Debug.Log("Last known :"+lastKnownPos);
                //Debug.Log("current  :"+mouseCurrentPos);

                var cubeRotation = (lastKnownPos.x - mouseCurrentPos.x) * sensitivity;
                //Debug.Log("cube rotation  :" + cubeRotation);

                cube.eulerAngles = new Vector3(cube.eulerAngles.x, cube.eulerAngles.y + cubeRotation, cube.eulerAngles.z);
                //touching
                lastKnownPos = mouseCurrentPos;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("Button Up");
                lerp = new Vector3(0, 0, 0);
                cubeLastAngles = cube.eulerAngles;
                lerp_target = new Vector3(0, cubeLastAngles.y % 90, 0);
                rotateRight = lerp_target.y > 45 ? true : false;
                Debug.Log("Off by angle: " + lerp_target);
                rotateToScreen = true;
            }
        }       
    }


    void RotateToScreen(bool rotateRight)
    {
        if (rotateRight)
        {   //rotate forward -
            Debug.Log("Rotatig forward");
            lerp = Vector3.Lerp(lerp, lerp_target, Time.deltaTime * lerpFactor);
            cube.eulerAngles = cube.eulerAngles + lerp;
        }
        else
        {
            //rotate back +
            Debug.Log("Rotatig backwards");
            lerp = Vector3.Lerp(lerp, lerp_target, Time.deltaTime * lerpFactor);
            cube.eulerAngles = cube.eulerAngles - lerp;
        }
    }
}

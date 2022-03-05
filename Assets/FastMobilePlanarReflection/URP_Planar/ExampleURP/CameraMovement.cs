using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    void Update()
    {
        float tempsin = Mathf.Sin(Time.realtimeSinceStartup / 4);
        transform.position = new Vector3(15*Mathf.Cos(Time.realtimeSinceStartup/4), 2*tempsin+6, 25 * tempsin);
        transform.rotation = Quaternion.Euler(0, -14.5f*Time.realtimeSinceStartup-90, 0);
    }
}   
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void SwipeLeft();
public delegate void SwipeRight();
public class SwipeController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] IntroAnimController introAnimController;
    public static SwipeLeft swipeLeft;
    public static SwipeRight swipeRight;
    private Vector2 fingerDown;
    private Vector2 fingerUp;
    public Vector2 initialPos;

    public bool detectSwipeOnlyAfterRelease = true;
    public bool isYRotation = false;
    public float SWIPE_THRESHOLD = 20f;
    public float speed = 2f;
    private void Update()
    {
        //TouchSwipe();
        if (Input.touchCount > 1)
            return;
        MouseSwipe();
    }

    private void MouseSwipe()
    {
        if (Input.GetMouseButtonDown(0))
        {
            initialPos = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            CheckMouseSwipe(Input.mousePosition);
        }
    }

    void CheckMouseSwipe(Vector3 finalPos)
    {
        float disX = Mathf.Abs(initialPos.x - finalPos.x);
        float disY = Mathf.Abs(initialPos.y - finalPos.y);

        if (disX > 0 || disY > 0)
        {
            if (disX > disY)
            {
                if (initialPos.x > finalPos.x)
                {
                    OnSwipeLeft();
                }
                else
                {
                    OnSwipeRight();
                }
            }
            else
            {
                if (isYRotation)
                {
                    if (initialPos.y > finalPos.y)
                    {
                        OnSwipeDown();
                    }
                    else
                    {
                        OnSwipeUp();
                    }
                }

            }
        }
        else
        {
            Debug.Log("Not swipe ___________");
        }
    }

    //private void TouchSwipe()
    //{
    //    foreach (Touch touch in Input.touches)
    //    {
    //        if (touch.phase == TouchPhase.Began)
    //        {
    //            fingerUp = touch.position;
    //            fingerDown = touch.position;
    //        }

    //        if (touch.phase == TouchPhase.Moved)
    //        {
    //            if (!detectSwipeOnlyAfterRelease)
    //            {
    //                fingerDown = touch.position;
    //                CheckTouchSwipe();
    //            }
    //        }

    //        if (touch.phase == TouchPhase.Ended)
    //        {
    //            fingerDown = touch.position;
    //            CheckTouchSwipe();
    //        }
    //    }
    //}

    //void CheckTouchSwipe()
    //{
    //    if (VerticalMove() > SWIPE_THRESHOLD && VerticalMove() > HorizontalValMove())
    //    {
    //        if (fingerDown.y - fingerUp.y > 0)
    //        {
    //            OnSwipeUp();
    //        }
    //        else if (fingerDown.y - fingerUp.y < 0)
    //        {
    //            OnSwipeDown();
    //        }
    //        fingerUp = fingerDown;
    //    }

    //    //Check if Horizontal swipe
    //    else if (HorizontalValMove() > SWIPE_THRESHOLD && HorizontalValMove() > VerticalMove())
    //    {
    //        //Debug.Log("Horizontal");
    //        if (fingerDown.x - fingerUp.x > 0)
    //        {
    //            OnSwipeRight();
    //        }
    //        else if (fingerDown.x - fingerUp.x < 0)
    //        {
    //            OnSwipeLeft();
    //        }
    //        fingerUp = fingerDown;
    //    }

    //    else
    //    {
    //        //Debug.Log("No Swipe!");
    //    }
    //}
    //float VerticalMove()
    //{
    //    return Mathf.Abs(fingerDown.y - fingerUp.y);
    //}

    //float HorizontalValMove()
    //{
    //    return Mathf.Abs(fingerDown.x - fingerUp.x);
    //}

    void OnSwipeUp()
    {

        if (introAnimController.canSwipe)
        {
            Debug.Log("Swipe UP");
        }


    }

    void OnSwipeDown()
    {

        if (introAnimController.canSwipe)
        {
            Debug.Log("Swipe Down");
        }
    }

    void OnSwipeLeft()
    {

        Debug.Log("Swipe Left");
        if (introAnimController.canSwipe)
        {
            //NextScreen();
            if (swipeLeft != null)
                swipeLeft();

        }
    }

    void OnSwipeRight()
    {

        Debug.Log("Swipe Right" + introAnimController.canSwipe);
        if (introAnimController.canSwipe)
        {
            //PreviousScreen();
            if (swipeRight != null)
                swipeRight();
        }
    }
}

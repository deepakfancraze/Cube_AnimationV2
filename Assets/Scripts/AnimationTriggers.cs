using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTriggers : MonoBehaviour
{
    [SerializeField] MomentSequenceController sequenceController;
    [SerializeField] Animator cubeAnimator;
    [SerializeField] Animator legendryBallAnimator;
    [SerializeField] Animator platinumBallAnimator;
    [SerializeField] Animator genisisBallAnimator;
    [SerializeField] Animator epicBallAnimator;
    [SerializeField] Animator rareBallAnimator;
    [SerializeField] Animator commonBallAnimator;

    [SerializeField] RuntimeAnimatorController swipeController;
    [SerializeField] RuntimeAnimatorController cubeRotateController;
    [SerializeField] VideoRecorder videoRecorder;
    [SerializeField] IntroAnimController introAnimController;
    public static Action animationsHaveEnded;
    public static VideoPlay videoPlay;
    public static VideoPause videoPause;

    private Animator currentRareBallAnimator;
    public void AnimationEnd()
    {
        Debug.Log("Animation Ended");
        sequenceController.MomentEnded();

    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            NextScreen();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            PreviousScreen();
        }
    }

    private void OnEnable()
    {
        CubeDataController.setDefaultAnimationBeforeStart += SetDefaultBallAnimation;
        EventManager.Subscribe(EventManager.EventType.StateDecided, OnStateDecided);
        SwipeController.swipeLeft += OnSwipeLeft;
        SwipeController.swipeRight += OnSwipeRight;
        IntroAnimController.ballRotate += StartBallRotatingAnimation;
        IntroAnimController.cubeRotate += RotateCube;
        CubeDataController.onEverythingLoaded += EverythingLoaded;
    }
    private void OnDisable()
    {
        CubeDataController.setDefaultAnimationBeforeStart -= SetDefaultBallAnimation;
        EventManager.Unsubscribe(EventManager.EventType.StateDecided, OnStateDecided);
        SwipeController.swipeLeft -= OnSwipeLeft;
        SwipeController.swipeRight -= OnSwipeRight;
        IntroAnimController.ballRotate -= StartBallRotatingAnimation;
        IntroAnimController.cubeRotate -= RotateCube;
        CubeDataController.onEverythingLoaded -= EverythingLoaded;

    }


    void OnStateDecided(object data)
    {
        State state = (State)data;
        //if (state == State.Interaction)
        //    this.GetComponent<Animator>().runtimeAnimatorController = swipeController;
        //else
        //    this.GetComponent<Animator>().runtimeAnimatorController = cubeRotateController;

    }
    private void RotateCube()
    {
        if (StateController.GetState() == State.Recording || StateController.GetState() == State.Video)
        {
            if (VideoRecorder.instance.recordingState == RecordingState.VideoRecording)
                this.transform.parent.GetComponent<Animator>().SetTrigger("startRotating");
            else
                this.GetComponent<Animator>().SetTrigger("screenShots");
        }
    }



    private void EverythingLoaded()
    {
        StartIntroAnimation();
    }

    public void NextScreen()
    {
        cubeAnimator.SetTrigger("nextScreen");
    }

    public void PreviousScreen()
    {
        cubeAnimator.SetTrigger("previousScreen");
    }

    public void StartBallRotatingAnimation(string rarity)
    {
        switch (rarity)
        {
            case "EPIC":
                epicBallAnimator.SetTrigger("startRotate");
                currentRareBallAnimator = epicBallAnimator;
                break;
            case "GENESIS":
                genisisBallAnimator.SetTrigger("startRotate");
                currentRareBallAnimator = genisisBallAnimator;

                break;
            case "LEGENDARY":
                legendryBallAnimator.SetTrigger("startRotate");
                currentRareBallAnimator = legendryBallAnimator;

                break;
            case "PLATINUM":
                platinumBallAnimator.SetTrigger("startRotate");
                currentRareBallAnimator = platinumBallAnimator;

                break;
            case "RARE":
                rareBallAnimator.SetTrigger("startRotate");
                currentRareBallAnimator = rareBallAnimator;

                break;
            case "COMMON":
                commonBallAnimator.SetTrigger("startRotate");
                currentRareBallAnimator = commonBallAnimator;

                break;

        }
    }

    public void SetDefaultBallAnimation()
    {
        if (currentRareBallAnimator != null)
        {
            currentRareBallAnimator.SetTrigger("setDefault");
        }
    }


    public void StartIntroAnimation()
    {
        //cubeAnimator.SetTrigger("startIntro");
        //EventManager.Dispatch(EventManager.EventType.IntroAnimStarted, null);
    }

    //public void IntroAnimFinished()
    //{
    //    VideoScreenController.canSwipe = true;
    //}

    public void CubeRotationFinished()
    {
        if (animationsHaveEnded != null)
        {
            animationsHaveEnded.Invoke();
        }
        OBSRecorder.Instance.KillRecording();
        ScreenshotFromBottomAnimEvent();
    }

    public void EndingAuioFadeOut()
    {
        if (CubeDataController.IsCommentryOn)
        {
            IntroAnimController.audioPause?.Invoke(true);
        }
    }
    void OnSwipeUp()
    {
    }

    void OnSwipeDown()
    {

    }

    void OnSwipeLeft()
    {
        NextScreen();
    }

    void OnSwipeRight()
    {
        PreviousScreen();
    }

    public void PlayVideoInteraction()
    {
        if (videoPlay != null)
        {
            videoPlay();
        }
    }

    public void PauseVideo()
    {
        Debug.LogWarning("video pause");
        if (videoPause != null)
        {
            videoPause();
        }
    }

    void ScreenshotFromSideAnimEvent()
    {
        EventManager.Dispatch(EventManager.EventType.CanMakeScreenshot, null);
    }

    void ScreenshotFromBottomAnimEvent()
    {
        EventManager.Dispatch(EventManager.EventType.MakeScreenshotWithBottomCamera, null);
    }

}
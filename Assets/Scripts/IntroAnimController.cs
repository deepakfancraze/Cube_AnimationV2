using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void BallRotate(string ballCategory);
public delegate void CubeRotate();
public delegate void PlayVideoInAnimation();
public delegate void VideoPlay();
public delegate void VideoStop();
public delegate void VideoPause();
public class IntroAnimController : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] RuntimeAnimatorController legendryAnimController;
    [SerializeField] RuntimeAnimatorController platinumAnimController;
    [SerializeField] RuntimeAnimatorController genisisAnimController;
    [SerializeField] RuntimeAnimatorController epicAnimController;
    [SerializeField] RuntimeAnimatorController rareAnimController;
    [SerializeField] RuntimeAnimatorController commonAnimController;
    [SerializeField] VideoScreenController videoScreenController;
    [SerializeField] GameObject videoThumbnail;

    [SerializeReference]
    Animator videoLogoAnimator;

    public bool isCubeRotaionForTesting;
    public static BallRotate ballRotate;
    public static VideoPlay videoPlay;
    public static VideoStop videoStop;
    public static CubeRotate cubeRotate;

    public static Action backGroundVideoPlay;
    public static Action<bool> audioPause;

    internal Animator currentAnimator;

    public static IntroAnimController instance;
    string catagory;

    public static bool audioIsPlaying;
    public bool canSwipe;
    bool isIntroAnimFinished = false;
    private void Awake()
    {
        instance = this;
        currentAnimator = this.GetComponent<Animator>();
        //instance.currentAnimator.SetTrigger("startIntro");
    }

    private void OnEnable()
    {
        CubeDataController.setDefaultAnimationBeforeStart += SetDefaultIntroAnimation;
        CubeDataController.onEverythingLoaded += OnEverythingLoaded;
        EventManager.Subscribe(EventManager.EventType.StateDecided, OnStateDecided);
        VideoInfo.scaleDownVideo += ScaleDwonOnVideoFinished;
        VideoInfo.manageTumbnailOnVideo += HandleThumbnail;

        OBSRecorder.obsApplicationHasStarted += ObsApplicationStarted;

    }

    private void OnDisable()
    {
        CubeDataController.setDefaultAnimationBeforeStart -= SetDefaultIntroAnimation;
        CubeDataController.onEverythingLoaded -= OnEverythingLoaded;
        EventManager.Unsubscribe(EventManager.EventType.StateDecided, OnStateDecided);
        VideoInfo.scaleDownVideo -= ScaleDwonOnVideoFinished;
        VideoInfo.manageTumbnailOnVideo -= HandleThumbnail;
        OBSRecorder.obsApplicationHasStarted -= ObsApplicationStarted;


    }

    void OnStateDecided(object data)
    {
        State state = (State)data;
        isCubeRotaionForTesting = state == State.Video || state == State.Recording;
    }

    public static void SetIntroAnimController(string rarity)
    {
        Debug.Log("Rarity _____________" + rarity);
        instance.catagory = rarity;
        switch (rarity)
        {
            case "EPIC":
                instance.currentAnimator.runtimeAnimatorController = instance.epicAnimController;
                break;
            case "GENESIS":
                instance.currentAnimator.runtimeAnimatorController = instance.genisisAnimController;
                break;
            case "LEGENDARY":
                instance.currentAnimator.runtimeAnimatorController = instance.legendryAnimController;

                break;
            case "PLATINUM":
                instance.currentAnimator.runtimeAnimatorController = instance.platinumAnimController;
                break;
            case "RARE":
                instance.currentAnimator.runtimeAnimatorController = instance.rareAnimController;
                break;
            case "COMMON":
                instance.currentAnimator.runtimeAnimatorController = instance.commonAnimController;
                break;
        }
    }

    void OnEverythingLoaded()
    {
        if (StateController.GetState() == State.Recording)
        {
            if (VideoRecorder.instance.recordingState == RecordingState.VideoRecording)
            {
                OBSRecorder.Instance.StartRecording();
            }
            else
            {
                instance.currentAnimator.SetTrigger("startIntro");
            }
        }
        else
        {
            instance.currentAnimator.SetTrigger("startIntro");
        }

    }

    public void ObsApplicationStarted()
    {
        //StartCoroutine(ShowIConAtStarting());
        instance.currentAnimator.SetTrigger("startGlitch");
    }

    IEnumerator ShowIConAtStarting()
    {
        yield return new WaitForSeconds(1);
    }

    public void StartIntroAnimation()
    {
        instance.currentAnimator.SetTrigger("startIntro");
    }
    public void IntroAnimationStarted()
    {
        EventManager.Dispatch(EventManager.EventType.IntroAnimStarted, null);
    }
    public void GlitchStarted()
    {
        EventManager.Dispatch(EventManager.EventType.AudioPlay, null);

    }

    void SetDefaultIntroAnimation()
    {
        instance.currentAnimator.SetTrigger("setDefault");
    }
    public void BallRotating()
    {
        if (VideoRecorder.instance.recordingState == RecordingState.VideoRecording)
        {
            if (ballRotate != null)
            {
                ballRotate(catagory);
            }
        }

    }

    void IntroAnimationFinished()
    {

    }

    public void CubeIntroAnim()
    {
        if (StateController.GetState() == State.Recording || StateController.GetState() == State.Video)
        {
            instance.currentAnimator.SetTrigger("NonInteractive");
        }
        else
        {
            instance.currentAnimator.SetTrigger("Interactive");
        }
    }


    public void NonIntaractiveAnimationFinishing()
    {
        if (CubeDataController.IsCommentryOn)
            audioPause?.Invoke(true);
    }
    public void NonIntaractiveAnimationFinished()
    {
        isIntroAnimFinished = true;
        //if (CubeDataController.IsCommentryOn)
        //    audioPause?.Invoke(true);

        if (VideoRecorder.instance.recordingState != RecordingState.ScreenshotRecording)
        {
            if (videoPlay != null)
                videoPlay();
        }
        else
        {
            StartCoroutine(SkipVideoPLayerOnScreenShots());
        }

    }

    public void StartInteraction()
    {
        canSwipe = true;
        if (videoPlay != null)
        {
            videoPlay();
        }
    }



    private void HandleThumbnail(bool isOn)
    {
        videoThumbnail.gameObject.SetActive(isOn);
        if (!isOn)
        {
            CubeDataController.audioCanBeEnabled?.Invoke();
            audioIsPlaying = true;
        }
        else
        {
            CubeDataController.audioCanBeDisabled?.Invoke();
            audioIsPlaying = false;
        }
    }

    void AudioCanBeEnabledAnimEvent()
    {
        CubeDataController.audioCanBeEnabled?.Invoke();
        audioIsPlaying = true;
        videoLogoAnimator.SetTrigger("fadeIn");
    }

    private void ScaleDwonOnVideoFinished()
    {
        videoLogoAnimator.SetTrigger("fadeOut");
        instance.currentAnimator.SetTrigger("StopVideo");
        StartCoroutine(WaitForScaleDown());

    }

    private IEnumerator WaitForScaleDown()
    {
        if (CubeDataController.IsCommentryOn)
        {
            audioPause?.Invoke(false);
        }
        yield return new WaitUntil(() => isIntroAnimFinished);

        yield return new WaitForSeconds(1f);
        if (cubeRotate != null)
            cubeRotate();
       

    }

    void PlayVideoRecording()
    {
        if (StateController.GetState() != State.Interaction)
        {
            if (VideoRecorder.instance.recordingState != RecordingState.ScreenshotRecording)
            {
                //if (videoPlay != null)
                //{
                //    videoPlay();
                //}
                backGroundVideoPlay?.Invoke();
            }
            // else
            // {
            //     StartCoroutine(SkipVideoPLayerOnScreenShots());
            // }
        }

    }



    private IEnumerator SkipVideoPLayerOnScreenShots()
    {
        yield return new WaitUntil(() => isIntroAnimFinished);
        yield return new WaitForSeconds(1);

        isIntroAnimFinished = false;
        if (cubeRotate != null)
            cubeRotate();
    }

}

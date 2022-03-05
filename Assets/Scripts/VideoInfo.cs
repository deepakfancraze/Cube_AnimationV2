using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public delegate void ScaleDownVideo();
public delegate void ManageTumbnailOnVideoEnded(bool isOn);
public class VideoInfo : MonoBehaviour
{
    public static ScaleDownVideo scaleDownVideo;
    public static ManageTumbnailOnVideoEnded manageTumbnailOnVideo;

    float videoLength;
    float startTime;
    [SerializeField]
    Animator cubeAnimator;
    [SerializeField]
    private string videoFileName;
    VideoPlayer videoplayer = null;
    State state;

    int scalingDownAnimFrameCount=13;
    int initVideoScalingDownFrame;
    private void Awake()
    {
        videoplayer = GetComponent<VideoPlayer>();
    }


    void Start()
    {

        videoplayer.loopPointReached += CheckOver;
        //videoLength = (float)videoplayer.clip.length;
        //Debug.Log("Video Length: " + videoLength);
    }
    private void OnEnable()
    {
        //VideoScreenController.videoPlay += StartVideo;
        //VideoScreenController.videoStop += StopVideo;
        IntroAnimController.backGroundVideoPlay += BackGroundVideoPLay;
        AnimationTriggers.videoPlay += ResumeVideo;
        IntroAnimController.videoPlay += StartVideo;
        AnimationTriggers.videoPause += PauseVideo;
        EventManager.Subscribe(EventManager.EventType.StateDecided, OnStateDecided);
    }

    private void OnDisable()
    {
        //VideoScreenController.videoPlay -= StartVideo;
        //VideoScreenController.videoStop -= StopVideo;
        IntroAnimController.backGroundVideoPlay -= BackGroundVideoPLay;

        AnimationTriggers.videoPlay -= ResumeVideo;
        IntroAnimController.videoPlay -= StartVideo;
        AnimationTriggers.videoPause -= PauseVideo;
        EventManager.Unsubscribe(EventManager.EventType.StateDecided, OnStateDecided);

    }

    void OnStateDecided(object data)
    {
        state = (State)data;
    }

    [ContextMenu("Play")]
    private void ResumeVideo()
    {
        videoplayer.Play();
    }

    private void BackGroundVideoPLay()
    {
        videoplayer.SetDirectAudioMute(0, true);
        StartCoroutine(HoldBeforeVideoPLay());
    }
    private void StartVideo()
    {
        Debug.LogError("Playing_______________");
        if (state == State.Interaction)
        {
            videoplayer.Play();
        }
        else
        {
            if (CubeDataController.instance.isCommentryOn)
                videoplayer.SetDirectAudioMute(0, false);
            StartCoroutine(HoldBeforeVideoPLay());
        }
        StartCoroutine(WaitForVideoFirstFrame());
    }

    IEnumerator HoldBeforeVideoPLay()
    {
        yield return new WaitForEndOfFrame();
        videoplayer.Prepare();
        while (videoplayer.isPrepared == false)
        { yield return null; }
        videoplayer.Play();
    }

    IEnumerator WaitForVideoFirstFrame()
    {
        yield return new WaitUntil(() => videoplayer.isPlaying);

        if (state == State.Interaction)
        {
            videoplayer.SetDirectAudioMute(0, true);
            videoplayer.isLooping = true;
            if (manageTumbnailOnVideo != null)
                manageTumbnailOnVideo(false);
        }
        else
        {
            //videoplayer.frame = 0;
            Debug.LogError("........................................" + videoplayer.frameCount);
            initVideoScalingDownFrame = (int)videoplayer.frameCount - scalingDownAnimFrameCount;
            isRecodrFrameRate = true;
            IntroAnimController.instance.currentAnimator.SetTrigger("PlayVideo");
        }
    }
    bool isRecodrFrameRate = false;
    private void Update()
    {
        if (!isRecodrFrameRate)
            return;
        if (isRecodrFrameRate && initVideoScalingDownFrame==videoplayer.frame)
        {
            Debug.Log("video over_____________");
            isRecodrFrameRate = false;
            VideoEnded();
        }
    }

    private void PauseVideo()
    {
        videoplayer.Pause();
    }
    private void StopVideo()
    {
        videoplayer.Stop();
        Debug.Log("stop, 1");
        videoplayer.frame = 0;
    }

    void CheckOver(UnityEngine.Video.VideoPlayer vp)
    {
        //Debug.Log("video over_____________");
        //isRecodrFrameRate = false;
        //VideoEnded();
    }

    void VideoEnded()
    {
        Debug.Log("triger video end animation");
        if (cubeAnimator != null)
            cubeAnimator.SetTrigger("videoEnded"); // try setting bool and using update

        if (state != State.Interaction)
        {
            //if (manageTumbnailOnVideo != null)
            //{
            //    manageTumbnailOnVideo(true);
            //}
            if (scaleDownVideo != null)
                scaleDownVideo();
        }
    }


}

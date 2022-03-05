using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoScreenController : MonoBehaviour
{
    // Start is called before the first frame update
    //[SerializeField]
    //public  bool canSwipe;
    //public static VideoPlay videoPlay;
    //public static VideoStop videoStop;
    //public static float screenScaleValue;

    //public GameObject closeButton, playButton;
    //public Vector3 videoScreenCurrentScale;
    //public Vector3 videoScreenCurrentPosition;
    //public Quaternion videoScreenCurrentRotation;
    //public Vector3 videoScreenNewScale;
    //public Vector3 videoScreenNewPosition = new Vector3(0, 0, -0.083f);
    //public float speed = 2f;
    //public float duration = 0.5f;
    //private bool isVideoBigScreenOn = false;


    //public Transform targetTransform;
    //public Transform parent;
    //public Transform mainParent;
    //void Start()
    //{
    //    videoScreenCurrentPosition = this.transform.localPosition;
    //    videoScreenCurrentScale = this.transform.localScale;
    //    videoScreenCurrentRotation = this.transform.rotation;
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    //private void OnEnable()
    //{
    //}

    //private void OnDisable()
    //{
    //}

    //private void ScaleDwonOnVideoFinished()
    //{
    //    if (isVideoBigScreenOn)
    //        HideBigScreen();

    //}

    //[ContextMenu("Show big Screen")]
    //public void ShowVideoOnBigScreen()
    //{
    //    videoScreenNewScale = new Vector3(screenScaleValue, screenScaleValue, 0.03f);
    //    canSwipe = false;
    //    transform.localPosition = videoScreenNewPosition;
    //    playButton.SetActive(false);
    //    this.transform.SetParent(parent);
    //    StartCoroutine(LerpVideoScreen(videoScreenCurrentScale, videoScreenNewScale, duration, true));

    //}
    //[ContextMenu("Back To Normal Screen")]
    //public void HideBigScreen()
    //{
    //    this.transform.SetParent(mainParent);
    //    this.transform.SetSiblingIndex(1);
    //    StartCoroutine(LerpVideoScreen(videoScreenNewScale, videoScreenCurrentScale, duration, false));

    //}
    //IEnumerator LerpVideoScreen(Vector3 fromPosition, Vector3 toPosition, float time, bool isMAximize)
    //{
    //    float i = 0.0f;
    //    float rate = (1.0f / time) * speed;
    //    while (i < 1.0f)
    //    {
    //        i += Time.deltaTime * rate;
    //        transform.localScale = Vector3.Lerp(fromPosition, toPosition, i);
    //        if (isMAximize)
    //        {
    //            transform.localPosition = Vector3.Lerp(transform.position, targetTransform.position, i);
    //            transform.localRotation = Quaternion.Lerp(transform.rotation, targetTransform.rotation, i);
    //        }
    //        else
    //        {
    //            transform.localPosition = Vector3.Lerp(transform.localPosition, videoScreenCurrentPosition, i);
    //            transform.localRotation = Quaternion.Lerp(transform.localRotation, videoScreenCurrentRotation, i);
    //        }

    //        yield return null;
    //    }

    //    AfterCompleteScreenAnimation(isMAximize);
    //}

    //private void AfterCompleteScreenAnimation(bool isMaximize)
    //{
    //    if (!isMaximize)
    //        ScreenMinimize();
    //    else
    //        ScreenMaximize();
    //}

    //private void ScreenMaximize()
    //{
    //    //videoplayer.Play();   
    //    if (videoPlay!=null)
    //        videoPlay();
    //    isVideoBigScreenOn = true;
    //    closeButton.SetActive(true);
    //}

    //private void ScreenMinimize()
    //{
    //    //videoplayer.Stop();
    //    if (videoStop != null)
    //        videoStop();
    //    transform.localPosition = videoScreenCurrentPosition;
    //    closeButton.SetActive(false);
    //    playButton.SetActive(true);
    //    isVideoBigScreenOn = false;
    //    canSwipe = true;

       
    //}
}

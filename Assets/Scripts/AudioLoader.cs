using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AudioLoader : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField]
    AudioClip commonRare1;
    [SerializeField]
    AudioClip commonRare2;

    [SerializeField]
    AudioClip otherRarity1;
    [SerializeField]
    AudioClip otherRarity2;

    [SerializeField]
    AudioClip legendary1;
    [SerializeField]
    AudioClip legendary2;
    AudioClip myClip;
    static AudioLoader instance;
    static bool keepFadingOut;
    static bool keepFadingIn;
    CubeRarityType rarityType;
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        Debug.Log("Starting to download the audio...");
    }


    public static void GetAudio(Action ac, string isMute, string url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            instance.StartCoroutine(instance.GetAudioClip(url, ac));
        }
        //if (isMute == "FALSE")
        //{
        //}
        //else
        //{
        //    instance.audioSource.clip = null;
        //}
    }
    private void OnEnable()
    {
        CubeDataController.onEverythingLoaded += OnEverythingLoaded;
        IntroAnimController.audioPause += AudioPlayNPause;
        //EventManager.Subscribe(EventManager.EventType.IntroAnimStarted, PlayAudio);
        EventManager.Subscribe(EventManager.EventType.AudioPlay, PlayAudio);
        EventManager.Subscribe(EventManager.EventType.AudioFadeOut, PlayAudio);
    }


    private void AudioPlayNPause(bool isPuase)
    {
        //if (isPuase)
        //    audioSource.Pause();
        //else
        //    audioSource.Play();

        if (isPuase)
            AudioFadingOut();
        else
            SetSecondAudio();

    }



    private void PlayAudio(object data)
    {
        if (audioSource.clip != null && VideoRecorder.instance.recordingState == RecordingState.VideoRecording)
        {
            Debug.LogError("Playing ____________");
            instance.audioSource.volume = 1;

            audioSource.Play();
        }
    }

    private void AudioFadingOut()
    {
        if (audioSource.clip != null && VideoRecorder.instance.recordingState == RecordingState.VideoRecording)
        {
            Debug.LogError("Fade out _____________________");
            instance.StartCoroutine(FadeOut(instance.audioSource.clip, 0.05f));
        }
    }

    private void AudioFadingIn()
    {
        if (audioSource.clip != null && VideoRecorder.instance.recordingState == RecordingState.VideoRecording)
        {
            Debug.LogError("Fade out _____________________");
            instance.StartCoroutine(FadeIn(instance.audioSource.clip, 0.05f));
        }
    }

    private void OnDisable()
    {
        CubeDataController.onEverythingLoaded -= OnEverythingLoaded;
    }

    void OnEverythingLoaded()
    {
        //audioSource.Play();
    }

    IEnumerator GetAudioClip(string url, Action ac)
    {
        //Debug.LogError("Audio Url _________"+url);
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.Log(www.error);
            }
            else
            {
                myClip = DownloadHandlerAudioClip.GetContent(www);
                audioSource.clip = myClip;
                ac?.Invoke();
                Debug.Log("Audio is playing.");
            }
        }
    }


    public static void SetFirstAudio(CubeRarityType cubeRarityType)
    {
        instance.rarityType = cubeRarityType;
        switch (cubeRarityType)
        {
            case CubeRarityType.legendary:
                SetAudiClip(instance.legendary1);
                break;
            case CubeRarityType.epic:
            case CubeRarityType.genesis:
            case CubeRarityType.platinum:
                SetAudiClip(instance.otherRarity1);
                break;
            case CubeRarityType.rare:
            case CubeRarityType.common:
                SetAudiClip(instance.commonRare1);
                break;

        }
    }

    public static void SetSecondAudio()
    {
        switch (instance.rarityType)
        {
            case CubeRarityType.legendary:
                SetAudiClip(instance.legendary2);
                break;
            case CubeRarityType.epic:
            case CubeRarityType.genesis:
            case CubeRarityType.platinum:
                SetAudiClip(instance.otherRarity2);
                break;
            case CubeRarityType.rare:
            case CubeRarityType.common:
                SetAudiClip(instance.commonRare2);
                break;

        }
        instance.audioSource.volume = 0;
        instance.PlayAudio(null);
        instance.AudioFadingIn();
    }
    private static void SetAudiClip(AudioClip clip)
    {
        instance.audioSource.clip = clip;
    }

    static IEnumerator FadeOut(AudioClip clip, float speed)
    {
        keepFadingOut = true;
        keepFadingIn = false;
        float audioVolume = instance.audioSource.volume;
        while (instance.audioSource.volume > 0 && keepFadingOut)
        {
            audioVolume -= speed;
            instance.audioSource.volume = audioVolume;
            yield return new WaitForSeconds(0.1f);
        }

    }

    static IEnumerator FadeIn(AudioClip clip, float speed)
    {
        keepFadingIn = true;
        keepFadingOut = false;


        float audioVolume = instance.audioSource.volume;
        while (instance.audioSource.volume < 1 && keepFadingIn)
        {
            audioVolume += speed;
            instance.audioSource.volume = audioVolume;
            yield return new WaitForSeconds(0.1f);
        }

    }


    // public void pauseAudio(){
    //     audioSource.Pause();
    // }

    // public void playAudio(){
    //     audioSource.Play();
    // }

    // public void stopAudio(){
    //     audioSource.Stop();

    // }
}
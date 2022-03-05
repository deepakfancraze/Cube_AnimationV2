using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
#endif
using UnityEngine;

public class VideoRecorder : MonoBehaviour
{
    [SerializeField] internal bool shouldRecord;
    [SerializeField] internal RecordingState recordingState = RecordingState.VideoRecording;

    [SerializeField] int videoWidth;
    [SerializeField] int videoHeight;
    [SerializeField] int screenshotWidth;
    [SerializeField] int screenshotHeight;



    int videoIndex = 0;

    public static Action recordingStopped;

    public static VideoRecorder instance;

    public static bool IsRecording => instance.shouldRecord;

    public static int RecordingWidth => instance.GetWidth();

    public static int RecordingHeight => instance.GetHeight();

    public static string fileName;
    private void Start()
    {
        instance = this;
    }

    private void OnEnable()
    {
        CubeDataController.onEverythingLoaded += OnEverythingLoaded;
        EventManager.Subscribe(EventManager.EventType.IntroAnimStarted, OnIntroAnimStarted);
        EventManager.Subscribe(EventManager.EventType.StateDecided, OnStateDecided);
        EventManager.Subscribe(EventManager.EventType.MakeScreenshotWithBottomCamera, OnMakeScreenshotWithBottomCamera);
        EventManager.Subscribe(EventManager.EventType.BottomScreenshotWasMade, OnBottomScreenshotWasMade);
        EventManager.Subscribe(EventManager.EventType.RecordingStateChanged, OnRecordingStateChanged);
    }

    private void OnDisable()
    {
        CubeDataController.onEverythingLoaded -= OnEverythingLoaded;
        EventManager.Unsubscribe(EventManager.EventType.IntroAnimStarted, OnIntroAnimStarted);
        EventManager.Unsubscribe(EventManager.EventType.StateDecided, OnStateDecided);
        EventManager.Unsubscribe(EventManager.EventType.MakeScreenshotWithBottomCamera, OnMakeScreenshotWithBottomCamera);
        EventManager.Unsubscribe(EventManager.EventType.BottomScreenshotWasMade, OnBottomScreenshotWasMade);
        EventManager.Unsubscribe(EventManager.EventType.RecordingStateChanged, OnRecordingStateChanged);
        recordingStopped = null;
        if (shouldRecord)
            StopRecording();
    }

    void OnIntroAnimStarted(object data)
    {
        StartCoroutine(WaitForEndOfFrame());
    }

    IEnumerator WaitForEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        if (shouldRecord)
            StartRecording();
    }

    void OnRecordingStateChanged(object data)
    {
        recordingState = RecordingState.ScreenshotRecording;
        Time.timeScale = 3;
    }

    void OnStateDecided(object data)
    {
        State state = (State)data;

        shouldRecord = state == State.Recording;
    }

    void OnEverythingLoaded()
    {
    }

    void OnMakeScreenshotWithBottomCamera(object data)
    {
        if (shouldRecord)
            StopRecording();
    }

    void OnBottomScreenshotWasMade(object data)
    {
        recordingStopped?.Invoke();
    }

    void OnAnimationsHaveEnded()
    {
        if (shouldRecord)
            StopRecording();
    }

    // Start is called before the first frame update
    void StartRecording()
    {
#if UNITY_EDITOR
        RecorderWindow recorderWindow = (RecorderWindow)EditorWindow.GetWindow(typeof(RecorderWindow));
        if (!recorderWindow.IsRecording() && VideoRecorder.instance.recordingState == RecordingState.ScreenshotRecording)
        {
            var videoRecorder = ScriptableObject.CreateInstance<MovieRecorderSettings>();
            var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();

            CheckingCreatingFolders();
            //videoRecorder.OutputFile = Application.dataPath+ "\\Recordings\\MomentVideo" + videoIndex.ToString();
            // videoRecorder.OutputFile = "C:\\UNITY_TEMP\\Videos\\Cube" + CubeDataController.VisualisedIndex.ToString() + "\\MomentVideo" + videoIndex.ToString();
            if (recordingState == RecordingState.VideoRecording)
                videoRecorder.OutputFile = "C:\\UNITY_TEMP\\Videos\\" + CubeDataController.FileName + "\\" + CubeDataController.FileName + "-0";
            else
                videoRecorder.OutputFile = "C:\\UNITY_TEMP\\DummyVideos\\" + CubeDataController.FileName + "\\" + CubeDataController.FileName + "-0";
            //Settings settings = new Settings();
            //videoRecorder.ImageInputSettings = settings;
            videoRecorder.ImageInputSettings = new GameViewInputSettings
            {
                OutputWidth = GetWidth(),
                OutputHeight = GetHeight(),
            };
            controllerSettings.FrameRate = 24f;
            controllerSettings.CapFrameRate = true;
            controllerSettings.AddRecorderSettings(videoRecorder);
            recorderWindow.SetRecorderControllerSettings(controllerSettings);
            recorderWindow.StartRecording();
        }

        videoIndex += 1;
#endif
    }

    int GetWidth()
    {
        if (recordingState == RecordingState.VideoRecording)
        {
            return videoWidth;
        }
        else return screenshotWidth;
    }

    int GetHeight()
    {
        if (recordingState == RecordingState.VideoRecording)
        {
            return videoHeight;
        }
        else return screenshotHeight;
    }

    void CheckingCreatingFolders()
    {
        if (!Directory.Exists("C:/UNITY_TEMP"))
        {
            Directory.CreateDirectory("C:/UNITY_TEMP");
        }

        if (!Directory.Exists("C:/UNITY_TEMP/Videos"))
        {
            Directory.CreateDirectory("C:/UNITY_TEMP/Videos");
        }

        if (!Directory.Exists("C:/UNITY_TEMP/DummyVideos"))
        {
            Directory.CreateDirectory("C:/UNITY_TEMP/DummyVideos");
        }

        // if (!Directory.Exists("C:/UNITY_TEMP/Videos/Cube" + CubeDataController.VisualisedIndex.ToString()))
        // {
        //     Directory.CreateDirectory("C:/UNITY_TEMP/Videos/Cube" + CubeDataController.VisualisedIndex.ToString());
        // }
        if (!Directory.Exists("C:/UNITY_TEMP/Videos/" + CubeDataController.FileName))
        {
            Directory.CreateDirectory("C:/UNITY_TEMP/Videos/" + CubeDataController.FileName);
        }

        if (!Directory.Exists("C:/UNITY_TEMP/DummyVideos/" + CubeDataController.FileName))
        {
            Directory.CreateDirectory("C:/UNITY_TEMP/DummyVideos/" + CubeDataController.FileName);
        }
    }

    void StopRecording()
    {
#if UNITY_EDITOR
        //RecorderWindow recorderWindow = (RecorderWindow)EditorWindow.GetWindow(typeof(RecorderWindow));
        //OBSRecorder.Instance.KillRecording();

        //if (recorderWindow.IsRecording())
        //{
        //    recorderWindow.StopRecording();
        //}
#endif
    }
}

public enum RecordingState
{
    VideoRecording,
    ScreenshotRecording,
}
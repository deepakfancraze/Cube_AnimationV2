using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
#endif
using UnityEngine;
using UnityEngine.Rendering;

public class ScreenshotMaker : MonoBehaviour
{
    [SerializeField] Camera myCamera;
    [SerializeField] Camera bottomCamera;
    public static string fileName;

    int index = 1;

    bool isfirstScreenShotDone;
    void OnEnable()
    {
        EventManager.Subscribe(EventManager.EventType.CanMakeScreenshot, OnCanMakeScreenshot);
        EventManager.Subscribe(EventManager.EventType.MakeScreenshotWithBottomCamera, OnMakeScreenshotWithBottomCamera);
    }

    void OnDisable()
    {
        EventManager.Unsubscribe(EventManager.EventType.CanMakeScreenshot, OnCanMakeScreenshot);
        EventManager.Unsubscribe(EventManager.EventType.MakeScreenshotWithBottomCamera, OnMakeScreenshotWithBottomCamera);
    }

    void OnCanMakeScreenshot(object data)
    {
        if (StateController.GetState() != State.Recording) { return; }

        myCamera.gameObject.SetActive(true);
        bottomCamera.gameObject.SetActive(false);

        CheckingCreatingFolders();

        if (VideoRecorder.instance.recordingState == RecordingState.ScreenshotRecording)
        {
            if (VideoRecorder.RecordingWidth == 1920)
            {
                if (!isfirstScreenShotDone)
                {
                    ScreenCapture.CaptureScreenshot("C:/UNITY_TEMP/Screenshots/" + CubeDataController.FileName + "/" + CubeDataController.FileName + "-" + "Thumbnail" + ".png");
                    isfirstScreenShotDone = true;
                }
                return;
            }
            else
            {
                ScreenCapture.CaptureScreenshot("C:/UNITY_TEMP/Screenshots/" + CubeDataController.FileName + "/" + CubeDataController.FileName + "-" + "Surface-" + index.ToString() + ".png");
            }
        }


        index += 1;
    }

    void CheckingCreatingFolders()
    {
        if (!Directory.Exists("C:/UNITY_TEMP"))
        {
            Directory.CreateDirectory("C:/UNITY_TEMP");
        }

        if (!Directory.Exists("C:/UNITY_TEMP/Screenshots"))
        {
            Directory.CreateDirectory("C:/UNITY_TEMP/Screenshots");
        }

        // if (!Directory.Exists("C:/UNITY_TEMP/Screenshots/Cube" + CubeDataController.VisualisedIndex.ToString()))
        // {
        //     Directory.CreateDirectory("C:/UNITY_TEMP/Screenshots/Cube" + CubeDataController.VisualisedIndex.ToString());
        // } 
        if (!Directory.Exists("C:/UNITY_TEMP/Screenshots/" + CubeDataController.FileName))
        {
            Directory.CreateDirectory("C:/UNITY_TEMP/Screenshots/" + CubeDataController.FileName);
        }
    }

    void OnMakeScreenshotWithBottomCamera(object data)
    {
        if (StateController.GetState() != State.Recording) { return; }

        myCamera.gameObject.SetActive(false);
        bottomCamera.gameObject.SetActive(true);


        StartCoroutine(WaitFrame());
    }

    void TryRecording()
    {
#if UNITY_EDITOR
        RecorderWindow recorderWindow = (RecorderWindow)EditorWindow.GetWindow(typeof(RecorderWindow));
        if (!recorderWindow.IsRecording())
        {
            var videoRecorder = ScriptableObject.CreateInstance<MovieRecorderSettings>();
            var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();

            CheckingCreatingFolders();

            videoRecorder.OutputFile = "C:\\UNITY_TEMP\\bottomcamera";
            videoRecorder.ImageInputSettings = new GameViewInputSettings
            {
                OutputWidth = VideoRecorder.RecordingWidth,
                OutputHeight = VideoRecorder.RecordingHeight,
            };
            controllerSettings.FrameRate = 60f;
            controllerSettings.CapFrameRate = false;
            controllerSettings.AddRecorderSettings(videoRecorder);
            recorderWindow.SetRecorderControllerSettings(controllerSettings);
            recorderWindow.StartRecording();
        }
#endif
    }

    void StopRecording()
    {
#if UNITY_EDITOR
        RecorderWindow recorderWindow = (RecorderWindow)EditorWindow.GetWindow(typeof(RecorderWindow));
        if (recorderWindow.IsRecording())
        {
            recorderWindow.StopRecording();
        }
#endif
    }

    IEnumerator WaitFrame()
    {
        Time.timeScale = 1;
        yield return new WaitForEndOfFrame();
        TryRecording();
        //Debug.Break();
        yield return new WaitForSeconds(2f);

        CheckingCreatingFolders();

        // ScreenCapture.CaptureScreenshot("C:/UNITY_TEMP/Screenshots/Cube" + CubeDataController.VisualisedIndex.ToString() + "/Screenshot" + index.ToString() + ".png");

        if (VideoRecorder.RecordingWidth != 1920)
        {
            if (VideoRecorder.instance.recordingState == RecordingState.ScreenshotRecording)
                ScreenCapture.CaptureScreenshot("C:/UNITY_TEMP/Screenshots/" + CubeDataController.FileName + "/" + CubeDataController.FileName + "-" + "Surface-" + index.ToString() + ".png");
            index += 1;
        }
        yield return new WaitForSeconds(2f);
        StopRecording();
        //Debug.Break();
        yield return new WaitForSeconds(2f);
        isfirstScreenShotDone = false;
        myCamera.gameObject.SetActive(true);
        bottomCamera.gameObject.SetActive(false);
        yield return new WaitForSeconds(2f);
        index = 1;
        if (VideoRecorder.instance.recordingState == RecordingState.ScreenshotRecording)
            Time.timeScale = 3;
        EventManager.Dispatch(EventManager.EventType.BottomScreenshotWasMade, null);
    }
}

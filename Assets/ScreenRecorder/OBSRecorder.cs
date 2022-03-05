using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Diagnostics;


public class OBSRecorder : MonoBehaviour
{

    public bool isRecording = false;
    private Process obs;
    public static OBSRecorder Instance { get; private set; }
    public static Action obsApplicationHasStarted;
    [SerializeField]
    private  float timeDiffObsApplication = 3.5f;

    // Use this for initialization
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            return;
        }

        DontDestroyOnLoad(gameObject);

        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
        }

        //if (!isRecording) {
        //	StartRecording ();
        //}

    }

 


    public void OnApplicationQuit()
    {
        if (Instance != this)
        {
            return;
        }
        if (isRecording)
        {
            KillRecording();
        }
    }

    public void StartRecording()
    {
        if (isRecording)
        {
            return;
        }

        foreach (var process in Process.GetProcessesByName("obs64"))
        {
            process.Kill();
        }

        obs = new Process();
        obs.StartInfo.FileName = "C:\\Program Files\\obs-studio\\bin\\64bit\\obs64.exe";
        obs.StartInfo.Arguments = "--startrecording --minimize-to-tray";
        obs.StartInfo.WorkingDirectory = "C:\\Program Files\\obs-studio\\bin\\64bit";
        obs.StartInfo.CreateNoWindow = true;
        obs.StartInfo.UseShellExecute = false;
        obs.Start();
    }


    void DataReceived(object sender, DataReceivedEventArgs eventArgs)
    {
        // Handle it
        UnityEngine.Debug.LogError("Successfully launched app_ Call Back_________________________");

    }
    public void KillRecording()
    {
        if (!isRecording)
        {
            return;
        }
        isRecording = false;
        obs.Kill();
    }


    // Example of using OBS Screen Recording based on whether a specific scene is loaded.
    /*
	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (Instance != this) {
			return;
		}
		if (scene.name == "Scene 1") {
			StartRecording ();
		}  else if (scene.name == "Menu") {
			KillRecording ();
		}
	}
	*/

    private void Update()
    {
        if (StateController.GetState() == State.Recording)
        {
            if (VideoRecorder.instance.recordingState == RecordingState.VideoRecording)
            {
                if (isRecording)
                    return;
                KillRunningInstances();
            }
        }

    }
    void KillRunningInstances()
    {
        //NOTE: GetProcessByName() doesn't seem to work on Win7
        //Process[] running = Process.GetProcessesByName("notepad");
        Process[] running = Process.GetProcesses();
        foreach (Process process in running)
        {
            try
            {
                if (!process.HasExited && process.ProcessName == "obs64")
                {
                    StartCoroutine(WaitForObsStart());
                    isRecording = true;
                }
            }
            catch (System.InvalidOperationException)
            {
                //do nothing
                UnityEngine.Debug.Log("***** InvalidOperationException was caught!");
            }
        }
    }

    IEnumerator WaitForObsStart()
    {
        yield return new WaitForSeconds(timeDiffObsApplication);
        UnityEngine.Debug.LogError("Successfully launched app_________________________");
        obsApplicationHasStarted?.Invoke();

    }
}

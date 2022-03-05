using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

public class VideoLoader : MonoBehaviour
{


    static IEnumerator LoadVideoCoroutine(Action<VideoClip> action, string url)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        //action?.Invoke(www.downloadHandler.data)
    }


    public static IEnumerator loadVideoFromThisURL(Action<string,string> action, string _url,string videoName,string isAudio )
    {
        UnityWebRequest _videoRequest = UnityWebRequest.Get(_url);

        yield return _videoRequest.SendWebRequest();

        if (_videoRequest.isDone == false || _videoRequest.error != null)
        { Debug.Log("Request = " + _videoRequest.error); }

        Debug.Log("Video Done - " + _videoRequest.isDone);

        byte[] _videoBytes = _videoRequest.downloadHandler.data;

        string _pathToFile = Path.Combine(Application.persistentDataPath, videoName+".mp4");
        File.WriteAllBytes(_pathToFile, _videoBytes);
        Debug.Log(_pathToFile);
        action?.Invoke(_pathToFile,isAudio);

        //StartCoroutine(this.playThisURLInVideo(_pathToFile));
        yield return null;
    }


   
}

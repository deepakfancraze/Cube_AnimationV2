using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataUploader : MonoBehaviour
{
    [SerializeField] CubeDataController dataController;
    public string JsonForRecording;
    public string jsonToShowForSwiping;
    
    void Start()
    {
        //dataController.convertJsonToData("{\"element1\":{\"player_name\":\"playername\",\"moment_length\":\"25s.\",\"border_color\":\"#f44336\",\"rarity\":\"GENESIS\",\"image_url\":\"imageurl\"},\"element2\":{\"action_type\":\"SEVEN\",\"image_url\":\"imageurl\",\"moment_date\":\"APR 02 2011\",\"team_name\":\"JAP\"},\"element3\":{\"player_jersey\":24},\"element4\":{\"score\":\"Ind 144/7(20) v Aus 130/8(20)\",\"image_url\":\"http://aries.ektf.hu/~gregtom6/CubeExample/DarkPacksVintageV2.png\"},\"element5\":{\"team_logo\":\"teamlogo\"}}");
        StartCoroutine(WaitFrame());
    }

    IEnumerator WaitFrame()
    {
        yield return new WaitForEndOfFrame();
#if UNITY_EDITOR
        if (VideoRecorder.IsRecording)
        {
            dataController.convertJsonToData(JsonForRecording);
        }
        else
        {
            dataController.convertJsonToData(jsonToShowForSwiping);
        }
#endif

    }

    public void GetFromJavaScript(string json)
    {
        Debug.Log("Call From JAva Script_____________________________________________________"+json);
        StartCoroutine(WaitSomeFrame(json));
    }

    IEnumerator WaitSomeFrame(string json)
    {
        yield return new WaitForEndOfFrame();
#if UNITY_WEBGL
        dataController.convertJsonToData(json);
#endif

    }
}

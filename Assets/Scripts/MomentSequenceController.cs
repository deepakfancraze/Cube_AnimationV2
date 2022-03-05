using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomentSequenceController : MonoBehaviour
{
    [SerializeField] MomentListContainer momentsList;

    [SerializeField] CubeDataController dataController;

    void Start()
    {
        StartSequencingMoments();
    }

    void StartSequencingMoments()
    {
        Debug.Log("Current Video : " + momentsList.moments[momentsList.currentMoment] + "_" + momentsList.currentMoment);
        if (momentsList.currentMoment >= momentsList.moments.Count)
        {
            AllMomentsPlayed();
            return ;
        }

        Debug.Log("Setting moment data");
        dataController.SetMomentData(momentsList.moments[momentsList.currentMoment]);

        if (momentsList.showAllMoments)
        {
            momentsList.currentMoment++;
        }

        //dataController.convertJsonToData("{\"element1\":{\"player_name\":\"playername\",\"moment_length\":\"25s.\",\"rarity\":\"GENESIS\",\"image_url\":\"imageurl\"},\"element2\":{\"action_type\":\"SEVEN\",\"image_url\":\"imageurl\",\"moment_date\":\"APR 02 2011\",\"team_name\":\"JAP\"},\"element3\":{\"player_jersey\":24},\"element4\":{\"score\":\"Ind 144/7(20) v Aus 130/8(20)\",\"image_url\":\"imageurl\"},\"element5\":{\"team_logo\":\"teamlogo\"}}");

    }

    void AllMomentsPlayed()
    {
        Debug.Log("All videos recorder");
        momentsList.currentMoment = 0;
    }
    public void MomentEnded()
    {
        Debug.Log("Moment has ended");
        if (momentsList.showAllMoments)
        {
            dataController.ResetScene();
        }
    }

}

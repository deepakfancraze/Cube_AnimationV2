using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "MomentData" , menuName = "MomentData", order = 1)]
public class MomentDataContainer : ScriptableObject
{
    //1st name
    [Space]
    [SerializeField] public VideoClip momentClip;
    [SerializeField] public string playerName;
    //2nd score a,b name a,b, home ,away 
    [Space]
    [SerializeField] public string team_a_name;
    [SerializeField] public string team_a_scoreRun;
    [SerializeField] public string team_a_scoreWic;
    [SerializeField] public string team_a_homeAway;
    [Space]
    [SerializeField] public string team_b_name;
    [SerializeField] public string team_b_scoreRun;
    [SerializeField] public string team_b_scoreWic;
    [SerializeField] public string team_b_homeAway;
    //3rd player number
    [Space]
    [SerializeField] public string playerNumber;
    //Sprites
    [Space]
    public Sprite packImage;
    public Sprite video_SS_BG;
    public Sprite numberBG;
    public Sprite teamLogoImage;
    public Sprite assosiationLogoImage;
    public Sprite companyLogoImage;
}

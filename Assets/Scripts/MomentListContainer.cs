using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName ="MomentList" , menuName = "MomentList" , order = 2)]
public class MomentListContainer : ScriptableObject
{
    public int currentMoment;
    public bool showAllMoments;
    public bool interactable;
    public List<MomentDataContainer> moments;
}

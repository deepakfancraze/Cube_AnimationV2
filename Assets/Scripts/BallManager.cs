using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    [SerializeField] GameObject epicBall;
    [SerializeField] GameObject genesisBall;
    [SerializeField] GameObject legendaryBall;
    [SerializeField] GameObject platinumBall;
    [SerializeField] GameObject rareBall;
    [SerializeField] GameObject commonBall;

    static BallManager instance;

    private void Start()
    {
        instance = this;
    }

    public static void SetBall(CubeRarityType cubeRarityType)
    {
        instance.epicBall.SetActive(cubeRarityType == CubeRarityType.epic);
        instance.genesisBall.SetActive(cubeRarityType == CubeRarityType.genesis);
        instance.legendaryBall.SetActive(cubeRarityType == CubeRarityType.legendary);
        instance.platinumBall.SetActive(cubeRarityType == CubeRarityType.platinum);
        instance.rareBall.SetActive(cubeRarityType == CubeRarityType.rare);
        instance.commonBall.SetActive(cubeRarityType == CubeRarityType.common);
    }
}

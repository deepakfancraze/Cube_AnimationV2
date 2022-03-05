using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightingController : MonoBehaviour
{
    static LightingController instance;
    [SerializeField]
    List<GameObject> lightobject;

    private void Awake()
    {
        instance = this;
    }

    public static void SetLighting(CubeRarityType rarity)
    {
        Debug.Log("Rarity _____________" + rarity);
        int index = (int)rarity;
        for (int i = 0; i < instance.lightobject.Count; i++)
        {
            instance.lightobject[i].SetActive(false);
        }
        instance.lightobject[index].SetActive(true);
    }
}

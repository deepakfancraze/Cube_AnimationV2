using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderDisabler : MonoBehaviour
{
    [SerializeField] GameObject loaderGameObject;

    private void OnEnable()
    {
        CubeDataController.onEverythingLoaded += OnEverythingLoaded;
    }

    private void OnDisable()
    {
        CubeDataController.onEverythingLoaded -= OnEverythingLoaded;
    }

    void OnEverythingLoaded()
    {
        loaderGameObject.SetActive(false);
    }
}

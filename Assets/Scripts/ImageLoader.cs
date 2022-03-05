using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageLoader : MonoBehaviour
{
    [SerializeField] string url;
    [SerializeField] Renderer thisRenderer;

    static ImageLoader instance;

    void Start()
    {
        instance = this;

        GetTexture2D(Done, url);

        if (thisRenderer != null)
            thisRenderer.material.color = Color.red;
    }

    void Done(Texture2D texture2D)
    {

    }

    static IEnumerator LoadImageCoroutine(Action<Texture2D> action, string url)
    {
        Debug.Log("Loading.....");
        WWW wwwLoader = new WWW(url);
        yield return wwwLoader;
        Debug.Log("Loaded");

        if (instance.thisRenderer != null)
        {
            instance.thisRenderer.material.color = Color.white;
            instance.thisRenderer.material.mainTexture = wwwLoader.texture;
        }

        action?.Invoke(wwwLoader.texture);

    }

    public static void GetTexture2D(Action<Texture2D> action, string url)
    {
        instance.StartCoroutine(LoadImageCoroutine(action, url));
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public string sortingLayerName = string.Empty; //initialization before the methods
    public int orderInLayer = 0;
    public Renderer MyRenderer;

    // Start is called before the first frame update
    void Update()
    {
        SetSortingLayer();
    }

    void SetSortingLayer()
    {
        if (sortingLayerName != string.Empty)
        {
            MyRenderer.sortingLayerName = sortingLayerName;
            MyRenderer.sortingOrder = orderInLayer;
        }
    }
}

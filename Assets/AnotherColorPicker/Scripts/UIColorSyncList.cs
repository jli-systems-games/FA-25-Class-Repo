using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIColorSyncList : MonoBehaviour
{

    public Image colorSource; 


    public List<Image> targetList = new List<Image>(); 


    public bool autoUpdate = true;

    private Color lastColor;

    void Start()
    {
        if (colorSource != null)
        {
            lastColor = colorSource.color;
            updateAllColor();
        }
    }

    void Update()
    {
        if (!autoUpdate || colorSource == null) return;

 
        if (colorSource.color != lastColor)
        {
            lastColor = colorSource.color;
            updateAllColor();
        }
    }

    public void updateAllColor()
    {
        if (colorSource == null) return;

        Color nowColor = colorSource.color;

        foreach (var img in targetList)
        {
            if (img != null)
                img.color = nowColor;
        }

    }
}
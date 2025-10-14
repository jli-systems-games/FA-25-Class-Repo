using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using Unity.VisualScripting;

public class Customizer : MonoBehaviour
{
    public List<Sprite> customizerHat = new List<Sprite>();
    public List<Sprite> customizerDress = new List<Sprite>();
    public List<Sprite> customizerShoe = new List<Sprite>();

    private void Start()
    {
        #region Update Data Lists

        Data.data_skinHat.Clear();
        Data.data_skinDress.Clear();
        Data.data_skinShoe.Clear();

        foreach(Sprite sprite in customizerHat)
        {
            Data.data_skinHat.Add(sprite);
        }

        foreach (Sprite sprite in customizerDress)
        {
            Data.data_skinDress.Add(sprite);
        }

        foreach (Sprite sprite in customizerShoe)
        {
            Data.data_skinShoe.Add(sprite);
        }

        #endregion
    }
}
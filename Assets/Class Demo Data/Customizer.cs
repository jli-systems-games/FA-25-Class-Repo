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

        Data1.data_skinHat.Clear();
        Data1.data_skinDress.Clear();
        Data1.data_skinShoe.Clear();

        foreach(Sprite sprite in customizerHat)
        {
            Data1.data_skinHat.Add(sprite);
        }

        foreach (Sprite sprite in customizerDress)
        {
            Data1.data_skinDress.Add(sprite);
        }

        foreach (Sprite sprite in customizerShoe)
        {
            Data1.data_skinShoe.Add(sprite);
        }

        #endregion
    }
}
using UnityEngine;
using System.Collections.Generic;

public class Customixer : MonoBehaviour
{
    public List<Sprite> customizerHats=new List<Sprite>();
    public List<Sprite> customizerDress = new List<Sprite>();

    private void Start()
    {
        #region Update Data Lists

        Data.data_skinHat.Clear();
        Data.data_skinDress.Clear();

        foreach(Sprite sprite in customizerHats)
        {
            Data.data_skinHat.Add(sprite);
        }
        foreach (Sprite sprite in customizerDress)
        {
            Data.data_skinDress.Add(sprite);
        }
        #endregion
    }
}
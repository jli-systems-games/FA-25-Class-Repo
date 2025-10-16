using UnityEngine;
using System.Collections.Generic;
using System.Data;

public class Customizer : MonoBehaviour
{
    #region Character Outfit Lists
    public List<Sprite> customizerHats = new List<Sprite>();
    public List<Sprite> customizerDress = new List<Sprite>();
    public List<Sprite> customizerShoes = new List<Sprite>();

    #endregion 
    private void Start()
    {
        #region Update Data Lists
        //clear lists
        Data.data_skinHat.Clear();
        Data.data_skinDress.Clear();
        Data.data_skinShoes.Clear();

        //populate lists
        foreach(Sprite sprite in customizerHats)
        {
            Data.data_skinHat.Add(sprite);
        }

        foreach(Sprite sprite in customizerDress)
        {
            Data.data_skinDress.Add(sprite);
        }

        foreach (Sprite sprite in customizerShoes)
        {
            Data.data_skinShoes.Add(sprite);
        }

        #endregion
    }
}

using UnityEngine;

public class ViolinPlayScene : MonoBehaviour
{
    [Header("Bodies")]
    public GameObject[] bodyOptions;

    [Header("Strings")]
    public GameObject[] stringOptions;
    public GameObject[] string2Options;
    public GameObject[] string3Options;
    public GameObject[] string4Options;
    void Start()
    {
        int bodyIndex = CentralData.current.selectedBodyIndex;
        int stringIndex = CentralData.current.selectedStringIndex;
        int string2Index = CentralData.current.selectedString2Index;
        int string3Index = CentralData.current.selectedString3Index;
        int string4Index = CentralData.current.selectedString4Index;


        for (int i = 0; i < bodyOptions.Length; i++)
        {
            bodyOptions[i].SetActive(i == bodyIndex);
        }

  
        for (int i = 0; i < stringOptions.Length; i++)
        {
            stringOptions[i].SetActive(i == stringIndex);
        }
        for (int i = 0; i < string2Options.Length; i++)
        {
            string2Options[i].SetActive(i == string2Index);
        }
        for (int i = 0; i < string3Options.Length; i++)
        {
            string3Options[i].SetActive(i == string3Index);
        }
        for (int i = 0; i < string4Options.Length; i++)
        {
            string4Options[i].SetActive(i == string4Index);
        }
    }
}

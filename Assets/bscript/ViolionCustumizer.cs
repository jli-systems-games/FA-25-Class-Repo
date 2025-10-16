using UnityEngine;
using UnityEngine.SceneManagement;

public class ViolinCustomizer : MonoBehaviour
{
    [Header("Bodies")]
    public GameObject[] bodyOptions;  

    [Header("Strings")]
    public GameObject[] stringOptions;
    public GameObject[] string2Options;
    public GameObject[] string3Options;
    public GameObject[] string4Options;

    public void ChooseBody(int index)
    {
        CentralData.current.selectedBodyIndex = index;

       
        for (int i = 0; i < bodyOptions.Length; i++)
        {
            bodyOptions[i].SetActive(i == index);
        }
    }

    public void ChooseString(int index)
    {
        CentralData.current.selectedStringIndex = index;

      
        for (int i = 0; i < stringOptions.Length; i++)
        {
            stringOptions[i].SetActive(i == index);
        }
    }
    public void ChooseString2(int index)
    {
        CentralData.current.selectedString2Index = index;


        for (int i = 0; i < string2Options.Length; i++)
        {
            string2Options[i].SetActive(i == index);
        }
    }
    public void ChooseString3(int index)
    {
        CentralData.current.selectedString3Index = index;


        for (int i = 0; i < string3Options.Length; i++)
        {
            string3Options[i].SetActive(i == index);
        }
    }
    public void ChooseString4(int index)
    {
        CentralData.current.selectedString4Index = index;


        for (int i = 0; i < string4Options.Length; i++)
        {
            string4Options[i].SetActive(i == index);
        }
    }
    public void GoToPlayScene()
    {
        SceneManager.LoadScene("PlayScene");
    }
}

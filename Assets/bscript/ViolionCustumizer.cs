using UnityEngine;
using UnityEngine.SceneManagement;

public class ViolinCustomizer : MonoBehaviour
{
    [Header("Bodies")]
    public GameObject[] bodyOptions;  

    [Header("Strings")]
    public GameObject[] stringOptions;

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

    public void GoToPlayScene()
    {
        SceneManager.LoadScene("PlayScene");
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class ViolinCustomizer : MonoBehaviour
{
    [Header("Bodies")]
    public GameObject[] bodyOptions;

    [Header("Strings")]
    public GameObject[][] stringOptions = new GameObject[4][]; // optional if you have organized each string's options separately
    public GameObject[] string1Options;
    public GameObject[] string2Options;
    public GameObject[] string3Options;
    public GameObject[] string4Options;

    private void Awake()
    {
        // Assign manually since Unity can’t serialize jagged arrays
        stringOptions[0] = string1Options;
        stringOptions[1] = string2Options;
        stringOptions[2] = string3Options;
        stringOptions[3] = string4Options;
    }

    public void ChooseBody(int index)
    {
        CentralData.current.selectedBodyIndex = index;

        // Show chosen one, hide others
        for (int i = 0; i < bodyOptions.Length; i++)
        {
            bodyOptions[i].SetActive(i == index);
        }
    }

    public void ChooseString(int stringIndex, int optionIndex)
    {
        CentralData.current.selectedStringIndices[stringIndex] = optionIndex;

        GameObject[] options = stringOptions[stringIndex];
        for (int i = 0; i < options.Length; i++)
        {
            options[i].SetActive(i == optionIndex);
        }
    }

    public void GoToPlayScene()
    {
        SceneManager.LoadScene("PlayScene");
    }
}

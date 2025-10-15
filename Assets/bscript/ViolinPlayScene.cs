using UnityEngine;

public class ViolinPlayScene : MonoBehaviour
{
    [Header("Bodies")]
    public GameObject[] bodyOptions;

    [Header("Strings")]
    public GameObject[] string1Options;
    public GameObject[] string2Options;
    public GameObject[] string3Options;
    public GameObject[] string4Options;

    void Start()
    {
        // Load chosen body
        int bodyIndex = CentralData.current.selectedBodyIndex;
        for (int i = 0; i < bodyOptions.Length; i++)
        {
            bodyOptions[i].SetActive(i == bodyIndex);
        }

        // Load chosen strings
        int[] stringChoices = CentralData.current.selectedStringIndices;

        GameObject[][] stringSets = new GameObject[4][]
        {
            string1Options, string2Options, string3Options, string4Options
        };

        for (int s = 0; s < 4; s++)
        {
            for (int i = 0; i < stringSets[s].Length; i++)
            {
                stringSets[s][i].SetActive(i == stringChoices[s]);
            }
        }
    }
}

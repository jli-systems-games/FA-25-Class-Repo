using UnityEngine;

public class ViolinPlayScene : MonoBehaviour
{
    [Header("Bodies")]
    public GameObject[] bodyOptions;

    [Header("Strings")]
    public GameObject[] stringOptions;

    void Start()
    {
        int bodyIndex = CentralData.current.selectedBodyIndex;
        int stringIndex = CentralData.current.selectedStringIndex;

       
        for (int i = 0; i < bodyOptions.Length; i++)
        {
            bodyOptions[i].SetActive(i == bodyIndex);
        }

  
        for (int i = 0; i < stringOptions.Length; i++)
        {
            stringOptions[i].SetActive(i == stringIndex);
        }
    }
}

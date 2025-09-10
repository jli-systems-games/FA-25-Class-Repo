using UnityEngine;
using UnityEngine.UI;

public class Missleading : MonoBehaviour
{
    public Button buttonP;
    public Button buttonSpace;

    void Start()
    {
        buttonP.onClick.AddListener(Win);
        buttonSpace.onClick.AddListener(Lose);
    }

    void Win()
    {
        Debug.Log("P right");
    }

    void Lose()
    {
        Debug.Log("P wrong");
    }
}
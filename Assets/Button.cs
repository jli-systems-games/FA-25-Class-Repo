using UnityEngine;
using System.Collections.Generic;

public class Button : MonoBehaviour
{
    private GameManager _gm;
    public List<string> scenePool=new List<string>();

    private void Start()
    {
        _gm = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }
    public void ButtonPressed(string newScene)
    {
        _gm.SceneSwitcher(newScene);
    }
    public void RandomScene()
    {
        int randomIndex = Random.Range(0, scenePool.Count);
        if (randomIndex == scenePool.Count)
            randomIndex--;

        _gm.SceneSwitcher(scenePool[randomIndex]);
    }
}

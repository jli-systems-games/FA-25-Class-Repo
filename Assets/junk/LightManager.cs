using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LightManager : MonoBehaviour
{
    public Transform[] spawnPoints;
    public GameObject hiddenItemPrefab;
    private GameObject currentHiddenItem;

    public Light[] allLights;
    public Color redColor = Color.red;
    public Color yellowColor = Color.yellow;
    public Color blueColor = Color.blue;
    public Color greenColor = Color.green;

    public ColorManager colorManager;
    public string transitionSceneName = "TransitionScene";

    void Start()
    {
        GenerateRoom();
        RefreshLights();
    }

    void Update()
    {
        DetectHiddenItemInput();
    }

    void GenerateRoom()
    {
        Transform randomSpot = spawnPoints[Random.Range(0, spawnPoints.Length)];
        currentHiddenItem = Instantiate(hiddenItemPrefab, randomSpot.position, Quaternion.identity);
    }

    public void RefreshLights()
    {
        foreach (var light in allLights)
            light.enabled = false;

        List<int> indices = new List<int>();
        for (int i = 0; i < allLights.Length; i++) indices.Add(i);

        for (int i = 0; i < 4; i++)
        {
            int idx = indices[Random.Range(0, indices.Count)];
            indices.Remove(idx);
            Light l = allLights[idx];
            l.color = GetColorFromTheme(colorManager.CurrentThemeIndex);
            l.enabled = true;
        }
    }

    private Color GetColorFromTheme(int index)
    {
        switch (index)
        {
            case 0: return redColor;
            case 1: return yellowColor;
            case 2: return blueColor;
            case 3: return greenColor;
            default: return Color.white;
        }
    }

    void DetectHiddenItemInput()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            //if (Physics.Raycast(ray, out hit, 100f))
            //{
            //    if (hit.collider.gameObject == currentHiddenItem)
            //    {
            //        GameManager.Instance.isReturningFromTransition = true;
            //        SceneManager.LoadScene("TransitionScene");
            //    }
            //}
        }
    }
}

using UnityEngine;

public class ClickToActivateLoop : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsToActivate;
    private int currentIndex = 0;
    public GameObject bg;
    void Start()
    {
        
        foreach (var obj in objectsToActivate)
        {
            obj.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            ShowNextObject();
            bg.SetActive(false);
        }
    }

    private void ShowNextObject()
    {
       
        foreach (var obj in objectsToActivate)
        {
            obj.SetActive(false);
        }

      
        objectsToActivate[currentIndex].SetActive(true);

      
        currentIndex = (currentIndex + 1) % objectsToActivate.Length;
    }
}

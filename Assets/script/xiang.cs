using UnityEngine;

public class ClickCycle : MonoBehaviour
{
    public GameObject[] items;   
    public AudioSource clickAudio;
    public int nextButton;
    private int currentIndex = 0;
    public GameObject muyu;
    public GameObject huang;
    void Start()
    {
        
        ShowOnlyCurrent();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            
            items[currentIndex].SetActive(false);

           
            currentIndex = (currentIndex + 1) % items.Length;

           
            items[currentIndex].SetActive(true);

            nextButton ++;
            if (clickAudio != null)
                clickAudio.Play();
        }
        if (nextButton == 15)
        {
            muyu.SetActive(true);
            huang.SetActive(true);
        }
    }

    void ShowOnlyCurrent()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].SetActive(i == currentIndex);
        }
    }
}

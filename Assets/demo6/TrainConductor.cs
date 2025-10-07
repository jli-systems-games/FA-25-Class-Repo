using UnityEngine;

public class TrainConductor : MonoBehaviour
{
    public GameObject fireworkone;
    public GameObject fireworktwo;
    public GameObject fireworkthree;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TrainController.onTrainArrival += AnnouceTrain;
        TrainController.fire2act += second;


    }

    void AnnouceTrain()
    {
        Debug.Log("Train here");

        fireworkone.SetActive(true);
    }
    void second()
    {
        Debug.Log("Train here");

        fireworktwo.SetActive(true);
    }
    void thirdone()
    {
        fireworkthree.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}

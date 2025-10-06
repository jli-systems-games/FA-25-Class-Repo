using UnityEngine;

public class TrainConductor : MonoBehaviour
{
    private void Start()
    {
        TrainController.onTrainArrival += AnnounceTrain; //Subscribe a function from another script's function (-= to unsubscribe)    
    }

    void AnnounceTrain()
    {
        Debug.Log("Train here!");
    }
}

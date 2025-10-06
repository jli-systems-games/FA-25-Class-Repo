using UnityEngine;

public class TrainConductor : MonoBehaviour
{
    private void Start()
    {
        TrainController.onTrainArrival += AnnounceTrain;
    }

    void AnnounceTrain()
    {
        Debug.Log("Train here!");
    }
}

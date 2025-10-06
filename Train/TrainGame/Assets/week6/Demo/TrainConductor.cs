using UnityEngine;

public class TrainConductor : MonoBehaviour
{
    private void Start()
    {
        DemoTrain.onTrainArrival += AnnounceTrain;
    }
    void AnnounceTrain()
    {
        Debug.Log("Train here");
    }
}

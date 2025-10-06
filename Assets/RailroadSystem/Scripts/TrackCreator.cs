using UnityEngine;
using Unity.Cinemachine; // new namespace!
using UnityEngine.Splines;

public class TrackCreator : MonoBehaviour
{
    [SerializeField] SplineContainer track;
    [SerializeField] bool loopedTrack = false;

    private Spline spline;

    void Start()
    {
        GenerateTrack();
    }

    public void GenerateTrack()
    {
        if (!track)
        {
            Debug.LogError("No spline track assigned.");
            return;
        }

        spline = track.Spline;
        spline.Closed = loopedTrack;

        // Example: clear old points and add new ones from child transforms
        spline.Clear();

        foreach (Transform child in track.transform)
        {
            spline.Add(new BezierKnot(child.localPosition));
        }

        // You can use CinemachineSplineDolly or CinemachineSplineCart to follow this spline
        Debug.Log($"Generated spline with {spline.Count} waypoints. Looped: {loopedTrack}");
    }
}

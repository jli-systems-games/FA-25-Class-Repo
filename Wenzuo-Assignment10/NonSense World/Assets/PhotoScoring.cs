using UnityEngine;

public class PhotoScoring : MonoBehaviour
{
    public KeyCode snapKey = KeyCode.F;
    public UnityEngine.UI.Text uiText; // 可空
    Camera cam; void Awake() { cam = GetComponent<Camera>(); }

    void Update()
    {
        if (!Input.GetKeyDown(snapKey)) return;
        var planes = GeometryUtility.CalculateFrustumPlanes(cam);
        int weird = 0, landmark = 0;

        foreach (var r in UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None))
        {
            if (!GeometryUtility.TestPlanesAABB(planes, r.bounds)) continue;
            var go = r.gameObject;
           
           
        }

        int score = weird * 10 + landmark * 25 + Random.Range(0, 6);
        string msg = $"📸 Shot Score {score} (weird:{weird}, landmark:{landmark})";
        if (uiText) uiText.text = msg; else Debug.Log(msg);
    }
}

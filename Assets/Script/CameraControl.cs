using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("Cameras")]
    public Camera cameraA;
    public Camera cameraB;

    [Header("Input")]
    public KeyCode key = KeyCode.M;

    private bool isSwitched = false;

    void Start()
    {
        SetActiveCamera(cameraA, true);
        SetActiveCamera(cameraB, false);
    }

    void Update()
    {
        if (Input.GetKey(key))
        {
            if (!isSwitched)
            {
                isSwitched = true;
                SetActiveCamera(cameraA, false);
                SetActiveCamera(cameraB, true);
            }
        }
        else
        {
            if (isSwitched)
            {
                SetActiveCamera(cameraA, true);
                SetActiveCamera(cameraB, false);
                isSwitched = false;
            }
        }
    }

    private void SetActiveCamera(Camera cam, bool active)
    {
        if (!cam) return;
        cam.enabled = active;
        var listener = cam.GetComponent<AudioListener>();
        if (listener) listener.enabled = active;
    }
}
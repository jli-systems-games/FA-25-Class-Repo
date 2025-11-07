using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    [Header("Cameras")]
    public Camera cameraA;
    public Camera cameraB;

    [Header("Input")]
    public KeyCode key = KeyCode.N;

    private int currentCamera = 0;

    void Start()
    {
        SetActiveCamera(cameraA, true);
        SetActiveCamera(cameraB, false);
    }

    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            currentCamera = (currentCamera + 1) % 2;
            
            if (currentCamera == 0)
            {
                SetActiveCamera(cameraA, true);
                SetActiveCamera(cameraB, false);
            }
            else
            {
                SetActiveCamera(cameraA, false);
                SetActiveCamera(cameraB, true);
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
using UnityEngine;

public class UIAutoDestruct : MonoBehaviour
{
    public float lifetime = 0.3f;
    public bool destroyInsteadOfHide = false;
    public bool unscaledTime = true;

    private float _t;

    private void OnEnable() { _t = 0f; } // Destroy when trigger


    private void Update()
    {
        _t += unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        if (_t >= lifetime)
        {
            if (destroyInsteadOfHide) Destroy(gameObject);
            else gameObject.SetActive(false);
        }
    }
}
using UnityEngine;

public class SlimeCanvas : MonoBehaviour
{
    public Transform pulseTarget;
    public float pulseScale = 0.98f;
    public float pulseDuration = 0.08f;
    public float camJiggle = 0.02f;

    Vector3 _origScale, _origPos;
    float _t; bool _pulsing;

    void Start()
    {
        if (!pulseTarget) pulseTarget = Camera.main.transform;
        _origScale = pulseTarget.localScale;
        _origPos = pulseTarget.localPosition;
    }

    public void Pulse()
    {
        _pulsing = true;
        _t = 0f;
    }

    void Update()
    {
        if (_pulsing)
        {
            _t += Time.deltaTime / pulseDuration;
            float a = Mathf.Sin(Mathf.Clamp01(_t) * Mathf.PI);
            float s = Mathf.Lerp(1f, pulseScale, a);
            pulseTarget.localScale = _origScale * s;
            pulseTarget.localPosition = _origPos + (Vector3)(Random.insideUnitCircle * camJiggle * a);
            if (_t >= 1f)
            {
                _pulsing = false;
                pulseTarget.localScale = _origScale;
                pulseTarget.localPosition = _origPos;
            }
        }
    }
}

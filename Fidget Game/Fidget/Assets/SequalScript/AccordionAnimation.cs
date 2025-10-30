using UnityEngine;

public class AccordionHoldPlay : MonoBehaviour
{
    public Animator animator;
    public string idleState = "Idle";
    public string accordionState = "ACCORDION";
    int _idleHash, _accHash;
    bool _playing;

    void Reset() { animator = GetComponent<Animator>(); }
    void Awake()
    {
        _idleHash = Animator.StringToHash(idleState);
        _accHash = Animator.StringToHash(accordionState);
    }
    void Start()
    {
        if (animator) animator.Play(_idleHash, 0, 0f);
        _playing = false;
    }
    void Update()
    {
        bool hold = Input.GetKey(KeyCode.W);
        if (hold)
        {
            if (!_playing)
            {
                animator.Play(_accHash, 0, 0f);
                _playing = true;
            }
        }
        else
        {
            if (_playing)
            {
                animator.Play(_idleHash, 0, 0f);
                _playing = false;
            }
        }
    }
}

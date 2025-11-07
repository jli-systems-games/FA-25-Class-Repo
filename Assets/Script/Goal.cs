using UnityEngine;

public class Goal : MonoBehaviour
{
    [Header("Settings")]
    public bool requireAllCoins = true;

    [Header("Visuals")]
    public Renderer targetRenderer;
    public Material lockedMaterial;
    public Material unlockedMaterial;

    [Header("Colliders")]
    public Collider triggerZone;
    public Collider blockerCollider;

    [Header("Hint")]
    public bool showHintWhenLocked = true;

    private bool _unlocked = false;

    private void OnEnable()
    {
        GamePlayControl.OnAllCoinsCollected += Unlock;
    }

    private void OnDisable()
    {
        GamePlayControl.OnAllCoinsCollected -= Unlock;
    }

    private void Awake()
    {
        if (triggerZone) triggerZone.isTrigger = true;

        var rb = GetComponent<Rigidbody>();
        if (!rb) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;

        ApplyLockedState();
    }
    private void ApplyLockedState()
    {
        _unlocked = false;
        if (targetRenderer && lockedMaterial) targetRenderer.sharedMaterial = lockedMaterial;
        if (blockerCollider) blockerCollider.enabled = true;
        if (triggerZone) triggerZone.enabled = true;
    }

    private void Unlock()
    {
        _unlocked = true;
        if (targetRenderer && unlockedMaterial) targetRenderer.sharedMaterial = unlockedMaterial;
        if (blockerCollider) blockerCollider.enabled = false;
        if (triggerZone) triggerZone.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (requireAllCoins && !_unlocked)
        {
            if (showHintWhenLocked)
                GamePlayControl.ShowToast("Collect all coins!");
            return;
        }

        GamePlayControl.OnGoalReached();
    }
}
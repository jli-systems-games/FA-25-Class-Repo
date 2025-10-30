using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float radius = 12f;
    public float force = 1200f;
    public float upwardModifier = 0f;
    public LayerMask affectedLayers = ~0;
    public bool ignoreSelf = true;

    public KeyCode testKey;
    public float cooldown;
    bool ready = true;

    public GameObject effectObject;
    public float effectDuration = 2f;

    Rigidbody selfRb;

    void Awake()
    {
        selfRb = GetComponent<Rigidbody>();
        if (effectObject) effectObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(testKey)) TryExplode();
    }

    public void TryExplode()
    {
        if (!ready) return;
        Explode();
        StartCooldown();
    }

    void Explode()
    {
        var cols = Physics.OverlapSphere(transform.position, radius, affectedLayers, QueryTriggerInteraction.Ignore);
        foreach (var c in cols)
        {
            var rb = c.attachedRigidbody;
            if (rb == null) continue;
            if (ignoreSelf && (rb == selfRb || rb.transform.root == transform.root)) continue;
            rb.AddExplosionForce(force, transform.position, radius, upwardModifier, ForceMode.Impulse);
        }
        
        if (effectObject) effectObject.SetActive(true);
    }

    void StartCooldown()
    {
        ready = false;
        StopAllCoroutines();
        
        if (effectObject) StartCoroutine(ShowEffectRoutine());
        
        StartCoroutine(CooldownRoutine());
    }

    System.Collections.IEnumerator CooldownRoutine()
    {
        float t = 0f;
        while (t < cooldown)
        {
            t += Time.deltaTime;
            yield return null;
        }
        ready = true;
    }

    System.Collections.IEnumerator ShowEffectRoutine()
    {
        yield return new WaitForSeconds(effectDuration);
        if (effectObject) effectObject.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0f, 0.15f);
        Gizmos.DrawSphere(transform.position, radius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
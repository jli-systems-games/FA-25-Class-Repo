using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float fuseSeconds = 1.2f;
    public float radius = 10f;
    public float force = 1500f;
    public float maxDamage = 80f;
    public LayerMask damageMask = ~0;
    public GameObject vfx;
    public AudioClip sfx;

    void OnEnable()
    {
        StartCoroutine(Fuse());
    }

    IEnumerator Fuse()
    {
        yield return new WaitForSeconds(fuseSeconds);
        Explode();
    }

    void Explode()
    {
        if (vfx) Instantiate(vfx, transform.position, Quaternion.identity);
        if (sfx) AudioSource.PlayClipAtPoint(sfx, transform.position);
        Collider[] cols = Physics.OverlapSphere(transform.position, radius, damageMask);
        for (int i = 0; i < cols.Length; i++)
        {
            Rigidbody r = cols[i].attachedRigidbody;
            if (r != null) r.AddExplosionForce(force, transform.position, radius, 1.5f, ForceMode.Impulse);
            CarHealth h = cols[i].GetComponentInParent<CarHealth>();
            if (h != null)
            {
                float d = Vector3.Distance(transform.position, h.transform.position);
                float t = Mathf.Clamp01(1f - d / radius);
                h.Damage(maxDamage * t);
            }
        }
        Destroy(gameObject);
    }
}

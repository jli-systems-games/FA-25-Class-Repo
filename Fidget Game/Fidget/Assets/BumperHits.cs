using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BumperHitSFX : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource source;       // �Ƽ����� Car ��������
    public AudioClip clip;           // Ϊ��ʱ�� source.clip
    [Range(0f, 1f)] public float baseVolume = 0.9f;

    [Header("Trigger Filter")]
    public string requiredTag = "";  // ����=��ɸѡ������ "World" / "Prop"
    public float minRelativeSpeed = 1.5f; // ������С����ٶȣ�m/s��
    public float cooldown = 0.2f;    // ��ȴ���룩������һ������������

    [Header("Volume By Impact")]
    public float speedForFullVolume = 12f; // ����ٶȴﵽ��ֵʱ����=baseVolume

    Rigidbody carRB;
    float nextTime;

    void Reset()
    {
        // ȷ������ Trigger
        var c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    void Awake()
    {
        // �����ҳ��ĸ���
        carRB = GetComponentInParent<Rigidbody>();
        if (!source) source = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (Time.time < nextTime) return;
        if (!source) return;

        if (!string.IsNullOrEmpty(requiredTag) && !other.CompareTag(requiredTag))
            return;

        // ��������ٶ�
        float rel = GetRelativeSpeed(other.attachedRigidbody);

        if (rel < minRelativeSpeed) return;

        float k = Mathf.Clamp01(rel / Mathf.Max(0.01f, speedForFullVolume));
        float vol = baseVolume * k;

        var useClip = clip ? clip : source.clip;
        if (useClip) source.PlayOneShot(useClip, vol);

        nextTime = Time.time + cooldown;
    }

    float GetRelativeSpeed(Rigidbody otherRB)
    {
        Vector3 vCar = carRB ? carRB.linearVelocity : Vector3.zero;
        Vector3 vOther = (otherRB ? otherRB.linearVelocity : Vector3.zero);
        return (vCar - vOther).magnitude;
    }

    // ��ѡ����������ڡ�����������ײ���� Trigger����Ҳ�죬ȡ������ע�Ͳ��� Collider ȡ�� IsTrigger
    /*
    void OnCollisionEnter(Collision c)
    {
        if (Time.time < nextTime || !source) return;
        if (!string.IsNullOrEmpty(requiredTag) && !c.collider.CompareTag(requiredTag)) return;

        float rel = c.relativeVelocity.magnitude;
        if (rel < minRelativeSpeed) return;

        float k = Mathf.Clamp01(rel / Mathf.Max(0.01f, speedForFullVolume));
        float vol = baseVolume * k;

        var useClip = clip ? clip : source.clip;
        if (useClip) source.PlayOneShot(useClip, vol);
        nextTime = Time.time + cooldown;
    }
    */
}

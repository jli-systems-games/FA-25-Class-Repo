using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ElementUnit2D : MonoBehaviour
{
    [Header("Render & VFX")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform vfxMuzzle;
    [SerializeField] private ParticleSystem hitFlashPS;

    [Header("Audio (Optional)")]
    [SerializeField] private AudioSource audioSrc;

    public ElementData Data { get; private set; }
    public float CurrentHealth { get; private set; }
    public bool IsAlive => CurrentHealth > 0f;

    Color baseColor;

    private void Reset()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(ElementData data)
    {
        Data = data;
        CurrentHealth = data.maxHealth;

        if (spriteRenderer)
        {
            if (data.bodySprite) spriteRenderer.sprite = data.bodySprite;
            baseColor = data.color;
            spriteRenderer.color = baseColor;
            spriteRenderer.sortingLayerName = "Units";
            spriteRenderer.sortingOrder = 0;
        }

        if (!audioSrc) audioSrc = GetComponent<AudioSource>();
        name = $"Unit_{data.displayName}";
    }

    public void PlayAttackVFX()
    {
        if (Data != null && Data.attackVFXPrefab != null)
        {
            var spawnPos = vfxMuzzle ? vfxMuzzle.position : transform.position;
            var vfx = Instantiate(Data.attackVFXPrefab, spawnPos, Quaternion.identity);
            var r = vfx.GetComponent<Renderer>();
            var psr = vfx.GetComponent<ParticleSystemRenderer>();
            if (psr) psr.sortingLayerName = "VFX";
            if (r) r.sortingLayerName = "VFX";
            Destroy(vfx, 2f);
        }
    }

    public void TakeDamage(float dmg)
    {
        if (!IsAlive) return;
        CurrentHealth = Mathf.Max(0f, CurrentHealth - dmg);
        if (audioSrc && Data && Data.hitSfx) audioSrc.PlayOneShot(Data.hitSfx);
        if (hitFlashPS) hitFlashPS.Play();
        StopAllCoroutines();
        StartCoroutine(HitFlashCo());
    }

    IEnumerator HitFlashCo()
    {
        float t = 0f;
        while (t < 0.06f)
        {
            t += Time.deltaTime;
            if (spriteRenderer) spriteRenderer.color = Color.Lerp(baseColor, Color.white, t / 0.06f);
            yield return null;
        }
        t = 0f;
        while (t < 0.1f)
        {
            t += Time.deltaTime;
            if (spriteRenderer) spriteRenderer.color = Color.Lerp(Color.white, baseColor, t / 0.1f);
            yield return null;
        }
        if (spriteRenderer) spriteRenderer.color = baseColor;
    }
}

using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

[System.Serializable]
public class LootEntry
{
    public GameObject prefab;
    [Range(0f, 1f)] public float weight = 1f;
}

public class PigDig : MonoBehaviour
{
    public Transform snoutPoint;
    public LayerMask diggableMask;
    public float digDistance = 1.4f;

    public KeyCode digKey = KeyCode.E;
    public bool mouseAlsoDig = true;

    public ParticleSystem dirtFXPrefab;
    public int emitRate = 160;
    public float speedMult = 1.6f;
    public float sizeMult = 1.2f;

    public bool attachToSnout = true;
    public Vector3 snoutLocalOffset = new Vector3(0f, 0.25f, 0.25f);
    public float normalOffset = 0.25f;
    public Vector3 worldOffset = Vector3.zero;

    public AudioSource audioSource;
    public AudioClip loopDigSfx;
    public float sfxFade = 10f;

    public List<LootEntry> lootTable;
    public float firstLootDelay = 0.15f;
    public float lootInterval = 0.6f;

    public float noiseRadius = 12f;
    public float noiseInterval = 0.35f;

    ParticleSystem fxInst;
    float lootT;
    float noiseT;
    bool wasHolding;
    float sfxVol;

    void Update()
    {
        bool holding = Input.GetKey(digKey) || (mouseAlsoDig && Input.GetMouseButton(0));
        float dt = Time.deltaTime;

        if (!snoutPoint) { StopFX(); return; }

        if (holding && Physics.Raycast(snoutPoint.position, Vector3.down, out RaycastHit hit, digDistance, diggableMask))
        {
            if (!wasHolding)
            {
                lootT = firstLootDelay;
                noiseT = 0f;
                StartFX(GetFXPos(hit.point, hit.normal), GetFXRot(hit.normal));
            }
            UpdateFX(GetFXPos(hit.point, hit.normal), GetFXRot(hit.normal), dt);

            lootT -= dt;
            if (lootT <= 0f)
            {
                SpawnLoot(hit.point + Vector3.up * 0.05f);
                lootT = lootInterval;
            }

            noiseT -= dt;
            if (noiseT <= 0f)
            {
                NoiseSystem.Broadcast(hit.point, noiseRadius);
                noiseT = noiseInterval;
            }
        }
        else
        {
            StopFX();
            lootT = 0f;
            noiseT = 0f;
        }

        wasHolding = holding;
    }

    Vector3 GetFXPos(Vector3 hitPoint, Vector3 normal)
    {
        if (attachToSnout && snoutPoint) return snoutPoint.TransformPoint(snoutLocalOffset);
        return hitPoint + normal * normalOffset + worldOffset;
    }

    Quaternion GetFXRot(Vector3 normal)
    {
        if (attachToSnout && snoutPoint) return Quaternion.LookRotation(snoutPoint.forward, Vector3.up);
        return Quaternion.LookRotation(normal, Vector3.forward);
    }

    void StartFX(Vector3 pos, Quaternion rot)
    {
        if (!fxInst && dirtFXPrefab)
        {
            fxInst = Instantiate(dirtFXPrefab, pos, rot);
            var main = fxInst.main;
            main.loop = false;
            main.startSpeedMultiplier *= speedMult;
            main.startSizeMultiplier *= sizeMult;
            fxInst.Play();
        }
        if (audioSource && loopDigSfx)
        {
            if (!audioSource.isPlaying) { audioSource.clip = loopDigSfx; audioSource.loop = true; audioSource.volume = 0f; audioSource.Play(); }
        }
    }

    void UpdateFX(Vector3 pos, Quaternion rot, float dt)
    {
        if (fxInst)
        {
            fxInst.transform.SetPositionAndRotation(pos, rot);
            int count = Mathf.RoundToInt(emitRate * dt);
            if (count > 0) fxInst.Emit(count);
        }
        if (audioSource && audioSource.isPlaying)
        {
            sfxVol = Mathf.MoveTowards(sfxVol, 1f, sfxFade * dt);
            audioSource.volume = sfxVol;
        }
    }

    void StopFX()
    {
        if (fxInst)
        {
            Destroy(fxInst.gameObject, 1.5f);
            fxInst = null;
        }
        if (audioSource && audioSource.isPlaying)
        {
            sfxVol = Mathf.MoveTowards(sfxVol, 0f, sfxFade * Time.deltaTime * 2f);
            audioSource.volume = sfxVol;
            if (sfxVol <= 0.001f) { audioSource.Stop(); audioSource.clip = null; }
        }
    }

    void SpawnLoot(Vector3 pos)
    {
        var drop = PickLoot();
        if (drop) Instantiate(drop, pos, Quaternion.identity);
    }

    GameObject PickLoot()
    {
        if (lootTable == null || lootTable.Count == 0) return null;
        float total = 0f; foreach (var e in lootTable) total += Mathf.Max(0f, e.weight);
        if (total <= 0f) return null;
        float r = Random.value * total, acc = 0f;
        foreach (var e in lootTable) { acc += Mathf.Max(0f, e.weight); if (r <= acc) return e.prefab; }
        return lootTable[lootTable.Count - 1].prefab;
    }

    void OnDrawGizmosSelected()
    {
        if (!snoutPoint) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(snoutPoint.position, snoutPoint.position + Vector3.down * digDistance);
    }
}

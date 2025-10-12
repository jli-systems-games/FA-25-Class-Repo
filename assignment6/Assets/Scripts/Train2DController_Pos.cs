using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class Train2DController_Pos : MonoBehaviour
{
    [Header("Path (world positions)")]
    public List<Vector3> pathPositions = new List<Vector3>();

    [Header("Tap Throttle (SPACE)")]
    [Tooltip("Each SPACE press adds this much speed instantly (units/s).")]
    public float tapImpulse = 1.25f;
    [Tooltip("Natural drag pulling speed down (units/s²).")]
    public float dragPerSec = 1.15f;
    [Tooltip("Absolute speed cap (units/s).")]
    public float maxSpeed = 8f;

    [Header("Target Speed Window (stable zone)")]
    public float stableMin = 1.8f;
    public float stableMax = 4.8f;

    [Header("Stability (0~1) base")]
    [Tooltip("Recovery per second when inside the window.")]
    public float stabilityRecoverPerSec = 0.50f;
    [Tooltip("Base drain per second when outside the window (before scaling).")]
    public float stabilityDrainPerSec = 0.32f;
    [Tooltip("Extra drain proportional to distance from the window.")]
    public float stabilityDistanceFactor = 0.08f;

    [Header("Assist (strong rescue near zero)")]
    [Tooltip("Below this stability, taps pause drains and grant extra gain.")]
    public float assistBand = 0.35f;    
    [Tooltip("During assist, each tap adds this much stability immediately.")]
    public float assistTapGain = 0.14f;      
    [Tooltip("After each tap in assist band, drains are PAUSED for this long.")]
    public float assistTapPause = 0.45f;   
    [Tooltip("When stability < assistBand AND inZone, bump recover by this factor.")]
    public float assistZoneRecoverBoost = 1.6f; 
    [Tooltip("Cap the total drain per second when stability < assistBand (safety).")]
    public float assistMaxDrainPerSec = 0.24f;

    [Header("Stall (grace on zero stability)")]
    public bool useStallGrace = true;
    public float stallGraceSeconds = 1.50f;
    public float stallDragPerSec = 5.00f;
    [Tooltip("Stability gained per tap during stall.")]
    public float stallTapStabilityGain = 0.12f;
    [Tooltip("Stability needed to leave stall.")]
    public float stallRecoverThreshold = 0.15f;

    [Header("Movement")]
    public float arriveThreshold = 0.08f;
    public bool isAlive = true;

    [Header("Visual heading")]
    [Tooltip("Sprite faces RIGHT -> 0 | UP -> -90 | DOWN -> 90 | LEFT -> 180")]
    public float spriteAngleOffset = 270f;

    public float speed { get; private set; } = 0f;
    public float stability01 { get; private set; } = 1f;

    private int _index = 0;
    private Rigidbody2D _rb;

    float _stallTimer = 0f;
    bool _stalled = false;

    [Header("Tap feel (optional combo)")]
    public float comboWindow = 0.28f;
    public float comboBonusPerTap = 0.12f;
    public int comboMax = 4;
    float _lastTapTime = -999f;
    int _combo = 0;

    float _tapGraceTimer = 0f;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.isKinematic = true; 
        var col = GetComponent<Collider2D>();
        if (col && col.isTrigger)
            Debug.LogWarning("[Train] Train's Collider2D should NOT be trigger.");
    }

    void Update()
    {
        if (!isAlive || pathPositions == null || pathPositions.Count == 0) return;

        if (_stalled)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                stability01 = Mathf.Clamp01(stability01 + stallTapStabilityGain);
                ApplyTapSpeedImpulse(0.6f);
                _tapGraceTimer = assistTapPause; 
            }

            if (speed > 0f)
            {
                speed -= stallDragPerSec * Time.deltaTime;
                if (speed < 0f) speed = 0f;
            }

            _stallTimer -= Time.deltaTime;

            DrainOrRecoverStability(inZone: false, applyingAssist: true);

            GameManager2D.I?.SetSpeedAndStability(speed, stability01, false);

            if (stability01 >= stallRecoverThreshold)
            {
                _stalled = false;
                _stallTimer = 0f;
            }
            else if (_stallTimer <= 0f)
            {
                isAlive = false;
                GameManager2D.I?.GameOver();
                return;
            }
            else
            {
                return; 
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ApplyTapSpeedImpulse(1f);

            if (stability01 < assistBand)
            {
                stability01 = Mathf.Clamp01(stability01 + assistTapGain);
                _tapGraceTimer = assistTapPause;
            }
        }

        if (speed > 0f)
        {
            speed -= dragPerSec * Time.deltaTime;
            if (speed < 0f) speed = 0f;
        }

        if (_index >= pathPositions.Count)
        {
            isAlive = false;
            GameManager2D.I?.Win();
            return;
        }

        Vector3 target = pathPositions[_index];
        Vector2 pos = transform.position;
        Vector2 toTarget = (Vector2)target - pos;

        if (toTarget.magnitude <= arriveThreshold)
        {
            _index++;
            if (_index >= 8) TrimPassedPoints();
        }
        else
        {
            Vector2 step = toTarget.normalized * speed * Time.deltaTime;
            transform.position = pos + step;

            if (step.sqrMagnitude > 1e-6f)
            {
                float ang = Mathf.Atan2(step.y, step.x) * Mathf.Rad2Deg + spriteAngleOffset;
                transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    Quaternion.Euler(0, 0, ang),
                    0.25f
                );
            }
        }

        bool inZone = speed >= stableMin && speed <= stableMax;
        DrainOrRecoverStability(inZone, applyingAssist: false);

        GameManager2D.I?.SetSpeedAndStability(speed, stability01, inZone);

        if (stability01 <= 0f)
        {
            stability01 = 0f;
            if (useStallGrace)
            {
                _stalled = true;
                _stallTimer = stallGraceSeconds;
                return;
            }
            else
            {
                isAlive = false;
                GameManager2D.I?.GameOver();
                return;
            }
        }

        if (_tapGraceTimer > 0f) _tapGraceTimer -= Time.deltaTime;
    }

    void DrainOrRecoverStability(bool inZone, bool applyingAssist)
    {
        float dt = Time.deltaTime;

        bool lowBand = stability01 < assistBand;
        bool pauseDrains = lowBand && (_tapGraceTimer > 0f);

        if (inZone)
        {
            float rec = stabilityRecoverPerSec;

            if (lowBand) rec *= assistZoneRecoverBoost;
            stability01 = Mathf.Clamp01(stability01 + rec * dt);
            return;
        }

        if (pauseDrains && (lowBand || applyingAssist))
        {
            return;
        }

        float dist =
            (speed < stableMin) ? (stableMin - speed) :
            (speed > stableMax) ? (speed - stableMax) : 0f;

        float extra = dist * stabilityDistanceFactor;

        float totalDrainPerSec = stabilityDrainPerSec + extra;
        if (lowBand)
            totalDrainPerSec = Mathf.Min(totalDrainPerSec, assistMaxDrainPerSec);

        stability01 = Mathf.Clamp01(stability01 - totalDrainPerSec * dt);
    }

    void ApplyTapSpeedImpulse(float scale)
    {
        if (Time.time - _lastTapTime <= comboWindow) _combo = Mathf.Min(_combo + 1, comboMax);
        else _combo = 0;
        _lastTapTime = Time.time;

        float bonus = 1f + _combo * comboBonusPerTap; 
        speed = Mathf.Min(maxSpeed, speed + tapImpulse * bonus * scale);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isAlive) return;

        if (other.CompareTag("Obstacle") || other.GetComponentInParent<Obstacle2D>() != null)
        {
            Debug.Log($"[Train] Hit obstacle via {other.name}");
            isAlive = false;
            GameManager2D.I?.GameOver();
        }
    }

    public void SetNewPathPositions(List<Vector3> pts, int startIndex = 0)
    {
        pathPositions = (pts != null) ? new List<Vector3>(pts) : new List<Vector3>();
        _index = Mathf.Clamp(startIndex, 0, Mathf.Max(0, pathPositions.Count - 1));
    }

    public void AppendPositions(List<Vector3> more)
    {
        if (more == null || more.Count == 0) return;
        Vector3 cur = CurrentTargetPos();
        if (pathPositions.Count == 0) pathPositions.Add(cur);
        foreach (var p in more)
            if ((p - cur).sqrMagnitude > 1e-6f) pathPositions.Add(p);
    }

    public Vector3 CurrentTargetPos()
    {
        if (_index >= 0 && _index < pathPositions.Count) return pathPositions[_index];
        if (pathPositions.Count > 0) return pathPositions[pathPositions.Count - 1];
        return transform.position;
    }

    public void TrimPassedPoints()
    {
        if (_index <= 0) return;
        pathPositions.RemoveRange(0, _index);
        _index = 0;
    }
}


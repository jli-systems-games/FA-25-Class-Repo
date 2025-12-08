using System.Collections;
using UnityEngine;
using MoreMountains.TopDownEngine;
using Unity.Cinemachine;   // 引用 Cinemachine 3 (如果是旧版请改回 Cinemachine)

[RequireComponent(typeof(LineRenderer))]
public class CharacterRockThrower : MonoBehaviour
{
    [Header("输入设置")]
    public KeyCode ThrowKey = KeyCode.E;

    [Header("投掷参数")]
    [Tooltip("如果不填，游戏开始时会自动寻找名为 'WeaponAttachment' 的子物体")]
    public Transform HandTransform;
    public float MinForce = 10f;
    public float MaxForce = 25f;
    public float MaxChargeTime = 1.0f;
    public float ThrowUpwardForce = 3f;

    [Header("蓄力手感 (Juice)")]
    [Range(0.1f, 1f)]
    public float ChargeSpeedMultiplier = 0.3f;

    [Tooltip("每次光圈出现的间隔时间")]
    public float PulseInterval = 0.4f;

    [Tooltip("光圈收缩的动画时长")]
    public float RingDuration = 0.8f;

    public GameObject ChargeRingPrefab;

    [Header("相机震动 (Cinemachine)")]
    [Tooltip("可以不拖，脚本会自动从 Brain 里找当前正在用的 CinemachineCamera")]
    public CinemachineCamera VCam; // 如果报错找不到类型，说明你用的是旧版Cinemachine，请改为 CinemachineVirtualCamera
    public float ShakeIntensity = 2f;
    public float ShakeDuration = 0.1f;
    public float ShakeFrequency = 4f;   // 蓄力 pulse 时抖动频率

    const float BaseFrequency = 2f;      // 非震动状态的基础频率

    [Header("视觉效果")]
    public float LineWidth = 0.5f;
    public int LinePoints = 20;

    private Character _character;
    private CharacterMovement _characterMovement;
    private CharacterRun _characterRun;
    private LineRenderer _lineRenderer;
    private float _chargeTimer;
    private bool _isCharging;
    private GameObject _heldRock;
    private Collider _playerCollider;

    private float _initialWalkSpeed;
    private float _initialRunSpeed;
    private float _pulseTimer;

    private Coroutine _shakeCoroutine;
    private CinemachineBasicMultiChannelPerlin _perlin;
    private bool _cinemachineInitialized = false;

    void Start()
    {
        _character = GetComponent<Character>();
        _playerCollider = GetComponent<Collider>();

        _characterMovement = GetComponent<CharacterMovement>();
        _characterRun = GetComponent<CharacterRun>();

        if (_characterMovement != null)
        {
            _initialWalkSpeed = _characterMovement.WalkSpeed;
            _initialRunSpeed = _characterRun != null ? _characterRun.RunSpeed : _initialWalkSpeed;
        }

        SetupLineRenderer();
        CheckAndFixHandTransform();
    }

    void Update()
    {
        // 每帧先尝试初始化一次 Cinemachine（直到成功为止）
        TryInitCinemachine();

        if (HandTransform == null)
        {
            CheckAndFixHandTransform();
            if (HandTransform == null) return;
        }

        FindAndFixRock();

        if (Input.GetKeyDown(ThrowKey))
        {
            if (_heldRock == null) return;
            StartCharging();
        }

        if (_heldRock != null && _isCharging)
        {
            if (Input.GetKey(ThrowKey))
            {
                _chargeTimer += Time.deltaTime;

                if (_chargeTimer >= MaxChargeTime)
                {
                    _chargeTimer = MaxChargeTime;
                }
                else
                {
                    HandleChargePulse();
                }

                ApplySlowDown();

                float currentForce = Mathf.Lerp(MinForce, MaxForce, _chargeTimer / MaxChargeTime);
                DrawTrajectory(currentForce);
            }

            if (Input.GetKeyUp(ThrowKey))
            {
                StopCharging();
                ThrowRock();
            }
        }
        else if (_isCharging)
        {
            StopCharging();
        }
    }

    // —— 只要没成功找到 active 相机上的 Perlin，就持续尝试 —— 
    void TryInitCinemachine()
    {
        if (_cinemachineInitialized) return;

        // 1. 尝试从 Camera.main 上的 Brain 获取当前正在用的虚拟相机
        if (VCam == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                var brain = mainCam.GetComponent<CinemachineBrain>();
                // 注意：Unity 6 / Cinemachine 3 使用 CinemachineCamera
                // 如果你是旧版 (Cinemachine 2.x)，这里请改为 CinemachineVirtualCamera
                if (brain != null && brain.ActiveVirtualCamera is CinemachineCamera camFromBrain)
                {
                    VCam = camFromBrain;
                }
            }
        }

        // 2. 兜底：场景里随便找一个 CinemachineCamera
        if (VCam == null)
        {
            VCam = Object.FindFirstObjectByType<CinemachineCamera>();
        }

        if (VCam != null)
        {
            _perlin = VCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
            if (_perlin == null)
            {
                return;
            }

            // 一旦找到，就强制把初始状态设成“静止”
            _perlin.AmplitudeGain = 0f;
            _perlin.FrequencyGain = BaseFrequency;

            _cinemachineInitialized = true;
        }
    }

    Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
            Transform result = FindDeepChild(child, name);
            if (result != null) return result;
        }
        return null;
    }

    void CheckAndFixHandTransform()
    {
        if (HandTransform == null)
        {
            Transform foundAttachment = FindDeepChild(transform, "WeaponAttachment");
            if (foundAttachment != null)
            {
                HandTransform = foundAttachment;
            }
        }
    }

    void SetupLineRenderer()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.startWidth = LineWidth;
        _lineRenderer.endWidth = LineWidth;
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.startColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        _lineRenderer.endColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        _lineRenderer.positionCount = LinePoints;
        _lineRenderer.enabled = false;
    }

    void StartCharging()
    {
        _isCharging = true;
        _chargeTimer = 0f;
        _pulseTimer = PulseInterval;
        if (_lineRenderer != null) _lineRenderer.enabled = true;
    }

    void StopCharging()
    {
        _isCharging = false;
        if (_lineRenderer != null) _lineRenderer.enabled = false;

        if (_characterMovement != null)
        {
            _characterMovement.WalkSpeed = _initialWalkSpeed;
            if (_characterRun != null) _characterRun.RunSpeed = _initialRunSpeed;
            _characterMovement.MovementSpeed = _initialWalkSpeed;
        }
    }

    void ApplySlowDown()
    {
        if (_characterMovement == null) return;

        float targetWalk = _initialWalkSpeed * ChargeSpeedMultiplier;
        float targetRun = _initialRunSpeed * ChargeSpeedMultiplier;

        _characterMovement.WalkSpeed = targetWalk;
        if (_characterRun != null) _characterRun.RunSpeed = targetRun;

        if (_character.MovementState.CurrentState == CharacterStates.MovementStates.Running)
        {
            _characterMovement.MovementSpeed = targetRun;
        }
        else
        {
            _characterMovement.MovementSpeed = targetWalk;
        }
    }

    void HandleChargePulse()
    {
        _pulseTimer += Time.deltaTime;
        if (_pulseTimer >= PulseInterval)
        {
            _pulseTimer = 0f;
            TriggerPulseEffect();
        }
    }

    void TriggerPulseEffect()
    {
        // —— 相机抖动 —— 
        if (_perlin != null)
        {
            if (_shakeCoroutine != null) StopCoroutine(_shakeCoroutine);
            _shakeCoroutine = StartCoroutine(ShakeCinemachine(ShakeDuration, ShakeIntensity, ShakeFrequency));
        }

        // —— 光圈特效 —— 
        GameObject ring = null;
        if (ChargeRingPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.1f;
            ring = Instantiate(ChargeRingPrefab, spawnPos, Quaternion.Euler(90, 0, 0));
        }
        else
        {
            ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            if (ring.GetComponent<Collider>() != null) Destroy(ring.GetComponent<Collider>());
            ring.name = "AutoPulseRing";
            ring.transform.position = transform.position + Vector3.up * 0.05f;
            ring.transform.localScale = new Vector3(3f, 0.01f, 3f);

            Renderer r = ring.GetComponent<Renderer>();
            if (r != null)
            {
                r.material = new Material(Shader.Find("Standard"));
                r.material.SetFloat("_Mode", 3);
                r.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                r.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                r.material.SetInt("_ZWrite", 0);
                r.material.DisableKeyword("_ALPHATEST_ON");
                r.material.EnableKeyword("_ALPHABLEND_ON");
                r.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                r.material.renderQueue = 3000;
                r.material.color = new Color(1f, 1f, 1f, 0.3f);
            }
        }

        if (ring != null)
        {
            StartCoroutine(ShrinkRingRoutine(ring, RingDuration));
        }
    }

    IEnumerator ShakeCinemachine(float duration, float amplitude, float frequency)
    {
        if (_perlin == null) yield break;

        _perlin.AmplitudeGain = amplitude;
        _perlin.FrequencyGain = frequency;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        _perlin.AmplitudeGain = 0f;
        _perlin.FrequencyGain = BaseFrequency;
    }

    IEnumerator ShrinkRingRoutine(GameObject ring, float duration)
    {
        float timer = 0f;
        Vector3 startScale = ring.transform.localScale;
        Vector3 endScale = (ChargeRingPrefab == null) ? new Vector3(0f, 0.01f, 0f) : Vector3.zero;

        while (timer < duration)
        {
            if (ring == null) yield break;
            timer += Time.deltaTime;
            ring.transform.localScale = Vector3.Lerp(startScale, endScale, timer / duration);
            yield return null;
        }

        if (ring != null) Destroy(ring);
    }

    // ★★★ 这里是关键修改：整堆石头一起扔，不再钻进子物体 ★★★
    void FindAndFixRock()
    {
        // 如果已经握着一整堆了，就不用每帧重新搞
        if (_heldRock != null) return;

        if (HandTransform == null || HandTransform.childCount == 0)
        {
            _heldRock = null;
            return;
        }

        // WeaponAttachment 下面的第一个子物体，就是“整堆石头”的根 (item_BigStone 1)
        Transform rockRoot = HandTransform.GetChild(0);
        _heldRock = rockRoot.gameObject;

        // 根上必须有 Rigidbody
        Rigidbody rootRb = _heldRock.GetComponent<Rigidbody>();
        if (rootRb == null)
        {
            rootRb = _heldRock.AddComponent<Rigidbody>();
        }

        // 拿在手上的时候，先禁用物理
        rootRb.isKinematic = true;
        rootRb.useGravity = false;
        rootRb.constraints = RigidbodyConstraints.FreezeRotation;

        // 子物体不允许有刚体，防止分裂
        Rigidbody[] allRbs = _heldRock.GetComponentsInChildren<Rigidbody>(true);
        foreach (var rb in allRbs)
        {
            if (rb.gameObject != _heldRock)
            {
                Destroy(rb);
            }
        }

        // 至少保证根有一个 Collider（用于 IgnoreCollision）
        Collider rootCol = _heldRock.GetComponent<Collider>();
        if (rootCol == null)
        {
            rootCol = _heldRock.AddComponent<BoxCollider>();
        }
    }

    Vector3 GetThrowDirection()
    {
        if (_character != null && _character.CharacterModel != null)
        {
            return _character.CharacterModel.transform.forward;
        }
        return transform.forward;
    }

    void DrawTrajectory(float force)
    {
        if (HandTransform == null) return;

        Vector3 origin = HandTransform.position;
        Vector3 forward = GetThrowDirection();
        Vector3 velocity = (forward * force) + (Vector3.up * ThrowUpwardForce);

        for (int i = 0; i < LinePoints; i++)
        {
            float time = i * 0.1f;
            Vector3 point = origin + velocity * time + 0.5f * Physics.gravity * time * time;
            _lineRenderer.SetPosition(i, point);
        }
    }

    void ThrowRock()
    {
        if (_heldRock == null) return;

        float finalForce = Mathf.Lerp(MinForce, MaxForce, _chargeTimer / MaxChargeTime);
        Vector3 throwDir = GetThrowDirection();

        _heldRock.transform.SetParent(null);

        // ---【核心修复】对整堆石头做一次“绝育”：只让根参与物理 ---
        OptimizeRockStructure(_heldRock);
        // ----------------------------------------------------

        Rigidbody rb = _heldRock.GetComponent<Rigidbody>();
        Collider rockCollider = _heldRock.GetComponent<Collider>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;

            if (_playerCollider != null && rockCollider != null)
            {
                Physics.IgnoreCollision(_playerCollider, rockCollider, true);
                StartCoroutine(ResetCollisionDelay(_playerCollider, rockCollider, 1f));
            }

#if UNITY_6000_0_OR_NEWER
            rb.linearVelocity = Vector3.zero;
#else
            rb.velocity = Vector3.zero;
#endif

            rb.AddForce(throwDir * finalForce + Vector3.up * ThrowUpwardForce, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);
        }

        var rockLogic = _heldRock.GetComponent<RockImpactLogic>();
        if (rockLogic != null)
        {
            rockLogic.ActivateAttack(this.gameObject);
        }

        _heldRock = null;
    }

    // —— 把内部所有“会搞事的东西”全部关掉，只保留根的碰撞和刚体 —— 
    void OptimizeRockStructure(GameObject rock)
    {
        // 1. 删除所有 Joint（关节）
        Joint[] joints = rock.GetComponentsInChildren<Joint>(true);
        foreach (var j in joints) Destroy(j);

        // 2. 删除所有 Health（避免 breakable 分裂）
        Health[] healths = rock.GetComponentsInChildren<Health>(true);
        foreach (var h in healths) Destroy(h);

        // 3. 根以外的 Rigidbody 全部干掉
        Rigidbody[] rbs = rock.GetComponentsInChildren<Rigidbody>(true);
        foreach (var r in rbs)
        {
            if (r.gameObject != rock)
            {
                r.isKinematic = true;
                Destroy(r);
            }
        }

        // 4. 根以外的 Collider 全部关掉，只用根的体积来撞
        Collider[] colliders = rock.GetComponentsInChildren<Collider>(true);
        foreach (var col in colliders)
        {
            if (col.gameObject != rock)
            {
                col.enabled = false;
            }
        }

        // 5. 把所有“隐藏碎片”直接删掉（有些 breakable 会提前藏碎片）
        foreach (Transform child in rock.transform)
        {
            if (!child.gameObject.activeSelf)
            {
                Destroy(child.gameObject);
            }
        }
    }

    IEnumerator ResetCollisionDelay(Collider p, Collider r, float delay)
    {
        Physics.IgnoreCollision(p, r, true);
        yield return new WaitForSeconds(delay);
        if (p != null && r != null) Physics.IgnoreCollision(p, r, false);
    }
}

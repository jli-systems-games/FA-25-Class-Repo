using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.TopDownEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(LineRenderer))]
public class CharacterRockThrower : MonoBehaviour
{
    [Header("输入设置")]
    public KeyCode ThrowKey = KeyCode.E;

    [Header("投掷参数")]
    public Transform HandTransform;
    public float BigRockSpeed = 10f;
    public float SmallRockSpeed = 20f;
    public float MaxChargeTime = 1.0f;
    public float MaxRange = 30f;
    public float SmallRockMassThreshold = 3f;
    public int MaxBounces = 3;

    [Header("蓄力手感")]
    public float PulseInterval = 0.4f;
    public float RingDuration = 0.8f;
    public GameObject ChargeRingPrefab;

    [Header("相机震动")]
    public CinemachineCamera VCam;
    public float ShakeIntensity = 2f;
    public float ShakeDuration = 0.1f;
    public float ShakeFrequency = 4f;
    const float BaseFrequency = 2f;

    [Header("视觉效果")]
    public float LineWidth = 0.08f;
    public int SegmentsPerBounce = 15;
    public Color LineColorStart = new Color(0.8f, 0.8f, 0.8f, 0.8f);
    public Color LineColorBounce1 = new Color(1f, 0.8f, 0.2f, 0.7f);
    public Color LineColorBounce2 = new Color(1f, 0.5f, 0.2f, 0.6f);
    public Color LineColorEnd = new Color(1f, 0.2f, 0.2f, 0.5f);
    public bool ShowBounceIndicators = true;
    public float BounceIndicatorSize = 0.3f;

    [Header("调试")]
    public bool EnableDebugLogs = false;

    private Character _character;
    private CharacterMovement _characterMovement;
    private CharacterRun _characterRun;
    private LineRenderer _lineRenderer;
    private float _chargeTimer;
    private bool _isCharging;
    private GameObject _heldRock;

    // 【修改】记录初始速度
    private float _defaultWalkSpeed;
    private float _defaultRunSpeed;

    private float _pulseTimer;
    private Coroutine _shakeCoroutine;
    private CinemachineBasicMultiChannelPerlin _perlin;
    private bool _cinemachineInitialized = false;
    private List<GameObject> _bounceIndicators = new List<GameObject>();
    private bool _hasValidRock = false;
    private bool _isSmallRock = false;

    void Start()
    {
        InitializeComponents();
        SetupLineRenderer();
        CheckAndFixHandTransform();
    }

    void InitializeComponents()
    {
        _character = GetComponent<Character>();
        _characterMovement = GetComponent<CharacterMovement>();
        _characterRun = GetComponent<CharacterRun>();

        // 【关键】记录满速状态
        if (_characterMovement != null) _defaultWalkSpeed = _characterMovement.WalkSpeed;
        if (_characterRun != null) _defaultRunSpeed = _characterRun.RunSpeed;
    }

    void Update()
    {
        TryInitCinemachine();

        if (HandTransform == null)
        {
            CheckAndFixHandTransform();
            if (HandTransform == null) { _hasValidRock = false; return; }
        }

        FindAndFixRock();

        if (Input.GetKeyDown(ThrowKey))
        {
            if (_heldRock == null)
            {
                if (EnableDebugLogs) Debug.LogWarning("[RockThrower] 没有可投掷的石头！");
                return;
            }
            StartCharging();
        }

        if (_heldRock != null && _isCharging)
        {
            if (Input.GetKey(ThrowKey)) UpdateCharging();
            if (Input.GetKeyUp(ThrowKey))
            {
                _isCharging = false;
                if (_lineRenderer != null) _lineRenderer.enabled = false;
                ClearBounceIndicators();
                ThrowRock(); // 发射
            }
        }
        else if (_isCharging)
        {
            StopCharging();
        }
    }

    // --- 辅助函数保持简洁 ---
    void TryInitCinemachine()
    {
        if (_cinemachineInitialized) return;
        if (VCam == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                var brain = mainCam.GetComponent<CinemachineBrain>();
                if (brain != null && brain.ActiveVirtualCamera is CinemachineCamera camFromBrain) VCam = camFromBrain;
            }
        }
        if (VCam == null) VCam = Object.FindFirstObjectByType<CinemachineCamera>();
        if (VCam != null)
        {
            _perlin = VCam.GetComponent<CinemachineBasicMultiChannelPerlin>();
            if (_perlin == null) return;
            _perlin.AmplitudeGain = 0f; _perlin.FrequencyGain = BaseFrequency;
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
            if (foundAttachment != null) HandTransform = foundAttachment;
        }
    }

    void SetupLineRenderer()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.startWidth = LineWidth; _lineRenderer.endWidth = LineWidth;
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.startColor = LineColorStart; _lineRenderer.endColor = LineColorEnd;
        _lineRenderer.enabled = false;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(LineColorStart, 0.0f), new GradientColorKey(LineColorBounce1, 0.33f), new GradientColorKey(LineColorBounce2, 0.66f), new GradientColorKey(LineColorEnd, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.8f, 0.0f), new GradientAlphaKey(0.6f, 1.0f) }
        );
        _lineRenderer.colorGradient = gradient;
    }

    void StartCharging()
    {
        _isCharging = true; _chargeTimer = 0f; _pulseTimer = PulseInterval;
        if (_lineRenderer != null) _lineRenderer.enabled = true;
    }

    void UpdateCharging()
    {
        _chargeTimer += Time.deltaTime;
        if (_chargeTimer >= MaxChargeTime) _chargeTimer = MaxChargeTime;
        else HandleChargePulse();
        DrawTrajectory();
    }

    void StopCharging()
    {
        _isCharging = false;
        if (_lineRenderer != null) _lineRenderer.enabled = false;
        ClearBounceIndicators();
    }

    void HandleChargePulse()
    {
        _pulseTimer += Time.deltaTime;
        if (_pulseTimer >= PulseInterval) { _pulseTimer = 0f; TriggerPulseEffect(); }
    }

    void TriggerPulseEffect()
    {
        if (_perlin != null) { if (_shakeCoroutine != null) StopCoroutine(_shakeCoroutine); _shakeCoroutine = StartCoroutine(ShakeCinemachine(ShakeDuration, ShakeIntensity, ShakeFrequency)); }
        GameObject ring = CreateChargeRing();
        if (ring != null) StartCoroutine(ShrinkRingRoutine(ring, RingDuration));
    }

    GameObject CreateChargeRing()
    {
        GameObject ring = null;
        if (ChargeRingPrefab != null) { Vector3 spawnPos = transform.position + Vector3.up * 0.1f; ring = Instantiate(ChargeRingPrefab, spawnPos, Quaternion.Euler(90, 0, 0)); }
        else { ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder); Destroy(ring.GetComponent<Collider>()); ring.transform.position = transform.position + Vector3.up * 0.05f; ring.transform.localScale = new Vector3(3f, 0.01f, 3f); }
        return ring;
    }

    IEnumerator ShakeCinemachine(float duration, float amplitude, float frequency) { if (_perlin == null) yield break; _perlin.AmplitudeGain = amplitude; _perlin.FrequencyGain = frequency; float timer = 0f; while (timer < duration) { timer += Time.deltaTime; yield return null; } _perlin.AmplitudeGain = 0f; _perlin.FrequencyGain = BaseFrequency; }
    IEnumerator ShrinkRingRoutine(GameObject ring, float duration) { if (ring == null) yield break; float timer = 0f; Vector3 startScale = ring.transform.localScale; Vector3 endScale = (ChargeRingPrefab == null) ? new Vector3(0f, 0.01f, 0f) : Vector3.zero; while (timer < duration) { if (ring == null) yield break; timer += Time.deltaTime; ring.transform.localScale = Vector3.Lerp(startScale, endScale, timer / duration); yield return null; } if (ring != null) Destroy(ring); }

    void FindAndFixRock()
    {
        if (_heldRock != null) { _hasValidRock = true; return; }
        if (HandTransform == null || HandTransform.childCount == 0) { _heldRock = null; _hasValidRock = false; return; }
        Transform rockRoot = HandTransform.GetChild(0);
        _heldRock = rockRoot.gameObject;
        Rigidbody rootRb = _heldRock.GetComponent<Rigidbody>();
        if (rootRb == null) rootRb = _heldRock.AddComponent<Rigidbody>();
        rootRb.isKinematic = true; rootRb.useGravity = false;
        _isSmallRock = rootRb.mass < SmallRockMassThreshold;
        Rigidbody[] allRbs = _heldRock.GetComponentsInChildren<Rigidbody>(true);
        foreach (var rb in allRbs) if (rb.gameObject != _heldRock) Destroy(rb);
        Collider rootCol = _heldRock.GetComponent<Collider>();
        if (rootCol == null) rootCol = _heldRock.AddComponent<BoxCollider>();
        _hasValidRock = true;
    }

    Vector3 GetThrowDirection() { if (_character != null && _character.CharacterModel != null) return _character.CharacterModel.transform.forward; return transform.forward; }

    void DrawTrajectory()
    {
        if (HandTransform == null || _lineRenderer == null) return;
        Vector3 startPos = HandTransform.position;
        Vector3 direction = GetThrowDirection(); direction.y = 0; direction.Normalize();
        List<Vector3> trajectoryPoints = new List<Vector3>();
        CalculateBounceTrajectory(startPos, direction, trajectoryPoints);
        _lineRenderer.positionCount = trajectoryPoints.Count;
        _lineRenderer.SetPositions(trajectoryPoints.ToArray());
        UpdateBounceIndicators(trajectoryPoints);
    }

    // 省略轨迹计算，代码量太大且未变动，保持原样即可...
    void CalculateBounceTrajectory(Vector3 s, Vector3 d, List<Vector3> p)
    {
        // 请使用之前的轨迹计算代码，逻辑不变 
        ClearBounceIndicators(); Vector3 c = s; Vector3 cd = d; int b = 0; float rd = MaxRange; p.Add(c);
        while (b < MaxBounces && rd > 0)
        {
            if (Physics.Raycast(c, cd, out RaycastHit hit, rd))
            {
                // 简单的忽略自身判定
                if (hit.transform.IsChildOf(transform)) { c += cd * 0.1f; continue; }
                AddSegmentPoints(p, c, hit.point, SegmentsPerBounce); p.Add(hit.point);
                Vector3 n = hit.normal; n.y = 0; n.Normalize(); cd = Vector3.Reflect(cd, n); cd.y = 0; cd.Normalize();
                rd -= Vector3.Distance(c, hit.point); c = hit.point + cd * 0.05f; b++;
            }
            else { AddSegmentPoints(p, c, c + cd * rd, SegmentsPerBounce); p.Add(c + cd * rd); break; }
        }
    }
    void AddSegmentPoints(List<Vector3> p, Vector3 s, Vector3 e, int seg) { for (int i = 1; i <= seg; i++) p.Add(Vector3.Lerp(s, e, i / (float)seg)); }
    void UpdateBounceIndicators(List<Vector3> tp) { if (!ShowBounceIndicators) return; List<Vector3> bp = new List<Vector3>(); for (int i = SegmentsPerBounce; i < tp.Count; i += SegmentsPerBounce) if (i < tp.Count) bp.Add(tp[i]); while (_bounceIndicators.Count < bp.Count) { GameObject i = GameObject.CreatePrimitive(PrimitiveType.Sphere); Destroy(i.GetComponent<Collider>()); i.transform.localScale = Vector3.one * BounceIndicatorSize; _bounceIndicators.Add(i); } for (int i = 0; i < _bounceIndicators.Count; i++) { if (i < bp.Count) { _bounceIndicators[i].SetActive(true); _bounceIndicators[i].transform.position = bp[i]; } else { _bounceIndicators[i].SetActive(false); } } }
    void ClearBounceIndicators() { foreach (var i in _bounceIndicators) if (i != null) i.SetActive(false); }

    // --- 【修改核心】投掷逻辑 ---
    void ThrowRock()
    {
        if (_heldRock == null) return;

        float throwSpeed = _isSmallRock ? SmallRockSpeed : BigRockSpeed;
        Vector3 throwDir = GetThrowDirection();
        throwDir.y = 0; throwDir.Normalize();
        Vector3 spawnPos = HandTransform.position;
        float fixedHeight = spawnPos.y;

        OptimizeRockStructure(_heldRock);

        // 1. 克隆石头（解决捡起消失问题）
        GameObject projectile = Instantiate(_heldRock, spawnPos, Quaternion.identity);
        projectile.name = _heldRock.name + "_Projectile";

        // 2. 配置物理
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        Collider col = projectile.GetComponent<Collider>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = false;
            rb.mass = 10f;
            rb.linearDamping = 0f; // 【防吸墙】确保空气阻力为0
            rb.angularDamping = 0.05f;
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // 连续检测防穿墙
            rb.linearVelocity = throwDir * throwSpeed;
            rb.angularVelocity = Vector3.up * 3f;

            // 【防吸墙】设置无摩擦材质
            if (col != null)
            {
                PhysicsMaterial zeroFriction = new PhysicsMaterial("ZeroFriction");
                zeroFriction.dynamicFriction = 0f;
                zeroFriction.staticFriction = 0f;
                zeroFriction.bounciness = 1f; // 满弹性
                zeroFriction.frictionCombine = PhysicsMaterialCombine.Minimum;
                zeroFriction.bounceCombine = PhysicsMaterialCombine.Maximum;
                col.material = zeroFriction;
                col.isTrigger = false;
            }

            // 添加防吸墙弹跳逻辑
            var bounce = projectile.AddComponent<RockBounceController>();
            bounce.Initialize(MaxBounces, EnableDebugLogs, throwSpeed);
        }

        // 3. 激活攻击（防自身反弹：传入所有碰撞体）
        var logic = projectile.GetComponent<RockImpactLogic>();
        if (logic != null)
        {
            Collider[] allMyColliders = this.GetComponentsInChildren<Collider>();
            logic.ActivateAttack(this.gameObject, allMyColliders);
        }

        // 4. 销毁手里石头
        Destroy(_heldRock);
        _heldRock = null;
        _hasValidRock = false;

        // 5. 【恢复速度】
        if (_characterMovement != null) { _characterMovement.WalkSpeed = _defaultWalkSpeed; _characterMovement.MovementSpeed = _defaultWalkSpeed; }
        if (_characterRun != null) { _characterRun.RunSpeed = _defaultRunSpeed; }

        if (EnableDebugLogs) Debug.Log($"[RockThrower] 发射完成，速度恢复");
    }

    void OptimizeRockStructure(GameObject rock)
    {
        if (rock == null) return;
        Joint[] joints = rock.GetComponentsInChildren<Joint>(true); foreach (var j in joints) if (j != null) Destroy(j);
        Health[] healths = rock.GetComponentsInChildren<Health>(true); foreach (var h in healths) if (h != null) h.Invulnerable = true;
        Rigidbody[] rbs = rock.GetComponentsInChildren<Rigidbody>(true); foreach (var r in rbs) if (r != null && r.gameObject != rock) Destroy(r);
        Collider[] colliders = rock.GetComponentsInChildren<Collider>(true); foreach (var col in colliders) if (col != null && col.gameObject != rock) col.enabled = false;
    }

    void OnDisable() { if (_isCharging) StopCharging(); }
    void OnDestroy() { ClearBounceIndicators(); }

    // --- 【修改核心】弹跳控制器（防吸墙版） ---
    public class RockBounceController : MonoBehaviour
    {
        private int _maxBounces;
        private int _currentBounces = 0;
        private bool _enableDebugLogs;
        private Rigidbody _rb;
        private float _constantSpeed;
        private bool _isDestroying = false;

        // 【防吸墙关键】记录上一帧的速度
        private Vector3 _lastVelocity;

        public void Initialize(int maxBounces, bool debugLogs, float speed)
        {
            _maxBounces = maxBounces; _enableDebugLogs = debugLogs; _constantSpeed = speed;
            _rb = GetComponent<Rigidbody>();

            TrailRenderer trail = gameObject.AddComponent<TrailRenderer>();
            trail.time = 0.5f; trail.startWidth = 0.3f; trail.endWidth = 0.05f;
            trail.material = new Material(Shader.Find("Sprites/Default"));
        }

        void FixedUpdate()
        {
            if (_isDestroying || _rb == null) return;

            // 【关键步骤】每一帧物理更新前，把当前速度记下来
            // 当撞墙的那一帧发生时，_rb.velocity 可能会被物理引擎归零
            // 但我们需要用撞墙前的速度来计算反弹
            _lastVelocity = _rb.linearVelocity;

            // 保持恒定速度（防止因摩擦力慢慢停下来）
            // 只要没归零，就强制拉回恒定速度
            if (_rb.linearVelocity.magnitude > 0.1f)
            {
                Vector3 flatVel = _rb.linearVelocity; flatVel.y = 0;
                _rb.linearVelocity = flatVel.normalized * _constantSpeed;
            }
        }

        void OnCollisionEnter(Collision collision)
        {
            if (_isDestroying) return;
            if (collision.gameObject.GetComponent<Character>() != null) return; // 撞人不管

            _currentBounces++;

            if (_currentBounces >= _maxBounces)
            {
                _isDestroying = true; Destroy(gameObject);
            }
            else if (_rb != null && collision.contacts.Length > 0)
            {
                Vector3 normal = collision.contacts[0].normal;
                normal.y = 0; normal.Normalize();

                // 【防吸墙关键】使用 LastVelocity 来计算反射，而不是当前的 _rb.linearVelocity
                // 因为此时 _rb.linearVelocity 可能已经被物理引擎减速甚至变成0了（导致吸墙）
                Vector3 inDirection = _lastVelocity;
                inDirection.y = 0;

                // 如果上一帧速度太小（极端情况），就用当前方向凑合
                if (inDirection.magnitude < 0.1f) inDirection = _rb.linearVelocity;

                // 计算反射向量
                Vector3 reflectDir = Vector3.Reflect(inDirection.normalized, normal);

                // 强制修正 y 轴
                reflectDir.y = 0; reflectDir.Normalize();

                // 赋给刚体，强行弹开
                _rb.linearVelocity = reflectDir * _constantSpeed;

                if (_enableDebugLogs) Debug.Log($"[Bounce] 墙壁反弹! 使用保存速度 {_lastVelocity.magnitude:F1} -> 强行修正为 {_constantSpeed:F1}");
            }
        }
    }
}
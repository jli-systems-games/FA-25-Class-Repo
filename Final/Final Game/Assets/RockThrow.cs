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
    private Collider _playerCollider;

    // 【新增】记录初始满速
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
        _playerCollider = GetComponent<Collider>();
        _characterMovement = GetComponent<CharacterMovement>();
        _characterRun = GetComponent<CharacterRun>();

        // 【新增】在游戏一开始记录满速状态
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

        // 原代码里的 ForceRestoreSpeed 我去掉了，改为在ThrowRock里恢复，这样更高效

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
                ThrowRock(); // 发射！
            }
        }
        else if (_isCharging)
        {
            StopCharging();
        }
    }

    // --- 辅助函数保持原样 ---
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
            if (foundAttachment != null) HandTransform = foundAttachment;
        }
    }

    void SetupLineRenderer()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.startWidth = LineWidth;
        _lineRenderer.endWidth = LineWidth;
        _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _lineRenderer.startColor = LineColorStart;
        _lineRenderer.endColor = LineColorEnd;
        _lineRenderer.enabled = false;
        // Gradient setup omitted for brevity, keeping yours
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(LineColorStart, 0.0f), new GradientColorKey(LineColorBounce1, 0.33f), new GradientColorKey(LineColorBounce2, 0.66f), new GradientColorKey(LineColorEnd, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.8f, 0.0f), new GradientAlphaKey(0.6f, 1.0f) }
        );
        _lineRenderer.colorGradient = gradient;
    }

    void StartCharging()
    {
        _isCharging = true;
        _chargeTimer = 0f;
        _pulseTimer = PulseInterval;
        if (_lineRenderer != null) _lineRenderer.enabled = true;
        if (EnableDebugLogs) Debug.Log("[RockThrower] 开始蓄力");
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
        if (_pulseTimer >= PulseInterval)
        {
            _pulseTimer = 0f;
            TriggerPulseEffect();
        }
    }

    void TriggerPulseEffect()
    {
        if (_perlin != null)
        {
            if (_shakeCoroutine != null) StopCoroutine(_shakeCoroutine);
            _shakeCoroutine = StartCoroutine(ShakeCinemachine(ShakeDuration, ShakeIntensity, ShakeFrequency));
        }
        GameObject ring = CreateChargeRing();
        if (ring != null) StartCoroutine(ShrinkRingRoutine(ring, RingDuration));
    }

    GameObject CreateChargeRing()
    {
        // 保持你原来的逻辑
        GameObject ring = null;
        if (ChargeRingPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.1f;
            ring = Instantiate(ChargeRingPrefab, spawnPos, Quaternion.Euler(90, 0, 0));
        }
        else
        {
            // (省略 create primitive 代码，保持你的原样)
            ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Destroy(ring.GetComponent<Collider>());
            ring.transform.position = transform.position + Vector3.up * 0.05f;
            ring.transform.localScale = new Vector3(3f, 0.01f, 3f);
        }
        return ring;
    }

    IEnumerator ShakeCinemachine(float duration, float amplitude, float frequency)
    {
        if (_perlin == null) yield break;
        _perlin.AmplitudeGain = amplitude;
        _perlin.FrequencyGain = frequency;
        float timer = 0f;
        while (timer < duration) { timer += Time.deltaTime; yield return null; }
        _perlin.AmplitudeGain = 0f;
        _perlin.FrequencyGain = BaseFrequency;
    }

    IEnumerator ShrinkRingRoutine(GameObject ring, float duration)
    {
        if (ring == null) yield break;
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

    void FindAndFixRock()
    {
        if (_heldRock != null) { _hasValidRock = true; return; }
        if (HandTransform == null || HandTransform.childCount == 0) { _heldRock = null; _hasValidRock = false; return; }

        Transform rockRoot = HandTransform.GetChild(0);
        _heldRock = rockRoot.gameObject;

        Rigidbody rootRb = _heldRock.GetComponent<Rigidbody>();
        if (rootRb == null) rootRb = _heldRock.AddComponent<Rigidbody>();
        rootRb.isKinematic = true;
        rootRb.useGravity = false;

        // 保持你原来的逻辑
        _isSmallRock = rootRb.mass < SmallRockMassThreshold;

        Rigidbody[] allRbs = _heldRock.GetComponentsInChildren<Rigidbody>(true);
        foreach (var rb in allRbs) if (rb.gameObject != _heldRock) Destroy(rb);

        Collider rootCol = _heldRock.GetComponent<Collider>();
        if (rootCol == null) rootCol = _heldRock.AddComponent<BoxCollider>();
        // 在手里的时候，不要让Collider干扰玩家
        if (rootCol != null && _playerCollider != null) Physics.IgnoreCollision(rootCol, _playerCollider, true);

        _hasValidRock = true;
        if (EnableDebugLogs) Debug.Log($"[RockThrower] 找到石头: {_heldRock.name}");
    }

    Vector3 GetThrowDirection()
    {
        if (_character != null && _character.CharacterModel != null) return _character.CharacterModel.transform.forward;
        return transform.forward;
    }

    void DrawTrajectory()
    {
        // 保持原来的逻辑
        if (HandTransform == null || _lineRenderer == null) return;
        Vector3 startPos = HandTransform.position;
        Vector3 direction = GetThrowDirection(); direction.y = 0; direction.Normalize();
        List<Vector3> trajectoryPoints = new List<Vector3>();
        CalculateBounceTrajectory(startPos, direction, trajectoryPoints);
        _lineRenderer.positionCount = trajectoryPoints.Count;
        _lineRenderer.SetPositions(trajectoryPoints.ToArray());
        UpdateBounceIndicators(trajectoryPoints);
    }

    // 省略 CalculateBounceTrajectory, AddSegmentPoints, UpdateBounceIndicators, ClearBounceIndicators 
    // 请直接复制你原有的代码，因为这些不用改
    void CalculateBounceTrajectory(Vector3 startPos, Vector3 startDir, List<Vector3> points)
    {
        // 使用你原有的代码
        ClearBounceIndicators();
        Vector3 currentPos = startPos;
        Vector3 currentDir = startDir;
        int bounceCount = 0;
        float remainingDistance = MaxRange;
        points.Add(currentPos);
        while (bounceCount < MaxBounces && remainingDistance > 0)
        {
            if (Physics.Raycast(currentPos, currentDir, out RaycastHit hit, remainingDistance))
            {
                if (hit.collider == _playerCollider) { currentPos += currentDir * 0.1f; continue; }
                AddSegmentPoints(points, currentPos, hit.point, SegmentsPerBounce);
                points.Add(hit.point);
                Vector3 normal = hit.normal; normal.y = 0; normal.Normalize();
                currentDir = Vector3.Reflect(currentDir, normal); currentDir.y = 0; currentDir.Normalize();
                float distanceTraveled = Vector3.Distance(currentPos, hit.point);
                remainingDistance -= distanceTraveled;
                currentPos = hit.point + currentDir * 0.05f;
                bounceCount++;
            }
            else
            {
                Vector3 endPos = currentPos + currentDir * remainingDistance;
                AddSegmentPoints(points, currentPos, endPos, SegmentsPerBounce);
                points.Add(endPos);
                break;
            }
        }
    }

    void AddSegmentPoints(List<Vector3> points, Vector3 start, Vector3 end, int segments)
    {
        for (int i = 1; i <= segments; i++) { float t = i / (float)segments; points.Add(Vector3.Lerp(start, end, t)); }
    }

    void UpdateBounceIndicators(List<Vector3> trajectoryPoints)
    {
        // 使用你原有的代码
        if (!ShowBounceIndicators) return;
        List<Vector3> bouncePoints = new List<Vector3>();
        for (int i = SegmentsPerBounce; i < trajectoryPoints.Count; i += SegmentsPerBounce) if (i < trajectoryPoints.Count) bouncePoints.Add(trajectoryPoints[i]);
        while (_bounceIndicators.Count < bouncePoints.Count)
        {
            GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            indicator.name = "BounceIndicator"; Destroy(indicator.GetComponent<Collider>());
            indicator.transform.localScale = Vector3.one * BounceIndicatorSize;
            _bounceIndicators.Add(indicator);
        }
        for (int i = 0; i < _bounceIndicators.Count; i++)
        {
            if (i < bouncePoints.Count) { _bounceIndicators[i].SetActive(true); _bounceIndicators[i].transform.position = bouncePoints[i]; }
            else { _bounceIndicators[i].SetActive(false); }
        }
    }

    void ClearBounceIndicators() { foreach (var indicator in _bounceIndicators) if (indicator != null) indicator.SetActive(false); }

    // 【修改核心】ThrowRock
    void ThrowRock()
    {
        if (_heldRock == null) return;

        float throwSpeed = _isSmallRock ? SmallRockSpeed : BigRockSpeed;
        Vector3 throwDir = GetThrowDirection();
        throwDir.y = 0; throwDir.Normalize();
        Vector3 spawnPos = HandTransform.position;
        float fixedHeight = spawnPos.y;

        OptimizeRockStructure(_heldRock);

        // 1. 克隆石头（解决消失问题）
        GameObject projectile = Instantiate(_heldRock, spawnPos, Quaternion.identity);
        projectile.name = _heldRock.name + "_Projectile";

        // 2. 配置克隆体的物理
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        Collider col = projectile.GetComponent<Collider>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = false;
            rb.mass = 10f;
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.linearVelocity = throwDir * throwSpeed;
            rb.angularVelocity = Vector3.up * 3f;

            // 确保材质无摩擦
            if (col != null)
            {
                PhysicsMaterial zeroFriction = new PhysicsMaterial("ZeroFriction");
                zeroFriction.dynamicFriction = 0f; zeroFriction.staticFriction = 0f; zeroFriction.bounciness = 0f;
                col.material = zeroFriction;
            }

            // 添加弹跳逻辑
            var bounce = projectile.AddComponent<RockBounceController>();
            bounce.Initialize(MaxBounces, null, EnableDebugLogs, throwSpeed, fixedHeight);
        }

        // 3. 激活攻击逻辑，并传入 _playerCollider 做物理忽略
        var logic = projectile.GetComponent<RockImpactLogic>();
        if (logic != null)
        {
            // 这里的 this.gameObject 是 Player, _playerCollider 也是 Player 的 Collider
            logic.ActivateAttack(this.gameObject, _playerCollider);
        }

        // 4. 销毁手中的石头，模拟“扔出去了”
        Destroy(_heldRock);
        _heldRock = null;
        _hasValidRock = false;

        // 5. 【恢复速度】手动设置回 Start 时记录的速度
        if (_characterMovement != null)
        {
            _characterMovement.WalkSpeed = _defaultWalkSpeed;
            _characterMovement.MovementSpeed = _defaultWalkSpeed;
        }
        if (_characterRun != null)
        {
            _characterRun.RunSpeed = _defaultRunSpeed;
        }

        if (EnableDebugLogs) Debug.Log($"[RockThrower] 石头已发射，速度已重置");
    }

    void OptimizeRockStructure(GameObject rock)
    {
        if (rock == null) return;
        Joint[] joints = rock.GetComponentsInChildren<Joint>(true);
        foreach (var j in joints) if (j != null) Destroy(j);

        // 保持你的逻辑
        Health[] healths = rock.GetComponentsInChildren<Health>(true);
        foreach (var h in healths) if (h != null) h.Invulnerable = true;

        Rigidbody[] rbs = rock.GetComponentsInChildren<Rigidbody>(true);
        foreach (var r in rbs) if (r != null && r.gameObject != rock) Destroy(r);

        Collider[] colliders = rock.GetComponentsInChildren<Collider>(true);
        foreach (var col in colliders) if (col != null && col.gameObject != rock) col.enabled = false;
    }

    void OnDisable() { if (_isCharging) StopCharging(); }
    void OnDestroy() { ClearBounceIndicators(); }

    // 内部类 RockBounceController 保持原样
    public class RockBounceController : MonoBehaviour
    {
        private int _maxBounces;
        private int _currentBounces = 0;
        private bool _enableDebugLogs;
        private Rigidbody _rb;
        private float _fixedHeight;
        private float _constantSpeed;
        private bool _isDestroying = false;

        public void Initialize(int maxBounces, Collider c, bool debugLogs, float speed, float fixedHeight)
        {
            _maxBounces = maxBounces; _enableDebugLogs = debugLogs; _constantSpeed = speed; _fixedHeight = fixedHeight;
            _rb = GetComponent<Rigidbody>();
            TrailRenderer trail = gameObject.AddComponent<TrailRenderer>();
            trail.time = 0.5f; trail.startWidth = 0.3f; trail.endWidth = 0.05f;
            trail.material = new Material(Shader.Find("Sprites/Default"));
        }
        void FixedUpdate()
        {
            if (_isDestroying || _rb == null) return;
            Vector3 vel = _rb.linearVelocity; vel.y = 0;
            if (vel.magnitude > 0.1f) _rb.linearVelocity = vel.normalized * _constantSpeed;
        }
        void OnCollisionEnter(Collision collision)
        {
            if (_isDestroying) return;
            if (collision.gameObject.GetComponent<Character>() != null) return; // 撞人交给 ImpactLogic
            _currentBounces++;
            if (_currentBounces >= _maxBounces) { _isDestroying = true; Destroy(gameObject); }
            else if (_rb != null && collision.contacts.Length > 0)
            {
                Vector3 n = collision.contacts[0].normal; n.y = 0; n.Normalize();
                Vector3 v = _rb.linearVelocity; v.y = 0;
                Vector3 r = Vector3.Reflect(v.normalized, n);
                _rb.linearVelocity = r.normalized * _constantSpeed;
            }
        }
    }
}
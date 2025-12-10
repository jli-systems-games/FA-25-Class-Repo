using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;
using Unity.Cinemachine;

[RequireComponent(typeof(LineRenderer))]
public class CharacterRockThrower : MonoBehaviour
{
    [Header("键盘输入设置 (P1/P2)")]
    public KeyCode P1ThrowKey = KeyCode.E;
    public KeyCode P2ThrowKey = KeyCode.KeypadEnter;

    [Header("手柄输入设置 (P3/P4)")]
    [Tooltip("P3 的投掷键 (默认 Joystick 1 Button 1)")]
    public KeyCode P3ThrowKey = KeyCode.Joystick1Button1;
    [Tooltip("P4 的投掷键 (默认 Joystick 2 Button 1)")]
    public KeyCode P4ThrowKey = KeyCode.Joystick2Button1;

    [Header("投掷参数")]
    [Tooltip("如果不填，游戏开始时会自动寻找名为 'WeaponAttachment' 的子物体")]
    public Transform HandTransform;
    [Tooltip("大石头飞行速度")]
    public float BigRockSpeed = 10f;
    [Tooltip("小石头飞行速度（大石头的2倍）")]
    public float SmallRockSpeed = 20f;
    [Tooltip("最大蓄力时间")]
    public float MaxChargeTime = 1.0f;
    [Tooltip("射程距离（覆盖全场）")]
    public float MaxRange = 30f;
    [Tooltip("石头大小判断阈值")]
    public float SmallRockMassThreshold = 3f;
    [Tooltip("最大反弹次数")]
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
    [Tooltip("轨迹线段数（每次反弹）")]
    public int SegmentsPerBounce = 15;
    public Color LineColorStart = new Color(0.8f, 0.8f, 0.8f, 0.8f);
    public Color LineColorBounce1 = new Color(1f, 0.8f, 0.2f, 0.7f);
    public Color LineColorBounce2 = new Color(1f, 0.5f, 0.2f, 0.6f);
    public Color LineColorEnd = new Color(1f, 0.2f, 0.2f, 0.5f);
    [Tooltip("显示碰撞点指示器")]
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

    private float _initialWalkSpeed;
    private float _initialRunSpeed;
    private float _pulseTimer;

    private Coroutine _shakeCoroutine;
    private CinemachineBasicMultiChannelPerlin _perlin;
    private bool _cinemachineInitialized = false;

    private List<GameObject> _bounceIndicators = new List<GameObject>();
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

        if (_characterMovement != null)
        {
            _initialWalkSpeed = _characterMovement.WalkSpeed;
            _initialRunSpeed = _characterRun != null ? _characterRun.RunSpeed : _initialWalkSpeed;
        }

        if (EnableDebugLogs)
            Debug.Log($"[RockThrower] 初始化完成 - 初始速度 WalkSpeed: {_initialWalkSpeed}, RunSpeed: {_initialRunSpeed}");
    }

    void Update()
    {
        TryInitCinemachine();

        if (HandTransform == null)
        {
            CheckAndFixHandTransform();
            if (HandTransform == null) return;
        }

        FindAndFixRock();

        // 强制速度恢复检查
        if (!_isCharging)
        {
            ForceRestoreSpeed();
        }

        // =====================================================================
        // 【死绑版本】4 个玩家各自固定一个按键
        // =====================================================================
        bool inputDown = false;
        bool inputPressed = false;
        bool inputUp = false;

        if (_character != null)
        {
            string id = string.IsNullOrEmpty(_character.PlayerID) ? "" : _character.PlayerID.Trim();

            if (EnableDebugLogs)
            {
                Debug.Log($"[RockThrower] PlayerID = \"{id}\"");
            }

            if (id == "Player1")
            {
                // P1：键盘 E
                inputDown = Input.GetKeyDown(KeyCode.E);
                inputPressed = Input.GetKey(KeyCode.E);
                inputUp = Input.GetKeyUp(KeyCode.E);
            }
            else if (id == "Player2")
            {
                // P2：小键盘 Enter
                inputDown = Input.GetKeyDown(KeyCode.KeypadEnter);
                inputPressed = Input.GetKey(KeyCode.KeypadEnter);
                inputUp = Input.GetKeyUp(KeyCode.KeypadEnter);
            }
            else if (id == "Player3")
            {
                // P3：手柄 1 Button1（假定是叉）
                inputDown = Input.GetKeyDown(KeyCode.Joystick1Button1);
                inputPressed = Input.GetKey(KeyCode.Joystick1Button1);
                inputUp = Input.GetKeyUp(KeyCode.Joystick1Button1);
            }
            else if (id == "Player4")
            {
                // P4：手柄 2 Button1
                inputDown = Input.GetKeyDown(KeyCode.Joystick2Button1);
                inputPressed = Input.GetKey(KeyCode.Joystick2Button1);
                inputUp = Input.GetKeyUp(KeyCode.Joystick2Button1);
            }

            // 完全不再读取 LinkedInputManager 的 ShootButton，避免引擎里的映射干扰
        }
        // =====================================================================

        if (EnableDebugLogs && inputDown && _character != null)
        {
            Debug.Log($"[RockThrower] {_character.PlayerID} 投掷键按下（inputDown = true）");
        }

        // 逻辑处理
        if (inputDown)
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
            if (inputPressed)
            {
                UpdateCharging();
            }

            if (inputUp)
            {
                // 停止蓄力并投掷
                _isCharging = false;
                if (_lineRenderer != null) _lineRenderer.enabled = false;
                ClearBounceIndicators();
                ForceRestoreSpeed();
                ThrowRock();
            }
        }
        else if (_isCharging)
        {
            // 异常中断（如石头没了）
            StopCharging();
        }
    }

    void ForceRestoreSpeed()
    {
        if (_characterMovement == null) return;

        bool needRestore = false;
        if (Mathf.Abs(_characterMovement.WalkSpeed - _initialWalkSpeed) > 0.01f) needRestore = true;
        if (_characterRun != null && Mathf.Abs(_characterRun.RunSpeed - _initialRunSpeed) > 0.01f) needRestore = true;

        if (needRestore)
        {
            _characterMovement.WalkSpeed = _initialWalkSpeed;
            _characterMovement.MovementSpeed = _initialWalkSpeed;
            if (_characterRun != null) _characterRun.RunSpeed = _initialRunSpeed;
        }
    }

    void TryInitCinemachine()
    {
        if (_cinemachineInitialized) return;
        if (VCam == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                var brain = mainCam.GetComponent<CinemachineBrain>();
                if (brain != null && brain.ActiveVirtualCamera is CinemachineCamera camFromBrain)
                    VCam = camFromBrain;
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
            if (foundAttachment != null)
            {
                HandTransform = foundAttachment;
                if (EnableDebugLogs) Debug.Log("[RockThrower] 自动找到 WeaponAttachment");
            }
        }
    }

    void FindAndFixRock()
    {
        if (_heldRock != null) return;
        if (HandTransform == null || HandTransform.childCount == 0)
        {
            _heldRock = null;
            return;
        }

        Transform rockRoot = HandTransform.GetChild(0);
        _heldRock = rockRoot.gameObject;

        Rigidbody rootRb = _heldRock.GetComponent<Rigidbody>();
        if (rootRb == null) rootRb = _heldRock.AddComponent<Rigidbody>();

        rootRb.isKinematic = true;
        rootRb.useGravity = false;
        rootRb.constraints = RigidbodyConstraints.FreezeRotation;

        _isSmallRock = rootRb.mass < SmallRockMassThreshold;

        // 清理子物体刚体
        Rigidbody[] allRbs = _heldRock.GetComponentsInChildren<Rigidbody>(true);
        foreach (var rb in allRbs)
        {
            if (rb.gameObject != _heldRock) Destroy(rb);
        }

        Collider rootCol = _heldRock.GetComponent<Collider>();
        if (rootCol == null) rootCol = _heldRock.AddComponent<BoxCollider>();
    }

    Vector3 GetThrowDirection()
    {
        if (_character != null && _character.CharacterModel != null)
        {
            return _character.CharacterModel.transform.forward;
        }
        return transform.forward;
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

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(LineColorStart, 0.0f),
                new GradientColorKey(LineColorBounce1, 0.33f),
                new GradientColorKey(LineColorBounce2, 0.66f),
                new GradientColorKey(LineColorEnd, 1.0f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0.8f, 0.0f),
                new GradientAlphaKey(0.6f, 1.0f)
            }
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
        ForceRestoreSpeed();
        if (EnableDebugLogs) Debug.Log($"[RockThrower] 停止蓄力 - 蓄力时间: {_chargeTimer:F2}s");
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
        GameObject ring = null;
        if (ChargeRingPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.1f;
            ring = Instantiate(ChargeRingPrefab, spawnPos, Quaternion.Euler(90, 0, 0));
        }
        else
        {
            ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            Destroy(ring.GetComponent<Collider>());
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
                float chargeRatio = _chargeTimer / MaxChargeTime;
                Color ringColor = Color.Lerp(Color.white, Color.red, chargeRatio);
                ringColor.a = 0.3f;
                r.material.color = ringColor;
            }
        }
        return ring;
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

    void DrawTrajectory()
    {
        if (HandTransform == null || _lineRenderer == null) return;
        Vector3 startPos = HandTransform.position;
        Vector3 direction = GetThrowDirection();
        direction.y = 0;
        direction.Normalize();
        List<Vector3> trajectoryPoints = new List<Vector3>();
        CalculateBounceTrajectory(startPos, direction, trajectoryPoints);
        _lineRenderer.positionCount = trajectoryPoints.Count;
        _lineRenderer.SetPositions(trajectoryPoints.ToArray());
        UpdateBounceIndicators(trajectoryPoints);
    }

    void CalculateBounceTrajectory(Vector3 startPos, Vector3 startDir, List<Vector3> points)
    {
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
                if (hit.collider == _playerCollider)
                {
                    currentPos += currentDir * 0.1f;
                    continue;
                }
                AddSegmentPoints(points, currentPos, hit.point, SegmentsPerBounce);
                points.Add(hit.point);
                Vector3 normal = hit.normal;
                normal.y = 0;
                normal.Normalize();
                currentDir = Vector3.Reflect(currentDir, normal);
                currentDir.y = 0;
                currentDir.Normalize();
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
        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;
            points.Add(Vector3.Lerp(start, end, t));
        }
    }

    void UpdateBounceIndicators(List<Vector3> trajectoryPoints)
    {
        if (!ShowBounceIndicators) return;
        List<Vector3> bouncePoints = new List<Vector3>();
        for (int i = SegmentsPerBounce; i < trajectoryPoints.Count; i += SegmentsPerBounce)
        {
            if (i < trajectoryPoints.Count) bouncePoints.Add(trajectoryPoints[i]);
        }
        while (_bounceIndicators.Count < bouncePoints.Count)
        {
            GameObject indicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            indicator.name = "BounceIndicator";
            Destroy(indicator.GetComponent<Collider>());
            indicator.transform.localScale = Vector3.one * BounceIndicatorSize;
            Renderer renderer = indicator.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = new Material(Shader.Find("Standard"));
                renderer.material.SetFloat("_Mode", 3);
                renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                renderer.material.SetInt("_ZWrite", 0);
                renderer.material.EnableKeyword("_ALPHABLEND_ON");
                renderer.material.renderQueue = 3000;
            }
            _bounceIndicators.Add(indicator);
        }
        for (int i = 0; i < _bounceIndicators.Count; i++)
        {
            if (i < bouncePoints.Count)
            {
                _bounceIndicators[i].SetActive(true);
                _bounceIndicators[i].transform.position = bouncePoints[i];
                Renderer renderer = _bounceIndicators[i].GetComponent<Renderer>();
                if (renderer != null)
                {
                    float ratio = (i + 1) / (float)MaxBounces;
                    Color color = Color.Lerp(LineColorBounce1, LineColorEnd, ratio);
                    color.a = 0.7f;
                    renderer.material.color = color;
                }
            }
            else _bounceIndicators[i].SetActive(false);
        }
    }

    void ClearBounceIndicators()
    {
        foreach (var indicator in _bounceIndicators)
        {
            if (indicator != null) indicator.SetActive(false);
        }
    }

    void ThrowRock()
    {
        if (_heldRock == null)
        {
            if (EnableDebugLogs) Debug.LogWarning("[RockThrower] ThrowRock: 石头已丢失！");
            ForceRestoreSpeed();
            return;
        }

        float throwSpeed = _isSmallRock ? SmallRockSpeed : BigRockSpeed;
        Vector3 throwDir = GetThrowDirection();
        throwDir.y = 0;
        throwDir.Normalize();
        Vector3 spawnPos = HandTransform.position;
        float fixedHeight = spawnPos.y;

        if (EnableDebugLogs) Debug.Log($"[RockThrower] 发射石头 - 速度: {throwSpeed:F1}, 小石头: {_isSmallRock}");

        OptimizeRockStructure(_heldRock);
        Collider rockCollider = _heldRock.GetComponent<Collider>();
        _heldRock.transform.SetParent(null, false);
        _heldRock.transform.position = spawnPos;
        _heldRock.transform.rotation = Quaternion.identity;

        Rigidbody rb = _heldRock.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = false;
            rb.mass = 10f;
            rb.linearDamping = 0f;
            rb.angularDamping = 0f;
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.interpolation = RigidbodyInterpolation.Interpolate;

            if (rockCollider != null)
            {
                PhysicsMaterial zeroFriction = new PhysicsMaterial("ZeroFriction");
                zeroFriction.dynamicFriction = 0f;
                zeroFriction.staticFriction = 0f;
                zeroFriction.bounciness = 0f;
                zeroFriction.frictionCombine = PhysicsMaterialCombine.Minimum;
                zeroFriction.bounceCombine = PhysicsMaterialCombine.Minimum;
                rockCollider.material = zeroFriction;
                rockCollider.isTrigger = false;
            }

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.linearVelocity = throwDir * throwSpeed;
            rb.angularVelocity = Vector3.up * 3f;

            var bounceController = _heldRock.AddComponent<RockBounceController>();
            bounceController.Initialize(MaxBounces, null, EnableDebugLogs, throwSpeed, fixedHeight);
        }

        var rockLogic = _heldRock.GetComponent<RockImpactLogic>();
        if (rockLogic != null) rockLogic.ActivateAttack(this.gameObject);

        _heldRock = null;
        ForceRestoreSpeed();
        if (EnableDebugLogs) Debug.Log($"[RockThrower] 石头已发射，引用已清除");
    }

    void OptimizeRockStructure(GameObject rock)
    {
        if (rock == null) return;
        Joint[] joints = rock.GetComponentsInChildren<Joint>(true);
        foreach (var j in joints) if (j != null) Destroy(j);
        Health[] healths = rock.GetComponentsInChildren<Health>(true);
        foreach (var h in healths) if (h != null) { h.Invulnerable = true; h.InitialHealth = 1000; h.CurrentHealth = 1000; }
        Rigidbody[] rbs = rock.GetComponentsInChildren<Rigidbody>(true);
        foreach (var r in rbs) if (r != null && r.gameObject != rock) { r.isKinematic = true; Destroy(r); }
        Collider[] colliders = rock.GetComponentsInChildren<Collider>(true);
        foreach (var col in colliders) if (col != null && col.gameObject != rock) col.enabled = false;
        foreach (Transform child in rock.transform) if (child != null && !child.gameObject.activeSelf) Destroy(child.gameObject);
        if (EnableDebugLogs) Debug.Log($"[RockThrower] 石头结构优化完成: {rock.name}");
    }

    void OnDestroy()
    {
        ClearBounceIndicators();
        foreach (var indicator in _bounceIndicators) if (indicator != null) Destroy(indicator);
        _bounceIndicators.Clear();
    }

    void OnDisable()
    {
        if (_isCharging) StopCharging();
    }

    public class RockBounceController : MonoBehaviour
    {
        private int _maxBounces;
        private int _currentBounces = 0;
        private bool _enableDebugLogs;
        private Rigidbody _rb;
        private TrailRenderer _trail;
        private bool _isDestroying = false;
        private float _fixedHeight;
        private float _constantSpeed;

        public void Initialize(int maxBounces, Collider playerCol, bool debugLogs, float speed, float fixedHeight)
        {
            _maxBounces = maxBounces;
            _enableDebugLogs = debugLogs;
            _constantSpeed = speed;
            _rb = GetComponent<Rigidbody>();
            _fixedHeight = fixedHeight;
            _trail = gameObject.AddComponent<TrailRenderer>();
            _trail.time = 0.5f;
            _trail.startWidth = 0.3f;
            _trail.endWidth = 0.05f;
            _trail.material = new Material(Shader.Find("Sprites/Default"));
            Gradient gradient = new Gradient();
            gradient.SetKeys(new GradientColorKey[] { new GradientColorKey(new Color(1f, 0.8f, 0.2f), 0.0f), new GradientColorKey(new Color(1f, 0.2f, 0.2f), 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(0.8f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });
            _trail.colorGradient = gradient;
        }

        void FixedUpdate()
        {
            if (_isDestroying || _rb == null) return;
            Vector3 vel = _rb.linearVelocity;
            if (Mathf.Abs(vel.y) > 0.01f) { vel.y = 0; _rb.linearVelocity = vel; }
            Vector3 currentVel = _rb.linearVelocity;
            currentVel.y = 0;
            float currentSpeed = currentVel.magnitude;
            if (currentSpeed > 0.1f && Mathf.Abs(currentSpeed - _constantSpeed) > 0.5f) _rb.linearVelocity = currentVel.normalized * _constantSpeed;
        }

        void OnCollisionEnter(Collision collision)
        {
            if (_isDestroying) return;
            Character hitChar = collision.gameObject.GetComponent<Character>();
            if (hitChar != null) return;
            _currentBounces++;
            if (_enableDebugLogs) Debug.Log($"[RockBounce] 第 {_currentBounces}/{_maxBounces} 次弹跳");
            if (collision.contacts.Length > 0) CreateBounceEffect(collision.contacts[0].point);
            if (_currentBounces >= _maxBounces)
            {
                _isDestroying = true;
                CreateDestroyEffect();
                if (_rb != null) { _rb.linearVelocity = Vector3.zero; _rb.isKinematic = true; }
                Destroy(gameObject, 0.3f);
            }
            else
            {
                if (_rb != null && collision.contacts.Length > 0)
                {
                    Vector3 normal = collision.contacts[0].normal;
                    normal.y = 0;
                    normal.Normalize();
                    Vector3 velocity = _rb.linearVelocity;
                    velocity.y = 0;
                    Vector3 direction = velocity.normalized;
                    Vector3 reflectDir = Vector3.Reflect(direction, normal);
                    reflectDir.y = 0;
                    if (reflectDir.magnitude < 0.5f || Vector3.Dot(reflectDir, direction) > 0.7f)
                    {
                        if (Mathf.Abs(normal.x) > Mathf.Abs(normal.z)) reflectDir = new Vector3(-direction.x, 0, direction.z);
                        else reflectDir = new Vector3(direction.x, 0, -direction.z);
                    }
                    reflectDir.Normalize();
                    float randomAngle = Random.Range(-3f, 3f);
                    reflectDir = Quaternion.Euler(0, randomAngle, 0) * reflectDir;
                    _rb.linearVelocity = reflectDir * _constantSpeed;
                    _rb.angularVelocity = Vector3.up * 3f;
                }
            }
        }

        void CreateBounceEffect(Vector3 position)
        {
            GameObject effectObj = new GameObject("BounceEffect");
            effectObj.transform.position = position;
            ParticleSystem ps = effectObj.AddComponent<ParticleSystem>();
            var main = ps.main; main.startLifetime = 0.3f; main.startSpeed = 3f; main.startSize = 0.2f; main.startColor = new Color(1f, 0.8f, 0.2f, 1f); main.gravityModifier = 0.5f; main.maxParticles = 20;
            var emission = ps.emission; emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 15) });
            var shape = ps.shape; shape.shapeType = ParticleSystemShapeType.Sphere; shape.radius = 0.3f;
            Destroy(effectObj, 1f);
        }

        void CreateDestroyEffect()
        {
            GameObject effectObj = new GameObject("DestroyEffect");
            effectObj.transform.position = transform.position;
            ParticleSystem ps = effectObj.AddComponent<ParticleSystem>();
            var main = ps.main; main.startLifetime = 0.5f; main.startSpeed = 5f; main.startSize = 0.4f; main.startColor = new Color(1f, 0.3f, 0.1f, 1f); main.gravityModifier = 1f; main.maxParticles = 50;
            var emission = ps.emission; emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 40) });
            var shape = ps.shape; shape.shapeType = ParticleSystemShapeType.Sphere; shape.radius = 0.5f;
            var colorOverLifetime = ps.colorOverLifetime; colorOverLifetime.enabled = true;
            Gradient gradient = new Gradient();
            gradient.SetKeys(new GradientColorKey[] { new GradientColorKey(new Color(1f, 0.8f, 0.2f), 0.0f), new GradientColorKey(new Color(1f, 0.2f, 0.1f), 1.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1f, 0.0f), new GradientAlphaKey(0f, 1.0f) });
            colorOverLifetime.color = gradient;
            Destroy(effectObj, 2f);
        }
    }
}

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BikeDriveFlatTurn_BirdCalibrated : MonoBehaviour
{
    [Header("Speed")]
    public float maxSpeed = 7.5f;
    public float maxReverseSpeed = 2.8f;
    public float accelRate = 10f;
    public float brakeRate = 20f;
    [Range(0, 1)] public float engineBrake = 0.7f;
    public float heavyBrakePerSecond = 40f;

    [Header("Steer (Flat Turn)")]
    public float turnSpeed = 150f;
    public float yawSmoothTime = 0.08f;
    public bool onlyTurnWhenMoving = false;

    [Header("Grounding")]
    public LayerMask groundMask = ~0;
    public float groundCheckRadius = 0.25f;
    public float groundCheckDistance = 1.0f;
    public float groundSnapDistance = 0.12f;
    [Range(0, 89)] public float maxGroundAngle = 60f;
    public float stickToGroundForce = 28f;
    public float extraGravity = 18f;

    [Header("Bridge / Step Assist")]
    public float stepHeight = 0.25f;
    public float stepCheckDistance = 0.5f;
    public float stepLiftSpeed = 12f;
    public float slopeLookAhead = 0.8f;

    [Header("Visual (optional)")]
    public Transform visualRoot;

    [Header("Bird (calibrated)")]
    public Transform bird;
    public float birdTurnSpeed = 120f;
    public Vector3 birdRestEuler = Vector3.zero;
    public Vector3 birdLeftEuler = new Vector3(0, -90, 0);

    [Header("Bell (optional)")]
    public Transform bell;
    public float bellSwingFreq = 8f;
    public float bellSwingAngle = 22f;

    [Header("Audio Engine")]
    public AudioSource engineSrc;
    public AudioClip engineLoop;
    public float engineVolMin = 0.05f;
    public float engineVolMax = 0.4f;
    public float enginePitchMin = 0.85f;
    public float enginePitchMax = 1.35f;

    [Header("Audio SFX")]
    public AudioSource sfxSrc;
    public AudioClip brakeClip;
    public AudioClip shiftClip;
    public float brakeSfxCooldown = 0.25f;

    [Header("Audio Turn")]
    public AudioSource turnLeftSrc;
    public AudioSource turnRightSrc;
    public AudioClip turnLeftLoop;
    public AudioClip turnRightLoop;
    public float turnVolMin = 0.08f;
    public float turnVolMax = 0.3f;
    public float turnPitchMin = 0.95f;
    public float turnPitchMax = 1.25f;

    public float CurrentSpeed => Mathf.Abs(_currentSpeed);

    Rigidbody _rb;
    float _currentSpeed;
    float _yawRate, _yawRateVel;
    bool _grounded;
    Vector3 _groundNormal = Vector3.up;
    Vector3 _groundPoint;
    float _lastBrakeSfxTime;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = true;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        if (!engineSrc) engineSrc = GetComponent<AudioSource>();
        if (engineSrc) { engineSrc.loop = true; engineSrc.playOnAwake = false; }
        if (!sfxSrc && engineSrc) sfxSrc = engineSrc;
        if (turnLeftSrc) { turnLeftSrc.loop = true; turnLeftSrc.playOnAwake = false; }
        if (turnRightSrc) { turnRightSrc.loop = true; turnRightSrc.playOnAwake = false; }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Time.time - _lastBrakeSfxTime >= brakeSfxCooldown && sfxSrc && brakeClip)
            {
                sfxSrc.PlayOneShot(brakeClip, 1f);
                _lastBrakeSfxTime = Time.time;
            }
        }

        if (visualRoot)
        {
            Vector3 fwd = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
            if (fwd.sqrMagnitude > 1e-4f) visualRoot.rotation = Quaternion.LookRotation(fwd.normalized, Vector3.up);
        }

        if (bird)
        {
            Quaternion rest = Quaternion.Euler(birdRestEuler);
            Quaternion left = Quaternion.Euler(birdLeftEuler);
            Quaternion target = Input.GetKey(KeyCode.Q) ? left : rest;
            bird.localRotation = Quaternion.RotateTowards(bird.localRotation, target, birdTurnSpeed * Time.deltaTime);
        }

        if (bell)
        {
            float phase = Input.GetKey(KeyCode.R) ? Time.time * bellSwingFreq : 0f;
            bell.localRotation = Quaternion.Euler(Mathf.Sin(phase) * bellSwingAngle, 0f, 0f);
        }

        UpdateEngineAudio();
        UpdateTurnAudio();
    }

    void FixedUpdate()
    {
        ProbeGround();

        int drive = 0;
        if (Input.GetKey(KeyCode.T)) drive = -1;
        else if (Input.GetKey(KeyCode.W)) drive = 1;

        if (Input.GetKey(KeyCode.E))
        {
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0f, heavyBrakePerSecond * Time.fixedDeltaTime);
        }
        else if (drive != 0)
        {
            float target = drive > 0 ? Mathf.Abs(maxSpeed) : -Mathf.Abs(maxReverseSpeed);
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, target, accelRate * Time.fixedDeltaTime);
        }
        else
        {
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, 0f, accelRate * engineBrake * Time.fixedDeltaTime);
        }

        float steer = 0f;
        if (Input.GetKey(KeyCode.Q)) steer -= 1f;
        if (Input.GetKey(KeyCode.R)) steer += 1f;

        bool canTurn = !onlyTurnWhenMoving || Mathf.Abs(_currentSpeed) > 0.05f;
        float targetYawRate = canTurn ? (steer * turnSpeed) : 0f;
        _yawRate = Mathf.SmoothDamp(_yawRate, targetYawRate, ref _yawRateVel, yawSmoothTime);
        float yaw = _yawRate * Time.fixedDeltaTime;
        _rb.MoveRotation(_rb.rotation * Quaternion.Euler(0f, yaw, 0f));

        Vector3 fwdFlat = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 driveDir = _currentSpeed >= 0f ? fwdFlat : -fwdFlat;

        Vector3 v = _rb.linearVelocity;
        Vector3 vUp = Vector3.Project(v, Vector3.up);
        Vector3 vF = Vector3.Project(v, driveDir);
        Vector3 vSide = v - vUp - vF;
        float spd01 = Mathf.Clamp01(Mathf.Abs(_currentSpeed) / Mathf.Max(0.01f, maxSpeed));
        float sideDamp = Mathf.Lerp(5f, 10f, spd01);
        vSide = Vector3.Lerp(vSide, Vector3.zero, Mathf.Clamp01(sideDamp * Time.fixedDeltaTime));
        _rb.linearVelocity = vUp + vSide + driveDir * _currentSpeed;

        if (_grounded)
        {
            float upDot = Vector3.Dot(_rb.linearVelocity, Vector3.up);
            if (upDot > 0f) _rb.linearVelocity -= Vector3.up * upDot;
            _rb.AddForce(Vector3.down * stickToGroundForce, ForceMode.Acceleration);
            float dist = Vector3.Dot(transform.position - _groundPoint, Vector3.up);
            if (Mathf.Abs(dist) < groundSnapDistance && Mathf.Abs(dist) > 0.0001f) _rb.MovePosition(_rb.position - Vector3.up * dist);
        }
        else
        {
            _rb.AddForce(Physics.gravity.normalized * extraGravity, ForceMode.Acceleration);
        }

        TryStepClimb(driveDir);
    }

    void UpdateEngineAudio()
    {
        if (!engineSrc || !engineLoop) return;
        float speed01 = Mathf.Clamp01(Mathf.Abs(_currentSpeed) / Mathf.Max(0.001f, maxSpeed));
        if (speed01 > 0.02f)
        {
            if (!engineSrc.isPlaying)
            {
                engineSrc.clip = engineLoop;
                engineSrc.loop = true;
                engineSrc.Play();
            }
        }
        else
        {
            if (engineSrc.isPlaying) engineSrc.Stop();
        }
        if (engineSrc.isPlaying)
        {
            engineSrc.volume = Mathf.Lerp(engineVolMin, engineVolMax, speed01);
            engineSrc.pitch = Mathf.Lerp(enginePitchMin, enginePitchMax, speed01);
        }
    }

    void UpdateTurnAudio()
    {
        bool left = Input.GetKey(KeyCode.Q);
        bool right = Input.GetKey(KeyCode.R);
        float speed01 = Mathf.Clamp01(Mathf.Abs(_currentSpeed) / Mathf.Max(0.001f, maxSpeed));

        if (left && !right && turnLeftSrc && turnLeftLoop)
        {
            if (!turnLeftSrc.isPlaying)
            {
                turnLeftSrc.clip = turnLeftLoop;
                turnLeftSrc.loop = true;
                turnLeftSrc.Play();
            }
            turnLeftSrc.volume = Mathf.Lerp(turnVolMin, turnVolMax, speed01);
            turnLeftSrc.pitch = Mathf.Lerp(turnPitchMin, turnPitchMax, speed01);
        }
        else if (turnLeftSrc && turnLeftSrc.isPlaying)
        {
            turnLeftSrc.Stop();
        }

        if (right && !left && turnRightSrc && turnRightLoop)
        {
            if (!turnRightSrc.isPlaying)
            {
                turnRightSrc.clip = turnRightLoop;
                turnRightSrc.loop = true;
                turnRightSrc.Play();
            }
            turnRightSrc.volume = Mathf.Lerp(turnVolMin, turnVolMax, speed01);
            turnRightSrc.pitch = Mathf.Lerp(turnPitchMin, turnPitchMax, speed01);
        }
        else if (turnRightSrc && turnRightSrc.isPlaying)
        {
            turnRightSrc.Stop();
        }
    }

    void ProbeGround()
    {
        _grounded = false;
        _groundNormal = Vector3.up;
        _groundPoint = _rb.position;
        Vector3 origin = transform.position + Vector3.up * 0.2f;
        if (Physics.SphereCast(origin, groundCheckRadius, Vector3.down, out RaycastHit hit, groundCheckDistance, groundMask, QueryTriggerInteraction.Ignore))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            if (angle <= maxGroundAngle)
            {
                _grounded = true;
                _groundNormal = hit.normal.normalized;
                _groundPoint = hit.point;
            }
        }
    }

    void TryStepClimb(Vector3 forward)
    {
        Vector3 up = Vector3.up;
        Vector3 foot = _rb.position + up * 0.05f;
        bool lowHit = Physics.Raycast(foot, forward, out var low, stepCheckDistance, groundMask, QueryTriggerInteraction.Ignore);
        bool highHit = Physics.Raycast(foot + up * stepHeight, forward, stepCheckDistance, groundMask, QueryTriggerInteraction.Ignore);
        if (lowHit && !highHit)
        {
            float lift = stepHeight * Mathf.Clamp01(stepLiftSpeed * Time.fixedDeltaTime);
            _rb.MovePosition(_rb.position + up * lift);
        }
    }
}

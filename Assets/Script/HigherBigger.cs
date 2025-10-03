using UnityEngine;
using Cinemachine;
using System.Collections.Generic;


public class OrbitReactiveController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform targetToRotate; 
    [SerializeField] private Vector3 rotateAxis = Vector3.up;
    [SerializeField] private float baseRotateSpeed = 10f;  // 初始角速度(度/秒)
    [SerializeField] private float speedPerParticle = 0.05f; // 每多1个粒子增加的角速度
    [SerializeField] private float rotateSmooth = 5f; // 旋转平滑

    [Header("Force Field")]
    [SerializeField] private ParticleSystemForceField forceField;
    [SerializeField] private ParticleSystem[] particleSources; // 受该力场影响的粒子系统
    [SerializeField] private float rangePerParticle = 0.01f; // 每个粒子带来的 EndRange 增量
    [SerializeField] private float minEndRange = 1f;
    [SerializeField] private float maxEndRange = 30f;

    [Header("Cinemachine")]
    [SerializeField] private CinemachineVirtualCamera vcam;
    [SerializeField] private float baseFOV = 60f;
    [SerializeField] private float fovPerParticle = 0.02f;
    [SerializeField] private float camSmooth = 3f;
    [SerializeField] private float baseVerticalArm = 1.5f;
    [SerializeField] private float verticalArmPerParticle = 0.003f;

    // 统计频率（避免每帧全量扫描带来开销）
    [SerializeField] private float countInterval = 0.1f;


    private float _baseEndRange;
    private float _currentRotateSpeed;
    private float _countTimer;
    private int _lastCount;


    private readonly Dictionary<ParticleSystem, ParticleSystem.Particle[]> _buffers =
        new Dictionary<ParticleSystem, ParticleSystem.Particle[]>();

    private Cinemachine3rdPersonFollow _tpFollow;

    void Awake()
    {
        if (forceField == null) Debug.LogError("ForceField 未指定");
        if (targetToRotate == null) Debug.LogError("TargetToRotate 未指定");

        _baseEndRange = forceField != null ? forceField.endRange : 10f;
        _currentRotateSpeed = baseRotateSpeed;

        if (vcam != null)
        {
            vcam.m_Lens.FieldOfView = baseFOV;
            _tpFollow = vcam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
            if (_tpFollow != null) _tpFollow.VerticalArmLength = baseVerticalArm;
        }

        // 预分配粒子缓冲
        if (particleSources != null)
        {
            foreach (var ps in particleSources)
            {
                if (ps == null) continue;
                int cap = Mathf.Max(64, ps.main.maxParticles);
                _buffers[ps] = new ParticleSystem.Particle[cap];
            }
        }
    }

    void Update()
    {
        // 统计力场范围内的粒子数
        _countTimer -= Time.deltaTime;
        if (_countTimer <= 0f)
        {
            _lastCount = CountParticlesInForceField();
            _countTimer = Mathf.Max(0.02f, countInterval);
        }

        // 根据粒子数计算目标角速度
        float targetSpeed = baseRotateSpeed + speedPerParticle * _lastCount;
        _currentRotateSpeed = Mathf.Lerp(_currentRotateSpeed, targetSpeed, Time.deltaTime * rotateSmooth);
        if (targetToRotate != null)
        {
            targetToRotate.Rotate(rotateAxis.normalized, _currentRotateSpeed * Time.deltaTime, Space.World);
        }

        // 扩大力场影响范围
        if (forceField != null)
        {
            float targetEndRange = Mathf.Clamp(_baseEndRange + rangePerParticle * _lastCount, minEndRange, maxEndRange);
            forceField.endRange = Mathf.Lerp(forceField.endRange, targetEndRange, Time.deltaTime * 3f);
        }

        // 相机
        if (vcam != null)
        {
            float targetFov = Mathf.Clamp(baseFOV + fovPerParticle * _lastCount, 10f, 120f);
            vcam.m_Lens.FieldOfView = Mathf.Lerp(vcam.m_Lens.FieldOfView, targetFov, Time.deltaTime * camSmooth);

            if (_tpFollow != null)
            {
                float targetArm = Mathf.Clamp(baseVerticalArm + verticalArmPerParticle * _lastCount, 0f, 10f);
                _tpFollow.VerticalArmLength = Mathf.Lerp(_tpFollow.VerticalArmLength, targetArm, Time.deltaTime * camSmooth);
            }
        }
    }


    private int CountParticlesInForceField()
    {
        if (forceField == null || particleSources == null) return 0;

        Vector3 center = forceField.transform.position;
        float rMin2 = forceField.startRange * forceField.startRange;
        float rMax2 = forceField.endRange * forceField.endRange;

        int total = 0;

        foreach (var ps in particleSources)
        {
            if (ps == null) continue;

            var main = ps.main;
            if (!_buffers.TryGetValue(ps, out var buf) || buf.Length < main.maxParticles)
            {
                buf = new ParticleSystem.Particle[Mathf.Max(buf != null ? buf.Length * 2 : 128, main.maxParticles)];
                _buffers[ps] = buf;
            }

            int alive = ps.GetParticles(buf);

            bool worldSpace = main.simulationSpace == ParticleSystemSimulationSpace.World;
            Transform pst = ps.transform;

            for (int i = 0; i < alive; i++)
            {
                Vector3 wp = worldSpace ? buf[i].position : pst.TransformPoint(buf[i].position);
                float d2 = (wp - center).sqrMagnitude;
                if (d2 >= rMin2 && d2 <= rMax2) total++;
            }
        }
        return total;
    }
}
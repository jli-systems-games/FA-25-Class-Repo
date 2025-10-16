using System.Collections.Generic;
using UnityEngine;

public class VehicleController2D : MonoBehaviour
{
    public Rigidbody2D bodyRb;
    public List<Rigidbody2D> wheelRbs = new List<Rigidbody2D>();
    public List<HingeJoint2D> hinges = new List<HingeJoint2D>();
    public List<int> wheelItemIndices = new List<int>();
    public float gravityScale = 2.5f;
    public float globalScale = 0.5f;
    public float driveMaxTorque = 1800f;

    public List<float> wheelRadius = new List<float>();
    public List<bool> driveMask = new List<bool>();
    public float overlapFactor = 0.55f;

    float currentMotorSpeed;
    bool driveOn;

    const float FallbackTargetDiameter = 1.2f;
    const float FallbackMinScale = 0.25f;
    const float FallbackMaxScale = 2.0f;

    float unstickTimer;
    public float unstickSpeedThreshold = 0.05f;
    public float unstickTime = 0.35f;
    public Vector2 unstickImpulse = new Vector2(0.55f, 0.22f);

    List<Collider2D> wheelCols = new List<Collider2D>();

    public void BuildFromData(Vector2 worldStart)
    {
        var v = Data.vehicle;
        var lib = Data.lib;

        var bodyGo = new GameObject("Body", typeof(SpriteRenderer), typeof(Rigidbody2D));
        var sr = bodyGo.GetComponent<SpriteRenderer>();
        sr.sprite = v.frameType == FrameType.Car ? lib.bodyCarSprite : lib.bodyBikeSprite;
        sr.sortingOrder = 10;
        bodyGo.transform.position = new Vector3(worldStart.x, worldStart.y, 0f);
        bodyGo.transform.localScale = Vector3.one * Mathf.Max(0.05f, globalScale);

        bodyRb = bodyGo.GetComponent<Rigidbody2D>();
        bodyRb.mass = Mathf.Max(0.1f, v.bodyMass) * globalScale;
        bodyRb.gravityScale = gravityScale;
        bodyRb.angularDamping = 1.2f;
        bodyRb.interpolation = RigidbodyInterpolation2D.Interpolate;
        bodyRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        bodyRb.centerOfMass = (v.centerOfMassOffset + new Vector2(0f, -0.12f)) * globalScale;
        bodyRb.freezeRotation = true;

        wheelRbs.Clear();
        hinges.Clear();
        wheelRadius.Clear();
        driveMask.Clear();
        wheelCols.Clear();
        wheelItemIndices.Clear();

        for (int i = 0; i < v.wheels.Count; i++)
        {
            var wd = v.wheels[i];
            var item = lib.wheelItems[Mathf.Clamp(wd.itemIndex, 0, lib.wheelItems.Count - 1)];

            var wheelGo = new GameObject("Wheel_" + i, typeof(SpriteRenderer), typeof(Rigidbody2D));
            var wr = wheelGo.GetComponent<SpriteRenderer>();
            wr.sprite = item.sprite;
            wr.sortingOrder = 11;

            float saved = wd.scale;
            float baseScale = saved > 0.0001f ? saved : ComputeFallbackScale(item.sprite, item.scale);
            float s = Mathf.Max(0.01f, baseScale) * Mathf.Max(0.05f, globalScale);
            wheelGo.transform.localScale = Vector3.one * s;

            Vector3 wp = bodyGo.transform.TransformPoint((Vector3)(wd.localPos * globalScale));
            wheelGo.transform.position = new Vector3(wp.x, wp.y, 0f);

            var wheelRb = wheelGo.GetComponent<Rigidbody2D>();
            wheelRb.gravityScale = gravityScale;
            wheelRb.angularDamping = item.angularDrag;
            wheelRb.interpolation = RigidbodyInterpolation2D.Interpolate;
            wheelRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            float rEst = Mathf.Max(wr.bounds.extents.x, wr.bounds.extents.y);
            wheelRadius.Add(Mathf.Max(0.01f, rEst));

            Collider2D col;
            bool forceCircle = false;
            if (wr.sprite && wr.sprite.name.ToLower().Contains("wheel2")) forceCircle = true;

            if (forceCircle || item.colliderMode == WheelColliderMode.Circle)
            {
                var c = wheelGo.AddComponent<CircleCollider2D>();
                c.radius = rEst / s;
                c.sharedMaterial = MakeMat(Mathf.Min(0.45f, item.baseFriction));
                col = c;
            }
            else if (item.colliderMode == WheelColliderMode.Box)
            {
                var c = wheelGo.AddComponent<BoxCollider2D>();
                c.size = wr.bounds.size / s;
                c.sharedMaterial = MakeMat(item.baseFriction);
                col = c;
            }
            else
            {
                var c = wheelGo.AddComponent<PolygonCollider2D>();
                c.autoTiling = false;
                c.usedByComposite = false;
                c.sharedMaterial = MakeMat(Mathf.Min(0.55f, item.baseFriction));
                col = c;
            }
            wheelCols.Add(col);

            var hj = wheelGo.AddComponent<HingeJoint2D>();
            hj.connectedBody = bodyRb;
            hj.autoConfigureConnectedAnchor = false;
            hj.connectedAnchor = bodyGo.transform.InverseTransformPoint(wheelGo.transform.position);
            hj.enableCollision = false;
            var m = hj.motor;
            m.motorSpeed = 0f;
            m.maxMotorTorque = Mathf.Max(driveMaxTorque, item.maxTorque);
            hj.motor = m;
            hj.useMotor = false;

            wheelRbs.Add(wheelRb);
            hinges.Add(hj);
            driveMask.Add(true);
            wheelItemIndices.Add(wd.itemIndex);
        }

        for (int i = 0; i < wheelCols.Count; i++)
            for (int j = i + 1; j < wheelCols.Count; j++)
                Physics2D.IgnoreCollision(wheelCols[i], wheelCols[j], true);

        UpdateDriveMask();
        ApplyMotorState();
    }

    float ComputeFallbackScale(Sprite sp, float itemScale)
    {
        if (!sp) return 1f;
        float dia = Mathf.Max(sp.bounds.size.x, sp.bounds.size.y);
        if (dia <= 0.0001f) return 1f;
        float s = (FallbackTargetDiameter / dia) * Mathf.Max(0.0001f, itemScale);
        return Mathf.Clamp(s, FallbackMinScale, FallbackMaxScale);
    }

    PhysicsMaterial2D MakeMat(float f)
    {
        var m = new PhysicsMaterial2D("w" + f.ToString("0.00"));
        m.friction = Mathf.Clamp01(f);
        m.bounciness = 0f;
        return m;
    }

    void FixedUpdate()
    {
        if (!bodyRb) return;

        UpdateDriveMask();
        ApplyMotorState();

        if (driveOn)
        {
            float lin = bodyRb.linearVelocity.magnitude;
            float angAvg = 0f;
            for (int i = 0; i < wheelRbs.Count; i++) angAvg += Mathf.Abs(wheelRbs[i].angularVelocity);
            angAvg /= Mathf.Max(1, wheelRbs.Count);
            if (lin < unstickSpeedThreshold && angAvg < 15f) unstickTimer += Time.fixedDeltaTime; else unstickTimer = 0f;
            if (unstickTimer >= unstickTime)
            {
                bodyRb.AddForce(unstickImpulse, ForceMode2D.Impulse);
                unstickTimer = 0f;
            }
        }
        else unstickTimer = 0f;
    }

    void UpdateDriveMask()
    {
        for (int i = 0; i < driveMask.Count; i++) driveMask[i] = true;
        int n = wheelRbs.Count;
        for (int i = 0; i < n; i++)
        {
            var pi = wheelRbs[i].position; float ri = wheelRadius[i];
            for (int j = i + 1; j < n; j++)
            {
                var pj = wheelRbs[j].position; float rj = wheelRadius[j];
                float r = Mathf.Min(ri, rj) * overlapFactor;
                if ((pi - pj).sqrMagnitude <= r * r)
                {
                    int bottom = pi.y <= pj.y ? i : j;
                    int other = bottom == i ? j : i;
                    driveMask[other] = false;
                    driveMask[bottom] = true;
                }
            }
        }
    }

    void ApplyMotorState()
    {
        for (int i = 0; i < hinges.Count; i++)
        {
            var hj = hinges[i];
            var m = hj.motor;
            m.motorSpeed = currentMotorSpeed;
            hj.motor = m;
            hj.useMotor = driveOn && driveMask[i];
        }
    }

    public void SetMotor(bool on) { driveOn = on; ApplyMotorState(); }
    public void SetMotorSpeedCW(float degreesPerSec) { currentMotorSpeed = -Mathf.Abs(degreesPerSec); ApplyMotorState(); }
    public void SetMotorSpeedCCW(float degreesPerSec) { currentMotorSpeed = Mathf.Abs(degreesPerSec); ApplyMotorState(); }
    public float XPos() { return bodyRb ? bodyRb.position.x : 0f; }
}

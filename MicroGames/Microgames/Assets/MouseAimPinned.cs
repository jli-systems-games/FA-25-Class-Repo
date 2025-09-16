using UnityEngine;

public class MouseAimPinned : MonoBehaviour
{
    [Header("Refs")]
    public Transform modelRoot; // 拖入 BottleRoot
    public Transform baseMarker; // 拖入瓶底
    public Transform muzzle; // 拖入瓶嘴
    public Camera cam; // 拖入主相机

    public Vector2 AimPoint { get; private set; }

    void Reset()
    {
        cam = Camera.main;
        if (!modelRoot) modelRoot = transform;
        if (!baseMarker) baseMarker = modelRoot;
    }

    void LateUpdate()
    {
        if (!cam || !muzzle || !modelRoot || !baseMarker) return;

        // 获取鼠标世界位置
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.forward, Vector3.zero); // Z=0 平面
        if (plane.Raycast(ray, out float t))
        {
            Vector3 mouseWorld = ray.GetPoint(t);
            AimPoint = new Vector2(mouseWorld.x, mouseWorld.y);

            // 瓶子直接朝向鼠标
            Vector2 direction = AimPoint - (Vector2)baseMarker.position;
            if (direction.sqrMagnitude > 0.01f)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f; // 初始向下
                modelRoot.localRotation = Quaternion.Euler(0, 0, angle);
            }
        }

        // 钉住底点（保持瓶底位置）
        Vector2 before = (Vector2)baseMarker.position;
        Vector2 after = (Vector2)baseMarker.position;
        modelRoot.position += (Vector3)(before - after);
    }
}
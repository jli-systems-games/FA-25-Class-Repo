using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Camera))]
public class GoalPhotoCheck : MonoBehaviour
{
    public KeyCode photoKey = KeyCode.F;
    public LayerMask mask = ~0;
    public float maxDistance = 120f;
    public TMP_Text uiText;     // 用 TMP_Text（可留空）

    PhotoFramingFX fx;

    void Awake() { fx = GetComponent<PhotoFramingFX>(); }

    void Update()
    {
        if (Input.GetKeyDown(photoKey))
            StartCoroutine(ShootRoutine());
    }

    IEnumerator ShootRoutine()
    {
        if (fx) yield return fx.ZoomIn();   // 2 秒慢慢变焦
        FindObjectOfType<PlayerSfx>()?.PlayPhoto();
        var cam = GetComponent<Camera>();
        var ray = new Ray(cam.transform.position, cam.transform.forward);
        if (Physics.Raycast(ray, out var hit, maxDistance, mask, QueryTriggerInteraction.Ignore))
        {
            if (hit.collider.GetComponentInParent<RareObjective>())
            {
                if (uiText) uiText.text = "📸 Nice shot! Loading next level…";
                FindObjectOfType<LevelDirector>()?.CompleteLevel();
            }
            else
            {
                if (uiText) uiText.text = "Not the target. Try again!";
            }
        }
        else
        {
            if (uiText) uiText.text = "Too far / nothing in sight.";
        }

        if (fx) yield return fx.ZoomOut();  // 0.2 秒回弹
    }
}

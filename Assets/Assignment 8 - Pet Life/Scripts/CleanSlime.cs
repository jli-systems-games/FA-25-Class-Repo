using System.Collections;
using UnityEngine;

public class CleanSlime : MonoBehaviour
{
    public Material dirtMaterial;

    public LayerMask slimeDirtLayer;
    public float cleaningAmount = 0.1f;

    private Vector2 currentPos;
    private Vector2 lastPos;
    private bool isMouseMoving = false;

    public GameStat statAsset;

    private void Start()
    {
        lastPos = Input.mousePosition;
    }

    void Update()
    {
        if (Data.isCleanMode)
        {
            if (Input.GetMouseButton(0))
            {
                Debug.Log("Mouse down");

                if (RaycastHitSlime())
                {
                    Debug.Log("Raycast hit slime");

                    currentPos = Input.mousePosition;

                    if (currentPos != lastPos)
                    {
                        isMouseMoving = true;
                    }
                    else
                    {
                        isMouseMoving = false;
                    }

                    if (isMouseMoving)
                    {
                        Debug.Log("clean swipe occured");
                        CleanSlimeFlick();
                    }

                    lastPos = currentPos;
                }
            }
        }

        UpdateDirtinessVisual();
    }

    private bool RaycastHitSlime()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, slimeDirtLayer))
        {
            return hit.transform == transform;
        }
        return false;
    }

    private void CleanSlimeFlick()
    {
        statAsset.dirtStat += cleaningAmount;

        if (statAsset.dirtStat > 100)
        {
            statAsset.dirtStat = 100;
        }

        Debug.Log(statAsset.dirtStat);
    }

    private void UpdateDirtinessVisual()
    {
        Color dirtColor = dirtMaterial.color;

        dirtColor.a = 1 - statAsset.dirtStat / 100f;

        dirtMaterial.color = dirtColor;
    }
}

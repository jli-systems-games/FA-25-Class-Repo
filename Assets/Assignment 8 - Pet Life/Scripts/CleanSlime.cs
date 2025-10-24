using System.Collections;
using UnityEngine;

public class CleanSlime : MonoBehaviour
{
    public Material dirtMaterial;

    public LayerMask slimeDirtLayer;

    private Vector2 currentPos;
    private Vector2 lastPos;
    private bool isMouseMoving = false;

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

            UpdateDirtinessVisual();
        }
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
        Data.dirtStat += 0.1f;

        if (Data.dirtStat > 100)
        {
            Data.dirtStat = 100;
        }

        Debug.Log(Data.dirtStat);
    }

    private void UpdateDirtinessVisual()
    {
        Color dirtColor = dirtMaterial.color;

        dirtColor.a = 1 - Data.dirtStat / 100f;

        dirtMaterial.color = dirtColor;
    }
}

using UnityEngine;

public class RaycastLogic : MonoBehaviour
{
    public float interactionDistance = 3f;
    public LayerMask interactableLayer;

    private void Start()
    {
        Data.isPlantFound = false;
        Data.isMushroomFound = false;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * interactionDistance, Color.red);

        if (Physics.Raycast(ray, out hit, interactionDistance, interactableLayer))
        {
            if (hit.collider.CompareTag("Plant"))
            {
                Debug.Log("Plant found!");
                ResetFoundBool();
                Data.isPlantFound = true;
            }
            else if (hit.collider.CompareTag("Mushroom"))
            {
                Debug.Log("mushroom found!");
                ResetFoundBool();
                Data.isMushroomFound = true;
            }
            else
            {
                ResetFoundBool();
            }
        }
        else
        {
            ResetFoundBool();
        }
    }

    void ResetFoundBool()
    {
        Data.isPlantFound = false;
        Data.isMushroomFound = false;

        Debug.Log("reset bool");
    }
}
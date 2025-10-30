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

        if (Data.capturePhoto) //make raycast only happen when you capture photo, so that camera snapping doesn't happen
        {
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
                else if (hit.collider.CompareTag("Nibu"))
                {
                    Debug.Log("Nibu found!");
                    ResetFoundBool();
                    Data.isNibuFound = true;
                }
                else if (hit.collider.CompareTag("Egg"))
                {
                    Debug.Log("Egg found!");
                    ResetFoundBool();
                    Data.isEggFound = true;
                }
                else if (hit.collider.CompareTag("Baby"))
                {
                    Debug.Log("Baby found!");
                    ResetFoundBool();
                    Data.isBabyFound = true;
                }
                else if (hit.collider.CompareTag("House"))
                {
                    Debug.Log("House found!");
                    ResetFoundBool();
                    Data.isHouseFound = true;
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
        else
        {
            return;
        }
    }

    void ResetFoundBool()
    {
        Data.isPlantFound = false;
        Data.isMushroomFound = false;
        Data.isNibuFound = false;
        Data.isEggFound = false;
        Data.isBabyFound = false;
        Data.isHouseFound = false;

        Debug.Log("reset bool");
    }
}
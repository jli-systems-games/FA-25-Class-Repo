using UnityEngine;

public class WeaponsRandomizer : MonoBehaviour
{
    public GameObject sawBladeSingle;
    public GameObject sawBladeMultipleCenter;
    public GameObject sawBladeMultipleRight;
    public GameObject sawBladeMultipleLeft;
    public GameObject sawBladeGround;
    public GameObject needleTrapStationary;
    public GameObject needleTrap;
    public GameObject greatAxe;

    void Start()
    {
        int randomWeaponIndex = Random.Range(0, 8);

        if (randomWeaponIndex == 0)
        {
            sawBladeSingle.SetActive(true);
        }
        else if (randomWeaponIndex == 1)
        {
            sawBladeMultipleCenter.SetActive(true);
        }
        else if (randomWeaponIndex == 2)
        {
            sawBladeMultipleRight.SetActive(true);
        }
        else if (randomWeaponIndex == 3)
        {
            sawBladeMultipleLeft.SetActive(true);
        }
        else if (randomWeaponIndex == 4)
        {
            sawBladeGround.SetActive(true);
        }
        else if (randomWeaponIndex == 5)
        {
            needleTrapStationary.SetActive(true);
        }
        else if (randomWeaponIndex == 6)
        {
            needleTrap.SetActive(true);
        }
        else
        {
            greatAxe.SetActive(true);
        }
    }
}

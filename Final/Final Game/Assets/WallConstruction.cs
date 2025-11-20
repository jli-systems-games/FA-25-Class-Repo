using UnityEngine;
using MoreMountains.TopDownEngine;

public class WallConstruction : MonoBehaviour
{
    public Transform WallMesh;
    public float CurrentScore = 0f;
    public float ScorePerLayer = 4f;
    public float HeightPerLayer = 1f;

    private void OnTriggerEnter(Collider other)
    {
        CharacterHandleWeapon characterHandleWeapon = other.GetComponent<CharacterHandleWeapon>();

        if (characterHandleWeapon != null && characterHandleWeapon.CurrentWeapon != null)
        {
            string weaponName = characterHandleWeapon.CurrentWeapon.WeaponName;
            float addScore = 0f;

            if (weaponName == "SmallStone")
            {
                addScore = 1f;
            }
            else if (weaponName == "BigStone")
            {
                addScore = 4f;
            }

            if (addScore > 0)
            {
                CurrentScore += addScore;
                UpdateWallHeight();
                characterHandleWeapon.ChangeWeapon(null, "EmptyHands");
            }
        }
    }

    void UpdateWallHeight()
    {
        if (WallMesh == null) return;
        float targetHeightY = 1f + (CurrentScore / ScorePerLayer) * HeightPerLayer;
        Vector3 newScale = WallMesh.localScale;
        newScale.y = targetHeightY;
        WallMesh.localScale = newScale;
    }
}
using UnityEngine;
using UnityEngine.UI;

public class PhotoPlaceOnResearch : MonoBehaviour
{
    public Image plantDisplayArea;
    public Image mushroomDisplayArea;
    public Image nibuDisplayArea;
    public Image eggDisplayArea;
    public Image babyDisplayArea;
    public Image houseDisplayArea;

    public Sprite plantSprite;
    public Sprite mushroomSprite;
    public Sprite nibuSprite;
    public Sprite eggSprite;
    public Sprite babySprite;
    public Sprite houseSprite;

    private bool picturedPlant = false;
    private bool picturedMushroom = false;
    private bool picturedNibu = false;
    private bool picturedEgg = false;
    private bool picturedBaby = false;
    private bool picturedHouse = false;


    public void Update()
    {
        if (Data.newPhotoTaken)
        {
            if (Data.isPlantFound)
            {

                if (!picturedPlant)
                {
                    plantSprite = CreatePersistentSprite(Data.photoSprite);

                    plantDisplayArea.sprite = plantSprite;
                    Debug.Log("Updated plant image");
                    picturedPlant = true;
                }
            }
            
            if (Data.isMushroomFound)
            {

                if (!picturedMushroom)
                {
                    mushroomSprite = CreatePersistentSprite(Data.photoSprite);

                    mushroomDisplayArea.sprite = mushroomSprite;
                    Debug.Log("Updated mushroom image");
                    picturedMushroom = true;
                }
            }

            if (Data.isNibuFound)
            {

                if (!picturedNibu)
                {
                    nibuSprite = CreatePersistentSprite(Data.photoSprite);

                    nibuDisplayArea.sprite = nibuSprite;
                    Debug.Log("Updated nibu image");
                    picturedNibu = true;
                }
            }

            if (Data.isEggFound)
            {

                if (!picturedEgg)
                {
                    eggSprite = CreatePersistentSprite(Data.photoSprite);

                    eggDisplayArea.sprite = eggSprite;
                    Debug.Log("Updated egg image");
                    picturedEgg = true;
                }
            }

            if (Data.isBabyFound)
            {

                if (!picturedBaby)
                {
                    babySprite = CreatePersistentSprite(Data.photoSprite);

                    babyDisplayArea.sprite = babySprite;
                    Debug.Log("Updated Baby image");
                    picturedBaby = true;
                }
            }

            if (Data.isHouseFound)
            {

                if (!picturedHouse)
                {
                    houseSprite = CreatePersistentSprite(Data.photoSprite);

                    houseDisplayArea.sprite = houseSprite;
                    Debug.Log("Updated House image");
                    picturedHouse = true;
                }
            }

            Data.newPhotoTaken = false;
        }

        if (picturedPlant && picturedMushroom && picturedEgg && picturedBaby && picturedHouse && picturedNibu)
        {
            Data.completedResearch = true;
        }
    }

    private Sprite CreatePersistentSprite(Sprite sourceSprite)
    {
        if (sourceSprite == null || sourceSprite.texture == null)
        {
            Debug.LogError("source sprite or texture is null");
            return null;
        }

        Texture2D sourceTex = sourceSprite.texture;

        Texture2D newTex = new Texture2D(sourceTex.width, sourceTex.height, sourceTex.format, false);

        newTex.SetPixels(sourceTex.GetPixels());
        newTex.Apply();

        Sprite newSprite = Sprite.Create(
            newTex,
            new Rect(0.0f, 0.0f, newTex.width, newTex.height),
            new Vector2(0.5f, 0.5f),
            sourceSprite.pixelsPerUnit
        );

        return newSprite;
    }
}

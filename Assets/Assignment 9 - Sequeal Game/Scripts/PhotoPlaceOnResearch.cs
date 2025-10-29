using UnityEngine;
using UnityEngine.UI;

public class PhotoPlaceOnResearch : MonoBehaviour
{
    public Image plantDisplayArea;
    public Image mushroomDisplayArea;

    public Sprite plantSprite;
    public Sprite mushroomSprite;

    private bool picturedPlant = false;
    private bool picturedMushroom = false;


    public void Update()
    {
        if (Data.newPhotoTaken)
        {
            if (Data.isPlantFound)
            {
                Debug.Log(picturedPlant);

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
                Debug.Log(picturedMushroom);

                if (!picturedMushroom)
                {
                    mushroomSprite = CreatePersistentSprite(Data.photoSprite);

                    mushroomDisplayArea.sprite = mushroomSprite;
                    Debug.Log("Updated mushroom image");
                    picturedMushroom = true;
                }
            }

            Data.newPhotoTaken = false;
        }
    }

    private Sprite CreatePersistentSprite(Sprite sourceSprite)
    {
        if (sourceSprite == null || sourceSprite.texture == null)
        {
            Debug.LogError("Source Sprite or Texture is null! Cannot create persistent copy.");
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

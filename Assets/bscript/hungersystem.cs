using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class HungerSystem : MonoBehaviour
{
    [Header("Hunger Settings")]
    public float hunger = 100f;
    public float maxHunger = 100f;
    public string sceneName;
    [Header("UI")]
    public Slider hungerSlider;
    public TMP_Text favoriteFoodText; 

    [Header("Favorite Food System")]
    public FoodType favoriteFood;
    private bool favoriteChosen = false;
    private HashSet<FoodType> eatenFoodTypes = new HashSet<FoodType>();

    void Start()
    {
        if (hungerSlider != null)
        {
            hungerSlider.maxValue = maxHunger;
            hungerSlider.value = hunger;
        }

        if (favoriteFoodText != null)
            favoriteFoodText.text = "discover your favorite food!";
    }

    void Update()
    {
        // the hungerbar going down
        hunger -= Time.deltaTime * 5f;
        hunger = Mathf.Clamp(hunger, 0, maxHunger);

        if (hungerSlider != null)
            hungerSlider.value = hunger;
        if(hunger == 0)
            SceneManager.LoadScene(sceneName);
    }

    public void Eat(Food food)
    {
        if (food == null) return;

        // kind of foods eaten
        eatenFoodTypes.Add(food.type);

  
        if (!favoriteChosen && eatenFoodTypes.Count >= 5)
        {
            FoodType[] types = new List<FoodType>(eatenFoodTypes).ToArray();
            favoriteFood = types[Random.Range(0, types.Length)];
            favoriteChosen = true;

            if (favoriteFoodText != null)
                favoriteFoodText.text = "favorite food: " + favoriteFood + "!";

            Debug.Log("Favorite food chosen: " + favoriteFood);
        }

        // hunger refill
        float refillAmount;

        if (!favoriteChosen)
        {
            
            refillAmount = 20f;
        }
        else
        {
           
            if (food.type == favoriteFood)
            {
                refillAmount = 30f; 
            }
            else
            {
                refillAmount = 5f; 
            }
        }

        hunger += refillAmount;
        hunger = Mathf.Clamp(hunger, 0, maxHunger);

        if (hungerSlider != null)
            hungerSlider.value = hunger;

        Debug.Log($"Ate {food.type}, +{refillAmount} hunger. Current hunger: {hunger}");
    }
}

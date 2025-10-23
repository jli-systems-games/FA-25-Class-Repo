using UnityEngine;
using UnityEngine.UI;

public class PetUI : MonoBehaviour
{
    public PetData petData;
    public Slider hungerBar;
    public Slider cleanBar;
    public Slider happyBar;

    private void Update()
    {
        hungerBar.value = petData.hunger / 100f;
        cleanBar.value = petData.cleanliness / 100f;
        happyBar.value = petData.happiness / 100f;
    }
}

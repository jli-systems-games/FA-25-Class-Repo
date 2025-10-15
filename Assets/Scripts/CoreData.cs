using PixelCrushers.DialogueSystem;

public static class CoreData
{
    public static int courage = 1;
    public static int logic = 1;
    public static int empathy = 1;
    public static int tech = 1;
    public static int remaining = 6;
    
    public static string setupSceneName = "Setup Menu";
    private static bool isInitialized = false;
    public static void Initialize()

    {
        if (isInitialized) return;
        
        //register functions to lua, a simplified coding system used in dialogue system
        Lua.RegisterFunction("CheckCourage", null, typeof(CoreData).GetMethod("CheckCourage"));
        Lua.RegisterFunction("CheckLogic", null, typeof(CoreData).GetMethod("CheckLogic"));
        Lua.RegisterFunction("CheckEmpathy", null, typeof(CoreData).GetMethod("CheckEmpathy"));
        Lua.RegisterFunction("CheckTech", null, typeof(CoreData).GetMethod("CheckTech"));
        
        isInitialized = true;
        UnityEngine.Debug.Log("CoreData initialized");
    }
    
    public static void Reset()
    {
        courage = 1;
        logic = 1;
        empathy = 1;
        tech = 1;
        remaining = 6;
    }

    //check if the player's attributes are greater than the threshold
    public static bool CheckCourage(double threshold) { return courage >= threshold; }
    public static bool CheckLogic(double threshold) { return logic >= threshold; }
    public static bool CheckEmpathy(double threshold) { return empathy >= threshold; }
    public static bool CheckTech(double threshold) { return tech >= threshold; }
}
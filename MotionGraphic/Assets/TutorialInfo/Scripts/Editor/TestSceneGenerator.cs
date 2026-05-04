using UnityEngine;
using UnityEditor;

public class TestSceneGenerator : EditorWindow
{
    [MenuItem("Tools/Generate Test Scene")]
    public static void ShowWindow()
    {
        GetWindow<TestSceneGenerator>("Test Scene Generator");
    }
    
    private Vector2 floorSize = new Vector2(20, 20);
    private int interactableCount = 5;
    private int collectableCount = 3;
    private bool addObstacles = true;
    private Color groundColor = new Color(0.3f, 0.3f, 0.3f);
    
    void OnGUI()
    {
        GUILayout.Label("Scene Generation Settings", EditorStyles.boldLabel);
        
        EditorGUILayout.Space();
        
        floorSize = EditorGUILayout.Vector2Field("Floor Size", floorSize);
        interactableCount = EditorGUILayout.IntSlider("Interactable Objects", interactableCount, 0, 20);
        collectableCount = EditorGUILayout.IntSlider("Collectables", collectableCount, 0, 20);
        addObstacles = EditorGUILayout.Toggle("Add Obstacles", addObstacles);
        groundColor = EditorGUILayout.ColorField("Ground Color", groundColor);
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Generate Scene", GUILayout.Height(40)))
        {
            GenerateScene();
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Clear Scene", GUILayout.Height(30)))
        {
            ClearScene();
        }
    }
    
    void GenerateScene()
    {
        // Clear existing test objects
        ClearScene();
        
        // Create parent container
        GameObject sceneRoot = new GameObject("TestScene");
        
        // 1. Create ground
        GameObject ground = CreateGround(sceneRoot.transform);
        
        // 2. Create player
        GameObject player = CreatePlayer(sceneRoot.transform);
        
        // 3. Create camera
        CreateCamera(player.transform);
        
        // 4. Create interactable objects
        CreateInteractables(sceneRoot.transform);
        
        // 5. Create collectables
        CreateCollectables(sceneRoot.transform);
        
        // 6. Create obstacles (optional)
        if (addObstacles)
        {
            CreateObstacles(sceneRoot.transform);
        }
        
        // 7. Setup lighting
        SetupLighting();
        
        Debug.Log("✨ Test scene generated successfully!");
        
        // Select the player in hierarchy
        Selection.activeGameObject = player;
    }
    
    GameObject CreateGround(Transform parent)
    {
        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.parent = parent;
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = new Vector3(floorSize.x / 10f, 1, floorSize.y / 10f);
        
        // Create and assign material
        Material groundMat = new Material(Shader.Find("Standard"));
        groundMat.color = groundColor;
        ground.GetComponent<Renderer>().material = groundMat;
        
        // Setup layer
        ground.layer = LayerMask.NameToLayer("Default");
        
        return ground;
    }
    
    GameObject CreatePlayer(Transform parent)
    {
        GameObject player = new GameObject("Player");
        player.transform.parent = parent;
        player.transform.position = new Vector3(0, 2, 0);
        
        // Add visual representation (capsule)
        GameObject playerModel = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        playerModel.name = "PlayerModel";
        playerModel.transform.parent = player.transform;
        playerModel.transform.localPosition = Vector3.zero;
        
        // Remove collider from model (CharacterController will handle collision)
        DestroyImmediate(playerModel.GetComponent<Collider>());
        
        // Color the player
        Material playerMat = new Material(Shader.Find("Standard"));
        playerMat.color = new Color(0.2f, 0.6f, 1f); // Blue
        playerModel.GetComponent<Renderer>().material = playerMat;
        
        // Add CharacterController
        CharacterController controller = player.AddComponent<CharacterController>();
        controller.radius = 0.5f;
        controller.height = 2f;
        controller.center = Vector3.zero;
        
        // Add PlayerController script
        player.AddComponent<PlayerController>();
        
        return player;
    }
    
    void CreateCamera(Transform playerTransform)
    {
        // Try to find existing main camera
        Camera mainCam = Camera.main;
        GameObject cameraObj;
        
        if (mainCam != null)
        {
            cameraObj = mainCam.gameObject;
        }
        else
        {
            cameraObj = new GameObject("Main Camera");
            cameraObj.tag = "MainCamera";
            cameraObj.AddComponent<Camera>();
            cameraObj.AddComponent<AudioListener>();
        }
        
        // Position camera
        cameraObj.transform.parent = playerTransform;
        cameraObj.transform.localPosition = new Vector3(0, 0.6f, 0);
        cameraObj.transform.localRotation = Quaternion.identity;
        
        // Add simple mouse look (optional - you might have your own)
        SimpleCameraController camController = cameraObj.GetComponent<SimpleCameraController>();
        if (camController == null)
        {
            camController = cameraObj.AddComponent<SimpleCameraController>();
        }
    }
    
    void CreateInteractables(Transform parent)
    {
        GameObject interactablesParent = new GameObject("Interactables");
        interactablesParent.transform.parent = parent;
        
        for (int i = 0; i < interactableCount; i++)
        {
            // Random position within floor bounds
            Vector3 randomPos = new Vector3(
                Random.Range(-floorSize.x / 2 + 2, floorSize.x / 2 - 2),
                0.5f,
                Random.Range(-floorSize.y / 2 + 2, floorSize.y / 2 - 2)
            );
            
            GameObject interactable = GameObject.CreatePrimitive(PrimitiveType.Cube);
            interactable.name = $"Interactable_{i}";
            interactable.transform.parent = interactablesParent.transform;
            interactable.transform.position = randomPos;
            interactable.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            
            // Random color
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
            interactable.GetComponent<Renderer>().material = mat;
            
            // Add interaction script
            interactable.AddComponent<InteractableObject>();
            
            // Set layer
            interactable.layer = LayerMask.NameToLayer("Default");
        }
    }
    
    void CreateCollectables(Transform parent)
    {
        GameObject collectablesParent = new GameObject("Collectables");
        collectablesParent.transform.parent = parent;
        
        for (int i = 0; i < collectableCount; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(-floorSize.x / 2 + 2, floorSize.x / 2 - 2),
                1f,
                Random.Range(-floorSize.y / 2 + 2, floorSize.y / 2 - 2)
            );
            
            GameObject collectable = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            collectable.name = $"Collectable_{i}";
            collectable.transform.parent = collectablesParent.transform;
            collectable.transform.position = randomPos;
            collectable.transform.localScale = Vector3.one * 0.5f;
            
            // Gold color
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(1f, 0.84f, 0f);
            mat.SetFloat("_Metallic", 0.8f);
            mat.SetFloat("_Glossiness", 0.9f);
            collectable.GetComponent<Renderer>().material = mat;
            
            // Add collectable script
            collectable.AddComponent<CollectableItem>();
            
            // Set layer
            collectable.layer = LayerMask.NameToLayer("Default");
        }
    }
    
    void CreateObstacles(Transform parent)
    {
        GameObject obstaclesParent = new GameObject("Obstacles");
        obstaclesParent.transform.parent = parent;
        
        int obstacleCount = Random.Range(3, 8);
        
        for (int i = 0; i < obstacleCount; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(-floorSize.x / 2 + 3, floorSize.x / 2 - 3),
                0,
                Random.Range(-floorSize.y / 2 + 3, floorSize.y / 2 - 3)
            );
            
            // Random obstacle type
            PrimitiveType type = Random.value > 0.5f ? PrimitiveType.Cube : PrimitiveType.Cylinder;
            GameObject obstacle = GameObject.CreatePrimitive(type);
            obstacle.name = $"Obstacle_{i}";
            obstacle.transform.parent = obstaclesParent.transform;
            obstacle.transform.position = randomPos;
            
            // Random size
            float height = Random.Range(1f, 3f);
            float width = Random.Range(0.5f, 2f);
            obstacle.transform.localScale = new Vector3(width, height, width);
            obstacle.transform.position += Vector3.up * (height / 2);
            
            // Dark color
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.2f, 0.2f, 0.2f);
            obstacle.GetComponent<Renderer>().material = mat;
        }
    }
    
    void SetupLighting()
    {
        // Check if there's already a directional light
        Light[] lights = FindObjectsOfType<Light>();
        Light directionalLight = null;
        
        foreach (Light light in lights)
        {
            if (light.type == LightType.Directional)
            {
                directionalLight = light;
                break;
            }
        }
        
        // Create one if it doesn't exist
        if (directionalLight == null)
        {
            GameObject lightObj = new GameObject("Directional Light");
            directionalLight = lightObj.AddComponent<Light>();
            directionalLight.type = LightType.Directional;
        }
        
        // Configure light
        directionalLight.transform.rotation = Quaternion.Euler(50, -30, 0);
        directionalLight.color = Color.white;
        directionalLight.intensity = 1f;
        
        // Set ambient lighting
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.4f, 0.4f, 0.4f);
    }
    
    void ClearScene()
    {
        GameObject testScene = GameObject.Find("TestScene");
        if (testScene != null)
        {
            DestroyImmediate(testScene);
            Debug.Log("Previous test scene cleared.");
        }
    }
}

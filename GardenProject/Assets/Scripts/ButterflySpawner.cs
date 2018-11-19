//--------------------------------------------------------------------------------
/*
 * This Script is used for spawning Butterflies.
 * Allow input to : Restart(Load StartScene and Input User ID again)
 * 
 * Used in Main Scene, attached to Empty GameObject "ButterflySpawner".
 * Require 2 GameObject variables : Butterfly Prefab, All Paths, and 2 Transform variables : Positive/Negative Corners
 * Butterfly Prefab can be found in Prefabs folder under Project Assets.
 * All Paths is found in Hierarchy.
 * The Corners can be found under Range in Hierarchy.
 */
//--------------------------------------------------------------------------------

using UnityEngine;  // Default Unity Script (MonoBehaviour, Header, Tooltip, HideInInspector, GameOject, Transform, FindObjectOfType, Instantiate, Vector3, Quaternion, Random, Destroy, Time, Input, KeyCode)
using UnityEngine.SceneManagement; // For SceneManager

public class ButterflySpawner : MonoBehaviour
{
    [Tooltip("Default: 5")]
    public int TotalNumberOfButterflies = 50;
    [Tooltip("In Prefabs")]
    public GameObject ButterflyPrefab;
    [Tooltip("Default: 1.0f")]
    public float SpawningDelay = 0.25f;
    public GameObject AllPaths;
    [Header("For Randomization of Spawning Position")]
    public Transform NegativeCorner;
    public Transform PositiveCorner;

    [HideInInspector]
    public int CurrentNumberOfButterflies = 0;
    private float SpawningTimer = 0.0f;
    private EditorPath[] Array;

    // Runs before Start()
    private void Awake()
    {
        // If DataCollector is alive
        if (DataCollector.Instance != null)
        {
            // Find the SteamVR eye GameObject and assign it to DataCollector
            DataCollector.Instance.user = FindObjectOfType<SteamVR_Camera>().gameObject;
        }
    }

    // Runs at the start of first frame
    private void Start()
    {
        // Alternative way to get all the children
        Array = AllPaths.GetComponentsInChildren<EditorPath>();

        // Initial spawn Butterflies and Set their Path
        for (int i = 0; i < TotalNumberOfButterflies; ++i)
        {
            Spawn();
        }
    }

    // Spawn Butterfly in Random position within the corners, and Randomize a path for it to follow.
    // Also Randomize Butterfly's flying speed
    private void Spawn()
    {
        // Spawn Butterfly
        GameObject Butterfly = Instantiate(ButterflyPrefab, Vector3.zero, Quaternion.identity);
        CurrentNumberOfButterflies++;

        // Randomize Spawning position
        Butterfly.transform.position = new Vector3(
            Random.Range(NegativeCorner.position.x, PositiveCorner.position.x),
            Random.Range(NegativeCorner.position.y, PositiveCorner.position.y),
            Random.Range(NegativeCorner.position.z, PositiveCorner.position.z));

        // Randomly choose a Path from All Paths
        int RandomNumber = Random.Range(0, Array.Length);

        // Randomize Butterfly's Flying Speed & Rotation Speed
        FlyOnPath Path = Butterfly.GetComponent<FlyOnPath>();
        Path.Speed = Random.Range(Path.Speed - 3.0f, Path.Speed);
        Path.RotationSpeed = Random.Range(Path.RotationSpeed - 3.0f, Path.RotationSpeed);

        // Set Path in FlyOnPath Script
        Path.PathToFollow = Array[RandomNumber].GetComponent<EditorPath>();
    }

    // Loads StartScene
    private void Restart()
    {
        SceneManager.LoadScene("StartScene");
        // Destroy current DataCollector Instance, as StartScene will have its new instance of DataCollector
        Destroy(DataCollector.Instance.gameObject);
    }

    // Update is called once per frame
    private void Update()
    {
        // Spawn Butterfly when Current Number of Butterflies is less than Total Number of Butterflies allowed
        if(CurrentNumberOfButterflies < TotalNumberOfButterflies)
        {
            SpawningTimer += Time.deltaTime;

            // When Reached Spawning Time
            if(SpawningTimer >= SpawningDelay)
            {
                Spawn();

                // Reset Timer
                SpawningTimer = 0.0f;
            }
        }

        // Proceed to Restart if 'R' is pressed
        if (Input.GetKey(KeyCode.R))
        {
            Restart();
        }
    }
}

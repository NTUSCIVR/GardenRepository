using UnityEngine;

public class ButterflySpawner : MonoBehaviour
{
    [Tooltip("Default: 5")]
    public int TotalNumberOfButterflies = 5;
    [Tooltip("In Butterfly(LandNTurn) -> Prefab")]
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

    private void Update()
    {
        if(CurrentNumberOfButterflies < TotalNumberOfButterflies)
        {
            SpawningTimer += Time.deltaTime;
            if(SpawningTimer >= SpawningDelay)
            {
                Spawn();
                SpawningTimer = 0.0f;
            }
        }
    }
}

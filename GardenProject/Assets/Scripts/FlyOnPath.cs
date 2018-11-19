//--------------------------------------------------------------------------------
/*
 * This Script is used for Butterflies to fly while following Path given when spawn.
 * 
 * Used in Main Scene, it's already attached to Butterfly Prefab in Prefabs folder under Project Assets.
 */
//--------------------------------------------------------------------------------

using UnityEngine;  // Default Unity Script (MonoBehaviour, GameObject, Tooltip, HideInInspector, Time, Destroy, Vector3, Trnasform, Quaternion)

public class FlyOnPath : MonoBehaviour
{
    [Tooltip("Flying Speed. Default: 5.0f")]
    public float Speed = 5.0f;
    [Tooltip("Default: 5.0f")]
    public float RotationSpeed = 5.0f;
    [Tooltip("Distance between Butterfly and Point. Default: 0.1f")]
    public float ReachDistance = 0.1f;
    public float FlightTime = 10.0f;

    [HideInInspector]
    public EditorPath PathToFollow;
    private int CurrentPointID = 0;
    private float FlightTimer = 0.0f;
    ButterflySpawner spawner;

    // Runs at the start of first frame
    private void Start()
    {
        spawner = GameObject.Find("ButterflySpawner").GetComponent<ButterflySpawner>();
    }

    // Update is called once per frame
    private void Update ()
    {
        FlightTimer += Time.deltaTime * Speed;

        // Kill Butterfly when its timer is up
        if(FlightTimer >= FlightTime)
        {
            // Kill Butterfly
            Destroy(gameObject);
            spawner.CurrentNumberOfButterflies--;
        }

        // Calculate Distance between Butterfly and Currently Target Point
        float Distance = Vector3.Distance(PathToFollow.PathObjects[CurrentPointID].position, transform.position);
        
        // Fly towards Currently Target Point
        transform.position = Vector3.MoveTowards(transform.position, PathToFollow.PathObjects[CurrentPointID].position, Speed * Time.deltaTime);

        // Rotate to face Currently Target Point
        Quaternion Rotation = Quaternion.LookRotation(PathToFollow.PathObjects[CurrentPointID].position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, Rotation, Time.deltaTime * RotationSpeed);

        // Set next Point when reached Currently Target Point
        if(Distance <= ReachDistance)
        {
            CurrentPointID++;
        }

        // Reset Target Point when reached the end of Path
        if(CurrentPointID >= PathToFollow.PathObjects.Count)
        {
            CurrentPointID = 0;
        }
	}
}

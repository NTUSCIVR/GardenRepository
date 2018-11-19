//--------------------------------------------------------------------------------
/*
 * This Script is used for visual feedback of drawing path(Developer)
 * 
 * Used in Main Scene, attached to Empty GameObject "FlyingPath_X", with X being the number of the path.
 * FlyingPath_X should contain multiple Empty GameObject(Which serves as point for Butterfly to fly towards)
 * And should be AllPaths found in Hierarchy.
 */
//--------------------------------------------------------------------------------

using UnityEngine;  // Default Unity Script (MonoBehaviour, Tooltip, HideInInspector, Color, Transform, Gizmos, GetComponentsInChildren, Vector3)
using System.Collections.Generic; // For List<>

public class EditorPath : MonoBehaviour
{
    [Tooltip("Used to draw Path and Points. Default: White")]
    public Color DrawingColor = Color.white;
    [Tooltip("Size of Points. Default: 0.3f")]
    public float PointSize = 0.3f;

    [HideInInspector]
    public List<Transform> PathObjects = new List<Transform>();
    private Transform[] Array;

    private void OnDrawGizmos()
    {
        // Set Gizmos color to DrawingColor
        Gizmos.color = DrawingColor;

        // Get all Transforms in children and put in Array
        Array = GetComponentsInChildren<Transform>();

        // Reset List of PathObjects
        PathObjects.Clear();

        // Add Points into List
        foreach (Transform PathObject in Array)
        {
            if(PathObject != this.transform)
            {
                PathObjects.Add(PathObject);
            }
        }

        // Draw Paths & Points
        for (int i = 0; i < PathObjects.Count; i++)
        {
            Vector3 Position = PathObjects[i].position;
            if (i > 0)
            {
                Vector3 Previous = PathObjects[i - 1].position;
                Gizmos.DrawLine(Previous, Position);
                Gizmos.DrawWireSphere(Position, PointSize);
            }
        }
    }
}

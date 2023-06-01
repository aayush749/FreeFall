using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class InfiniteScroller : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> planes;

    // a reference to the player motion controller, to know the index of the plane the player currently is in
    [SerializeField]
    PlayerMovementController playerMovementController;

    GameObject oldPlane;

    // Start is called before the first frame update
    void Start()
    {
        // Subscribe to OnFallOffDetect Event for the player
        playerMovementController.OnFallOffDetect += ShufflePlanes;
        playerMovementController.OnEnterNewPlane += DestroyOldPlane;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    private void ShufflePlanes(object sender, EventArgs args)
    {
        // collided with the fall off detection cube
        Debug.LogWarning("Collided with the fall-off plane");


        GameObject currentPlane = playerMovementController.GetCurrentPlane();

        var length = currentPlane.GetComponent<MeshRenderer>().bounds.size.z;
        var transform = currentPlane.transform;

        var angle = 90 - currentPlane.transform.rotation.eulerAngles.x;

        Vector3 newPos = new Vector3(
            transform.position.x, transform.position.y - length * (float) Math.Sin(ToRadians(angle)) / 1.5f,
            transform.position.z + 2 * length * (float) Math.Cos(ToRadians(angle))
            );

        // Choose a random plane from the list of current planes
        int idx = GetRandomPlaneIdxToInstantiate();

        GameObject newPlane = Instantiate<GameObject>(planes[idx], newPos, currentPlane.transform.rotation) as GameObject;

        // Set the current plane in playerMovementController
        playerMovementController.SetCurrentPlane(newPlane);

        // Set the old plane in this script to be the currentPlane,
        // which would be destroyed when player enters currently created new plane, and the OnEnterNewPlane method is triggered
        oldPlane = currentPlane;

//#if UNITY_EDITOR
//        UnityEditor.EditorApplication.isPaused = true;
//#endif
    }

    private void DestroyOldPlane(object sender, EventArgs e)
    {
//#if UNITY_EDITOR
//        UnityEditor.EditorApplication.isPaused = true;
//#endif
        Debug.LogWarning("Entering new plane");
        if (oldPlane != null)
        {
            Destroy(oldPlane);
            Debug.LogWarning("Destroyed old plane");
        }
    }

    private int GetRandomPlaneIdxToInstantiate()
    {
        return (int) Mathf.Round(
            UnityEngine.Random.Range(0.0f, planes.Count - 1));
    }

    public static double ToRadians(double angleInDegrees)
    {
        return (Math.PI / 180) * angleInDegrees;
    }
}

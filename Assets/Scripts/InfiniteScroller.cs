using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteScroller : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> planes;

    // a reference to the player motion controller, to know the index of the plane the player currently is in
    [SerializeField]
    PlayerMovementController playerMovementController;

    GameObject oldPlane;

    [SerializeField]
    private int planesToPassBeforeBeginDestroying = 2;

    private int planeDestroyEventsRaised = 0;

    Queue<GameObject> planesToDestroyQueue;

    private Coroutine oldPlanesDestroyerCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        // Subscribe to OnFallOffDetect Event for the player
        playerMovementController.OnFallOffDetect += ShufflePlanes;
        playerMovementController.OnEnterNewPlane += DestroyOldPlane;

        planesToDestroyQueue = new Queue<GameObject>();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    private void ShufflePlanes(object sender, EventArgs args)
    {
        // collided with the fall off detection cube

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

        planeDestroyEventsRaised++;

        // Destroy the plane only when specified number of planes have passed behind
        bool shouldBeginDestroyingPlanes = (planeDestroyEventsRaised % planesToPassBeforeBeginDestroying == 0);

        if (oldPlane != null && shouldBeginDestroyingPlanes)
        {
            oldPlanesDestroyerCoroutine = StartCoroutine(DestoryOldPlaneCoroutine());
        }
        else
        {
            if (oldPlane != null)
            {
                // enqueue the plane for destruction
                planesToDestroyQueue.Enqueue(oldPlane);
            }
        }
    }

    private IEnumerator DestoryOldPlaneCoroutine()
    {
        while(planesToDestroyQueue.Count != 0)
        {
            GameObject oldPlane = planesToDestroyQueue.Dequeue();
            Destroy(oldPlane);

            yield return new WaitForSeconds(1.0f);
        }

        // control reaches here when the coroutine has destroyed all the old planes
        // now we want to stop the coroutine
        if (oldPlanesDestroyerCoroutine != null)
        {
            StopCoroutine(oldPlanesDestroyerCoroutine);
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

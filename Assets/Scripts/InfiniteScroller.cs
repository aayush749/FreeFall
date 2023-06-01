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

    // Start is called before the first frame update
    void Start()
    {
        // Subscribe to OnFallOffDetect Event for the player
        playerMovementController.OnFallOffDetect += ShufflePlanes;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ShufflePlanes(object sender, EventArgs args)
    {
        // collided with the fall off detection cube
        Debug.LogWarning("Collided with the fall-off plane");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPaused = true;
#endif
    }
}

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

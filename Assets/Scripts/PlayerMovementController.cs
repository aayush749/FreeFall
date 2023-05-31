using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{

    // this field is set only once, when the velocity of player becomes zero at the start of the scene
    bool hasStopped = false;
    float limitToDetectStop = 1e-1f;
    // Start is called before the first frame update
    void Start()
    {
        // Give an initial velocity to the player
        transform.GetComponent<Rigidbody>().velocity = 1.5f * Vector3.forward;
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfStopped();

        if (hasStopped)
        {
            // TODO: send a message to the UI manager to show 'START' text
        }
    }

    private void CheckIfStopped()
    {
        // check if the velocity of the player has gone within a specific limit
        // do this only if the bool field 'hasStopped' hasn't been set yet
        if (!hasStopped)
        {
            if (transform.GetComponent<Rigidbody>().velocity.magnitude < limitToDetectStop)
            {
                hasStopped = true;
                Debug.LogWarning("Stopped!");
            }
        }
    }
}

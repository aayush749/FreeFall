﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{

    private Rigidbody rb = null;

    // this field is set only once, when the velocity of player becomes zero at the start of the scene
    bool hasStopped = false;
    float limitToDetectStop = 1e-1f;

    [SerializeField]
    bool canBeginMotion = false;

    [SerializeField]
    [Range(0f, 10.0f)]
    private float forwardForceMagnitude = 10.0f;

    [SerializeField]
    [Range(0.5f, 25.0f)]
    private float maxSpeedLimit = 25.0f;

    [SerializeField]
    [Range(0.5f, 25.0f)] 
    private float speedReductionRate = 5.0f;

    // Coroutine reference for speed reduction coroutine
    Coroutine speedReducerCoroutine = null;

    // This field is to track if the speed limit has been hit at least once during the gameplay
    bool hasHitSpeedLimitOnce = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Give an initial velocity to the player
        rb.velocity = 1.5f * Vector3.forward;
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfStopped();

        if (!canBeginMotion && hasStopped)
        {
            // TODO: send a message to the UI manager to show 'START' text

            // set 'canBeginMotion'
            canBeginMotion = true;
            Debug.Log("Can begin motion");
        }

        if (canBeginMotion)
        {
            if (Input.GetKey(KeyCode.W))
            {
                // move forward
                if (rb.velocity.magnitude > 0.5f * maxSpeedLimit)
                {
                    hasHitSpeedLimitOnce = true;
                    speedReducerCoroutine = StartCoroutine(SpeedReducerCoroutine());
                }
                else
                {
                    if (hasHitSpeedLimitOnce)
                    {
                        StopCoroutine(speedReducerCoroutine);
                        Debug.Log("Speed reduced now");
                    }
                    rb.velocity += Vector3.forward * forwardForceMagnitude;
                }
            }
        }

    }

    private IEnumerator SpeedReducerCoroutine()
    {
        Debug.Log("Reducing speed now.");
        Vector3 velocity = rb.velocity;

        if (velocity.magnitude > maxSpeedLimit)
        {
            velocity = new Vector3(
                velocity.x, velocity.y,
                velocity.z - speedReductionRate);
        }


        yield return new WaitForSeconds(0.5f);
    }

    private void CheckIfStopped()
    {
        // check if the velocity of the player has gone within a specific limit
        // do this only if the bool field 'hasStopped' hasn't been set yet
        if (!hasStopped)
        {
            if (rb.velocity.magnitude < limitToDetectStop)
            {
                hasStopped = true;
                Debug.LogWarning("Stopped!");
            }
        }
    }
}

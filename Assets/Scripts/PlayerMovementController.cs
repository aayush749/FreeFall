using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{

    private Rigidbody rb = null;

    // this field is set only once, when the velocity of player becomes zero at the start of the scene
    bool hasStopped = false;
    float limitToDetectStop = 1.0f;

    [SerializeField, Range(0.0f, 2.0f)]
    private float initialSpeed = 1.0f;

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

    [SerializeField]
    [Range(1.0f, 10.0f)]
    private float dashSpeedMultiplier = 2.0f;

    // Coroutine reference for speed reduction coroutine
    Coroutine speedReducerCoroutine = null;

    // This field is to track if the speed limit has been hit at least once during the gameplay
    bool hasHitSpeedLimitOnce = false;

    // Plane on which the player is currently on
    private GameObject plane;

    [SerializeField, Tooltip("The name of the plane used to detect fall from the plane, before it happens")]
    private string fallDetectionPlaneName;

    [SerializeField, Tooltip("The name of the plane used to detect entry in a new plane")]
    private string entryDetectionPlaneName;

    public event EventHandler OnFallOffDetect;
    public event EventHandler OnEnterNewPlane;

    public class PlayerDeadEventArgs
    {
        public ContactPoint pointOfCollision { get; set; }
    }


    public event EventHandler<PlayerDeadEventArgs> OnPlayerDead; // this event would be invoked when the player dies (goes below a certain speed)

    // Start is called before the first frame update
    void Start()
    {
        // Reset the time scale to 1
        Time.timeScale = 1;

        rb = GetComponent<Rigidbody>();
        // Give an initial velocity to the player
        rb.velocity = initialSpeed * Vector3.forward;

        // Find the only plane instantiated in the scene, and store it
        plane = GameObject.Find("Plane");
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfStopped();
        HandlePlayerMotion();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            if (other.name == fallDetectionPlaneName)
            {
                // invoke the event of fall detection
                OnFallOffDetect.Invoke(this, EventArgs.Empty);
            }
            else if (other.name == entryDetectionPlaneName)
            {
                // this is a new plane the player has entered
                // invoke the event of entering new plane
                OnEnterNewPlane.Invoke(this, EventArgs.Empty);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Finish")
        {
            // Initiate game over sequence
            
            // 1) Know the point in space where collision took place
            ContactPoint contact = collision.contacts[0];

            // 2) Invoke the event with the collision point
            PlayerDeadEventArgs eventArgs = new PlayerDeadEventArgs();
            eventArgs.pointOfCollision = contact;
            OnPlayerDead.Invoke(this, eventArgs);
        }
    }

    public GameObject GetCurrentPlane()
    {
        return plane;
    }

    public void SetCurrentPlane(GameObject plane)
    {
        this.plane = plane;
    }

    private void HandlePlayerMotion()
    {
        if (!canBeginMotion && hasStopped)
        {
            // TODO: send a message to the UI manager to show 'START' text

            // set 'canBeginMotion'
            canBeginMotion = true;
        }

        if (canBeginMotion)
        {
            // if player speed is over the max limit, fix it to max limit

            PreventSpeeding();

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
                    }
                    rb.velocity += Vector3.forward * forwardForceMagnitude;
                }
            }

            if (Input.GetKey(KeyCode.A))
            {
                // dash left (negative X)
                rb.angularVelocity = Vector3.zero;
                rb.velocity += Vector3.left * forwardForceMagnitude * dashSpeedMultiplier;
            }

            if (Input.GetKey(KeyCode.D))
            {
                // dash right (positive X)
                rb.angularVelocity = Vector3.zero;
                rb.velocity += Vector3.right * forwardForceMagnitude * dashSpeedMultiplier;
            }
        }
    }

    private void PreventSpeeding()
    {
        if (rb.velocity.magnitude > maxSpeedLimit)
        {
            // calculate the new z component for velocity
            // such that speed is equal to max limit
            var newVelocityZComponentSquared = Math.Pow(maxSpeedLimit, 2) - Mathf.Pow(rb.velocity.x, 2) - Mathf.Pow(rb.velocity.y, 2);

            // update the velocity z component
            rb.velocity = new Vector3(
                rb.velocity.x, rb.velocity.y,
                Mathf.Sqrt((float)newVelocityZComponentSquared));
        }
    }

    private IEnumerator SpeedReducerCoroutine()
    {
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

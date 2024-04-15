using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinalStateMachine : MonoBehaviour
{
    // State definitions
    // PatrolIdle is the initial state, PatrolForward means moving forwards, PatrolLeft90 means moving left 90 degrees, PatrolLeft180 means turning around, PatrolLeft90TurnAround is for turning the last 90 to turn around.
    // CoinIdle is the initial state when a coin is visible, CoinRight means turning right to face a coin, CoinLeft means turning left to face a coin, CoinForward means moving forwards to a coin.
    // GoalReached is when enough coins have been collected.
    enum State { PatrolIdle, PatrolForward, PatrolLeft90, PatrolLeft180, PatrolLeft90TurnAround, CoinIdle, CoinForward, GoalReached };

    // Direction a coin is in
    enum CoinDirection { Left, Right, Ahead };

    // The current state of the NPC
    private State currentState;

    // The coin that is presently targetted for collection
    private GameObject targetCoin;

    // The rigidbody of the NPC object
    private Rigidbody rigidBody;

    // The speed to be moved when moving forwards
    [SerializeField]
    private float moveSpeed = 1.0f;

    // Total number of coins collected by this NPC
    private int coinsCollected = 0;

    // Distance at which a obstacle is visible
    [SerializeField]
    private float obstacleVisibilityDistance = 0.5f;

    // Distance at which a coin is visible
    [SerializeField]
    private float coinVisibilityDistance = 4.0f;

    private int obstacleMask;
    private int coinMask;

    // The field of view in which the NPC can see coins
    [SerializeField]
    private float fieldOfView = 30f;

    [SerializeField]
    private int coinsCollectedGoal = 10;

    [SerializeField]
    private Text fsmScoreText;
    private string initialScoreText;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        initialScoreText = fsmScoreText.text;
        coinMask = LayerMask.GetMask("Coin");
        obstacleMask = LayerMask.GetMask("Obstacle");
        targetCoin = null;
    }

    // Update is called once per frame
    void Update()
    {
        fsmScoreText.text = initialScoreText + " " + coinsCollected + "/" + coinsCollectedGoal;
    }

    // Update is called once per Physics frame by default is every 0.02 seconds or rather 50 calls per second.
    void FixedUpdate()
    {
        this.ProcessStateMachineChanges();
        this.ProcessStateMachineActions();
    }

    void ProcessStateMachineChanges()
    {
        switch (this.currentState)
        {
            case State.PatrolIdle:
                if (this.CoinVisible())
                {
                    // Coin spotted change state to CoinIdle
                    this.currentState = State.CoinIdle;
                    Debug.Log("CurrentState: " + this.currentState);
                } 
                else
                {
                    // Coin not seen start patrol!
                    this.currentState = State.PatrolForward;
                    Debug.Log("CurrentState: " + this.currentState);
                }
                break;
            case State.PatrolForward:
                if (this.CoinVisible())
                {
                    this.currentState = State.CoinIdle;
                    Debug.Log("CurrentState: " + this.currentState);
                }
                else if (this.ObstacleVisible())
                {
                    this.currentState = State.PatrolLeft90;
                    Debug.Log("CurrentState: " + this.currentState);
                }
                break;
            case State.PatrolLeft90:
                if (this.CoinVisible())
                {
                    this.currentState = State.CoinIdle;
                    Debug.Log("CurrentState: " + this.currentState);
                }
                else if (this.ObstacleVisible())
                {
                    this.currentState = State.PatrolLeft180;
                    Debug.Log("CurrentState: " + this.currentState);
                }
                else
                {
                    this.currentState = State.PatrolForward;
                    Debug.Log("CurrentState: " + this.currentState);
                }
                break;
            case State.PatrolLeft180:
                if (this.CoinVisible())
                {
                    this.currentState = State.CoinIdle;
                    Debug.Log("CurrentState: " + this.currentState);
                }
                else if (this.ObstacleVisible())
                {
                    this.currentState = State.PatrolLeft90TurnAround;
                    Debug.Log("CurrentState: " + this.currentState);
                }
                else
                {
                    this.currentState = State.PatrolForward;
                    Debug.Log("CurrentState: " + this.currentState);
                }
                break;
            case State.PatrolLeft90TurnAround:
                if (this.CoinVisible())
                {
                    this.currentState = State.CoinIdle;
                    Debug.Log("CurrentState: " + this.currentState);
                }
                else
                {
                    this.currentState = State.PatrolForward;
                    Debug.Log("CurrentState: " + this.currentState);
                }
                break;
            case State.CoinIdle:
            case State.CoinForward:
                if (this.EnoughCoinsCollected())
                {
                    this.currentState = State.GoalReached;
                    Debug.Log("CurrentState: " + this.currentState);
                }
                else if (this.CoinCollected())
                {

                    this.currentState = State.PatrolIdle;
                    Debug.Log("CurrentState: " + this.currentState);
                } else
                {
                    currentState = State.CoinForward;
                }
                break;
            case State.GoalReached:
                // For now do nothing just be stationary
                break;
            default:
                Debug.Log("ProcessStateMachineChanges: State is invalid.");
                break;
        }
    }

    void ProcessStateMachineActions()
    {
        // Set everything to nill to begin with
        this.rigidBody.velocity = Vector3.zero;
        this.rigidBody.angularVelocity = Vector3.zero;

        switch (this.currentState)
        {
            case State.PatrolIdle:
                // Do nothing
                break;
            case State.PatrolForward:
                // Make the NPC move forward at the defined speed
                this.rigidBody.velocity = transform.forward * this.moveSpeed;
                break;
            case State.PatrolLeft90:
                this.transform.Rotate(0, -90, 0);
                break;
            case State.PatrolLeft180:
                this.transform.Rotate(0, -180, 0);
                break;
            case State.PatrolLeft90TurnAround:
                this.transform.Rotate(0, 90, 0);
                break;
            case State.CoinIdle:
                // Do nothing be idle!
                break;
            case State.CoinForward:
                this.rigidBody.velocity = transform.forward * this.moveSpeed;
                break;
            case State.GoalReached:
                // For now do nothing just be stationary
                break;
            default:
                Debug.Log("ProcessStateMachineActions: State is invalid.");
                break;
        }
    }

    bool CoinVisible()
    {
        // Determine if a coin is visible and if so, set it as the target and return true, else return false.
        // A coin is visible if it is visible without walls in the way for half the coin, use the position of the coin to determine if half is visible from the position of the capsule, this is simplified by using a raycast between the center of both NPC and Coin.
        Collider[] coinsInViewRadius = Physics.OverlapSphere(transform.position, coinVisibilityDistance, coinMask);

        foreach (Collider coinCollider in coinsInViewRadius)
        {
            Vector3 directionToCoin = (coinCollider.transform.position - transform.position).normalized;

            // Check if coin is within field of view
            if (Vector3.Angle(transform.forward, directionToCoin) < fieldOfView / 2)
            {
                float distanceToCoin = Vector3.Distance(transform.position, coinCollider.transform.position);

                // Check if there are obstructions between the NPC and the coin
                if (!Physics.Raycast(transform.position, directionToCoin, distanceToCoin, obstacleMask))
                {
                    if (coinCollider.gameObject.tag != "Coin")
                    {
                        // Assume it's one of the objects (cube, sphere, etc) that make up the coin instead of the gameObject we need.
                        targetCoin = coinCollider.transform.parent.gameObject;
                    } else
                    {
                        targetCoin = coinCollider.gameObject;
                    }
                    
                    return true;
                }
            }
        }
        return false;
    }

    bool ObstacleVisible()
    {
        // If a none coin obstacle is directly infront of the NPC less than half the distance of a maze cell's length

        // Debug draw the ray
        Debug.DrawRay(transform.position, transform.forward);

        // Use a raycast to find the next object for the ray
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        bool raycastResult = Physics.Raycast(ray, out hit, obstacleVisibilityDistance);
        
        if (hit.collider != null)
        {
            GameObject hitObject = hit.collider.gameObject;
            (bool isCoin, GameObject possibleCoin) = CheckIfCoinOrParentCoin(hitObject);
            // If raycast hit something and it's not a coin.
            return raycastResult && !isCoin;
        }
        return raycastResult;
    }

    bool CoinCollected()
    {
        // Has the coin been collected?
        return targetCoin == null;
    }

    bool EnoughCoinsCollected()
    {
        // Has the NPC found enough coins to be successful yet?
        return coinsCollected >= coinsCollectedGoal;
    }

    // Draw gizmo in editor
    void OnDrawGizmos()
    {
        // Draw field of view
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -fieldOfView / 2, 0) * transform.forward * coinVisibilityDistance);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, fieldOfView / 2, 0) * transform.forward * coinVisibilityDistance);

        // Draw vision distance
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, coinVisibilityDistance);

        // Draw attack distance
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, coinVisibilityDistance);
    }

    (bool, GameObject) CheckIfCoinOrParentCoin(GameObject objectToCheck)
    {
        // Return true if parent is a coin or the object is a coin in pos0
        // Return coin if true or null/random object if false in pos0, for pos1
        GameObject possibleCoin = null;
        if (objectToCheck.layer == coinMask && objectToCheck.gameObject.tag != "Coin")
        {
            // Assume it's one of the objects (cube, sphere, etc) that make up the coin instead of the gameObject we need.
            possibleCoin = objectToCheck.gameObject.transform.parent.gameObject;
        }
        else
        {
            possibleCoin = objectToCheck.gameObject;
        }
        return (possibleCoin != null && possibleCoin.gameObject.tag == "Coin", possibleCoin);
    }

    void OnCollisionEnter(Collision collision)
    {
        (bool isCoin, GameObject possibleCoin) = CheckIfCoinOrParentCoin(collision.gameObject);
        if (isCoin)
        {
            if (possibleCoin.gameObject == targetCoin)
            {
                targetCoin = null;
            }
            Destroy(possibleCoin.gameObject);
            coinsCollected += 1;
            // Reset position to avoid issues with coin rotation
        }
    }
}

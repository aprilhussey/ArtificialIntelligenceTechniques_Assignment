using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FiniteStateMachine : MonoBehaviour
{
    // State definitions
    public enum State { WanderForward, WanderLeft, WanderRight, WanderTurnAround, FaceCoin, CoinForward, GoalReached };

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
    private float coinVisibilityDistance = 5.0f;

    private int obstacleMask;
    private int coinMask;

    [SerializeField]
    private int coinsCollectedGoal = 10;

    // Variables for handling the score text
    [SerializeField]
    private Text fsmScoreText;
    private string initialScoreText;

    // Variables for handling timer text
    [SerializeField]
    private Text fsmTimerText;
    private string initialTimerText;
    private float initialTime;
    public float finalTime = 0;

    // Variables for handling decision text
    [SerializeField]
    private Text fsmDecisionText;
    private string initialDecisionText;
    public int decisionsMade = 0;

    // Implement global state needed for decision making
    public State currentState;

    public Vector3 coinDirection;
    public GameObject targetCoin = null;

    [SerializeField]
    public float cellDistance = 1.0f;
    public Vector3 targetCellPos;
    public Vector3 targetCoinPos;

    public bool movingToCell = false;
    public bool movingToCoin = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        initialScoreText = fsmScoreText.text;
        coinMask = LayerMask.GetMask("Coin");
        obstacleMask = LayerMask.GetMask("Obstacle");
        initialTime = Time.time;
        initialTimerText = fsmTimerText.text;
        initialDecisionText = fsmDecisionText.text;
    }

    // Update is called once per frame
    void Update()
    {

        fsmScoreText.text = initialScoreText + " " + coinsCollected + "/" + coinsCollectedGoal;
        float timeTaken = Time.time - initialTime;
        if (this.currentState != State.GoalReached)
        {
            if (!movingToCell && !movingToCoin)
            {
                // Increment decision
                decisionsMade++;
            }
            fsmTimerText.text = initialTimerText + " " + timeTaken.ToString("F2") + "s";
            fsmDecisionText.text = initialDecisionText + " " + decisionsMade;
            // Ready to perform state checks
            this.ProcessStateMachineChanges();
            this.ProcessStateMachineActions();
        } else
        {
            // Goal was reached.
            if (finalTime == 0)
            {
                fsmTimerText.text = initialTimerText + " " + timeTaken.ToString("F2") + "s";
                finalTime = timeTaken;
            }
        }
    }

    void ProcessStateMachineChanges()
    {
        switch (this.currentState)
        {
            case State.WanderForward:
                if (movingToCell)
                {
                    // Don't allow change of state whilst moving to cell
                    break;
                }
                else if (this.CoinVisible())
                {
                    // Coin spotted change state to CoinForward
                    this.currentState = State.CoinForward;
                } 
                else if (!this.ObstacleVisibleInDirection(-transform.right))
                {
                    // No Obstacle left
                    // Turn Left
                    this.currentState = State.WanderLeft;
                }
                else if (this.ObstacleVisibleInDirection(transform.forward) && this.ObstacleVisibleInDirection(-transform.right) && !this.ObstacleVisibleInDirection(transform.right))
                {
                    // Obstacle Forward, Obstacle Left, No Obstacle Right
                    // Turn Right
                    this.currentState = State.WanderRight;
                }
                else if (this.ObstacleVisibleInDirection(transform.forward) && this.ObstacleVisibleInDirection(-transform.right) && this.ObstacleVisibleInDirection(transform.right))
                {
                    // Obstacle Forward, Obstacle Left, Obstacle Right
                    // Turn Around
                    this.currentState = State.WanderTurnAround;
                } 
                break;
            case State.WanderLeft:
            case State.WanderRight:
            case State.WanderTurnAround:
                if (this.CoinVisible())
                {
                    // Coin spotted change state to CoinForward
                    this.currentState = State.CoinForward;
                }
                else
                {
                    // Work on the assumption that it will always be correct to push forward as we only get here if no obstacle was infront
                    this.currentState = State.WanderForward;
                }
                break;
            case State.CoinForward:
                if (coinDirection != transform.forward)
                {
                    // Coin is not infront!
                    this.currentState = State.FaceCoin;
                } 
                else if (CoinCollected() && Vector3.Distance(transform.position, targetCoinPos) <= 0.1)
                {
                    transform.position = targetCoinPos;
                    targetCoinPos = new Vector3(-1, -1, -1);
                    movingToCoin = false;
                    if (EnoughCoinsCollected())
                    {
                        this.currentState = State.GoalReached;
                    }
                    else
                    {
                        this.currentState = State.WanderForward;
                    }
                }
                break;
            case State.FaceCoin:
                // Will have been handled already so switch 
                if (coinDirection == transform.forward)
                {
                    this.currentState = State.CoinForward;
                }
                break;
            default:
                Debug.Log("ProcessStateMachineChanges: State is invalid.");
                break;
        }
    }

    private void MoveForwardOneCell()
    {
        if (Vector3.Distance(transform.position, targetCellPos) <= 0.1)
        {
            // Now in that cell
            movingToCell = false;
            transform.position = targetCellPos;
            targetCellPos = new Vector3(-1, -1, -1);
        }
        if (!this.ObstacleVisibleInDirection(transform.forward))
        {
            transform.position = Vector3.MoveTowards(transform.position, targetCellPos, moveSpeed * Time.deltaTime);
        } 
        else
        {
            // Failsafe assume that the obstacle was not properly accounted for and effectively end this state. Then move to the centre of closest cell.
            movingToCell = false;
            transform.position = new Vector3(Mathf.Round(transform.position.x), transform.position.y, Mathf.Round(transform.position.z));
        }
    }

    void ProcessStateMachineActions()
    {
        switch (this.currentState)
        {
            case State.WanderForward:
                if (!movingToCell)
                {
                    // Not presently moving but would like to move to the next cell
                    movingToCell = true;
                    targetCellPos = transform.position + transform.forward * cellDistance;
                }
                MoveForwardOneCell();
                break;
            case State.WanderLeft:
                transform.Rotate(0, -90, 0);
                break;
            case State.WanderRight:
                transform.Rotate(0, 90, 0);
                break;
            case State.WanderTurnAround:
                transform.Rotate(0, 180, 0);
                break;
            case State.CoinForward:
                transform.position = Vector3.MoveTowards(transform.position, targetCoinPos, moveSpeed * Time.deltaTime);
                break;
            case State.FaceCoin:
                transform.rotation = Quaternion.LookRotation(coinDirection);
                coinDirection = transform.forward;
                break;
            case State.GoalReached:
                // DO nothing goal is reached
                break;
            default:
                Debug.Log("ProcessStateMachineActions: State is invalid.");
                break;
        }
    }

    public bool ObstacleVisibleInDirection(Vector3 direction)
    {
        // If a none coin obstacle is in the direction of the NPC less than half the distance of a maze cell's length
        // Debug draw the ray
        Debug.DrawRay(transform.position, direction);

        // Use a raycast to find the next object for the ray
        Ray ray = new Ray(transform.position, direction);
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

    (bool, GameObject) CoinInDirection(Vector3 direction)
    {
        // Return a tuple containing a bool for whether a coin was seen, and a GameObject containing that coin otherwise return null.
        Ray ray = new Ray(transform.position, direction);
        RaycastHit hit;
        bool raycastResult = Physics.Raycast(ray, out hit, coinVisibilityDistance);

        if (hit.collider != null)
        {
            GameObject hitObject = hit.collider.gameObject;
            (bool isCoin, GameObject possibleCoin) = CheckIfCoinOrParentCoin(hitObject);
            // If raycast hit something and it's a coin.
            return (raycastResult && isCoin, possibleCoin);
        }
        return (raycastResult, null);
    }

    bool CoinVisible()
    {
        // Is there a coin in any direction, use 4 raycasts to check! Max distance raycast stopping for walls.
        // Check Front, Left, Right, Backwards
        List<Vector3> directionsToCheck = new List<Vector3> { transform.forward, -transform.right, transform.right, -transform.forward };

        foreach (Vector3 direction in directionsToCheck)
        {
            (bool coinVisible, GameObject possibleCoin) = CoinInDirection(direction);
            if (coinVisible)
            {
                coinDirection = direction;
                targetCoin = possibleCoin;
                movingToCoin = true;
                targetCoinPos = possibleCoin.transform.position;
                return true;
            }
        }
        return false;
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

    // Draw gizmos in editor
    void OnDrawGizmos()
    {
        // Draw Coin visibility distance
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, -transform.right * coinVisibilityDistance);
        Gizmos.DrawRay(transform.position, transform.right * coinVisibilityDistance);
        Gizmos.DrawRay(transform.position, transform.forward * coinVisibilityDistance);
        Gizmos.DrawRay(transform.position, -transform.forward * coinVisibilityDistance);

        // Draw obstacle vision distance
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, -transform.right * obstacleVisibilityDistance);
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(transform.position, transform.right * obstacleVisibilityDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * obstacleVisibilityDistance);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, -transform.forward * obstacleVisibilityDistance);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BehaviourTreeScript : MonoBehaviour
{
    // private void 
    private Selector topNode;
    // The rigidbody of the NPC object
    private Rigidbody rigidBody;
    // The speed to be moved when moving forwards
    [SerializeField]
    public float moveSpeed = 0.5f;

    // Variables for handling the score
    [SerializeField]
    private Text btScoreText;
    private string initialScoreText;

    // Variables for handling timer text
    [SerializeField]
    private Text btTimerText;
    private string initialTimerText;
    private float initialTime;
    public float finalTime = 0;

    // Variables for handling decision text
    [SerializeField]
    private Text btDecisionText;
    private string initialDecisionText;
    public int decisionsMade = 0;

    // Total number of coins collected by this NPC
    private int coinsCollected = 0;

    [SerializeField]
    private int coinsCollectedGoal = 10;

    // Distance at which a obstacle is visible
    [SerializeField]
    private float obstacleVisibilityDistance = 0.5f;

    // Distance at which a coin is visible
    [SerializeField]
    private float coinVisibilityDistance = 2.0f;

    private int obstacleMask;
    private int coinMask;

    // Implement global state needed for decision making
    public Vector3 coinDirection;
    public GameObject targetCoin;

    [SerializeField]
    public float cellDistance = 1.0f;
    public Vector3 targetCellPos;
    public Vector3 targetCoinPos;

    public bool movingToCell = false;
    public bool movingToCoin = false;

    // Start is called before the first frame update
    void Start()
    {
        initialScoreText = btScoreText.text;
        rigidBody = GetComponent<Rigidbody>();
        coinMask = LayerMask.GetMask("Coin");
        obstacleMask = LayerMask.GetMask("Obstacle");
        initialTime = Time.time;
        initialTimerText = btTimerText.text;
        initialDecisionText = btDecisionText.text;
        // Construct the behaviour tree
        ConstructBT();
    }

    // Update is called once per frame
    void Update()
    {
        btScoreText.text = initialScoreText + " " + coinsCollected + "/" + coinsCollectedGoal;
        float timeTaken = Time.time - initialTime;
        if (!EnoughCoinsCollected())
        {
            NodeState evaluateReturn = topNode.Evaluate();
            // Increment decision made each time we evaluate the top node, if not Running.
            if (evaluateReturn != NodeState.RUNNING)
            {
                decisionsMade++;
            }
            // Write new decision and timer text
            btTimerText.text = initialTimerText + " " + timeTaken.ToString("F2") + "s";
            btDecisionText.text = initialDecisionText + " " + decisionsMade;
        }
        else
        {
            // CoinsCollected
            if (finalTime == 0)
            {
                btTimerText.text = initialTimerText + " " + timeTaken.ToString("F2") + "s";
                finalTime = timeTaken;
            }
        }
        
    }

    public bool EnoughCoinsCollected()
    {
        // Has the NPC found enough coins to be successful yet?
        return coinsCollected >= coinsCollectedGoal;
    }

    public (bool, GameObject) CheckIfCoinOrParentCoin(GameObject objectToCheck)
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

    private void ConstructBT()
    {
        // Coin collection sequence
        CoinVisibleNode coinVisibleNode = new CoinVisibleNode(transform, coinVisibilityDistance, this);
        FaceCoin faceCoin = new FaceCoin(this, transform);
        MoveForwardUntilCoinIsCollected moveForwardUntilCoinIsCollected = new MoveForwardUntilCoinIsCollected(this, transform, cellDistance);
        Sequence coinCollectSequence = new Sequence(new List<Node> { faceCoin, moveForwardUntilCoinIsCollected });
        Sequence coinCollectionSequence = new Sequence(new List<Node> { coinVisibleNode, coinCollectSequence });

        // Wander selector
        NoObstacleRightFaceRight noObstacleRightFaceRight = new NoObstacleRightFaceRight(this, transform);
        MoveForwardIntoNextCell moveForwardIntoNextCell = new MoveForwardIntoNextCell(this, transform, cellDistance);
        Sequence noObstacleRightSequence = new Sequence(new List<Node> { noObstacleRightFaceRight, moveForwardIntoNextCell });
        NoObstacleLeftFaceLeft noObstacleLeftFaceLeft = new NoObstacleLeftFaceLeft(this, transform);
        Sequence noObstacleLeftSequence = new Sequence(new List<Node> { noObstacleLeftFaceLeft, moveForwardIntoNextCell });
        TurnAround turnAround = new TurnAround(this, transform);
        Selector wanderSelector = new Selector(new List<Node> { noObstacleLeftSequence, moveForwardIntoNextCell, noObstacleRightSequence, turnAround });

        topNode = new Selector(new List<Node> { coinCollectionSequence, wanderSelector });
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
}

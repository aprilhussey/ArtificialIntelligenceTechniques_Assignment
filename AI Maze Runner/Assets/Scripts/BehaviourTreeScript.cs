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
    private float moveSpeed = 1.0f;
    // X and Z coords to set the NPC to initially
    [SerializeField]
    private int xCoord = 10;
    [SerializeField]
    private int zCoord = 10;

    // Variables for handling the score
    [SerializeField]
    private Text btScoreText;
    private string initialScoreText;

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

    // The field of view in which the NPC can see coins
    [SerializeField]
    private float fieldOfView = 30f;

    private int obstacleMask;
    private int coinMask;

    public Vector3 coinDirection;
    public GameObject targetCoin;

    [SerializeField]
    public float cellDistance = 1.0f;
    public Vector3 targetCellPos;

    public bool movingToNextCell = false;

    // Start is called before the first frame update
    void Start()
    {
        initialScoreText = btScoreText.text;
        rigidBody = GetComponent<Rigidbody>();
        coinMask = LayerMask.GetMask("Coin");
        obstacleMask = LayerMask.GetMask("Obstacle");
        // Construct the behaviour tree
        ConstructBT();
        // Set pos based on set coords
        transform.position = new Vector3(xCoord, transform.position.y, zCoord);
    }

    // Update is called once per frame
    void Update()
    {
        btScoreText.text = initialScoreText + " " + coinsCollected + "/" + coinsCollectedGoal;

        if (movingToNextCell)
        {
            // Handle logic to move to the next cell
            MoveTowardsNextCell();
        } else
        {
            // Else continue normal evaluation
            topNode.Evaluate();
        }
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

    public void MoveTowardsNextCell()
    {
        float step = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetCellPos, step);

        // If at position end this
        if (transform.position == targetCellPos)
        {
            movingToNextCell = false;
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
        TurnAround turnAround = new TurnAround(transform);
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
        Gizmos.DrawRay(transform.position, transform.right * obstacleVisibilityDistance);
        Gizmos.DrawRay(transform.position, transform.forward * obstacleVisibilityDistance);
        Gizmos.DrawRay(transform.position, -transform.forward * obstacleVisibilityDistance);
    }
}

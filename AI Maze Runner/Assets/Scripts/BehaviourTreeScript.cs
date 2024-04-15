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

    public GameObject targetCoin;

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
        topNode.Evaluate();
    }

    // Update is called once per Physics frame by default is every 0.02 seconds or rather 50 calls per second.
    void FixedUpdate()
    { 
       
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

    private void ConstructBT()
    {
        // Move forwards
        ForwardsNode forwardsNode = new ForwardsNode(rigidBody, transform, moveSpeed);

        // Coin aquisition sequence
        ForwardsAquisitionNode forwardsAquisitionNode = new ForwardsAquisitionNode(transform, coinVisibilityDistance, coinMask, obstacleMask, fieldOfView, this, rigidBody, moveSpeed);
        CoinVisibleNode coinVisibleNode = new CoinVisibleNode(transform, coinVisibilityDistance, coinMask, obstacleMask, fieldOfView, this);
        Sequence aquisitionSequence = new Sequence(new List<Node> { coinVisibleNode, forwardsAquisitionNode });

        // Handle Obstacle
        RotateLeft180Node rotateLeft180Node = new RotateLeft180Node(transform);
        RotateLeft90Node rotateLeft90Node = new RotateLeft90Node(transform);
        RotateLeft270Node rotateLeft270Node = new RotateLeft270Node(transform);
        StopNode stopNode = new StopNode(transform, rigidBody);
        ObstacleFrontNode obstacleFrontNode = new ObstacleFrontNode(transform, this, obstacleVisibilityDistance);
        Sequence obstacleSequence = new Sequence(new List<Node> { obstacleFrontNode, stopNode, rotateLeft90Node, obstacleFrontNode, rotateLeft180Node, obstacleFrontNode, rotateLeft270Node});

        topNode = new Selector(new List<Node> { aquisitionSequence, obstacleSequence, forwardsNode });
    }

    // Draw gizmos in editor
    void OnDrawGizmos()
    {
        // Draw obstacle ray
        Gizmos.DrawRay(transform.position, transform.forward);

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
}

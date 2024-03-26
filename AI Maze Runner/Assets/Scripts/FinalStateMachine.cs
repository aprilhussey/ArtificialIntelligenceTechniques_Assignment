using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalStateMachine : MonoBehaviour
{
    // State definitions
    // PatrolIdle is the initial state, PatrolForward means moving forwards, PatrolLeft90 means moving left 90 degrees, PatrolLeft180 means turning around, PatrolLeft90TurnAround is for turning the last 90 to turn around.
    // CoinIdle is the initial state when a coin is visible, CoinRight means turning right to face a coin, CoinLeft means turning left to face a coin, CoinForward means moving forwards to a coin.
    // GoalReached is when enough coins have been collected.
    enum State { PatrolIdle, PatrolForward, PatrolLeft90, PatrolLeft180, PatrolLeft90TurnAround, CoinIdle, CoinRight, CoinLeft, CoinForward, GoalReached };

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

    // The amount to rotate on one update when homing on a coin
    [SerializeField]
    private float rotationAmount = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
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
                } 
                else
                {
                    // Coin not seen start patrol!
                    this.currentState = State.PatrolForward;
                }
                break;
            case State.PatrolForward:
                if (this.CoinVisible())
                {
                    this.currentState = State.CoinIdle;
                }
                else if (this.ObstacleVisible())
                {
                    this.currentState = State.PatrolLeft90;
                    // Stop the npc stationary
                    this.rigidBody.velocity = transform.forward * 0;
                }
                break;
            case State.PatrolLeft90:
                if (this.CoinVisible())
                {
                    this.currentState = State.CoinIdle;
                }
                else if (this.ObstacleVisible())
                {
                    this.currentState = State.PatrolLeft180;
                }
                break;
            case State.PatrolLeft180:
                if (this.CoinVisible())
                {
                    this.currentState = State.CoinIdle;
                }
                else if (this.ObstacleVisible())
                {
                    this.currentState = State.PatrolLeft90TurnAround;
                }
                break;
            case State.PatrolLeft90TurnAround:
                if (this.CoinVisible())
                {
                    this.currentState = State.CoinIdle;
                }
                else if (this.ObstacleVisible())
                {
                    this.currentState = State.PatrolLeft90TurnAround;
                }
                break;
            case State.CoinIdle:
            case State.CoinRight:
            case State.CoinLeft:
                this.ProcessCoinCollectStateChanges();
                break;
            case State.CoinForward:
                if (this.CoinCollected())
                {
                    this.currentState = State.CoinIdle;
                } 
                else if (this.EnoughCoinsCollected())
                {
                    this.currentState = State.GoalReached;
                }
                break;
            default:
                Debug.Log("ProcessStateMachineChanges: State is invalid.");
                break;
        }
    }

    void ProcessStateMachineActions()
    {
        switch (this.currentState)
        {
            case State.PatrolIdle:
                // Do nothing be idle!
                break;
            case State.PatrolForward:
                // Make the NPC move forward at the defined speed
                this.rigidBody.velocity = transform.forward * this.moveSpeed;
                break;
            case State.PatrolLeft90:
                break;
            case State.PatrolLeft180:
                break;
            case State.PatrolLeft90TurnAround:
                break;
            case State.CoinIdle:
                // Do nothing be idle!
                break;
            case State.CoinRight:
                break;
            case State.CoinLeft:
                break;
            case State.CoinForward:
                break;
            default:
                Debug.Log("ProcessStateMachineActions: State is invalid.");
                break;
        }
    }

    void ProcessCoinCollectStateChanges()
    {
        CoinDirection direction = GetCoinDirection();
        switch (direction)
        {
            case CoinDirection.Left:
                currentState = State.CoinLeft;
                break;
            case CoinDirection.Right:
                currentState = State.CoinRight;
                break;
            case CoinDirection.Ahead:
                currentState = State.CoinForward;
                break;
            default:
                Debug.Log("Coin Direction was not valid");
                break;
        }
    }

    bool CoinVisible()
    {
        // Determine if a coin is visible and if so, set it as the target and return true, else return false.
        // A coin is visible if it is visible without walls in the way for half the coin, use the position of the coin to determine if half is visible from the position of the capsule, both are assumed to be in the middle.
        return false;
    }

    CoinDirection GetCoinDirection()
    {
        // Return what direction the coin is in, if it is within where the collider of the capsule will be when it gets to that position return ahead, if it's to the left return left, and if it's to the right return right.
        return CoinDirection.Ahead;
    }

    bool ObstacleVisible()
    {
        // If a none coin obstacle is directly infront of the NPC less than half the distance of a maze cell's length
        return false;
    }

    bool CoinCollected()
    {
        // Is a coin needing to be collected?
        return false;
    }

    bool CoinAhead()
    {
        // Is there a coin ahead?
        return false;
    }

    bool EnoughCoinsCollected()
    {
        // Has the NPC found enough coins to be successful yet?
        return false;
    }
}
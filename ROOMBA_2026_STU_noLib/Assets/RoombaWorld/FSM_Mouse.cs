using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_Mouse", menuName = "Finite State Machines/FSM_Mouse", order = 1)]
public class FSM_Mouse : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/

    public MOUSE_Blackboard blackboard;
    public GoToTarget goToTarget;
    public SteeringContext steeringContext;
    public GameObject roomba;   
    public GameObject movementTarget;
    private float maxSpeed = 2;
    private float maxAcceleration = 4;
    private float normalSpeed;
    private float normalAcceleration;
    private float doingPooTime = 0;

    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        blackboard = GetComponent<MOUSE_Blackboard>();
        goToTarget = GetComponent<GoToTarget>();
        steeringContext = GetComponent<SteeringContext>();
        normalSpeed = steeringContext.maxSpeed;
        normalAcceleration = steeringContext.maxAcceleration;
        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */
        
        steeringContext.maxSpeed = normalSpeed;
        steeringContext.maxAcceleration = normalAcceleration;
        base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* STAGE 1: create the states with their logic(s)
         *-----------------------------------------------
         
        State varName = new State("StateName",
            () => { }, // write on enter logic inside {}
            () => { }, // write in state logic inside {}
            () => { }  // write on exit logic inisde {}  
        );

         */
        State goingPoint = new State("Going_Point",
           () => { /* COMPLETE */
           
                movementTarget = new GameObject("MouseTarget");
                movementTarget.transform.position = LocationHelper.RandomWalkableLocation();  
                goToTarget.enabled = true;
                goToTarget.target = movementTarget; 
                },
            () => { }, 
           
           () => {/* COMPLETE */
                goToTarget.enabled = false;
                Destroy(movementTarget);
                }
        );
        
        State goingExit = new State("GoingExit",
           () => {
                Instantiate(blackboard.pooPrefab, transform.position, Quaternion.identity);
                goToTarget.enabled = true;
                goToTarget.target = LocationHelper.RandomEntryExitPoint();
                },
            () => { }, 
           
           () => {
                }
        );
        State goingExitQuickly = new State("GoingExitQuickly",
           () => {
                GetComponent<Renderer>().material.color = Color.green;
                steeringContext.maxSpeed = normalSpeed * maxSpeed;
                steeringContext.maxAcceleration = normalAcceleration * maxAcceleration;
                goToTarget.enabled = true;
                goToTarget.target = LocationHelper.NearestExitPoint(gameObject);
                },
            () => { }, 
           
           () => {
                }
        );
        State reachedExit = new State("reachedExit",
           () => {
               GameObject.Destroy(gameObject);
                },
            () => { }, 
           
           () => {
                }
        );
       

        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------

        Transition varName = new Transition("TransitionName",
            () => { }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        */
         Transition locationRached = new Transition("TransitionRached",
            () => { return goToTarget.routeTerminated();}, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );
        
         Transition roombaDetected = new Transition("RoombaDetected",
            () => { roomba = SensingUtils.FindInstanceWithinRadius(gameObject, "ROOMBA", blackboard.roombaDetectionRadius);
                return roomba != null;}, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */ 
        
        AddStates(goingPoint, goingExit, goingExitQuickly, reachedExit);
        AddTransition(goingPoint, locationRached, goingExit);
        AddTransition(goingPoint, roombaDetected, goingExitQuickly);
        AddTransition(goingExit, roombaDetected, goingExitQuickly);
        AddTransition(goingExit, locationRached, reachedExit);
        AddTransition(goingExitQuickly, locationRached, reachedExit);


        /* STAGE 4: set the initial state
         
        initialState = ... 

         */
         
        initialState = goingPoint;
    }

}


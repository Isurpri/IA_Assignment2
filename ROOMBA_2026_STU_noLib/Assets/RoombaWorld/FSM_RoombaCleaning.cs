using FSMs;
using UnityEngine;
using Steerings;
using UnityEditor.Experimental.GraphView;

[CreateAssetMenu(fileName = "FSM_RoombaCleaning", menuName = "Finite State Machines/FSM_RoombaCleaning", order = 1)]
public class FSM_RoombaCleaning : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/
    public ROOMBA_Blackboard blackboard;
    public GoToTarget goToTarget;
    public SteeringContext steeringContext;
    private GameObject dust;
    private GameObject poo;
    private float maxSpeed = 2;
    private float maxAcceleration = 4;
    private float normalSpeed;
    private float normalAcceleration;
    private float cleaningPooTime = 0;
    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */

        blackboard = GetComponent<ROOMBA_Blackboard>(); 
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

         */

        FiniteStateMachine roombaPatrolling = ScriptableObject.CreateInstance<FSM_RoombaPatrolling>();
        roombaPatrolling.Name = "roombaPatrolling";
        
        
        State goingDust = new State("GoingDust",
           () => {
                goToTarget.enabled = true;
                goToTarget.target = dust;
                },
            () => { }, 
           
           () => {
                goToTarget.enabled = false;
                }
        );
        
        State cleaningDust = new State("CleaningDust",
            () =>
            {
                if (dust != null)
                    GameObject.Destroy(dust);
                dust = null;
            },

            () => { },

            () => { }
        );
        State goingPoo = new State("GoingPoo",
           () => {
                steeringContext.maxSpeed *= maxSpeed;
                steeringContext.maxAcceleration *= maxAcceleration;
                goToTarget.enabled = true;
                goToTarget.target = poo;
                },
            () => { }, 
           
           () => {
                goToTarget.enabled = false;
                }
        );
        
        State cleaningPoo = new State("CleaningPoo",
            () => {
                cleaningPooTime = 0f;
                blackboard.StartSpinning();
                },

            () => { cleaningPooTime += Time.deltaTime; },

            () => {
                blackboard.StopSpinning();
                if (dust != null)
                    GameObject.Destroy(poo);
                poo = null;
            }
        );

        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------

        Transition varName = new Transition("TransitionName",
            () => { }, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );

        */
        Transition dustDetected = new Transition("DustDetected",
            () => { 
                dust = SensingUtils.FindInstanceWithinRadius(gameObject, "DUST", blackboard.dustDetectionRadius);
                return dust != null; },
            () => { }
        );
        Transition dustReached = new Transition("DustReached",
            () => { 
                dust = SensingUtils.FindInstanceWithinRadius(gameObject, "DUST", blackboard.dustReachedRadius);
                return dust != null; },
            () => { }
        ); 
        Transition cleaningDustDone = new Transition("CleaningDustDone",
            () => { return dust == null;
                    },
            () => { }
        );
        Transition pooDetected = new Transition("PooDetected",
            () => { 
                poo = SensingUtils.FindInstanceWithinRadius(gameObject, "POO", blackboard.pooDetectionRadius);
                return poo != null; },
            () => { }
        );
        Transition pooReached = new Transition("PooReached",
            () => { 
                poo = SensingUtils.FindInstanceWithinRadius(gameObject, "POO", blackboard.pooReachedRadius);
                return poo != null; },
            () => { }
        );
        Transition cleaningPooDone = new Transition("CleaningPooDone",
            () => { return cleaningPooTime >= blackboard.pooCleaningTime;
                    },
            () => { }
        );
        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
            
        AddStates(...);

        AddTransition(sourceState, transition, destinationState);

         */ 
        AddStates(roombaPatrolling, goingDust, cleaningDust, goingPoo, cleaningPoo);

        AddTransition(roombaPatrolling, pooDetected, goingPoo);
        AddTransition(goingDust, pooDetected, goingPoo);
        AddTransition(goingPoo, pooDetected, goingPoo);
        AddTransition(goingPoo, pooReached, cleaningPoo);
        AddTransition(cleaningPoo, cleaningPooDone, roombaPatrolling);
        
        AddTransition(roombaPatrolling, dustDetected, goingDust);
        AddTransition(goingDust, dustReached, cleaningDust);
        AddTransition(cleaningDust, cleaningDustDone, roombaPatrolling);

        /* STAGE 4: set the initial state
         
        initialState = ... 

         */

        initialState = roombaPatrolling;
    }
}

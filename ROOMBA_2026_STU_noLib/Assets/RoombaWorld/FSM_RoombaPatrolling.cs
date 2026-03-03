using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_RoombaPatrolling", menuName = "Finite State Machines/FSM_RoombaPatrolling", order = 1)]
public class FSM_Roomba : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/
    public ROOMBA_Blackboard blackboard;
    public GoToTarget goToTarget;
    public SteeringContext steeringContext;
    public override void OnEnter()
    {
        /* Write here the FSM initialization code. This code is execute every time the FSM is entered.
         * It's equivalent to the on enter action of any state 
         * Usually this code includes .GetComponent<...> invocations */
        blackboard = GetComponent<ROOMBA_Blackboard>();
        goToTarget = GetComponent<GoToTarget>();
        steeringContext = GetComponent<SteeringContext>();
        
        base.OnEnter(); // do not remove
    }

    public override void OnExit()
    {
        /* Write here the FSM exiting code. This code is execute every time the FSM is exited.
         * It's equivalent to the on exit action of any state 
         * Usually this code turns off behaviours that shouldn't be on when one the FSM has
         * been exited. */
        base.DisableAllSteerings();
        base.OnExit();
    }

    public override void OnConstruction()
    {
        /* STAGE 1: create the states with their logic(s)
         *-----------------------------------------------
         */
        State goingPoint = new State("Going_Point",
           () => { /* COMPLETE */
                goToTarget.enabled = true;
                goToTarget.target = LocationHelper.RandomPatrolPoint();       
                },
            () => { }, 
           
           () => {/* COMPLETE */
                goToTarget.enabled = false;
                }
        );



        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------

        */
        Transition locationRached = new Transition("TransitionRached",
            () => { return goToTarget.routeTerminated();}, // write the condition checkeing code in {}
            () => { }  // write the on trigger code in {} if any. Remove line if no on trigger action needed
        );


        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------
         
         */ 
        AddStates(goingPoint);
        AddTransition(goingPoint, locationRached, goingPoint);


        /* STAGE 4: set the initial state
         
        initialState = ... 

         */
        initialState = goingPoint;
    }
}

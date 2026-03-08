using FSMs;
using UnityEngine;
using Steerings;

[CreateAssetMenu(fileName = "FSM_RoombaCharging", menuName = "Finite State Machines/FSM_RoombaCharging", order = 1)]
public class FSM_RoombaCharging : FiniteStateMachine
{
    /* Declare here, as attributes, all the variables that need to be shared among
     * states and transitions and/or set in OnEnter or used in OnExit 
     * For instance: steering behaviours, blackboard, ...*/
    public ROOMBA_Blackboard blackboard;
    public GoToTarget goToTarget;
    public SteeringContext steeringContext;
    private GameObject chargingStation;
    private GameObject nearestChargingStation;

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

        FiniteStateMachine roombaCleaning = ScriptableObject.CreateInstance<FSM_RoombaCleaning>();
        roombaCleaning.Name = "roombaCleaning";

        State goingChargingPoint = new State("Going_Charging_Point",
           () => { 
                goToTarget.enabled = true;

                GameObject[] stations = GameObject.FindGameObjectsWithTag("ENERGY");
                
                nearestChargingStation = stations[0];
                float minDistance = SensingUtils.DistanceToTarget(gameObject, nearestChargingStation);

                foreach (GameObject station in stations)
                {
                    float distance = SensingUtils.DistanceToTarget(gameObject, station);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nearestChargingStation = station;
                    }
                }
                goToTarget.target = nearestChargingStation;
                },
            () => { }, 
           
           () => {
                goToTarget.enabled = false;
                }
        );
        State recharging = new State("Recharging",
           () => {
                blackboard.startRecharging();       
                },
            () => { }, 
           
           () => {
                blackboard.stopRecharging();
                }
        );


        /* STAGE 2: create the transitions with their logic(s)
         * ---------------------------------------------------

        */
        Transition batteryLow = new Transition("BatteryLow",
            () => {
                return blackboard.EnergyIsLow();
                },
            () => { }
        );

        Transition chargingPointReached = new Transition("ChargingPointReached",
            () => {
                chargingStation = SensingUtils.FindInstanceWithinRadius(gameObject, "ENERGY", blackboard.chargingStationReachedRadius);
                return chargingStation != null;
                },
            () => { }
        );

        Transition batteryFull = new Transition("BatteryFull",
            () => {
                return blackboard.EnergyIsFull();
            },
            () => { }
        );



        /* STAGE 3: add states and transitions to the FSM 
         * ----------------------------------------------

         */ 
        AddStates(goingChargingPoint, recharging, roombaCleaning);

        AddTransition(roombaCleaning, batteryLow, goingChargingPoint);
        AddTransition(goingChargingPoint, chargingPointReached, recharging);
        AddTransition(recharging, batteryFull, roombaCleaning);


        /* STAGE 4: set the initial state

         */
        initialState = roombaCleaning;
    }
}

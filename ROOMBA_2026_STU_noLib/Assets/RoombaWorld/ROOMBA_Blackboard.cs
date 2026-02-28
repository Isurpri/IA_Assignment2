
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ROOMBA_Blackboard : MonoBehaviour {

    public float dustDetectionRadius = 60;  // radius for dust detection   
    public float pooDetectionRadius = 150;   // radius for poo detection    

    public float dustReachedRadius = 5; // dust reachability radius
    public float pooReachedRadius = 5;  // poo reachability radius
    public float chargingStationReachedRadius = 4;  // reachability radius

    public float pooCleaningTime = 2; // time to clean poo (= spinning time)

    public float energyConsumptionPerSecond = 1;    
    public float energyRechargePerSecond = 15;
    public float minCharge = 15;    // min threshold. If currentCharge is below this figure go to recharging station    
    public float maxCharge = 99;    // max threshold. Leave charging station if currentCharge reaches this level

    public float currentCharge = 100;

    private TextMesh energyLine;
    
    
    
	void Start () {
        energyLine = GameObject.Find("EnergyLine").GetComponent<TextMesh>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
	}
	
	void Update () {
        Discharge(Time.deltaTime);
        energyLine.text = "Charge: " + Mathf.RoundToInt(currentCharge);
        if (spinning)
        {
            transform.Rotate(0, 0,  spinSpeed * Time.deltaTime);
            float sinValue = Mathf.Sin(Time.time * colorSpeed);
            float t = (sinValue + 1.0f) / 2.0f;
            spriteRenderer.color = Color.Lerp(spinningColorA, spinningColorB, t);
        }
        if (recharging)
        {
            currentCharge = currentCharge + Time.deltaTime * energyRechargePerSecond;
            if (currentCharge > 100) currentCharge = 100;
            float sinValue = Mathf.Sin(Time.time * colorSpeed);
            float t = (sinValue + 1.0f) / 2.0f;
            spriteRenderer.color = Color.Lerp(chargingColorA, chargingColorB, t);
        }
    }
    
    // invoke these method to start/stop spinning
    public void StartSpinning() { spriteRenderer.color = spinningColorA; spinning = true; }
    public void StopSpinning() { spriteRenderer.color = originalColor; spinning = false; }

    // invoke these methods to start/stop recharging
    public void startRecharging() { spriteRenderer.color = chargingColorA; recharging = true; }
    public void stopRecharging() { spriteRenderer.color = originalColor; recharging = false; }

    // these boolean functions help writing transition conditions regarding energy level
    public bool EnergyIsLow () { return currentCharge < minCharge; }
    public bool EnergyIsFull () { return currentCharge >= maxCharge; }
    
    // invoked by Update to subtract energy. 
    private void Discharge (float deltaTime)
    {
        currentCharge = currentCharge - deltaTime * energyConsumptionPerSecond;
        if (currentCharge < 0) currentCharge = 0;
    }
    
    // private stuff for color changing and spinning and recharging
    
    private SpriteRenderer spriteRenderer;
    
    private bool spinning = false;
    private bool recharging = false;
    private float spinSpeed = 720;
    private float colorSpeed = 5f;
    
    private Color originalColor;
    private Color spinningColorA = new Color(0.9f, 0, 0, 1);
    private Color spinningColorB = new Color(0.5f, 0, 0, 1);
    
    private Color chargingColorA = new Color(0.52f, 0.808f, 0.980f, 1);
    private Color chargingColorB = new Color(0.12f, 0.478f, 0.706f, 1);
}

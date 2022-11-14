using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host;
using Fusion;

public class Pointsmanager : MonoBehaviour
{
    private ConnectionManager coach;
    private CompanyManager comManager;
    private int pipeplacepoint = 500;
    private int piperemovepoint = 200;
    private int victorypoints = 4000;
    private float time;
    private bool roundWinner;
    
    void Start()
    {
        coach = GetComponent<ConnectionManager>();
        comManager = GetComponent<CompanyManager>();
    }

    void Update()
    {
        if (roundWinner) { 
            if (time < victorypoints)
               // time += Time.deltaTime; // Need to find the right thing for Time, it does not have deltaTime :p
        }
    }
    void AddPoints(string company)
    {
        NetworkObject nObject = coach._spawnedUsers[comManager._companies[company]];

        nObject.GetComponent<PlayerData>().points += pipeplacepoint;
    }
    void RemovePoints(string company)
    {
        NetworkObject nObject = coach._spawnedUsers[comManager._companies[company]];
        nObject.GetComponent<PlayerData>().points -= piperemovepoint;
    }

    void CalculateRoundPoints(string company)
    {
        NetworkObject nObject = coach._spawnedUsers[comManager._companies[company]];
        //doesnt have to be list, just smth that i checks if there is 1 winner already

        if (!roundWinner) {
            nObject.GetComponent<PlayerData>().points += victorypoints;
            roundWinner = true;
        } 
        else {
            // time float has to be an int (we are removing an float from an int..)
           // nObject.GetComponent<PlayerData>().points -= victorypoints - time * 10;
        }
    }
}


// Two functions, add points, remove points with parameters. Make a public instance so you can always call those functions.
// Or what you can technically also do is add a reference in the game manager in the script so you can always call the game manager. 

// If company places a pipe, 500 points is subtracted

// If company removes a pipe, 200 is added 



// If company is first and connects a to b, 4000 points is added

// If company is second and connects a to b, 3000 points is added

// If company is third and connects a to b, 2000 points is added

// If company is fourth and connects a to b, 1000 points is added

// If company is fifth and connects a to b, 0 points is added

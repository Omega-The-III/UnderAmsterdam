using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion.XR.Host;
using Fusion;

public class Pointsmanager : MonoBehaviour
{
    private int pipeplacepoint = 500;
    private int piperemovepoint = 200;
    private int victorypoints = 4000;
    private int bonusPoints = 2000;
    private float time;
    private bool roundWinner;

    void Update()
    {
        if (roundWinner)
        {
            if (time < victorypoints)
                time += Time.deltaTime;
        }

                if(Input.GetKeyDown("space")) {
        Debug.Log("spawned Users: ");
        foreach(var player in ConnectionManager.Instance._spawnedUsers) {
            Debug.Log("player: " + player.Key);
            Debug.Log("nObject: " + player.Value);
        }
        }
    }
    public void AddPoints(string company)
    {
        NetworkObject nObject = ConnectionManager.Instance._spawnedUsers[CompanyManager.Instance._companies[company]];
        nObject.GetComponent<PlayerData>().points += pipeplacepoint;
    }

    public void TeamworkBonus(string company)
    {
        Debug.Log("ADDING BONUS TO: " + company);
        foreach(var player in   ConnectionManager.Instance._spawnedUsers) {
            Debug.Log("player: " + player.Key);
            Debug.Log("nObject: " + player.Value);
        }
        Debug.Log("nObject: " + ConnectionManager.Instance._spawnedUsers[CompanyManager.Instance._companies[company]]);
        NetworkObject nObject = ConnectionManager.Instance._spawnedUsers[CompanyManager.Instance._companies[company]];
        nObject.GetComponent<PlayerData>().points += bonusPoints;
    }
    public void RemovePoints(string company)
    {
        NetworkObject nObject = ConnectionManager.Instance._spawnedUsers[CompanyManager.Instance._companies[company]];
        nObject.GetComponent<PlayerData>().points -= piperemovepoint;
    }

    public void CalculateRoundPoints(string company)
    {
        NetworkObject nObject = ConnectionManager.Instance._spawnedUsers[CompanyManager.Instance._companies[company]];

        if (!roundWinner)
        {
            nObject.GetComponent<PlayerData>().points += victorypoints;
            roundWinner = true;
        }
        else
        {
            nObject.GetComponent<PlayerData>().points -= victorypoints - (int)time * 10;
        }
    }
}
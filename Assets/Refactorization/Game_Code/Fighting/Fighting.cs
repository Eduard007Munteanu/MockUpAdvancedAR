using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Meta.Voice.Net.WebSockets;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.Rendering;
using UnityEngine;

public class Fighting{


    List<EnemyMob> enemyMobs;
    List<DefaultMob> defaultMobs;

    private float militaryMightPower;
    private float othersMightPower;
    

    //private float buildingMightPower = 500f; //Hardcoded

    //private float tempbuildingMightPower = 500f;  //Hardcoded

    private DefaultBuild theBuilding; 



    private float totalEnemyMightPower = 0;

    private float totalMobMightPower = 0;

    private bool isTriggered = false;

    



    public Fighting(List<EnemyMob> currentEnemyMobs, List<DefaultMob> currentDefaultMobs, DefaultBuild building){
        enemyMobs = currentEnemyMobs;
        defaultMobs = currentDefaultMobs;
        theBuilding = building;

       
        militaryMightPower = 20f;  //Should be actually received from the general class

        othersMightPower = militaryMightPower / 5; 

        //enemyMightPower = 2 * militaryMightPower;
    }



    



    void AddEnemyMob(EnemyMob enemyMob){
        enemyMobs.Add(enemyMob);
    }

    void RemoveEnemyMob(EnemyMob enemyMob){
        enemyMobs.Remove(enemyMob);
    }

    void AddDefaultMob(DefaultMob defaultMob){
        DefaultBuild build= defaultMob.GetBuildingAssignedTo();

        defaultMobs.Add(defaultMob);
    }

    void RemoveDefaultMob(DefaultMob defaultMob){
        DefaultBuild build= defaultMob.GetBuildingAssignedTo();
        


        defaultMobs.Remove(defaultMob);
    }

    

    public (List<DefaultMob>, List<EnemyMob>, DefaultBuild) SimulateFighting(){
        CalculateTotalMightPower();
        CalculateMobBattleWinner();


        Debug.Log("enemyMob list count is of in SimulateFighting" + enemyMobs.Count);


        var survivors = new List<EnemyMob>(enemyMobs);
        return (defaultMobs, survivors, theBuilding);
    }

    public void TriggerMoveToAllEnemy(List<EnemyMob> enemyMobs){
        if(isTriggered){
            Debug.Log("At TriggerMoveToAllEnemy, we set it to true before loop");
            for(int i = 0; i < enemyMobs.Count; i++){
                EnemyMob enemyMob = enemyMobs[i];
                Debug.Log("At TriggerMoveToAllEnemy, we set it to true after loop");
                enemyMob.SetMoving(true);
            }
        }
    }

    public void InitTrigger(){
        isTriggered = true;
    }



    private void DealDamageToTheBuilding(){
        if (theBuilding != null)
        {
            theBuilding.SetHP(theBuilding.GetHP() - totalEnemyMightPower);
            Debug.Log("In DealDamageToBuilding, remaining health " + theBuilding.GetHP());
            
            if (theBuilding.GetHP() <= 0)
            {
                GameObject.Destroy(theBuilding.gameObject);
                theBuilding = null;
                InitTrigger();
            }
        }

    }





    public void CalculateMobBattleWinner(){
        float result = totalMobMightPower - totalEnemyMightPower;
        if(result <= 0){
            if(theBuilding != null && enemyMobs.Count == 0){
                DealDamageToTheBuilding();
            } else {
                // InitTrigger();
                EnemyWonCalculation();
                Debug.Log("Is trigger from CalculateMobBattleWinner has been called");
            }
        } else if(result > 0){
            MobsWonCalculation();
        }
    }



    void MobsWonCalculation(){
        Debug.Log("We called in Fighting MobsWonCalculation");
        float tempTotalEnemyMightPower = totalEnemyMightPower;
        totalEnemyMightPower = 0f;


        foreach (var enemy in enemyMobs) {
            if (enemy != null)
                GameObject.Destroy(enemy.gameObject);
        }
        enemyMobs.Clear();


        for(int i = 0; i < defaultMobs.Count; i++){
            float individualMobPower = defaultMobs[i].GetMightPower();
            float result = tempTotalEnemyMightPower - individualMobPower;
            
            if(result > 0){
                defaultMobs[i].SetMightPower(0);
                GameObject.Destroy(defaultMobs[i].gameObject);
                tempTotalEnemyMightPower = result;
            } else{
                defaultMobs[i].SetMightPower(Math.Abs(result));
                tempTotalEnemyMightPower = 0;
            }
        }

        defaultMobs.RemoveAll(m => m == null);
        
    }

    void EnemyWonCalculation() {
        Debug.Log("We called in Fighting EnemyWonCalculation");
        float tempTotalMobMightPower = totalMobMightPower;
        totalMobMightPower = 0f;


        foreach (var defaultMob in defaultMobs) {
            if (defaultMob != null)
                GameObject.Destroy(defaultMob.gameObject);
        }
        defaultMobs.Clear();


        for(int i = 0; i < enemyMobs.Count; i++){
            float individualEnemyPower = enemyMobs[i].GetMightPower();
            float result = tempTotalMobMightPower - individualEnemyPower;
            if(result > 0){
                enemyMobs[i].SetMightPower(0);
                GameObject.Destroy(enemyMobs[i].gameObject);
                tempTotalMobMightPower = result;
            } else{
                enemyMobs[i].SetMightPower(Math.Abs(result));
                tempTotalMobMightPower = 0;
            }
        }

        enemyMobs.RemoveAll(m => m == null);
        


    }



    void CalculateTotalMightPower(){
        
        totalEnemyMightPower =  0;//enemyMightPower * enemyMobs.Count;
        foreach(EnemyMob enemyMOBOS in enemyMobs ){
            totalEnemyMightPower += enemyMOBOS.GetMightPower();
        }





        Debug.Log("The nice TotalEnemyMightPower is " + totalEnemyMightPower);
        
        //totalMobMightPower =  (othersMightPower * defaultMobs["other"].Count ) + (militaryMightPower * defaultMobs["military"].Count);

        totalMobMightPower = 0;
        foreach(DefaultMob MOBOS in defaultMobs){
            totalMobMightPower += MOBOS.GetMightPower();
        }


        Debug.Log("The nice TotalMobMightPower is " + totalMobMightPower);
        //Debug.Log("The nice defaultMobs['other'] is " + defaultMobs["other"] + " and it's count of " + defaultMobs["other"].Count +  "defaultMobs['military'] is " + defaultMobs["military"] + " and it's count of " + defaultMobs["military"].Count);
    }

   




     







}
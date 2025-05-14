using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Meta.Voice.Net.WebSockets;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.Rendering;
using UnityEngine;
using System.Linq;

public class Fighting{


    List<EnemyMob> enemyMobs;
    List<DefaultMob> defaultMobs;


    

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

        
        Debug.Log("AT FIGHTING > Enemy powers: " +
            string.Join(", ", enemyMobs.Select(m => m.GetMightPower())));
        
        Debug.Log("AT FIGHTING > Default powers: " +
            string.Join(", ", defaultMobs.Select(m => m.GetMightPower())));
       
        
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


       


        // var survivors = new List<EnemyMob>(enemyMobs);


        Debug.Log("AT FIGHTING SIMULATEFIGHTING > EnemyMobs Count is " + enemyMobs.Count);
        Debug.Log("At FIGHTING SIMULATEFIGHTING > DefaultMobs Count is " + defaultMobs.Count);

        Debug.Log("AT FIGHTING SIMULATEFIGHTING > Enemy powers: " +
            string.Join(", ", enemyMobs.Select(m => m.GetMightPower())));
        
        Debug.Log("AT FIGHTING SIMULATEFIGHTING  > Default powers: " +
            string.Join(", ", defaultMobs.Select(m => m.GetMightPower())));


        return (new List<DefaultMob>(defaultMobs), new List<EnemyMob>(enemyMobs), theBuilding);
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
            if(theBuilding != null && defaultMobs.Count == 0){
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
        Debug.Log("AT FIGHTING > We called in Fighting MobsWonCalculation");
        float tempTotalEnemyMightPower = totalEnemyMightPower;
        totalEnemyMightPower = 0f;


        foreach (var enemy in enemyMobs) {
            if (enemy != null){
                enemy.SetMightPower(0);
                GameObject.Destroy(enemy.gameObject);
            }
                
        }

        Debug.Log("AT FIGHTING > Mobs Won Calculation updated enemyMob power : " +
            string.Join(", ", enemyMobs.Select(m => m.GetMightPower())));


        //enemyMobs.Clear();




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
                break;
            }
        }

        Debug.Log("AT FIGHTING > Mobs Won Calculation updated defaultMobs power : " +
            string.Join(", ", defaultMobs.Select(m => m.GetMightPower())));

        //defaultMobs.RemoveAll(m => m == null);



        enemyMobs.RemoveAll(m => m.GetMightPower() == 0);
        defaultMobs.RemoveAll(m => m.GetMightPower() == 0);


        Debug.Log("AT FIGHTING > EnemyMobs Count is " + enemyMobs.Count);
        Debug.Log("At FIGHTING > DefaultMobs Count is " + defaultMobs.Count);




        
    }

    





    void EnemyWonCalculation() {
        Debug.Log("AT FIGHTING > We called in Fighting EnemyWonCalculation");
        
        float tempTotalMobMightPower = totalMobMightPower;
        totalMobMightPower = 0f;

        // Destroy all friendly mobs
        foreach (var mob in defaultMobs)
        {
            if (mob != null)
                mob.SetMightPower(0);
                GameObject.Destroy(mob.gameObject);
        }

        Debug.Log("AT FIGHTING > Enemy Won Calculation updated defaultMobs power : " +
            string.Join(", ", defaultMobs.Select(m => m.GetMightPower())));

        
        for (int i = 0; i < enemyMobs.Count; i++)
        {
            if (enemyMobs[i] == null) continue;

            float enemyPower = enemyMobs[i].GetMightPower();

            float result = tempTotalMobMightPower - enemyPower;

            if (result > 0)
            {
                enemyMobs[i].SetMightPower(0);
                GameObject.Destroy(enemyMobs[i].gameObject);
                tempTotalMobMightPower = result;
            }
            else
            {
                enemyMobs[i].SetMightPower(Mathf.Abs(result));
                tempTotalMobMightPower = 0;
                break;
            }
        }

        Debug.Log("AT FIGHTING > Enemy Won Calculation updated enemyMob power : " +
            string.Join(", ", enemyMobs.Select(m => m.GetMightPower())));




        enemyMobs.RemoveAll(m => m.GetMightPower() == 0);
        defaultMobs.RemoveAll(m => m.GetMightPower() == 0);

        Debug.Log("AT FIGHTING > EnemyMobs Count is " + enemyMobs.Count);
        Debug.Log("At FIGHTING > DefaultMobs Count is " + defaultMobs.Count);


        // Safe cleanup: remove entries whose GameObject is destroyed
        //enemyMobs.RemoveAll(m => m == null || m.gameObject == null);
        //defaultMobs.RemoveAll(m => m == null || m.gameObject == null);


        

    }




    void CalculateTotalMightPower(){
        
        totalEnemyMightPower =  0;//enemyMightPower * enemyMobs.Count;
        foreach(EnemyMob enemyMOBOS in enemyMobs ){
            totalEnemyMightPower += enemyMOBOS.GetMightPower();
        }


        Debug.Log("AT FIGHTING > TotalEnemyMightPower is " + totalEnemyMightPower);



        totalMobMightPower = 0;
        foreach(DefaultMob MOBOS in defaultMobs){
            totalMobMightPower += MOBOS.GetMightPower();
        }

        Debug.Log("AT FIGHTING > totalMobMightPower is " + totalMobMightPower);


        
        
    }

   




     







}
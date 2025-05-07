using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Meta.Voice.Net.WebSockets;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;

public class Fighting{


    List<EnemyMob> enemyMobs;
    Dictionary<string,List<DefaultMob>> defaultMobs;

    private float militaryMightPower;
    private float othersMightPower;
    private float enemyMightPower;

    private float buildingMightPower = 500f; //Hardcoded



    private float totalEnemyMightPower = 0;

    private float totalMobMightPower = 0;

    



    public Fighting(List<EnemyMob> currentEnemyMobs, Dictionary<string,List<DefaultMob>> currentDefaultMobs){
        enemyMobs = currentEnemyMobs;
        defaultMobs = currentDefaultMobs;

       
        militaryMightPower = 20f;  //Should be actually received from the general class

        othersMightPower = militaryMightPower / 5; 

        enemyMightPower = militaryMightPower;
    }



    



    void AddEnemyMob(EnemyMob enemyMob){
        enemyMobs.Add(enemyMob);
    }

    void RemoveEnemyMob(EnemyMob enemyMob){
        enemyMobs.Remove(enemyMob);
    }

    void AddDefaultMob(DefaultMob defaultMob){
        DefaultBuild build= defaultMob.GetBuildingAssignedTo();
        string type = build.GetBuildingClass();

        defaultMobs[type].Add(defaultMob);
    }

    void RemoveDefaultMob(DefaultMob defaultMob){
        DefaultBuild build= defaultMob.GetBuildingAssignedTo();
        string type = build.GetBuildingClass();


        defaultMobs[type].Remove(defaultMob);
    }

    

    public (Dictionary<string, List<DefaultMob>>, List<EnemyMob>) SimulateFighting(){
        CalculateTotalMightPower();
        CalculateMobBattleWinner();

        return (defaultMobs, enemyMobs);
    }



    public void CalculateMobBattleWinner(){
        float result = totalMobMightPower - totalEnemyMightPower;
        if(result < 0){
            EnemyWonCalculation();
        } else if(result > 0){
            MobsWonCalculation();
        }
    }



    void MobsWonCalculation(){
        float tempTotalEnemyMightPower = totalEnemyMightPower;
        enemyMobs.Clear();
        Dictionary<string, int> defaultMobsLength = new Dictionary<string, int>();
        foreach(var kvp in defaultMobs){
            string key = kvp.Key;
            List<DefaultMob> list = kvp.Value;
            defaultMobsLength[key] = list.Count;
        }

        if (defaultMobsLength.ContainsKey("other")) {
            int count = defaultMobsLength["other"];
            for (int i = count - 1; i >= 0; i--) {
                if (tempTotalEnemyMightPower > 0) {
                    tempTotalEnemyMightPower -= othersMightPower;
                    defaultMobs["other"].RemoveAt(i);
                }
            }
        }


        if (defaultMobsLength.ContainsKey("military")) {
            int count = defaultMobsLength["military"];
            for (int i = count - 1; i >= 0; i--) {
                if (tempTotalEnemyMightPower > 0) {
                    tempTotalEnemyMightPower -= militaryMightPower;
                    defaultMobs["military"].RemoveAt(i);
                }
            }
        }
    }

    void EnemyWonCalculation() {
        float tempTotalMobMightPower = totalMobMightPower;

        foreach (var list in defaultMobs.Values) {
            list.Clear(); 
        }

        for (int i = enemyMobs.Count - 1; i >= 0; i--) {
            if (tempTotalMobMightPower > 0) {
                tempTotalMobMightPower -= enemyMightPower;
                enemyMobs.RemoveAt(i);
            }
        }
    }



    void CalculateTotalMightPower(){
        totalEnemyMightPower =  enemyMightPower * enemyMobs.Count;
        Debug.Log("The nice TotalEnemyMightPower is " + totalEnemyMightPower);
        totalMobMightPower =  (othersMightPower * defaultMobs["other"].Count ) + (militaryMightPower * defaultMobs["military"].Count);
        Debug.Log("The nice TotalMobMightPower is " + totalMobMightPower);
        Debug.Log("The nice defaultMobs['other'] is " + defaultMobs["other"] + " and it's count of " + defaultMobs["other"].Count +  "defaultMobs['military'] is " + defaultMobs["military"] + " and it's count of " + defaultMobs["military"].Count);
    }

   




     







}
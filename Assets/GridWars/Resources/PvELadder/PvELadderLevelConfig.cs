using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class PvELadderLevelConfig : ScriptableObject {
    public float playerPowerRate, cpuPowerRate;
    [System.Serializable]
    public class UnitAdjustment { //in pct +/-
        public float speed = 1;
        public float damage = 1;
        public float maxHP = 1;
        public float regen = 1;
    }
    public UnitAdjustment chopper, tanker, tank, mobilesam;

    public void AdjustUnit(GameUnit _unit){
        Debug.Log(_unit.GetType().ToString());
        if (_unit.GetType() == typeof(Chopper)) {
            Chopper c = _unit as Chopper;
            Debug.Log(c.maxSpeed);
            c.maxSpeed *= chopper.speed;
            c.maxForwardSpeed *= chopper.speed;
            Debug.Log(c.maxSpeed);
            c.thrust *= chopper.speed;
        }
                
    }
	
}

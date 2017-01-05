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
        public float scale = 1;
    }
    [System.Serializable]
    public class PlayerUnitAdjustments {
        public string player;
        public UnitAdjustment chopper, tanker, tank, mobilesam;
    }

    public List<PlayerUnitAdjustments> unitAdjustments= new List<PlayerUnitAdjustments>() { {new PlayerUnitAdjustments {player = "Player"}}, {new PlayerUnitAdjustments{player = "CPU"}}};

    public string levelDescription;
    public string enemyName;
    public bool isBossLevel;
    public Boss boss;
    [System.Serializable]
    public class Boss {
        public GameUnit prefab;
        public UnitAdjustment bossAdjustments;
    }

    public void AdjustBossUnit (GameUnit _unit){
        if (_unit.GetType() == typeof(Tank)) {
            Tank k = _unit as Tank;
            UnitAdjustment tank = boss.bossAdjustments;
            k.maxSpeed *= tank.speed;
            k.AdjustThrustByFactor(tank.speed);
            k.AdjustMaxHitpointsByFactor(tank.maxHP);
            k.hitPoints = k.maxHitPoints;
            k.AdjustHitPointGenByFactor(tank.regen);
            k.AdjustWeaponsDamageByFactor(tank.damage);
            k.AdjustScaleByFactor(tank.scale);
            k.damageThrustAdjustment = 0;
            k.GetComponent<Rigidbody>().mass *= 40;
            k.AdjustThrustByFactor(40);
        }
    }


    public void AdjustUnit(GameUnit _unit, int playerNum){
        playerNum--;
        Debug.Log(_unit.GetType().ToString());
        if (_unit.GetType() == typeof(Chopper)) {
            Chopper c = _unit as Chopper;
            UnitAdjustment chopper = unitAdjustments[playerNum].chopper;
            c.maxSpeed *= chopper.speed;
            c.maxForwardSpeed *= chopper.speed;
            c.AdjustThrustByFactor(chopper.speed);
            c.AdjustMaxHitpointsByFactor(chopper.maxHP);
            c.AdjustHitPointGenByFactor(chopper.regen);
            c.AdjustWeaponsDamageByFactor(chopper.damage);
        }
        if (_unit.GetType() == typeof(Tank)) {
            Tank k = _unit as Tank;
            UnitAdjustment tank = unitAdjustments[playerNum].tank;
            k.maxSpeed *= tank.speed;
            k.AdjustThrustByFactor(tank.speed);
            k.AdjustMaxHitpointsByFactor(tank.maxHP);
            k.AdjustHitPointGenByFactor(tank.regen);
            k.AdjustWeaponsDamageByFactor(tank.damage);
        }
        if (_unit.GetType() == typeof(Tanker)) {
            Tanker t = _unit as Tanker;
            UnitAdjustment tanker = unitAdjustments[playerNum].tanker;
            t.maxSpeed *= tanker.speed;
            t.AdjustThrustByFactor(tanker.speed);
            t.AdjustMaxHitpointsByFactor(tanker.maxHP);
            t.AdjustHitPointGenByFactor(tanker.regen);
            t.AdjustWeaponsDamageByFactor(tanker.damage);
        }
        if (_unit.GetType() == typeof(MobileSAM)) {
            MobileSAM m = _unit as MobileSAM;
            UnitAdjustment mobilesam = unitAdjustments[playerNum].mobilesam;
            m.maxSpeed *= mobilesam.speed;
            m.AdjustThrustByFactor(mobilesam.speed);
            m.AdjustMaxHitpointsByFactor(mobilesam.maxHP);
            m.AdjustHitPointGenByFactor(mobilesam.regen);
            m.AdjustWeaponsDamageByFactor(mobilesam.damage);
        }         
    }
}

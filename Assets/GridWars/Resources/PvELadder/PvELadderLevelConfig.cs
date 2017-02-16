using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PvEConfig{
    [CreateAssetMenu]
    public class PvELadderLevelConfig : ScriptableObject {
        public float playerPowerRate = 1, cpuPowerRate = 1;
        public float gameSpeed = 1;

        [System.Serializable]
        public class UnitAdjustment { //in pct +/-
            public float speed = 1;
            public float damage = 1;
            public float fireRate = 1;
            public float maxHP = 1;
            public float regen = 1;
            public float scale = 1;
        }
        [System.Serializable]
        public class PlayerUnitAdjustments {
            public string player;
            public UnitAdjustment chopper = new UnitAdjustment();
            public UnitAdjustment tanker = new UnitAdjustment();
            public UnitAdjustment tank = new UnitAdjustment();
            public UnitAdjustment mobilesam = new UnitAdjustment();
        }

        public List<PlayerUnitAdjustments> unitAdjustments= new List<PlayerUnitAdjustments>() { {new PlayerUnitAdjustments {player = "Player"}}, {new PlayerUnitAdjustments{player = "CPU"}}};

        public string levelDescription;
        public string enemyName;
        public Texture2D enemyAvatar;
        public bool isBossLevel;
        public Boss boss;

        public bool randomizeUnitMod;
        public float randomizeInterval; //how often do we randomize

        #region RandomizeUnitMods

        public RandomizeUnit random = new RandomizeUnit();

        [System.Serializable]
        public class RandomizeUnit {
            public List<PlayerUnitAdjustments> randomUnitAdjustments= new List<PlayerUnitAdjustments>() { {new PlayerUnitAdjustments {player = "Player"}}, {new PlayerUnitAdjustments{player = "CPU"}}};    
        }
       
        List<int> rand = new List<int>(){0,1,2,3};
        int last = -1;
        public void Randomize(){
            if (rand.Count == 0) {
                rand = new List<int>(){0,1,2,3};
            }
            int thisRand = rand.PickRandom<int>();
            rand.Remove(thisRand);
            for (int i = 0; i < 2; i++) {
                switch (last) {
                    case 0:
                        unitAdjustments[i].chopper = new UnitAdjustment();
                        break;
                    case 1:
                        unitAdjustments[i].tanker = new UnitAdjustment();
                        break;
                    case 2:
                        unitAdjustments[i].tank = new UnitAdjustment();
                        break;
                    case 3: 
                        unitAdjustments[i].mobilesam = new UnitAdjustment();
                        break;
                }
                Debug.Log("This rand: " + thisRand + " at " + Time.time);
                switch (thisRand) {
                    case 0:
                        unitAdjustments[i].chopper = random.randomUnitAdjustments[i].chopper;
                        break;
                    case 1:
                        unitAdjustments[i].tanker = random.randomUnitAdjustments[i].tanker;
                        break;
                    case 2:
                        unitAdjustments[i].tank = random.randomUnitAdjustments[i].tank;
                        break;
                    case 3: 
                        unitAdjustments[i].mobilesam = random.randomUnitAdjustments[i].mobilesam;
                        break;
                }
            }
            last = thisRand;
        }

        void OnDestroy(){
            if (randomizeUnitMod) {

            }
        }

        #endregion

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
                k.AdjustWeaponsFireRateByFactor(tank.fireRate);
                k.AdjustWeaponsClipReloadTimeByFactor(tank.fireRate);
                k.AdjustWeaponsDamageByFactor(tank.damage);
                k.AdjustScaleByFactor(tank.scale);
                k.damageThrustAdjustment = 0;
                k.GetComponent<Rigidbody>().mass *= 40;
                k.AdjustThrustByFactor(40);
            }
            if (_unit.GetType() == typeof(Chopper)) {
                Chopper k = _unit as Chopper;
                k.immuneToGround = true;
                UnitAdjustment chopper = boss.bossAdjustments;
                k.maxSpeed *= chopper.speed;
                k.AdjustThrustByFactor(chopper.speed);
                k.AdjustMaxHitpointsByFactor(chopper.maxHP);
                k.hitPoints = k.maxHitPoints;
                k.AdjustHitPointGenByFactor(chopper.regen);
                k.AdjustWeaponsDamageByFactor(chopper.damage);
                k.AdjustWeaponsFireRateByFactor(chopper.fireRate);
                k.AdjustWeaponsClipReloadTimeByFactor(chopper.fireRate);
                k.AdjustScaleByFactor(chopper.scale);
                k.AdjustFireThrottle(1);
                k.damageThrustAdjustment = 0;
                //k.GetComponent<Rigidbody>().mass *= 40;
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
                c.AdjustWeaponsFireRateByFactor(chopper.fireRate);
                c.AdjustWeaponsClipReloadTimeByFactor(chopper.fireRate);
                if (chopper.fireRate != 1) {
                    c.AdjustFireThrottle(1);
                }

            }
            if (_unit.GetType() == typeof(Tank)) {
                Tank k = _unit as Tank;
                UnitAdjustment tank = unitAdjustments[playerNum].tank;
                k.maxSpeed *= tank.speed;
                k.AdjustThrustByFactor(tank.speed);
                k.AdjustMaxHitpointsByFactor(tank.maxHP);
                k.AdjustHitPointGenByFactor(tank.regen);
                k.AdjustWeaponsDamageByFactor(tank.damage);
                k.AdjustWeaponsFireRateByFactor(tank.fireRate);
                k.AdjustWeaponsClipReloadTimeByFactor(tank.fireRate);
                if (tank.fireRate != 1) {
                    k.AdjustFireThrottle(1);
                }
            }
            if (_unit.GetType() == typeof(Tanker)) {
                Tanker t = _unit as Tanker;
                UnitAdjustment tanker = unitAdjustments[playerNum].tanker;
                t.maxSpeed *= tanker.speed;
                t.AdjustThrustByFactor(tanker.speed);
                t.AdjustMaxHitpointsByFactor(tanker.maxHP);
                t.AdjustHitPointGenByFactor(tanker.regen);
                t.AdjustWeaponsDamageByFactor(tanker.damage);
                t.AdjustWeaponsFireRateByFactor(tanker.fireRate);
                t.AdjustWeaponsClipReloadTimeByFactor(tanker.fireRate);
                if (tanker.fireRate != 1) {
                    t.AdjustFireThrottle(1);
                }
            }
            if (_unit.GetType() == typeof(MobileSAM)) {
                MobileSAM m = _unit as MobileSAM;
                UnitAdjustment mobilesam = unitAdjustments[playerNum].mobilesam;
                m.maxSpeed *= mobilesam.speed;
                m.AdjustThrustByFactor(mobilesam.speed);
                m.AdjustMaxHitpointsByFactor(mobilesam.maxHP);
                m.AdjustHitPointGenByFactor(mobilesam.regen);
                m.AdjustWeaponsDamageByFactor(mobilesam.damage);
                m.AdjustWeaponsFireRateByFactor(mobilesam.fireRate);
                m.AdjustWeaponsClipReloadTimeByFactor(mobilesam.fireRate);
                if (mobilesam.fireRate != 1) {
                    m.AdjustFireThrottle(1);
                }
            }         
        }
    }
}

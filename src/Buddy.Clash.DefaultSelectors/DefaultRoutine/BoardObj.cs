namespace Buddy.Clash.DefaultSelectors
{
    using System;
    using System.Collections.Generic;
    
    public enum boardObjType
    {
        NONE,
        BUILDING,
        MOB,
        AOE,
        PROJECTILE
    }

    public enum transportType
    {
        NONE,
        AIR,
        GROUND
    }

    public enum targetType
    {
        NONE,
        ALL,
        GROUND,
        BUILDINGS,
        IGNOREBUILDINGS
    }

    public enum affectType
    {
        NONE,
        ALL,
        ONLY_OWN,
        ONLY_ENEMIES
    }


    public class BoardObj
    {
        public boardObjType type = boardObjType.NONE;        
        public transportType Transport = transportType.NONE; //-Mob (Air, Ground)
        public targetType TargetType = targetType.NONE; //-AttacksAir, TargetOnlyBuildings
        public affectType affectOn = affectType.NONE;
        //-        public bool isHero = false; решить вопрос с башнями - сюда или в бордТип
        public VectorAI Position;
        public bool own = true;
        public int ownerIndex = -1;
        public int pID = 0;
        public int Line = 0; //1-left, 2-right
        
        public CardDB.cardName Name = CardDB.cardName.unknown;
        public CardDB.Card card = CardDB.Instance.unknownCard;
        public uint GId = 0; //-All
        public int cost = 0; //All
        public int DeployTime = 0; //-All
        public int MaxHP = 0; //-All
        public int HP = 0; //-All
        public int Atk = 0; //-All Damage
        public int Shield = 0; //-Mob
        public int Speed = 0; //-Mob
        public int HitSpeed = 0; //-All
        public int MinRange = 0; //-Mob+AreaEffect Radius
        public int Range = 0; //-Mob+AreaEffect Radius
        public int SightRange = 0; //-Mob
        public int MaxTargets = 0; //-All
        public int DamageRadius = 0;
        public bool attacking = false;
        public BoardObj attacker = null;
        public bool attacked = false;
        public BoardObj target = null;
        public int DeathEffect = 0;
        public int Tower = 0; //1,2 - PrincessTower, 11,12 - KingsTower
        public int level = 0;


        public int LifeTime = 0; //-Buildings+AreaEffect LifeDuration
        public int SpawnNumber = 0; //-Mobs+Buildings
        public int SpawnTime = 0; //-Mobs+Buildings SpawnStartTime for balle and SpawnPauseTime for CardDB
        public int SpawnInterval = 0; //-Mobs+Buildings
        public int SpawnCharacterLevel = 0;

        public bool clone = false;
        public bool frozen = false;
        public int startFrozen = 0; //-dstatime or %
        public string extraData = "";
        

        public BoardObj()
        {

        }

        public BoardObj(BoardObj bo)
        {
            this.Name = bo.Name;
            this.card = bo.card;
            this.type = bo.type;
            this.Transport = bo.Transport;
            this.affectOn = bo.affectOn;
            this.Position = bo.Position;
            this.own = bo.own;
            this.pID = bo.pID; //-????????
            this.Line = bo.Line;
            this.GId = bo.GId;
            this.cost = bo.cost;
            this.DeployTime = bo.DeployTime;
            this.DamageRadius = bo.DamageRadius;
            this.MaxHP = bo.MaxHP;
            this.HP = bo.HP;
            this.Atk = bo.Atk;
            this.Shield = bo.Shield;
            this.Speed = bo.Speed;
            this.HitSpeed = bo.HitSpeed;
            this.MinRange = bo.MinRange;
            this.Range = bo.Range;
            this.SightRange = bo.SightRange;
            this.TargetType = bo.TargetType;
            this.MaxTargets = bo.MaxTargets;
            this.attacking = bo.attacking;
            this.attacked = bo.attacked;
            this.LifeTime = bo.LifeTime;
            this.SpawnNumber = bo.SpawnNumber;
            this.SpawnInterval = bo.SpawnInterval;
            this.SpawnTime = bo.SpawnTime;
            this.SpawnCharacterLevel = bo.SpawnCharacterLevel;
            this.frozen = bo.frozen;
            this.clone = bo.clone;
            this.startFrozen = bo.startFrozen;
            this.attacker = bo.attacker;
            this.target = bo.target;
            this.DeathEffect = bo.DeathEffect;
            this.Tower = bo.Tower;
            this.extraData = bo.extraData;
        }

        public BoardObj(CardDB.cardName cName, int lvl = 0)
        {
            CardDB.Card c = CardDB.Instance.getCardDataFromName(cName, lvl);
            this.card = c;
            this.Name = c.name;
            this.type = c.type;
            this.Transport = c.Transport;
            this.affectOn = c.affectType;
            //this.Position = c.Position;
            //this.own = c.own;
            //this.pID = c.pID; //-????????
            //this.Line = c.Line;
            //this.GId = c.GId;
            this.cost = c.cost;
            this.DeployTime = c.DeployTime;
            this.DamageRadius = c.DamageRadius;
            this.MaxHP = c.MaxHP;
            this.HP = c.MaxHP;
            this.Atk = c.Atk;
            this.Shield = c.Shield;
            this.Speed = c.Speed;
            this.HitSpeed = c.HitSpeed;
            this.MinRange = c.MinRange;
            this.Range = c.MaxRange;
            this.SightRange = c.SightRange;
            this.TargetType = c.TargetType;
            this.MaxTargets = c.MultipleTargets;
            //this.attacking = c.attacking;
            //this.attacked = c.attacked;
            this.LifeTime = c.LifeTime;
            this.SpawnNumber = c.SpawnNumber;
            this.SpawnInterval = c.SpawnInterval;
            this.SpawnTime = c.SpawnPause;
            this.SpawnCharacterLevel = c.SpawnCharacterLevel;
            //this.frozen = c.frozen;
            //this.startFrozen = c.startFrozen;
            //this.attacker = c.attacker;
            //this.target = c.target;
            this.DeathEffect = c.DeathEffect;
            //this.Tower = c.Tower;
        }
        
        public string ToString(bool printAll = false)
        {
            switch (this.type)
            {
                case boardObjType.AOE:
                    if (!printAll) return this.Name + " " + this.GId + " " + this.type + " " + this.Position + " " + this.level + " " + this.LifeTime;
                    else return this.Name + " " + this.GId + " " + this.type + " " + this.Position + " lvl:" + this.level + " LifeTime:" + this.LifeTime +
                        " cost:" + this.cost + " EffectType:" + this.affectOn + " TargetType:" + this.TargetType +
                        " buffSpeed:" + this.Speed + " buffHitSpeed:" + this.HitSpeed + " Radius:" + this.Range + " Atk:" + this.Atk +
                        " SpawnInterval:" + this.SpawnInterval + " SpawnCharacterLevel:" + this.SpawnCharacterLevel;
                case boardObjType.MOB:
                    if (!printAll) return this.Name + " " + this.GId + " " + this.type + " " + this.Position + " " + this.level + " " + this.LifeTime;
                    else return this.Name + " " + this.GId + " " + this.type + " " + this.Position + " lvl:" + this.level + " LifeTime:" + this.LifeTime +
                        " cost:" + this.cost + " EffectType:" + this.affectOn + " TargetType:" + this.TargetType +
                        " buffSpeed:" + this.Speed + " buffHitSpeed:" + this.HitSpeed + " Radius:" + this.Range + " Atk:" + this.Atk +
                        " SpawnInterval:" + this.SpawnInterval + " SpawnCharacterLevel:" + this.SpawnCharacterLevel;

            }
            return "Type ERROR " + this.Name + " " + this.GId + " " + this.type;
        }

        public bool aheadOf(BoardObj bo, bool home)
        {
            if ((this.Position.Y > bo.Position.Y) == home) return true;
            else return false;
        }

        public List<attackDef> getImmediateAttackers(Playfield p)
        {
            List<attackDef> attackersList = new List<attackDef>();
            if (this.HP > 0)
            {
                List<BoardObj> enemies = this.own ? p.enemyMinions : p.ownMinions;
                foreach (BoardObj e in enemies)
                {
                    if (e.Line != this.Line && this.Line != 3) continue;
                    attackDef bo1ad = new attackDef(this, e);
                    attackDef bo2ad = new attackDef(e, this);
                    if (bo1ad.time < bo2ad.time) attackersList.Add(bo1ad);
                    else if (bo2ad.time != int.MaxValue) attackersList.Add(bo2ad);
                }

                enemies = this.own ? p.enemyBuildings : p.ownBuildings;
                foreach (BoardObj e in enemies)
                {
                    if (e.Line != this.Line && this.Line != 3) continue;
                    attackDef bo1ad = new attackDef(this, e);
                    attackDef bo2ad = new attackDef(e, this);
                    if (bo1ad.time < bo2ad.time) attackersList.Add(bo1ad);
                    else if (bo2ad.time != int.MaxValue) attackersList.Add(bo2ad);
                }

                if (this.Tower == 0)
                {
                    enemies = this.own ? p.enemyTowers : p.ownTowers;
                    foreach (BoardObj e in enemies)
                    {
                        if (e.Line != this.Line) continue;
                        attackDef bo1ad = new attackDef(this, e);
                        attackDef bo2ad = new attackDef(e, this);
                        if (bo1ad.time < bo2ad.time) attackersList.Add(bo1ad);
                        else if (bo2ad.time != int.MaxValue) attackersList.Add(bo2ad);
                    }
                }

                attackersList.Sort((a, b) => a.time.CompareTo(b.time));
            }
            return attackersList;
        }

        public List<attackDef> getPossibleAttackers(Playfield p)
        {
            List<attackDef> attackersList = new List<attackDef>();
            if (this.HP > 0)
            {
                List<BoardObj> enemies = this.own ? p.enemyMinions : p.ownMinions;
                foreach (BoardObj e in enemies)
                {
                    if (e.Line != this.Line && this.Line != 3) continue;
                    attackDef ad = new attackDef(e, this);
                    if (!ad.empty) attackersList.Add(ad);
                }

                enemies = this.own ? p.enemyBuildings : p.ownBuildings;
                foreach (BoardObj e in enemies)
                {
                    if (e.Line != this.Line && this.Line != 3) continue;
                    attackDef ad = new attackDef(e, this);
                    if (!ad.empty) attackersList.Add(ad);
                }

                enemies = this.own ? p.enemyTowers : p.ownTowers;
                foreach (BoardObj e in enemies)
                {
                    if (e.Line != this.Line && this.Line != 3) continue;
                    attackDef ad = new attackDef(e, this);
                    if (!ad.empty) attackersList.Add(ad);
                }

                attackersList.Sort((a, b) => a.time.CompareTo(b.time));
            }
            return attackersList;
        }


        /*
        public void objectDied(Playfield p)
        {
            switch (this.type)
            {

                case boardObjType.BUILDING:
                    List<BoardObj> list = bo.own ? this.ownBuildings : this.enemyBuildings;
                    //if (bo.own) p.tempTrigger.ownBoDied++; //TODO triggers
                    //else p.tempTrigger.enemyBoDied++;
                    switch (this.Tower)
                    {
                        case 0: //just building
                            //TODO: possible effects
                            break;
                        case 1: goto case 2;
                        case 2:
                            //if (bo.own) p.tempTrigger.ownTowerDied++; //TODO triggers
                            //else p.tempTrigger.enemyTowerDied++;
                            p.ownTowersState = p.getTowersState();
                            break;
                    }
                    if (this.Tower == 0)
                    {

                    }
                    else if (bo.Tower < 10) //PrincessTower
                    {
                        towersState
                    }
                    else //KingsTower
                    {
                        if (bo.own)
                        {
                            //p.tempTrigger.ownTowerDied++; //TODO triggers
                            this.ownTowersState = towersState.allDestroyed;
                        }
                        else
                        {
                            //p.tempTrigger.enemyTowerDied++; //TODO triggers
                            this.enemyTowersState = towersState.allDestroyed;
                        }
                    }


                    break;
                case boardObjType.MOB:
                    //TODO: possible effects
                    break;
            }
        }*/
        
        

    }

}
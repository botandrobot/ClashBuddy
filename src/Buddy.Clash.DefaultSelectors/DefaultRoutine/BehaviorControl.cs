namespace Buddy.Clash.DefaultSelectors
{
    using Buddy.Clash.DefaultSelectors.Settings;
    using Buddy.Engine.Settings;
    using System.Collections.Generic;

    public class BehaviorControl : Behavior
    {
        public override string BehaviorName() { return "Control"; }
        //internal static ControlSettings Settings => SettingsManager.GetSetting<ControlSettings>("Control");



        public override Cast getBestCast(Playfield p)
        {
            Cast bc = null;
            group ownGroup = p.getGroup(true, 85, boPriority.byTotalNumber, 3000);
            if (ownGroup != null)
            {
                /*if (ownGroup.Position.)
                {

                }
                else*/
                {
                    int airAreaDPSBonus = 1000 - (ownGroup.hiHPboAirAreaDPS + ownGroup.avgHPboAirAreaDPS + ownGroup.lowHPboAirAreaDPS);
                    if (airAreaDPSBonus < 0) airAreaDPSBonus = 0;

                    int groundAreaDPSBonus = 800 - (ownGroup.hiHPboGroundAreaDPS + ownGroup.avgHPboAirAreaDPS + ownGroup.lowHPboGroundAreaDPS);
                    if (groundAreaDPSBonus < 0) groundAreaDPSBonus = 0;

                    int airDPSBonus = 500 - (ownGroup.hiHPboAirDPS + ownGroup.avgHPboAirDPS + ownGroup.lowHPboAirDPS);
                    if (airDPSBonus < 0) airDPSBonus = 0;

                    int flyBonus = 800 - (ownGroup.lowHPboAirTransport + ownGroup.avgHPboAirTransport + ownGroup.hiHPboAirTransport);
                    if (flyBonus < 0) flyBonus = 0;

                    int tankBonus = 1600 - ownGroup.hiHPboHP;
                    if (tankBonus < 0) tankBonus = 0;

                    int partyBonus = (15 - (ownGroup.lowHPbo.Count + ownGroup.avgHPbo.Count + ownGroup.hiHPbo.Count)) * 10;

                    Handcard retval = null;
                    int val = 0;
                    int tmpval = 0;
                    foreach (Handcard hc in p.ownHandCards)
                    {
                        tmpval = 0;
                        if (hc.card.DamageRadius > 1000)
                        {
                            if (hc.card.TargetType == targetType.ALL) tmpval += airAreaDPSBonus;
                            else if (hc.card.TargetType == targetType.GROUND) tmpval += groundAreaDPSBonus;
                        }
                        if (hc.card.TargetType == targetType.ALL) tmpval += airDPSBonus;
                        if (hc.card.Transport == transportType.AIR && hc.card.TargetType != targetType.BUILDINGS) tmpval += flyBonus;
                        if (hc.card.MaxHP > 600) tmpval += tankBonus + hc.card.MaxHP / 10;
                        tmpval += hc.card.SpawnNumber * partyBonus;
                        tmpval += hc.card.SpawnNumber * hc.card.Atk;
                        if (tmpval > val)
                        {
                            val = tmpval;
                            retval = hc;
                        }
                    }
                    bc = new Cast(retval.name, ownGroup.Position, retval);
                }
            }
            else if (p.ownMana >= 9)
            {
                if (p.noEnemiesOnMySide())
                {
                    if (p.ownMinions.Count == 0)
                    {
                        Handcard tank = p.getTankCard();
                        if (tank != null)
                        {
                            bc = new Cast(tank.name, p.getBackPosition(tank), tank);
                        }
                        else
                        {
                            List<Handcard> BuildingsCard = p.getCardsByType(boardObjType.BUILDING);
                            if (BuildingsCard.Count > 0)
                            {
                                Handcard hut = BuildingsCard[0];
                                if (hut.card.name != CardDB.cardName.goblinhut && BuildingsCard.Count > 0)
                                {
                                    int count = BuildingsCard.Count;
                                    for (int i = 1; i < count; i++)
                                    {
                                        if (BuildingsCard[i].card.name == CardDB.cardName.goblinhut)
                                        {
                                            hut = BuildingsCard[i];
                                            break;
                                        }
                                    }
                                }
                                bc = new Cast(hut.name, p.getBackPosition(hut), hut);
                            }
                            else
                            {
                                Handcard CheapestCard = p.getCheapestCard(boardObjType.MOB, targetType.ALL);
                                if (CheapestCard == null)
                                {
                                    CheapestCard = p.getCheapestCard(boardObjType.MOB, targetType.NONE);
                                    if (CheapestCard == null)
                                    {
                                        CheapestCard = p.getCheapestCard(boardObjType.BUILDING, targetType.NONE);
                                        if (CheapestCard == null)
                                        {
                                            CheapestCard = p.getCheapestCard(boardObjType.PROJECTILE, targetType.NONE);
                                            if (CheapestCard == null) bc = null;
                                            else bc = new Cast(CheapestCard.name, p.enemyBuildings[0].Position, CheapestCard);
                                        }
                                    }
                                }
                                bc = new Cast(CheapestCard.name, p.getBackPosition(CheapestCard), CheapestCard);
                            }
                        }
                    }
                    else
                    {
                        BoardObj m = p.getFrontMob();
                        Handcard hc = p.getPatnerForMobInPeace(m);
                        bc = new Cast(hc.name, m.Position, hc);
                    }
                }
                else
                {
                    p.rotateLines();
                    bc = p.bestCast;
                }
            }
            else
            {
                p.rotateLines();
                bc = p.bestCast;
            }

            if (bc != null) Helpfunctions.Instance.ErrorLog("BestCast:" + bc.SpellName + " " + bc.Position.ToString());
            else Helpfunctions.Instance.ErrorLog("BestCast: null");


            return bc;
        }

        public override float getPlayfieldValue(Playfield p)
        {
            if (p.value >= -2000000) return p.value;
            int retval = 0;
            return retval;
        }

        public override int getBoValue(BoardObj bo, Playfield p)
        {
            int retval = 5;
            return retval;
        }

        public override int getPlayCardPenality(CardDB.Card card, Playfield p)
        {
            return 0;
        }
    }
}
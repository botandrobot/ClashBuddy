using Robi.Common;
using Serilog;

namespace Robi.Clash.DefaultSelectors
{
    using System;
    using System.Collections.Generic;

    public class KnowledgeBase
    {
        private static readonly ILogger Logger = LogProvider.CreateLogger<KnowledgeBase>();
        Dictionary<CardDB.cardName, Dictionary<CardDB.cardName, int>> OppositeDB = new Dictionary<CardDB.cardName, Dictionary<CardDB.cardName, int>>();

        private static KnowledgeBase instance;

        public static KnowledgeBase Instance
        {
            get
            {
                return instance ?? (instance = new KnowledgeBase());
            }
        }

        private KnowledgeBase()
        {
            setupOppositeDB();
        }

        public Handcard getOppositeCard(Playfield p, BoardObj attacker, bool canWait = true)
        {
            Handcard bestCard = null;
            if (OppositeDB.ContainsKey(attacker.Name))
            {
                Dictionary<CardDB.cardName, int> tmp = OppositeDB[attacker.Name];
                int bestCardVal = -1;
                CardDB.cardName name;
                foreach (Handcard hc in p.ownHandCards)
                {
                    name = hc.card.name;
                    if (canWait || hc.manacost <= p.ownMana)
                    {
                        if (tmp.ContainsKey(name))
                        {
                            if (tmp[name] > bestCardVal)
                            {
                                bestCardVal = tmp[name];
                                bestCard = hc;
                                bestCard.val = bestCardVal;
                                bestCard.missingMana = hc.manacost - p.ownMana;
                            }
                            if (bestCardVal == 100) break;
                        }
                    }
                }
            }
            return bestCard;
        }

        public Handcard getOppositeCard(Playfield p, group Group, bool canWait = true, int gangSize = 5)
        {
            Handcard bestCard = null;
            int bestVal = int.MinValue;
            int tmpVal;
            foreach (Handcard hc in p.ownHandCards)
            {
                if (canWait || hc.manacost <= p.ownMana)
                {
                    tmpVal = 0;
                    int numAirTransport = Group.lowHPboAirTransport + Group.avgHPboAirTransport + Group.hiHPboAirTransport;
                    int groupSize = Group.lowHPbo.Count + Group.avgHPbo.Count + Group.hiHPbo.Count;
                    if (hc.card.aoeAir) tmpVal += 3;
                    if (hc.card.type == boardObjType.MOB) tmpVal += 2;
                    if (hc.card.Transport == transportType.AIR) tmpVal += 3;
                    if (numAirTransport > 0)
                    {
                        if (hc.card.TargetType == targetType.ALL) tmpVal += 20;
                        if (Group.lowHPboAirTransport >= gangSize && hc.card.aoeAir) tmpVal += 5;
                    }
                    if (groupSize > numAirTransport)
                    {
                        if (Group.lowHPbo.Count - Group.lowHPboAirTransport >= gangSize)
                        {
                            if (hc.card.aoeAir) tmpVal += 15;
                            if (hc.card.aoeGround) tmpVal += 15;
                        }
                        if (hc.card.TargetType == targetType.GROUND) tmpVal += 15;
                        if (hc.card.TargetType == targetType.ALL) tmpVal += 15;
                    }
                    foreach (BoardObj tmp in Group.hiHPbo)
                    {
                        if (OppositeDB.ContainsKey(tmp.card.name))
                        {
                            var opp = OppositeDB[tmp.card.name];
                            if (opp.ContainsKey(hc.card.name)) tmpVal += opp[hc.card.name] / 10;
                        }
                    }
                    foreach (BoardObj tmp in Group.avgHPbo)
                    {
                        if (OppositeDB.ContainsKey(tmp.card.name))
                        {
                            var opp = OppositeDB[tmp.card.name];
                            if (opp.ContainsKey(hc.card.name)) tmpVal += opp[hc.card.name] / 10;
                        }
                    }
                    foreach (BoardObj tmp in Group.lowHPbo)
                    {
                        if (OppositeDB.ContainsKey(tmp.card.name))
                        {
                            var opp = OppositeDB[tmp.card.name];
                            if (opp.ContainsKey(hc.card.name)) tmpVal += opp[hc.card.name] / 15;
                        }
                    }
                    if (bestVal < tmpVal)
                    {
                        bestVal = tmpVal;
                        bestCard = hc;
                        bestCard.val = tmpVal;
                        bestCard.missingMana = hc.manacost - p.ownMana;
                    }
                }
            }
            return bestCard;
        }

        public opposite getOppositeToAll(Playfield p, BoardObj defender, bool canWait = true)
        {
            opposite bestOpposite = null;
            List<attackDef> attackersList = defender.getPossibleAttackers(p);
            if (attackersList.Count < 1) return bestOpposite;

            List<attackDef> defendersList;
            CardDB.cardName aName;
            CardDB.cardName dName;
            Dictionary<CardDB.cardName, int> aopp;

            Dictionary<CardDB.cardName, opposite> allOpposite = new Dictionary<CardDB.cardName, opposite>();
            foreach (attackDef ad in attackersList)
            {
                if (OppositeDB.ContainsKey(ad.attacker.Name))
                {
                    aopp = OppositeDB[ad.attacker.Name];
                    //-1.Get all obj on board
                    defendersList = ad.attacker.getPossibleAttackers(p);
                    foreach (var def in defendersList)
                    {
                        dName = def.attacker.Name;
                        if (aopp.ContainsKey(dName))
                        {
                            if (!allOpposite.ContainsKey(dName)) allOpposite.Add(dName, new opposite(dName, aopp[dName], def.attacker, ad.attacker));
                            else allOpposite[dName].value += aopp[dName];
                        }
                    }

                    //-2.Get all cards
                    //TODO - enemy have opposite - it depends on his mana
                    if (defender.own)
                    {
                        foreach (Handcard hc in p.ownHandCards)
                        {
                            dName = hc.card.name;
                            if ((canWait || hc.manacost <= p.ownMana) && aopp.ContainsKey(dName))
                            {
                                hc.missingMana = hc.manacost - p.ownMana;
                                if (!allOpposite.ContainsKey(dName)) allOpposite.Add(dName, new opposite(dName, aopp[dName], hc, ad.attacker));
                                else allOpposite[dName].value += aopp[dName];
                            }
                        }
                    }
                }
                else
                {
                    Handcard tmp = null;
                    int count = p.ownHandCards.Count;
                    //TODO automatic creation opposite list
                    if (ad.attacker.Transport == transportType.AIR)
                    {
                        foreach (Handcard hc in p.ownHandCards)
                        {
                            if (canWait || hc.manacost <= p.ownMana)
                            {
                                if (hc.card.TargetType == targetType.ALL)
                                {
                                    if (ad.attacker.card.DamageRadius > 1000)
                                    {
                                        if (hc.card.MaxHP > ad.attacker.Atk)
                                        {
                                            if (tmp == null || tmp.card.MaxHP < hc.card.MaxHP) tmp = hc;
                                        }
                                    }
                                    else
                                    {
                                        if (hc.card.MaxHP > ad.attacker.Atk * 4 || hc.card.SpawnNumber > 3 || hc.card.SummonNumber > 3)
                                        {
                                            tmp = hc;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (ad.attacker.TargetType == targetType.ALL)
                        {
                            foreach (Handcard hc in p.ownHandCards)
                            {
                                if (canWait || hc.manacost <= p.ownMana)
                                {
                                    if (ad.attacker.card.DamageRadius > 1000 && hc.card.MaxHP > ad.attacker.Atk)
                                    {
                                        if (tmp == null || tmp.card.MaxHP < hc.card.MaxHP) tmp = hc;
                                    }
                                    else
                                    {
                                        if (hc.card.MaxHP > ad.attacker.Atk * 4 || hc.card.SpawnNumber > 3 || hc.card.SummonNumber > 3)
                                        {
                                            tmp = hc;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else if (ad.attacker.TargetType == targetType.BUILDINGS)
                        {
                            foreach (Handcard hc in p.ownHandCards)
                            {
                                if (canWait || hc.manacost <= p.ownMana)
                                {
                                    if (hc.card.Atk > ad.attacker.HP * 5 || hc.card.SpawnNumber > 3)
                                    {
                                        tmp = hc;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (Handcard hc in p.ownHandCards)
                            {
                                if (canWait || hc.manacost <= p.ownMana)
                                {
                                    if (hc.card.Transport == transportType.AIR)
                                    {
                                        if (tmp == null || tmp.card.SpawnNumber < hc.card.SpawnNumber || tmp.card.SummonNumber < hc.card.SummonNumber) tmp = hc;
                                    }
                                }
                            }
                            if (tmp == null)
                            {
                                foreach (Handcard hc in p.ownHandCards)
                                {
                                    if (canWait || hc.manacost <= p.ownMana)
                                    {
                                        if (ad.attacker.card.DamageRadius > 1000)
                                        {
                                            if (hc.card.MaxHP > ad.attacker.Atk)
                                            {
                                                if (tmp == null || tmp.card.MaxHP < hc.card.MaxHP) tmp = hc;
                                            }
                                        }
                                        else
                                        {
                                            if (hc.card.MaxHP > ad.attacker.Atk * 4 || hc.card.SpawnNumber > 3 || hc.card.SummonNumber > 3)
                                            {
                                                tmp = hc;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (tmp == null)
                    {
                        foreach (Handcard hc in p.ownHandCards)
                        {
                            if (hc.card.type == boardObjType.PROJECTILE)
                            {
                                if (tmp == null || tmp.card.Atk < hc.card.Atk) tmp = hc;
                            }
                        }
                    }
                    if (tmp == null && p.ownMana >= 9) tmp = p.getCheapestCard(boardObjType.NONE, targetType.NONE);
                    if (tmp != null && !allOpposite.ContainsKey(tmp.card.name))
                    {
                        tmp.missingMana = tmp.manacost - p.ownMana;
                        allOpposite.Add(tmp.card.name, new opposite(tmp.card.name, 10, tmp, ad.attacker));
                    }
                }
            }

            int oppCount = allOpposite.Count;
            if (oppCount > 0)
            {
                foreach (opposite opp in allOpposite.Values)
                {
                    if (OppositeDB.ContainsKey(opp.name))
                    {
                        aopp = OppositeDB[opp.name];
                        foreach (attackDef ad in attackersList)
                        {
                            aName = ad.attacker.Name;
                            if (aopp.ContainsKey(aName))
                            {
                                opp.value -= aopp[aName];
                                //TODO: test - repeat for each or break;
                            }
                        }
                    }
                    else
                    {
                        Logger.Debug("!OppositeDB.ContainsKey");
                    }
                    if (bestOpposite != null)
                    {
                        if (bestOpposite.value < opp.value) bestOpposite = opp;
                    }
                    else bestOpposite = opp;
                }

                if (bestOpposite != null && bestOpposite.target != null && bestOpposite.hc != null && bestOpposite.hc.card != null)
                {
                    bestOpposite.target.attacker = new BoardObj(bestOpposite.hc.card.name, bestOpposite.hc.lvl);
                }
            }
            return bestOpposite;
        }

        private void setupOppositeDB()
        {
            //archers

            OppositeDB.Add(CardDB.cardName.babydragon, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.tesla, 100 },
                { CardDB.cardName.threemusketeers, 100 },
                { CardDB.cardName.minionhorde, 95 },
                { CardDB.cardName.musketeer, 90 },
                { CardDB.cardName.archer, 90 }
            });
            OppositeDB.Add(CardDB.cardName.balloon, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.minionhorde, 100 },
                { CardDB.cardName.babydragon, 95 },
                { CardDB.cardName.wizard, 95 },
                { CardDB.cardName.infernotower, 90 },
                { CardDB.cardName.tesla, 90 },
                { CardDB.cardName.minion, 50 },
                { CardDB.cardName.musketeer, 50 }
            });

            //assassin(bandit)

            OppositeDB.Add(CardDB.cardName.barbarian, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.fireball, 100 },
                { CardDB.cardName.babydragon, 95 },
                { CardDB.cardName.bomber, 95 },
                { CardDB.cardName.minionhorde, 95 },
                { CardDB.cardName.skeletonarmy, 95 },
                { CardDB.cardName.barbarian, 90 },
                { CardDB.cardName.wizard, 90 },
                { CardDB.cardName.witch, 90 },
                { CardDB.cardName.valkyrie, 90 },
                { CardDB.cardName.bombtower, 90 },
                { CardDB.cardName.minion, 85 }
            });

            //battleram
            //bomber
            //bowler

            OppositeDB.Add(CardDB.cardName.darkprince, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.minionhorde, 100 },
                { CardDB.cardName.prince, 100 },
                { CardDB.cardName.darkprince, 95 },
                { CardDB.cardName.minipekka, 95 },
                { CardDB.cardName.tombstone, 95 },
                { CardDB.cardName.cannon, 90 },
                { CardDB.cardName.tesla, 90 }
            });

            //dartgoblin
            OppositeDB.Add(CardDB.cardName.electrowizard, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.fireball, 100 },
                { CardDB.cardName.lightning, 100 },
                { CardDB.cardName.skeletonarmy, 100 },
                { CardDB.cardName.icegolem, 100 },
                { CardDB.cardName.minionhorde, 90 },
                { CardDB.cardName.barbarian, 90 },
                { CardDB.cardName.angrybarbarian, 90 },
                { CardDB.cardName.guards, 80 },
                { CardDB.cardName.knight, 80 },
                { CardDB.cardName.minipekka, 80 },
                { CardDB.cardName.valkyrie, 80 },
                { CardDB.cardName.lumberjack, 80 },
                { CardDB.cardName.goblingang, 50 },
                { CardDB.cardName.princess, 50 }
            });
            //elitebarbarians
            //executioner
            //firespirits
            OppositeDB.Add(CardDB.cardName.giant, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.infernotower, 100 },
                { CardDB.cardName.skeletonarmy, 100 },
                { CardDB.cardName.tesla, 95 },
                { CardDB.cardName.tombstone, 95 },
                { CardDB.cardName.cannon, 90 },
                { CardDB.cardName.guards, 90 },
                { CardDB.cardName.barbarian, 85 },
                { CardDB.cardName.minionhorde, 85 },
                { CardDB.cardName.goblingang, 80 },
                { CardDB.cardName.witch, 80 },
                { CardDB.cardName.goblin, 50 },
                { CardDB.cardName.skeleton, 50 },
                { CardDB.cardName.speargoblin, 50 }
            });
            OppositeDB.Add(CardDB.cardName.giantskeleton, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.infernotower, 100 },
                { CardDB.cardName.skeletonarmy, 100 },
                { CardDB.cardName.minionhorde, 90 },
                { CardDB.cardName.witch, 90 },
                { CardDB.cardName.barbarian, 90 },
                { CardDB.cardName.minipekka, 90 },
                { CardDB.cardName.rocket, 90 },
                { CardDB.cardName.guards, 80 },
                { CardDB.cardName.goblin, 50 },
                { CardDB.cardName.skeleton, 50 }
            });

            //goblingang
            //goblins

            OppositeDB.Add(CardDB.cardName.goblinbarrel, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.arrows, 100 },
                { CardDB.cardName.fireball, 100 },
                { CardDB.cardName.valkyrie, 100 },
                { CardDB.cardName.bomber, 100 },
                { CardDB.cardName.archer, 95 },
                { CardDB.cardName.zap, 95 },
                { CardDB.cardName.minion, 90 },
                { CardDB.cardName.speargoblin, 90 }
            });
            OppositeDB.Add(CardDB.cardName.golem, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.barbarian, 100 },
                { CardDB.cardName.minionhorde, 100 },
                { CardDB.cardName.infernotower, 100 },
                { CardDB.cardName.skeletonarmy, 100 },
                { CardDB.cardName.prince, 90 },
                { CardDB.cardName.goblingang, 80 },
                { CardDB.cardName.witch, 50 },
                { CardDB.cardName.cannon, 50 },
                { CardDB.cardName.skeleton, 50 },
                { CardDB.cardName.goblin, 50 }
            });

            //guards

            OppositeDB.Add(CardDB.cardName.hogrider, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.barbarian, 100 },
                { CardDB.cardName.skeletonarmy, 100 },
                { CardDB.cardName.minionhorde, 100 },
                { CardDB.cardName.tombstone, 100 },
                { CardDB.cardName.goblingang, 95 },
                { CardDB.cardName.cannon, 90 },
            });

            //icegolem
            //icespirit
            OppositeDB.Add(CardDB.cardName.icewizard, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.lightning, 100 },
                { CardDB.cardName.fireball, 100 },
                { CardDB.cardName.babydragon, 100 },
                { CardDB.cardName.barbarian, 100 },
                { CardDB.cardName.musketeer, 100 },
                { CardDB.cardName.minipekka, 95 },
                { CardDB.cardName.knight, 90 },
                { CardDB.cardName.guards, 90 },
                { CardDB.cardName.wizard, 85 },
                { CardDB.cardName.goblin, 85 },
                { CardDB.cardName.goblingang, 85 },
                { CardDB.cardName.minionhorde, 85 },
                { CardDB.cardName.bombtower, 85 },
                { CardDB.cardName.princess, 80 }
            });
            //infernodragon
            //knight

            OppositeDB.Add(CardDB.cardName.lavahound, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.infernotower, 100 },
                { CardDB.cardName.minionhorde, 100 },
                { CardDB.cardName.babydragon, 100 },
                { CardDB.cardName.minion, 95 },
                { CardDB.cardName.wizard, 95 },
                { CardDB.cardName.threemusketeers, 95 },
                { CardDB.cardName.musketeer, 95 },
                { CardDB.cardName.icewizard, 95 },
                { CardDB.cardName.archer, 90 },
                { CardDB.cardName.speargoblin, 80 },
                { CardDB.cardName.rocket, 80 }
            });

            //lumberjack
            //megaminion

            OppositeDB.Add(CardDB.cardName.miner, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.archer, 100 },
                { CardDB.cardName.speargoblin, 100 },
                { CardDB.cardName.goblin, 100 },
                { CardDB.cardName.skeleton, 100 },
                { CardDB.cardName.knight, 100 },
                { CardDB.cardName.icewizard, 95 },
                { CardDB.cardName.minion, 100 },
                { CardDB.cardName.guards, 100 },
                { CardDB.cardName.barbarian, 100 },
            });
            OppositeDB.Add(CardDB.cardName.minionhorde, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.arrows, 100 },
                { CardDB.cardName.zap, 100 },
                { CardDB.cardName.wizard, 100 },
                { CardDB.cardName.fireball, 90 },
                { CardDB.cardName.minionhorde, 90 }
            });

            //minions

            OppositeDB.Add(CardDB.cardName.minipekka, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.tombstone, 100 },
                { CardDB.cardName.skeletonarmy, 95 },
                { CardDB.cardName.minionhorde, 95 },
                { CardDB.cardName.guards, 95 },
                { CardDB.cardName.freeze, 90 },
                { CardDB.cardName.babydragon, 90 },
                { CardDB.cardName.barbarian, 90 },
                { CardDB.cardName.goblingang, 90 },
                { CardDB.cardName.witch, 90 },
                { CardDB.cardName.cannon, 60 },
                { CardDB.cardName.goblin, 50 },
                { CardDB.cardName.skeleton, 50 },
                { CardDB.cardName.speargoblin, 50 },
                { CardDB.cardName.minion, 30 }
            });

            //musketeer
            //nightwitch

            OppositeDB.Add(CardDB.cardName.pekka, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.infernotower, 100 },
                { CardDB.cardName.skeletonarmy, 100 },
                { CardDB.cardName.minionhorde, 100 },
                { CardDB.cardName.barbarian, 80 },
                { CardDB.cardName.witch, 80 },
                { CardDB.cardName.graveyard, 80 },
                { CardDB.cardName.goblin, 50 },
                { CardDB.cardName.skeleton, 50 },
                { CardDB.cardName.guards, 50 }
            });
            OppositeDB.Add(CardDB.cardName.prince, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.tombstone, 100 },
                { CardDB.cardName.freeze, 95 },
                { CardDB.cardName.guards, 95 },
                { CardDB.cardName.skeletonarmy, 90 },
                { CardDB.cardName.infernotower, 90 },
                { CardDB.cardName.barbarian, 90 },
                { CardDB.cardName.goblingang, 90 },
                { CardDB.cardName.minionhorde, 85 },
                { CardDB.cardName.witch, 80 },
                { CardDB.cardName.goblin, 50 },
                { CardDB.cardName.skeleton, 50 },
                { CardDB.cardName.speargoblin, 50 },
                { CardDB.cardName.prince, 30 }
            });

            //princess

            OppositeDB.Add(CardDB.cardName.royalgiant, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.infernotower, 100 },
                { CardDB.cardName.skeletonarmy, 100 },
                { CardDB.cardName.minionhorde, 100 },
                { CardDB.cardName.goblingang, 95 },
                { CardDB.cardName.tombstone, 95 },
                { CardDB.cardName.witch, 95 },
                { CardDB.cardName.musketeer, 90 },
                { CardDB.cardName.minipekka, 90 },
                { CardDB.cardName.guards, 90 },
                { CardDB.cardName.barbarian, 90 },
                { CardDB.cardName.goblin, 50 },
                { CardDB.cardName.skeleton, 50 },
                { CardDB.cardName.speargoblin, 50 },
            });
            OppositeDB.Add(CardDB.cardName.skeletonarmy, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.arrows, 100 },
                { CardDB.cardName.valkyrie, 100 },
                { CardDB.cardName.bomber, 95 },
                { CardDB.cardName.fireball, 90 },
                { CardDB.cardName.witch, 90 }
            });

            //skeletons

            OppositeDB.Add(CardDB.cardName.sparky, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.infernotower, 100 },
                { CardDB.cardName.prince, 100 },
                { CardDB.cardName.darkprince, 100 },
                { CardDB.cardName.skeletonarmy, 100 },
                { CardDB.cardName.minionhorde, 95 },
                { CardDB.cardName.minion, 95 },
                { CardDB.cardName.guards, 95 },
                { CardDB.cardName.zap, 95 },
                { CardDB.cardName.rocket, 90 },
                { CardDB.cardName.barbarian, 90 },
                { CardDB.cardName.goblingang, 90 },
                { CardDB.cardName.skeleton, 50 },
                { CardDB.cardName.goblin, 50 }
            });

            //speargoblin

            OppositeDB.Add(CardDB.cardName.threemusketeers, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.fireball, 100 },
                { CardDB.cardName.barbarian, 100 },
                { CardDB.cardName.rocket, 100 },
                { CardDB.cardName.skeletonarmy, 100 },
                { CardDB.cardName.lightning, 100 },
                { CardDB.cardName.minionhorde, 95 },
                { CardDB.cardName.pekka, 90 },
                { CardDB.cardName.giantskeleton, 90 },
                { CardDB.cardName.freeze, 60 },
                { CardDB.cardName.zap, 50 },
            });
            OppositeDB.Add(CardDB.cardName.valkyrie, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.minionhorde, 100 },
                { CardDB.cardName.babydragon, 95 },
                { CardDB.cardName.wizard, 95 },
                { CardDB.cardName.infernotower, 90 },
                { CardDB.cardName.tesla, 90 },
                { CardDB.cardName.minion, 50 },
                { CardDB.cardName.musketeer, 50 }
            });
            OppositeDB.Add(CardDB.cardName.witch, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.valkyrie, 100 },
                { CardDB.cardName.lightning, 100 },
                { CardDB.cardName.fireball, 100 },
                { CardDB.cardName.babydragon, 95 },
                { CardDB.cardName.bomber, 95 },
                { CardDB.cardName.wizard, 95 },
                { CardDB.cardName.guards, 90 },
                { CardDB.cardName.barbarian, 90 },
                { CardDB.cardName.bombtower, 90 },
                { CardDB.cardName.knight, 90 },
                { CardDB.cardName.tesla, 90 },
                { CardDB.cardName.witch, 85 }
            });
            OppositeDB.Add(CardDB.cardName.wizard, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.valkyrie, 100 },
                { CardDB.cardName.lightning, 100 },
                { CardDB.cardName.fireball, 100 },
                { CardDB.cardName.babydragon, 95 },
                { CardDB.cardName.barbarian, 100 },
                { CardDB.cardName.bomber, 95 },
                { CardDB.cardName.wizard, 95 },
                { CardDB.cardName.guards, 90 },
                { CardDB.cardName.bombtower, 90 },
                { CardDB.cardName.knight, 90 },
                { CardDB.cardName.tesla, 90 },
                { CardDB.cardName.pekka, 85 },
                { CardDB.cardName.minipekka, 85 }
            });



            //Buildings
            //balloonbomb,
            OppositeDB.Add(CardDB.cardName.barbarianhut, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.minionhorde, 100 },
                { CardDB.cardName.babydragon, 100 },
                { CardDB.cardName.rocket, 100 },
                { CardDB.cardName.lightning, 100 },
                { CardDB.cardName.minions, 95 },
                { CardDB.cardName.balloon, 95 },
                { CardDB.cardName.prince, 95 },
                { CardDB.cardName.giantskeleton, 95 },
                { CardDB.cardName.infernodragon, 90 },
                { CardDB.cardName.megaminion, 90 },
                { CardDB.cardName.poison, 50 },
            });
            OppositeDB.Add(CardDB.cardName.bombtower, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.minionhorde, 100 },
                { CardDB.cardName.princess, 95 },
                { CardDB.cardName.babydragon, 95 },
                { CardDB.cardName.balloon, 95 },
                { CardDB.cardName.minions, 90 },
                { CardDB.cardName.royalgiant, 90 },
                { CardDB.cardName.infernodragon, 90 },
                { CardDB.cardName.megaminion, 90 },
                { CardDB.cardName.musketeer, 85 },
                { CardDB.cardName.blowdartgoblin, 85 },
                { CardDB.cardName.hogrider, 80 },
                { CardDB.cardName.golem, 80 },
            });
            OppositeDB.Add(CardDB.cardName.cannon, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.minionhorde, 100 },
                { CardDB.cardName.princess, 100 },
                { CardDB.cardName.blowdartgoblin, 100 },
                { CardDB.cardName.balloon, 100 },
                { CardDB.cardName.musketeer, 100 },
                { CardDB.cardName.babydragon, 95 },
                { CardDB.cardName.minions, 95 },
                { CardDB.cardName.skeletonarmy, 95 },
                { CardDB.cardName.fireball, 95 },
                { CardDB.cardName.lightning, 90 },
                { CardDB.cardName.barbarians, 90 },
                { CardDB.cardName.infernodragon, 90 },
                { CardDB.cardName.megaminion, 90 },
                { CardDB.cardName.poison, 85 },
                { CardDB.cardName.hogrider, 85 },
                { CardDB.cardName.golem, 80 },
                { CardDB.cardName.giant, 80 },
                { CardDB.cardName.royalgiant, 80 },
            });
            OppositeDB.Add(CardDB.cardName.elixircollector, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.hogrider, 100 },
                { CardDB.cardName.giant, 100 },
                { CardDB.cardName.balloon, 100 },
                { CardDB.cardName.royalgiant, 100 },
                { CardDB.cardName.golem, 100 },
                { CardDB.cardName.musketeer, 100 },
                { CardDB.cardName.barbarian, 100 },
                { CardDB.cardName.babydragon, 100 },
                { CardDB.cardName.minions, 100 },
                { CardDB.cardName.minionhorde, 100 },
                { CardDB.cardName.skeletonarmy, 90 },
                { CardDB.cardName.goblinbarrel, 90 },
                { CardDB.cardName.rocket, 85 },
                { CardDB.cardName.lightning, 85 },
                { CardDB.cardName.fireball, 85 },
                { CardDB.cardName.poison, 70 },
                { CardDB.cardName.arrows, 50 },
            });
            OppositeDB.Add(CardDB.cardName.firespirithut, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.rocket, 100 },
                { CardDB.cardName.lightning, 100 },
                { CardDB.cardName.bomber, 100 },
                { CardDB.cardName.royalgiant, 90 },
                { CardDB.cardName.giant, 90 },
                { CardDB.cardName.battleram, 90 },
                { CardDB.cardName.golem, 85 },
                { CardDB.cardName.giantskeleton, 80 },
                { CardDB.cardName.pekka, 80 },
                { CardDB.cardName.fireball, 80 },
                { CardDB.cardName.poison, 80 },
                { CardDB.cardName.princess, 50 },
                { CardDB.cardName.barbarians, 50 },
            });
            //giantskeletonbomb,
            OppositeDB.Add(CardDB.cardName.goblinhut, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.hogrider, 100 },
                { CardDB.cardName.giant, 100 },
                { CardDB.cardName.royalgiant, 100 },
                { CardDB.cardName.golem, 100 },
                { CardDB.cardName.musketeer, 100 },
                { CardDB.cardName.barbarian, 100 },
                { CardDB.cardName.skeletonarmy, 100 },
                { CardDB.cardName.rocket, 90 },
                { CardDB.cardName.lightning, 90 },
                { CardDB.cardName.fireball, 85 },
                { CardDB.cardName.poison, 50 },
            });
            OppositeDB.Add(CardDB.cardName.infernotower, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.rocket, 100 },
                { CardDB.cardName.lightning, 100 },
                { CardDB.cardName.skeletonarmy, 100 },
                { CardDB.cardName.minionhorde, 95 },
                { CardDB.cardName.goblingang, 95 },
                { CardDB.cardName.blowdartgoblin, 95 },
                { CardDB.cardName.princess, 95 },
                { CardDB.cardName.barbarians, 90 },
                { CardDB.cardName.speargoblins, 85 },
                { CardDB.cardName.fireball, 50 },
                { CardDB.cardName.poison, 50 },
            });
            //kingtowermiddle,
            OppositeDB.Add(CardDB.cardName.kingtower, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.balloon, 100 },
                { CardDB.cardName.hogrider, 100 },
                { CardDB.cardName.golem, 100 },
                { CardDB.cardName.goblinbarrel, 100 },
                { CardDB.cardName.pekka, 100 },
                { CardDB.cardName.minipekka, 100 },
                { CardDB.cardName.rocket, 100 },
                { CardDB.cardName.royalgiant, 95 },
                { CardDB.cardName.infernodragon, 95 },
                { CardDB.cardName.fireball, 95 },
                { CardDB.cardName.giant, 90 },
                { CardDB.cardName.battleram, 90 },
                { CardDB.cardName.poison, 90 },
                { CardDB.cardName.lightning, 90 },
                { CardDB.cardName.giantskeleton, 85 },
                { CardDB.cardName.skeletonarmy, 50 },
            });
            OppositeDB.Add(CardDB.cardName.mortar, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.minionhorde, 100 },
                { CardDB.cardName.balloon, 100 },
                { CardDB.cardName.lightning, 100 },
                { CardDB.cardName.goblinbarrel, 100 },
                { CardDB.cardName.hogrider, 95 },
                { CardDB.cardName.babydragon, 95 },
                { CardDB.cardName.barbarians, 95 },
                { CardDB.cardName.minipekka, 95 },
                { CardDB.cardName.prince, 95 },
                { CardDB.cardName.minions, 90 },
                { CardDB.cardName.royalgiant, 90 },
                { CardDB.cardName.goblingang, 90 },
                { CardDB.cardName.skeletonarmy, 85 },
                { CardDB.cardName.goblins, 85 },
                { CardDB.cardName.megaminion, 85 },
                { CardDB.cardName.golem, 85 },
                { CardDB.cardName.poison, 85 },
            });
            OppositeDB.Add(CardDB.cardName.princesstower, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.balloon, 100 },
                { CardDB.cardName.hogrider, 100 },
                { CardDB.cardName.golem, 100 },
                { CardDB.cardName.goblinbarrel, 100 },
                { CardDB.cardName.pekka, 100 },
                { CardDB.cardName.minipekka, 100 },
                { CardDB.cardName.rocket, 100 },
                { CardDB.cardName.royalgiant, 95 },
                { CardDB.cardName.infernodragon, 95 },
                { CardDB.cardName.fireball, 95 },
                { CardDB.cardName.giant, 90 },
                { CardDB.cardName.battleram, 90 },
                { CardDB.cardName.poison, 90 },
                { CardDB.cardName.lightning, 90 },
                { CardDB.cardName.giantskeleton, 85 },
                { CardDB.cardName.skeletonarmy, 50 },
            });
            //ragebarbarianbottle,
            //skeletoncontainer,
            OppositeDB.Add(CardDB.cardName.tesla, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.giant, 100 },
                { CardDB.cardName.golem, 100 },
                { CardDB.cardName.princess, 100 },
                { CardDB.cardName.blowdartgoblin, 100 },
                { CardDB.cardName.balloon, 100 },
                { CardDB.cardName.hogrider, 100 },
                { CardDB.cardName.musketeer, 100 },
                { CardDB.cardName.skeletonarmy, 95 },
                { CardDB.cardName.rocket, 95 },
                { CardDB.cardName.fireball, 95 },
                { CardDB.cardName.lightning, 90 },
                { CardDB.cardName.barbarians, 90 },
                { CardDB.cardName.royalgiant, 90 },
                { CardDB.cardName.infernodragon, 90 },
                { CardDB.cardName.megaminion, 90 },
                { CardDB.cardName.poison, 85 },
            });
            OppositeDB.Add(CardDB.cardName.tombstone, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.hogrider, 100 },
                { CardDB.cardName.giant, 100 },
                { CardDB.cardName.balloon, 100 },
                { CardDB.cardName.royalgiant, 100 },
                { CardDB.cardName.golem, 100 },
                { CardDB.cardName.musketeer, 100 },
                { CardDB.cardName.barbarian, 100 },
                { CardDB.cardName.babydragon, 100 },
                { CardDB.cardName.minions, 100 },
                { CardDB.cardName.minionhorde, 100 },
                { CardDB.cardName.skeletonarmy, 90 },
                { CardDB.cardName.rocket, 85 },
                { CardDB.cardName.lightning, 85 },
                { CardDB.cardName.fireball, 85 },
                { CardDB.cardName.poison, 70 },
                { CardDB.cardName.arrows, 50 },
            });
            OppositeDB.Add(CardDB.cardName.xbow, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.minionhorde, 100 },
                { CardDB.cardName.balloon, 100 },
                { CardDB.cardName.lightning, 100 },
                { CardDB.cardName.hogrider, 95 },
                { CardDB.cardName.babydragon, 95 },
                { CardDB.cardName.minions, 95 },
                { CardDB.cardName.barbarians, 95 },
                { CardDB.cardName.barbarianhut, 95 },
                { CardDB.cardName.infernodragon, 90 },
                { CardDB.cardName.megaminion, 85 },
                { CardDB.cardName.poison, 85 },
                { CardDB.cardName.prince, 85 },
                { CardDB.cardName.royalgiant, 80 },
                { CardDB.cardName.golem, 80 },
                { CardDB.cardName.fireball, 50 },
            });


        }

    }
}
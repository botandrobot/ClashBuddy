namespace Buddy.Clash.DefaultSelectors
{
    using System;
    using System.Collections.Generic;

	public class KnowledgeBase
	{

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

        public Handcard getOpposite(Playfield p, BoardObj attacker, bool canWait = true)
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
                                bestCard = hc;
                                bestCardVal = tmp[name];
                            }
                            if (bestCardVal == 100) break;
                        }
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
                                    if (hc.card.MaxHP > ad.attacker.Atk * 4 || hc.card.SpawnNumber > 3)
                                    {
                                        tmp = hc;
                                        break;
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
                                if (ad.attacker.card.DamageRadius > 1000 && hc.card.MaxHP > ad.attacker.Atk)
                                {
                                    if (tmp == null || tmp.card.MaxHP < hc.card.MaxHP) tmp = hc;
                                }
                                else
                                {
                                    if (hc.card.MaxHP > ad.attacker.Atk * 4 || hc.card.SpawnNumber > 3)
                                    {
                                        tmp = hc;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (ad.attacker.TargetType == targetType.BUILDINGS)
                        {
                            foreach (Handcard hc in p.ownHandCards)
                            {
                                if (hc.card.Atk > ad.attacker.HP * 5 || hc.card.SpawnNumber > 3)
                                {
                                    tmp = hc;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            foreach (Handcard hc in p.ownHandCards)
                            {
                                if (hc.card.Transport == transportType.AIR)
                                {
                                    if (tmp == null || tmp.card.SpawnNumber < hc.card.SpawnNumber) tmp = hc;
                                }
                            }
                            if (tmp == null)
                            {
                                foreach (Handcard hc in p.ownHandCards)
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
                                        if (hc.card.MaxHP > ad.attacker.Atk * 4 || hc.card.SpawnNumber > 3)
                                        {
                                            tmp = hc;
                                            break;
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
                    if (tmp != null) allOpposite.Add(tmp.card.name, new opposite(tmp.card.name, 10, tmp, ad.attacker));
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
                        Helpfunctions.Instance.logg("!OppositeDB.ContainsKey");
                    }
                    if (bestOpposite != null)
                    {
                        if (bestOpposite.value < opp.value) bestOpposite = opp;
                    }
                    else bestOpposite = opp;
                }

                if (bestOpposite != null && bestOpposite.target != null)
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

//bandit

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
//electrowizard
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
//icewizard
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
            OppositeDB.Add(CardDB.cardName.mortar, new Dictionary<CardDB.cardName, int>(){
                { CardDB.cardName.minipekka, 100 },
                { CardDB.cardName.goblinbarrel, 100 },
                { CardDB.cardName.prince, 100 },
                { CardDB.cardName.skeletonarmy, 100 },
                { CardDB.cardName.minionhorde, 100 },
                { CardDB.cardName.balloon, 100 },
                { CardDB.cardName.goblingang, 95 },
                { CardDB.cardName.babydragon, 95 },
                { CardDB.cardName.goblin, 90 }
            });
            OppositeDB.Add(CardDB.cardName.xbow, new Dictionary<CardDB.cardName, int>(){
                    { CardDB.cardName.prince, 100 },
                    { CardDB.cardName.minionhorde, 100 },
                    { CardDB.cardName.skeletonarmy, 100 },
                    { CardDB.cardName.balloon, 100 },
                    { CardDB.cardName.goblinbarrel, 95 },
                    { CardDB.cardName.hogrider, 95 },
                    { CardDB.cardName.babydragon, 95 }
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
            
        }

	}
}
using Robi.Common;
using Serilog;

namespace Robi.Clash.DefaultSelectors
{
    using Robi.Clash.DefaultSelectors.Settings;
    using Robi.Engine.Settings;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class BoardTester
    {
        private static readonly ILogger Logger = LogProvider.CreateLogger<BoardTester>();
        public Playfield btPlayfield;

        public BoardTester()
        {
            string dataFolder = Path.Combine("DefaultRoutine", "Data");
            if (!Directory.Exists(dataFolder))
            {
                Directory.CreateDirectory(dataFolder);
                return; // you should to create test.txt with test Playdield in this folder
            }
            string testFilePath = Path.Combine(dataFolder, "test.txt");
            btPlayfield = getPlayfield(testFilePath);
            btPlayfield.print();
        }

        public Playfield getPlayfield(string path)
        {
            string[] lines = new string[0] { };
            try
            {
                lines = System.IO.File.ReadAllLines(path);
                Logger.Debug("read test.txt {Length} lines", lines.Length);
            }
            catch
            {
                Logger.Error("Read failed.");
                return null;
            }

            Playfield p = new Playfield();
            foreach (string s in lines)
            {
                string[] tmp = s.Split(' ');
                int len = tmp.Length;
                if (len < 1) continue;
                BoardObj bo;
                switch (tmp[0])
                {
                    case "Data":
                        getBattleData(tmp, p);
                        continue;
                    case "Hand":
                        p.ownHandCards.Add(getHCfromHeader(tmp));
                        continue;
                    case "AOE":
                        bo = getBOfromHeader(tmp, p.ownerIndex); //predefined data
                        if (bo.own) p.ownAreaEffects.Add(bo);
                        else p.enemyAreaEffects.Add(bo);
                        continue;
                    case "BUILDING":
                        bo = getBOfromHeader(tmp, p.ownerIndex); //predefined data
                        int tower = 0;
                        switch (bo.Name)
                        {
                            case CardDB.cardName.princesstower:
                                tower = bo.Line;
                                if (bo.own)
                                {
                                    if (tower == 1) p.ownPrincessTower1 = bo;
                                    else p.ownPrincessTower2 = bo;
                                }
                                else
                                {
                                    if (tower == 1) p.enemyPrincessTower1 = bo;
                                    else p.enemyPrincessTower2 = bo;
                                }
                                break;
                            case CardDB.cardName.kingtower:
                                tower = 10 + bo.Line;
                                if (bo.own)
                                {
                                    if (p.ownerIndex == bo.ownerIndex) p.ownKingsTower = bo;
                                }
                                else p.enemyKingsTower = bo;
                                break;
                            case CardDB.cardName.kingtowermiddle: tower = 100; break;
                        }
                        if (tower == 0)
                        {
                            if (bo.own) p.ownBuildings.Add(bo);
                            else p.enemyBuildings.Add(bo);
                        }
                        continue;
                    case "MOB":
                        bo = getBOfromHeader(tmp, p.ownerIndex); //predefined data
                        if (bo.own) p.ownMinions.Add(bo);
                        else p.enemyMinions.Add(bo);
                        continue;
                }
            }
            p.home = p.ownKingsTower.Position.Y < 15250 ? true : false;

            p.initTowers();
            int i = 0;
            foreach (BoardObj t in p.ownTowers) if (t.Tower < 10) i += t.Line;
            int kingsLine = 0;
            switch (i)
            {
                case 0: kingsLine = 3; break;
                case 1: kingsLine = 2; break;
                case 2: kingsLine = 1; break;
            }
            foreach (BoardObj t in p.ownTowers) if (t.Tower > 9) t.Line = kingsLine;
            Logger.Debug("getPlayfield:OK");

            return p;

            //Set default settings for behaviour
            //Apply settings from this Logg
            //set Simulation stuff
            //save data

        }

        private void getBattleData(string[] line, Playfield p)
        {
            foreach (string s in line)
            {
                string[] tmp = s.Split(':');
                switch (tmp[0])
                {
                    case "bt":
                        string time = s.Substring(3);
                        p.BattleTime = TimeSpan.Parse(time);
                        continue;
                    case "owner":
                        p.ownerIndex = Convert.ToInt32(tmp[1]);
                        continue;
                    case "mana":
                        p.ownMana = Convert.ToInt32(tmp[1]);
                        continue;
                    case "nxtc":
                        p.nextCard = new Handcard(tmp[1], Convert.ToInt32(tmp[2]));
                        continue;
                }
            }
        }

        private Handcard getHCfromHeader(string[] line)
        {
            Handcard hc = new Handcard(line[2], Convert.ToInt32(line[3]));
            hc.position = Convert.ToInt32(line[1]);
            hc.manacost = Convert.ToInt32(line[4]);
            return hc;
        }

        private BoardObj getBOfromHeader(string[] line, int ownerIndex)
        {
            BoardObj bo = new BoardObj(CardDB.Instance.cardNamestringToEnum(line[2]));
            bo.ownerIndex = Convert.ToInt32(line[1]);
            bo.own = bo.ownerIndex == ownerIndex ? true : false;
            bo.GId = Convert.ToUInt32(line[3]);
            bo.Position = new VectorAI(line[4]);
            bo.Line = bo.Position.X > 8700 ? 2 : 1;
            bo.level = Convert.ToInt32(line[5]);
            bo.Atk = Convert.ToInt32(line[6]);
            bo.HP = Convert.ToInt32(line[7]);
            bo.Shield = Convert.ToInt32(line[8]);
            int len = line.Length;
            if (len > 9)
            {
                for (int i = 9; i < len; i++)
                {
                    string[] ss = line[i].Split(':');
                    switch (ss[0])
                    {
                        case "frozen":
                            bo.frozen = true;
                            bo.startFrozen = Convert.ToInt32(ss[1]);
                            continue;
                        case "LifeTime":
                            bo.LifeTime = Convert.ToInt32(ss[1]);
                            continue;
                        case "extraData":
                            bo.extraData = ss[1];
                            continue;
                    }
                }
            }
            //this.attacking = c.attacking;
            //this.attacked = c.attacked;
            //this.attacker = c.attacker;
            //this.target = c.target;
            return bo;
        }

        /*
        
        public void printSettings()
        {
            Helpfunctions.Instance.logg("#################### Settings #########################################");
            Helpfunctions.Instance.logg("path = " + Settings.Instance.path);
            Helpfunctions.Instance.logg("logpath = " + Settings.Instance.logpath);
            Helpfunctions.Instance.logg("logfile = " + Settings.Instance.logfile);
            Helpfunctions.Instance.logg("#################### Settings End #####################################");
        }
        */
    }

}
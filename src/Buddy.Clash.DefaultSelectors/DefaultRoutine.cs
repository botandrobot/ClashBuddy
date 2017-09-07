namespace Buddy.Clash.DefaultSelectors
{
    using System;
    using System.Text;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Engine;
    using Common;
    using Serilog;
    using System.IO;
    using Buddy.Engine.Settings;
    using Buddy.Clash.DefaultSelectors.Settings;

    public class DefaultRoutine : ActionSelectorBase
    {
        private static readonly ILogger Logger = LogProvider.CreateLogger<DefaultRoutine>();
        private readonly ConcurrentQueue<string> _spellQueue = new ConcurrentQueue<string>();

        internal static DefaultRoutineSettings Settings { get; } = new DefaultRoutineSettings();

        Helpfunctions help = Helpfunctions.Instance;

        DateTime starttime = DateTime.Now;
        
        //Behavior behave = new BehaviorControl();

        #region Implementation of IAuthored

        /// <summary> The name of the routine. </summary>
        public override string Name
        {
            get { return "DefaultRoutine"; }
        }

        /// <summary> The description of the routine. </summary>
        public override string Description
        {
            get { return "The default routine for Clash Royale."; }
        }

        /// <summary>The author of this routine.</summary>
        public override string Author
        {
            get { return "Vlad"; }
        }

        /// <summary>The version of this routine.</summary>
        public override Version Version
        {
            get { return new Version(0, 0, 0, 7); }
        }

        /// <summary>Unique Identifier.</summary>
        public override Guid Identifier
        {
            get { return new Guid("{591611D1-B5F2-4483-AF4F-B154153C40F7}"); }
        }

        #endregion

        #region Implementation of IRunnable
        /*
        /// <summary> The routine start callback. Do any initialization here. </summary>
        public void Start()
        {
            GameEventManager.NewGame += GameEventManagerOnNewGame;
            GameEventManager.GameOver += GameEventManagerOnGameOver;
            GameEventManager.QuestUpdate += GameEventManagerOnQuestUpdate;
            GameEventManager.ArenaRewards += GameEventManagerOnArenaRewards;

            if (Hrtprozis.Instance.settings == null)
            {
                Hrtprozis.Instance.setInstances();
                ComboBreaker.Instance.setInstances();
                PenalityManager.Instance.setInstances();
            }
            behave = sf.getBehaviorByName(DefaultRoutineSettings.Instance.DefaultBehavior);
            foreach (var tuple in _mulliganRules)
            {
                Exception ex;
                if (
                    !VerifyCondition(tuple.Item1, new List<string> { "mulliganData" }, out ex))
                {
                    Log.ErrorFormat("[Start] There is an error with a mulligan execution condition [{1}]: {0}.", ex,
                        tuple.Item1);
                    BotManager.Stop();
                }

                if (
                    !VerifyCondition(tuple.Item2, new List<string> { "mulliganData", "card" },
                        out ex))
                {
                    Log.ErrorFormat("[Start] There is an error with a mulligan card condition [{1}]: {0}.", ex,
                        tuple.Item2);
                    BotManager.Stop();
                }
            }
        }

        /// <summary> The routine tick callback. Do any update logic here. </summary>
        public void Tick()
        {
        }

        /// <summary> The routine stop callback. Do any pre-dispose cleanup here. </summary>
        public void Stop()
        {
            GameEventManager.NewGame -= GameEventManagerOnNewGame;
            GameEventManager.GameOver -= GameEventManagerOnGameOver;
            GameEventManager.QuestUpdate -= GameEventManagerOnQuestUpdate;
            GameEventManager.ArenaRewards -= GameEventManagerOnArenaRewards;
        }
        */
        #endregion



        public override void Initialize()
        { 
            SettingsManager.RegisterSettings(Name, Settings);

            foreach(var e in Buddy.Clash.Engine.Csv.CsvLogic.Characters.Entries)
            {
                Logger.Information("{TID}: {Name} has {ShieldHitpoints}", e.TID, e.Name, e.ShieldHitpoints);
            }
            help.logg("-----------------Initialize");
            CardDB cdb = CardDB.Instance;
        }


        public override void BattleStart()
        {
            help.setnewLoggFile();
            help.logg("-----------------BattleStart");
        }

        public override void BattleEnd()
        {
            help.logg("-----------------BattleEnd");
        }

        public override void Deinitialize()
        {
            SettingsManager.UnregisterSettings(Name);
            help.logg("-----------------Deinitialize");
            CardDB.Instance.uploadCardInfo();
        }


        //it just concept for ActionSelector  (TODO: in future connect it with NN or other DB)
        public override CastRequest GetNextCast()
        {
            //var ssdf  = Buddy.Clash.Engine.Csv.CsvLogic.SpellsCharacters

            help.logg("###################entrance############### " + DateTime.Now);

            List<BoardObj> ownMinions = new List<BoardObj>();
            List<BoardObj> enemyMinions = new List<BoardObj>();

            List<BoardObj> ownAreaEffects = new List<BoardObj>();
            List<BoardObj> enemyAreaEffects = new List<BoardObj>();

            List<BoardObj> ownBuildings = new List<BoardObj>();
            List<BoardObj> enemyBuildings = new List<BoardObj>();

            List<BoardObj> ownTowers = new List<BoardObj>();
            List<BoardObj> enemyTowers = new List<BoardObj>();
            BoardObj ownKingsTower = new BoardObj();
            BoardObj enemyKingsTower = new BoardObj();

            List<Handcard> ownHandCards = new List<Handcard>();


            var om = ClashEngine.Instance.ObjectManager;

            var battle = ClashEngine.Instance.Battle;
            if (battle == null || !battle.IsValid) return null;
            
            StringBuilder sb = new StringBuilder();
                        
            var lp = ClashEngine.Instance.LocalPlayer;
            var spells = ClashEngine.Instance.AvailableSpells;
            foreach (var spell in spells)
            {
                if (spell != null && spell.IsValid)
                {
                    int lvl = 1;
                    Handcard hc = new Handcard(spell.Name.Value, lvl); //hc.lvl = ??? TODO
                    hc.manacost = spell.ManaCost;
                    //hc.position = ??? TODO
                    //TODO:for all objects - if (new name) get actual params
                    ownHandCards.Add(hc);
                }
            }
            
            var aoes = om.OfType<Engine.NativeObjects.Logic.GameObjects.AreaEffectObject>();
            foreach (var aoe in aoes)
            {
                if (aoe != null && aoe.IsValid)
                {
                    //TODO: get static data for all objects
                    //Here we get dynamic data only
                    BoardObj bo = new BoardObj(CardDB.Instance.cardNamestringToEnum(aoe.LogicGameObjectData.Name.Value));
                    bo.GId = aoe.GlobalId;
                    bo.Position = new VectorAI(aoe.StartPosition);
                    bo.Line = bo.Position.X > 8700 ? 1 : 2;
                    //bo.level = TODO real value
                    //bo.Atk = TODO real value
                    bo.LifeTime = aoe.HealthComponent.RemainingTime; //TODO check this value
                    
                    bo.ownerIndex = (int)aoe.OwnerIndex;
                    bool own = bo.ownerIndex == lp.OwnerIndex ? true : false; //TODO: replace it on Friendly (for 2x2 mode)
                    bo.own = own;
                    if (own) ownAreaEffects.Add(bo);
                    else enemyAreaEffects.Add(bo);
                    //hc.position = ??? TODO
                    

                }
                
            }


            var chars = om.OfType<Engine.NativeObjects.Logic.GameObjects.Character>();
            foreach (var @char in chars)
            {
                //sb.Clear();
                //i++;
                //BoardObj bo = new BoardObj();

                var data = @char.LogicGameObjectData;

                if (data != null && data.IsValid)
                {
                    //TODO: get static data for all objects
                    //Here we get dynamic data only

                    BoardObj bo = new BoardObj(CardDB.Instance.cardNamestringToEnum(data.Name.Value));
                    bo.GId = @char.GlobalId;
                    bo.Position = new VectorAI(@char.StartPosition);
                    bo.Line = bo.Position.X > 8700 ? 1 : 2;
                    //bo.level = TODO real value
                    //bo.Atk = TODO real value
                    //this.frozen = TODO
                    //this.startFrozen = TODO
                    bo.HP = @char.HealthComponent.CurrentHealth; //TODO: check it
                    bo.Shield = @char.HealthComponent.CurrentShieldHealth; //TODO: check it
                    bo.LifeTime = @char.HealthComponent.LifeTime - @char.HealthComponent.RemainingTime; //TODO: check it of data.LifeTime, - find real value for battle stage
                    switch (bo.Name)
                    {
                        case CardDB.cardName.princesstower: bo.Tower = bo.Line; break;
                        case CardDB.cardName.kingtower: bo.Tower = 10 + bo.Line; break;
                        case CardDB.cardName.kingtowermiddle: bo.Tower = 100; break;
                    }
                    
                    bo.ownerIndex = (int)@char.OwnerIndex;
                    bool own = bo.ownerIndex == lp.OwnerIndex ? true : false; //TODO: replace it on Friendly (for 2x2 mode)
                    bo.own = own;
                    if (own)
                    {
                        switch (bo.type)
                        {
                            case boardObjType.MOB: ownMinions.Add(bo); break;
                            case boardObjType.BUILDING:
                                if (bo.Tower > 0)
                                {
                                    ownTowers.Add(bo);
                                    if (bo.Tower > 10) ownKingsTower = bo;
                                }
                                else ownBuildings.Add(bo);
                                break;
                        }
                    }
                    else
                    {
                        switch (bo.type)
                        {
                            case boardObjType.MOB: enemyMinions.Add(bo); continue;
                            case boardObjType.BUILDING:
                                if (bo.Tower > 0)
                                {
                                    enemyTowers.Add(bo);
                                    if (bo.Tower > 10) enemyKingsTower = bo;
                                }
                                else enemyBuildings.Add(bo);
                                break;
                        }
                    }
                    
                }

            }

            Playfield p = new Playfield();
            p.BattleTime = ClashEngine.Instance.Battle.BattleTime;
            p.ownerIndex = (int)lp.OwnerIndex;
            p.ownMana = (int)lp.Mana;
            //p.nextCard = TODO:

            p.ownHandCards = ownHandCards;
            p.ownAreaEffects = ownAreaEffects;
            p.ownMinions = ownMinions;
            p.ownBuildings = ownBuildings;
            p.ownTowers = ownTowers;
            p.ownKingsTower = ownKingsTower;
            p.enemyKingsTower = enemyKingsTower;
            
            p.enemyAreaEffects = enemyAreaEffects;
            p.enemyMinions = enemyMinions;
            p.enemyBuildings = enemyBuildings;
            p.enemyTowers = enemyTowers;

            p.home = p.ownTowers[0].Position.Y < 15250 ? true : false; int i = 0;

            foreach (BoardObj t in p.ownTowers) if (t.Tower < 10) i += t.Line;
            int kingsLine = 0;
            switch (i)
            {
                case 0: kingsLine = 3; break;
                case 1: kingsLine = 2; break;
                case 2: kingsLine = 1; break;
            }
            foreach (BoardObj t in p.ownTowers) if (t.Tower > 9) t.Line = kingsLine;

            p.print();
            help.logg("###Start_calc: " + DateTime.Now + "\r\n");
            
            Behavior behave = new BehaviorApollo();//change this to new Behavior
            //Behavior behave = new BehaviorControl();//change this to new Behavior
            Cast bc = behave.getBestCast(p);
            CastRequest retval = null;
            if (bc != null)
            {
                help.logg("Cast " + bc.ToString());
                retval = new CastRequest(bc.SpellName, bc.Position.ToVector2());
            }
            else help.logg("Waiting for...");

            help.logg("###End_calc: " + DateTime.Now + "\r\n");

            return retval;
            
        }

    }
}


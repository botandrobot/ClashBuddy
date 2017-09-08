// TODO: Make sure to figure out a way to use the general logger for that.
namespace Robi.Clash.DefaultSelectors
{
    using Robi.Common;
    using Serilog;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    /// <summary>
    ///     The helpfunctions.
    /// </summary>

    public class Helpfunctions
    {
        //ILogger Logger = LogProvider.CreateLogger<Helpfunctions>();

        //private static readonly ILog Log = Logger.GetLoggerInstanceForType();

        public string logFilePath = "defaultRoutine.log";
        private static Helpfunctions instance;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        public static Helpfunctions Instance
        {
            get { return instance ?? (instance = new Helpfunctions()); }
        }

        private Helpfunctions()
        {
            //System.IO.File.WriteAllText(Settings.Instance.logpath + Settings.Instance.logfile, "");
        }

        private bool writelogg = true;
                
        public void ErrorLog(string s)
        {
            logg(s);
            //Console.WriteLine(s);
        }

        public void setnewLoggFile()
        {
            string RoutineFolder = "DefaultRoutine";
            string AIlogFolderPath = Path.Combine(RoutineFolder, "AIlogs");
            logFilePath = Path.Combine(AIlogFolderPath, DateTime.Now.ToString("_yyyy-MM-dd_HH-mm-ss") + ".txt");
            if (!Directory.Exists(AIlogFolderPath)) Directory.CreateDirectory(AIlogFolderPath);
        }

        public void logg(string s)
        {
            if (!writelogg) return;
            try
            {
                using (StreamWriter sw = File.AppendText(logFilePath))
                {
                    sw.WriteLine(s);
                }
            }
            catch
            {
            }
            //Console.WriteLine(s);
        }

        public void logg(Handcard hc)
        {
            if (!writelogg) return;
            try
            {
                using (StreamWriter sw = File.AppendText(logFilePath))
                {
                    sw.WriteLine("Hand " + hc.position + " " + hc.card.name + " " + hc.lvl + " " + hc.manacost);
                }
            }
            catch
            {
                //TODO: other way to inform about this problem (m.b. line in main bot-log)
            }
        }

        public void logg(BoardObj bo, string extraData = "")
        {
            if (!writelogg) return;
            try
            {
                using (StreamWriter sw = File.AppendText(logFilePath))
                {
                    sw.WriteLine(bo.type + " " + bo.ownerIndex + " " + bo.Name + " " + bo.GId + " " + bo.Position.ToString() + " " + bo.level + " " + bo.Atk + " " + bo.HP + " " + bo.Shield
                        + (bo.frozen ? " frozen:" + bo.startFrozen : "")
                        + (bo.LifeTime > 0 ? " LifeTime:" + bo.LifeTime : "")
                        + (extraData != "" ? extraData : "")
                        );
                }
            }
            catch
            {
                //TODO: other way to inform about this problem (m.b. line in main bot-log)
            }
        }
        /*
        public void printAllFields(Robi.Clash.Engine.NativeObjects.Logic.GameObjects.Character @char, int num)
        {
            sb.Clear();
            sb.Append(num).Append(" ");
            FieldInfo[] fi = @char.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            sb.Append(fi.Length).Append(":fi ");
            foreach (FieldInfo f in fi) sb.AppendFormat("{0}: {1}\n", f.Name, f.GetValue(@char));
            
            var data = @char.LogicGameObjectData;
            if (data != null && data.IsValid)
            {
                fi = data.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                sb.Append(fi.Length).Append(":dataFI ");
                foreach (FieldInfo f in fi) sb.AppendFormat("{0}: {1}\n", f.Name, f.GetValue(data));
            }
            logg(sb.ToString());
        }*/

        public DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        /*
        public static List<T> printAllFielsd<T>(IEnumerable<T> source, int limit)
        {
            List<T> retlist = new List<T>();
            int i = 0;

            foreach (T item in source)
            {
                retlist.Add(item);
                i++;

                if (i >= limit) break;
            }
            return retlist;
        }
        */
        /*
        public void ErrorLog(string s)
        {
            //HREngine.API.Utilities.HRLog.Write(s);
            //Console.WriteLine(s);
            Log.Info(s);
        }*/

    }
}

using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Rainmeter;

namespace HakProcMem
{
    internal class Measure
    {
        internal Measure()
        {
        }

        private int Rank = 1;
        private string ProcessName = "Unknown";
        private long ProcessMemory = 0;

        internal void Reload(Rainmeter.API api, ref double maxValue)
        {
            int rank = api.ReadInt("Rank", 1);

            if (rank > 0)
            {
                Rank = rank;
            } else
            {
                Rank = 1;
            }
        }

        internal double Update()
        {
            var totalProcessMemory = new Dictionary<string, long>();

            Process[] allProcesses = Process.GetProcesses();

            foreach (Process process in allProcesses)
            {
                if (totalProcessMemory.ContainsKey(process.ProcessName))
                {
                    totalProcessMemory[process.ProcessName] += process.WorkingSet64;
                } else
                {
                    totalProcessMemory[process.ProcessName] = process.WorkingSet64;
                }
            }

            var ordered = totalProcessMemory.OrderByDescending(x => x.Value);

            var ranked = ordered.ElementAt(Rank - 1);
            ProcessName = ranked.Key;
            ProcessMemory = ranked.Value;
            return ProcessMemory;
        }

        internal string GetString()
        {
            string shortProcessName = ProcessName.Substring(0, Math.Min(8, ProcessName.Length));
            return string.Format("{0} {1}", shortProcessName, GetHumanReadable(ProcessMemory));
        }

        internal string GetHumanReadable(long memory)
        {
            string[] suffixes = new string[] { "B", "K", "M", "G", "P" };
            var value = memory / 1;
            for (int order = 0;  order <= suffixes.Length; order++)
            {
                var orderCheck = value / 1000;
                if (orderCheck < 1)
                {
                    return string.Format("{0}{1}", value, suffixes[order]);
                }

                value = orderCheck;
            }

            return "wat";
        }
        
    }
}

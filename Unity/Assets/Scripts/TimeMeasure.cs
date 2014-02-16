using UnityEngine;
using System.Collections;
using System;
using System.Diagnostics;
using System.Collections.Generic;

public class TimeMeasure : IDisposable
{
    private string name;
    private Stopwatch stopwatch;

    public static readonly Dictionary<string, Data> Stats = new Dictionary<string, Data>();

    public TimeMeasure(string name)
    {
        this.name = name;
        this.stopwatch = new Stopwatch();
        this.stopwatch.Start();
    }

    public void Dispose()
    {
        this.stopwatch.Stop();
        var elapsed = this.stopwatch.ElapsedMilliseconds;

        lock (TimeMeasure.Stats)
        {
            var stats = TimeMeasure.Stats[this.name];
            stats.Elapsed += elapsed;
            stats.Count++;
            stats.Min = Math.Min(stats.Min, elapsed);
            stats.Max = Math.Max(stats.Max, elapsed);
            TimeMeasure.Stats[this.name] = stats;
        }
    }

    public struct Data
    {
        public int Count;
        public long Elapsed;
        public long Min;
        public long Max;
    }
}

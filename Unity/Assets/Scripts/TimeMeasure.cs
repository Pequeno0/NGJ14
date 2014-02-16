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
            Data data;
            if (!TimeMeasure.Stats.TryGetValue(this.name, out data))
            {
                data = new Data();
                TimeMeasure.Stats.Add(this.name, data);
            }
            
            data.Elapsed += elapsed;
            data.Count++;
            data.Min = data.Min.HasValue ? Math.Min(data.Min.Value, elapsed) : elapsed;
            data.Max = data.Max.HasValue ? Math.Max(data.Max.Value, elapsed) : elapsed;
        }
    }

    public class Data
    {
        public int Count;
        public long Elapsed;
        public Nullable<long> Min;
        public Nullable<long> Max;
    }
}

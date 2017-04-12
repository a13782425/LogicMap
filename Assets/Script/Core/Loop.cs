using UnityEngine;
using System.Collections;
using System;

public class Loop
{
    public Loop(int bg, int p)
    {
        // TODO: Complete member initialization
        this.Start = bg;
        this.End = p;
    }
    public int Start { get; private set; }
    public int End { get; private set; }

    public static Loop Begin(int bg)
    {
        return new Loop(bg, 0);
    }

    public static Loop Count(int ed)
    {
        return new Loop(0, ed);
    }
    public static void ForEach<T>(T[] t, Action<T> process)
    {
        Loop.Count(t.Length).Do(x => process(t[x]));
    }

    public Loop Do(Action<int> process)
    {
        for (int i = Start; i < End; i++)
        {
            process(i);
        }
        return this;
    }

}

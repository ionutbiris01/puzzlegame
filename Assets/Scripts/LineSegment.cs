using System.Collections.Generic;
using UnityEngine;
public class LineSegment
{
    public Vector3 Start { get; }
    public Vector3 End { get; }

    public LineSegment(Vector3 start, Vector3 end)
    {
        Start = start;
        End = end;
    }
}

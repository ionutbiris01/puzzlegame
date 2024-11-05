using UnityEngine;

public class LineSegmentIntersection
{
    private const float DistanceThreshold = 0.001f;
    public static float SegmentDistance(Vector3 p1, Vector3 p2, Vector3 q1, Vector3 q2)
    {
        float d1 = Vector3.Distance(p1, q1);
        float d2 = Vector3.Distance(p1, q2);
        float d3 = Vector3.Distance(p2, q1);
        float d4 = Vector3.Distance(p2, q2);

        return Mathf.Min(d1, d2, d3, d4);
    }
    public static bool DoesIntersectWithAny(LineSegment[] existingSegments, LineSegment newSegment, out Vector3 intersectionPoint)
    {
        intersectionPoint = Vector3.zero;

        foreach (LineSegment segment in existingSegments)
        {
            float distance = SegmentDistance(segment.Start, segment.End, newSegment.Start, newSegment.End);

            if (distance < DistanceThreshold)
            {
                continue; 
            }

            if (Intersects(segment.Start, segment.End, newSegment.Start, newSegment.End, out intersectionPoint))
            {
                return true; 
            }
        }

        return false;
    }


    private static bool Intersects(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, out Vector3 intersection)
    {
        Vector3 r = p2 - p1;
        Vector3 s = p4 - p3;

        float rxs = Vector3.Cross(r, s).magnitude;

        if (Mathf.Approximately(rxs, 0))
        {
            intersection = Vector3.zero;
            return false;
        }

        Vector3 d = p3 - p1;
        float t = Vector3.Cross(d, s).magnitude / rxs;
        float u = Vector3.Cross(d, r).magnitude / rxs;

        if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
        {
            intersection = p1 + t * r;
            return true;
        }

        intersection = Vector3.zero;
        return false;
    }
}

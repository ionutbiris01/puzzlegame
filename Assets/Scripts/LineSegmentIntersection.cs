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

            if (DoIntersect(segment.Start, segment.End, newSegment.Start, newSegment.End, out intersectionPoint))
            {
                return true; 
            }
        }

        return false;
    }

    
    private static bool DoIntersect(Vector3 p1, Vector3 p2, Vector3 q1, Vector3 q2, out Vector3 intersectionPoint)
    {
        intersectionPoint = Vector3.zero;

        Vector3 d1 = p2 - p1;
        Vector3 d2 = q2 - q1;

        float denom = Vector3.Cross(d1, d2).magnitude;

        if (denom == 0)
        {
            return false;
        }

        Vector3 diff = q1 - p1;
        float t = Vector3.Cross(diff, d2).magnitude / denom;
        float u = Vector3.Cross(diff, d1).magnitude / denom;

        if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
        {
            intersectionPoint = p1 + t * d1;
            return true;
        }

        return false;
    }
}
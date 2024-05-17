using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mecoinpy
{
    public class Util
    {

        public static Vector2 ClosestPointOnLineSegment(Vector2 point, Vector2 segmentStart, Vector2 segmentEnd)
        {
            Vector2 segment = segmentEnd - segmentStart;
            Vector2 toPoint = point - segmentStart;
            float t = Mathf.Clamp(Vector2.Dot(toPoint, segment) / segment.sqrMagnitude, 0, 1);
            return segmentStart + segment * t;
        }
        public static Vector2 ClosestPointOnLineSegment(Vector2 segmentStart, Vector2 segmentEnd, Vector2 boxMin, Vector2 boxMax)
        {
            Vector2 segment = segmentEnd - segmentStart;
            Vector2 toMin = boxMin - segmentStart;
            float t = Mathf.Clamp(Vector2.Dot(toMin, segment) / segment.sqrMagnitude, 0, 1);
            return segmentStart + segment * t;
        }
    }
}

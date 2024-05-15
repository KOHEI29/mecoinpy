using System;
using UnityEngine;

namespace mecoinpy.Game
{
    public abstract class MyCollider
    {
        public abstract bool CheckCollision(MyCollider other);
    }

    public class AABB : MyCollider
    {
        public Vector2 Min { get; set; }
        public Vector2 Max { get; set; }

        public AABB(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
        }

        public override bool CheckCollision(MyCollider other)
        {
            if (other is AABB aabb)
                return CheckCollision(this, aabb);
            if (other is Circle circle)
                return CheckCollision(circle, this);
            if (other is Capsule capsule)
                return CheckCollision(this, capsule);

            return false;
        }

        private static bool CheckCollision(AABB a, AABB b)
        {
            // AABB同士の衝突判定
            return (a.Min.x <= b.Max.x && a.Max.x >= b.Min.x &&
                    a.Min.y <= b.Max.y && a.Max.y >= b.Min.y);
        }

        internal static bool CheckCollision(Circle circle, AABB aabb)
        {
            // 円とAABBの衝突判定
            float closestX = Math.Max(aabb.Min.x, Math.Min(circle.Center.x, aabb.Max.x));
            float closestY = Math.Max(aabb.Min.y, Math.Min(circle.Center.y, aabb.Max.y));

            float distanceX = circle.Center.x - closestX;
            float distanceY = circle.Center.y - closestY;

            return (distanceX * distanceX + distanceY * distanceY) < (circle.Radius * circle.Radius);
        }

        internal static bool CheckCollision(AABB aabb, Capsule capsule)
        {
            // AABBとカプセルの衝突判定
            // カプセルのセグメントをAABBの範囲でクランプして最も近い点を求め、そこからの距離をチェック
            Vector2 closestPoint = ClosestPointOnLineSegment(capsule.Start, capsule.End, aabb.Min, aabb.Max);
            return Vector2.Distance(closestPoint, aabb.Min) <= capsule.Radius;
        }

        private static Vector2 ClosestPointOnLineSegment(Vector2 segmentStart, Vector2 segmentEnd, Vector2 boxMin, Vector2 boxMax)
        {
            Vector2 segment = segmentEnd - segmentStart;
            Vector2 toMin = boxMin - segmentStart;
            float t = Mathf.Clamp(Vector2.Dot(toMin, segment) / segment.sqrMagnitude, 0, 1);
            return segmentStart + segment * t;
        }
    }

    public class Circle : MyCollider
    {
        public Vector2 Center { get; set; }
        public float Radius { get; set; }

        public Circle(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        public override bool CheckCollision(MyCollider other)
        {
            if (other is AABB aabb)
                return AABB.CheckCollision(this, aabb);
            if (other is Circle circle)
                return CheckCollision(this, circle);
            if (other is Capsule capsule)
                return CheckCollision(this, capsule);

            return false;
        }

        private static bool CheckCollision(Circle a, Circle b)
        {
            // 円同士の衝突判定
            float distanceX = a.Center.x - b.Center.x;
            float distanceY = a.Center.y - b.Center.y;
            float radiusSum = a.Radius + b.Radius;

            return (distanceX * distanceX + distanceY * distanceY) < (radiusSum * radiusSum);
        }

        internal static bool CheckCollision(Circle circle, Capsule capsule)
        {
            // 円とカプセルの衝突判定
            Vector2 closestPoint = ClosestPointOnLineSegment(circle.Center, capsule.Start, capsule.End);
            float distanceX = circle.Center.x - closestPoint.x;
            float distanceY = circle.Center.y - closestPoint.y;

            float combinedRadius = capsule.Radius + circle.Radius;
            return (distanceX * distanceX + distanceY * distanceY) < (combinedRadius * combinedRadius);
        }

        private static Vector2 ClosestPointOnLineSegment(Vector2 point, Vector2 segmentStart, Vector2 segmentEnd)
        {
            Vector2 segment = segmentEnd - segmentStart;
            Vector2 toPoint = point - segmentStart;
            float t = Mathf.Clamp(Vector2.Dot(toPoint, segment) / segment.sqrMagnitude, 0, 1);
            return segmentStart + segment * t;
        }
    }

    public class Capsule : MyCollider
    {
        public Vector2 Start { get; set; }
        public Vector2 End { get; set; }
        public float Radius { get; set; }

        public Capsule(Vector2 start, Vector2 end, float radius)
        {
            Start = start;
            End = end;
            Radius = radius;
        }

        public override bool CheckCollision(MyCollider other)
        {
            if (other is AABB aabb)
                return AABB.CheckCollision(aabb, this);
            if (other is Circle circle)
                return Circle.CheckCollision(circle, this);
            if (other is Capsule capsule)
                return CheckCollision(this, capsule);

            return false;
        }

        private static bool CheckCollision(Capsule a, Capsule b)
        {
            // カプセル同士の衝突判定
            float combinedRadius = a.Radius + b.Radius;
            return ClosestDistanceBetweenSegments(a.Start, a.End, b.Start, b.End) < (combinedRadius * combinedRadius);
        }

        private static float ClosestDistanceBetweenSegments(Vector2 aStart, Vector2 aEnd, Vector2 bStart, Vector2 bEnd)
        {
            Vector2 u = aEnd - aStart;
            Vector2 v = bEnd - bStart;
            Vector2 w = aStart - bStart;

            float a = Vector2.Dot(u, u);
            float b = Vector2.Dot(u, v);
            float c = Vector2.Dot(v, v);
            float d = Vector2.Dot(u, w);
            float e = Vector2.Dot(v, w);

            float D = a * c - b * b;
            float sc, sN, sD = D;
            float tc, tN, tD = D;

            if (D < Mathf.Epsilon)
            {
                sN = 0.0f;
                sD = 1.0f;
                tN = e;
                tD = c;
            }
            else
            {
                sN = (b * e - c * d);
                tN = (a * e - b * d);

                if (sN < 0.0f)
                {
                    sN = 0.0f;
                    tN = e;
                    tD = c;
                }
                else if (sN > sD)
                {
                    sN = sD;
                    tN = e + b;
                    tD = c;
                }
            }

            if (tN < 0.0f)
            {
                tN = 0.0f;
                if (-d < 0.0f)
                    sN = 0.0f;
                else if (-d > a)
                    sN = sD;
                else
                {
                    sN = -d;
                    sD = a;
                }
            }
            else if (tN > tD)
            {
                tN = tD;
                if ((-d + b) < 0.0f)
                    sN = 0.0f;
                else if ((-d + b) > a)
                    sN = sD;
                else
                {
                    sN = (-d + b);
                    sD = a;
                }
            }

            sc = (Mathf.Abs(sN) < Mathf.Epsilon ? 0.0f : sN / sD);
            tc = (Mathf.Abs(tN) < Mathf.Epsilon ? 0.0f : tN / tD);

            Vector2 dP = w + (u * sc) - (v * tc);
            return dP.sqrMagnitude;
        }
    }
}

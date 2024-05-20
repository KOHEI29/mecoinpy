using System;
using UnityEngine;

namespace mecoinpy.Game
{
    public abstract class MyCollider
    {
        private MyGameObject _transform = default;
        public MyGameObject GameObject => _transform;
        public MyCollider(MyGameObject go)
        {
            _transform = go;
        }
        public abstract bool CheckCollision(MyCollider other);
        public abstract bool CheckCollision(MyCollider other, out Vector2 contactVector);
    }

    public class AABB : MyCollider
    {
        public Vector2 Min { get; set; }
        public Vector2 Max { get; set; }

        public AABB(Vector2 min, Vector2 max, MyGameObject go) : base(go)
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
        public override bool CheckCollision(MyCollider other, out Vector2 contactVector)
        {
            if (other is AABB aabb)
            {
                return CheckCollision(this, aabb, out contactVector);
            }
            else
            {
                contactVector = Vector2.zero;
            }
            if (other is Circle circle)
                return CheckCollision(circle, this);
            if (other is Capsule capsule)
                return CheckCollision(this, capsule);

            return false;
        }

        private static bool CheckCollision(AABB a, AABB b)
        {
            var amax = a.Max + a.GameObject.Position;
            var amin = a.Min + a.GameObject.Position;
            var bmax = b.Max + b.GameObject.Position;
            var bmin = b.Min + b.GameObject.Position;
            // AABB同士の衝突判定
            return (amin.x <= bmax.x && amax.x >= bmin.x &&
                    amin.y <= bmax.y && amax.y >= bmin.y);
        }
        //競合のベクターも返す衝突判定
        private static bool CheckCollision(AABB a, AABB b, out Vector2 contactVector)
        {
            contactVector = Vector2.zero;

            if(CheckCollision(a, b))
            {
                var amax = a.Max + a.GameObject.Position;
                var amin = a.Min + a.GameObject.Position;
                var bmax = b.Max + b.GameObject.Position;
                var bmin = b.Min + b.GameObject.Position;

                //衝突するので、方向を計算
                float dx = Math.Min(amax.x - bmin.x, bmax.x - amin.x);
                float dy = Math.Min(amax.y - bmin.y, bmax.y - amin.y);

                // 最小の交差量を持つ方向を接触方向とする
                if (Mathf.Abs(dx) < Mathf.Abs(dy))
                {
                    // 左右方向のめり込みが小さい場合
                    if (amax.x - bmin.x < bmax.x - amin.x)
                    {
                        // 左方向にめり込んでいる
                        contactVector.x = amax.x - bmin.x;
                    }
                    else
                    {
                        // 右方向にめり込んでいる
                        contactVector.x = -(bmax.x - amin.x);
                    }
                }
                else
                {
                    // 上下方向のめり込みが小さい場合
                    if (amax.y - bmin.y < bmax.y - amin.y)
                    {
                        // 下方向にめり込んでいる
                        contactVector.y = -(amax.y - bmin.y);
                    }
                    else
                    {
                        // 上方向にめり込んでいる
                        contactVector.y = bmax.y - amin.y;
                    }
                }
                
                return true;
            }
            return false;            
        }

        internal static bool CheckCollision(Circle circle, AABB aabb)
        {
            var circlecenter = circle.Center + circle.GameObject.Position;
            var aabbmax = aabb.Max + aabb.GameObject.Position;
            var aabbmin = aabb.Min + aabb.GameObject.Position;
            // 円とAABBの衝突判定
            float closestX = Math.Max(aabbmin.x, Math.Min(circlecenter.x, aabbmax.x));
            float closestY = Math.Max(aabbmin.y, Math.Min(circlecenter.y, aabbmax.y));

            float distanceX = circlecenter.x - closestX;
            float distanceY = circlecenter.y - closestY;

            return (distanceX * distanceX + distanceY * distanceY) < (circle.Radius * circle.Radius);
        }

        internal static bool CheckCollision(AABB aabb, Capsule capsule)
        {
            var aabbmax = aabb.Max + aabb.GameObject.Position;
            var aabbmin = aabb.Min + aabb.GameObject.Position;
            // AABBとカプセルの衝突判定
            // カプセルのセグメントをAABBの範囲でクランプして最も近い点を求め、そこからの距離をチェック
            Vector2 closestPoint = Util.ClosestPointOnLineSegment(capsule.Start + capsule.GameObject.Position, capsule.End + capsule.GameObject.Position, aabbmin, aabbmax);
            return Vector2.Distance(closestPoint, aabbmin) <= capsule.Radius;
        }
    }

    public class Circle : MyCollider
    {
        public Vector2 Center { get; set; }
        public float Radius { get; set; }

        public Circle(Vector2 center, float radius, MyGameObject go) : base(go)
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
        public override bool CheckCollision(MyCollider other, out Vector2 contactVector)
        {
            contactVector = Vector2.zero;
            return CheckCollision(other);
        }

        private static bool CheckCollision(Circle a, Circle b)
        {
            var acenter = a.Center + a.GameObject.Position;
            var bcenter = b.Center + b.GameObject.Position;
            // 円同士の衝突判定
            float distanceX = acenter.x - bcenter.x;
            float distanceY = acenter.y - bcenter.y;
            float radiusSum = a.Radius + b.Radius;

            return (distanceX * distanceX + distanceY * distanceY) < (radiusSum * radiusSum);
        }

        internal static bool CheckCollision(Circle circle, Capsule capsule)
        {
            var circlecenter = circle.Center + circle.GameObject.Position;
            // 円とカプセルの衝突判定
            Vector2 closestPoint = Util.ClosestPointOnLineSegment(circlecenter, capsule.Start + capsule.GameObject.Position, capsule.End + capsule.GameObject.Position);
            float distanceX = circlecenter.x - closestPoint.x;
            float distanceY = circlecenter.y - closestPoint.y;

            float combinedRadius = capsule.Radius + circle.Radius;
            return (distanceX * distanceX + distanceY * distanceY) < (combinedRadius * combinedRadius);
        }
    }

    public class Capsule : MyCollider
    {
        public Vector2 Start { get; set; }
        public Vector2 End { get; set; }
        public float Radius { get; set; }

        public Capsule(Vector2 start, Vector2 end, float radius, MyGameObject go) : base(go)
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
        public override bool CheckCollision(MyCollider other, out Vector2 contactVector)
        {
            contactVector = Vector2.zero;
            return CheckCollision(other);
        }

        private static bool CheckCollision(Capsule a, Capsule b)
        {
            // カプセル同士の衝突判定
            float combinedRadius = a.Radius + b.Radius;
            return ClosestDistanceBetweenSegments(a.Start + a.GameObject.Position, a.End + a.GameObject.Position, b.Start + b.GameObject.Position, b.End + b.GameObject.Position) < (combinedRadius * combinedRadius);
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

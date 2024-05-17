using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mecoinpy.Game
{
    public class GameConst
    {
        public class Initialize
        {
            public static readonly Vector2 PlayerPosition = new Vector2(0f, 0f);
            public const float PlayerJumpVelocity= 12f;
        }
        //重力加速度
        public static readonly float DefaultGravityAcceleration = 9.81f;

        //タップとスワイプのしきい値（スクリーン座標）
        public const float SwipeThreshold = 0.5f;
        public const float SwipeThresholdSqr = SwipeThreshold * SwipeThreshold;
    }
}

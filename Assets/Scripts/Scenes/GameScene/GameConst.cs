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

        //ジャンプ構え状態の長さ(s)
        public const float JumpStandbySeconds = 0.3f;
        //ストンプの速度
        public static readonly Vector2 StompVecolity = new Vector2(0, -500f);
        //ストンプ着地の跳ね上がり速度
        public static readonly Vector2 StompBoundVecolity = new Vector2(0, 5f);
        //重力加速度
        public const float DefaultGravityAcceleration = 9.81f;

        //タップとスワイプのしきい値（スクリーン座標）
        public const float SwipeThreshold = 0.5f;
        public const float SwipeThresholdSqr = SwipeThreshold * SwipeThreshold;
    }
}

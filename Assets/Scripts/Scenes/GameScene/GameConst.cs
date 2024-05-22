using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mecoinpy.Game
{
    public class GameConst
    {
        public class Initialize
        {
            public static readonly Vector2 PlayerPosition = new Vector2(0f, -3f);
            public const float PlayerJumpVelocity = 12f;
            public const int PlayerWallJumpMax = 1;
            //エイム中にスローモーションになる時間
            public const float AimSeconds = 2f;
        }

        //ジャンプ構え状態の長さ(s)
        public const float JumpStandbySeconds = 0.3f;
        //ストンプの速度
        public static readonly Vector2 StompVelocity = new Vector2(0, -500f);
        //ストンプ着地の跳ね上がり速度
        public static readonly Vector2 StompBoundVelocity = new Vector2(0, 5f);
        //壁ジャンプの速度
        public const float PlayerWallJumpVelocity = 8f;
        //壁ジャンプの方向
        public static readonly Vector2 LeftWallJumpingVelocity = new Vector2(1f,2f).normalized;
        public static readonly Vector2 RightWallJumpingVelocity = new Vector2(-1f,2f).normalized;
        //重力加速度
        public const float DefaultGravityAcceleration = 9.81f;
        //スローモーション中のTimeScale
        public const float SlowTimeScale = 0.1f;

        //タップとスワイプのしきい値（スクリーン座標）
        public const float SwipeThreshold = 0.5f;
        public const float SwipeThresholdSqr = SwipeThreshold * SwipeThreshold;

        //エイムガイドの白いパーツの数
        public const int AimPartsCount = 8;
        //エイムガイドの白いパーツの間隔
        public const float AimPartsOffset = 0.2f;
        //所持果物とプレイヤーのオフセット
        public const float PossessFruitsOffsetWithPlayer = -1f;
        //所持果物の大きさ
        public const float PossessFruitsSize = 0.5f;
        //所持果物の間隔
        public const float PossessFruitsSpaceX = 0.15f;
        public const float PossessFruitsSpaceY = 0.15f;
        //所持果物の大きさ＋間隔
        public const float PossessFruitsOffsetX = PossessFruitsSize + PossessFruitsSpaceX;
        public const float PossessFruitsOffsetY = PossessFruitsSize + PossessFruitsSpaceY;

        //カメラの最小Y
        public const float CameraMinY = 1f;
        //カメラのプレイヤーに対するYオフセット
        public const float CameraOffsetY = 2f;
    }
}

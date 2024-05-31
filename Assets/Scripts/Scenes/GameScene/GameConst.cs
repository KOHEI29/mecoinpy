using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mecoinpy.Game
{
    public class GameConst
    {
        //初期化データ
        public class Initialize
        {
            public static readonly Vector2 PlayerPosition = new Vector2(0f, -3f);
            public const float PlayerJumpVelocity = 12f;
            public const int PlayerWallJumpMax = 1;
            //エイム中にスローモーションになる時間
            public const float AimSeconds = 2f;
            //制限時間
            public const float TimelimitMax = 20f;
            //体力
            public const int Health = 2;
        }

        //ジャンプ構え状態の長さ(s)
        public const float JumpStandbySeconds = 0.3f;
        //ストンプの速度
        public static readonly Vector2 StompVelocity = new Vector2(0, -500f);
        //ストンプ着地の跳ね上がり速度
        public static readonly Vector2 StompBoundVelocity = new Vector2(0, 5f);
        //敵を踏んだ時の跳ね上がり速度
        public static readonly Vector2 EnemyTreadVelocity = new Vector2(0, 7f);
        //敵に左から当たった時の飛ばされる速度
        public static readonly Vector2 EnemyHitLeftVelocity = new Vector2(-5f, 3f);
        //敵に右から当たった時の飛ばされる速度
        public static readonly Vector2 EnemyHitRightVelocity = new Vector2(5f, 3f);
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
        //課題果物の大きさ
        public const float RequireFruitsSize = 70f;
        //課題果物の感覚
        public const float RequireFruitsSpaceX = 5f;
        public const float RequireFruitsSpaceY = 5f;
        //課題果物の大きさ＋間隔
        public const float RequireFruitsOffsetX = RequireFruitsSize + RequireFruitsSpaceX;
        public const float RequireFruitsOffsetY = RequireFruitsSize + RequireFruitsSpaceY;
        //課題果物の吹き出しの基準Y
        public const float RequireBalloonBaseY = 150f;
        //課題の吹き出しが減っているのが見える割合。増やすとすぐ減る
        public const float RequireBalloonVisibleRatio = 0.9f;
        //吹き出しのフィルの色（課題未達成）
        public static readonly Color RequireBalloonColorStill = new Color(0f, 0f, 0f, 137f/255f);
        //吹き出しのフィルの色（課題達成済み）
        public static readonly Color RequireBalloonColorReady = new Color(0f, 1f, 41f/255f, 137f/255f);
        //吹き出しの右下のテキスト（課題達成）
        public static readonly string RequireBalloonTextReady = "READY!";
        //吹き出しの右下のテキスト（ボーナス中）
        public static readonly string RequireBalloonTextBonus = "BONUS\nSERVICE";
        //達成した課題の色
        public static readonly Color RequireClearColor = new Color(132f/255f, 132f/255f, 132f/255f);

        //カメラの最小Y
        public const float CameraMinY = 1f;
        //カメラのプレイヤーに対するYオフセット
        public const float CameraOffsetY = 2f;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace mecoinpy.Game
{
    //プレイヤーのデータ
    public class PlayerData : PhysicsObject
    {
        //所持ジャンプ玉
        private int _jumpBall = 0;
        public int JumpBall => _jumpBall;
        //ジャンプ玉最大値
        private int _jumpBallMax = 5;
        public int JumpBallMax => _jumpBallMax;

        //コンストラクタ
        public PlayerData() : base(new MyPhysics(), new AABB(new Vector2(-0.5f, -0.5f), new Vector2(0.5f, 0.5f)))
        {
            _jumpBall = JumpBallMax;
        }
    }
    //ステージデータ
    public class StageData
    {
        private StageObject[] _stageObjects = default;
        public StageObject[] StageObjects => _stageObjects;

        public StageData()
        {
            //***Debug仮データ作成
            _stageObjects = new StageObject[1];
            _stageObjects[0] = new StageObject(StageObject.ObjectType.WALL, new Vector2(0f, -10f), new Vector2(10f, 1f));
        }
    }
    public class StageObject : PhysicsObject
    {
        public enum ObjectType
        {
            DEFAULT = -1,
            WALL = 0,
            STEP,
        }
        private ObjectType _type = ObjectType.DEFAULT;
        public ObjectType Type => _type;
        private Vector2 _scale = Vector2.zero;
        public Vector2 Scale => _scale;
        public StageObject(ObjectType t, Vector2 position, Vector2 scale) : base(MyPhysics.KinematicObject(position), default)
        {
            _type = t;
            _scale = scale;
            _collider = new AABB(position-scale*0.5f, position+scale*0.5f);
        }
    }
}

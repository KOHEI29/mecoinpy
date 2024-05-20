using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace mecoinpy.Game
{
    //プレイヤーのデータ
    public class PlayerData
    {
        //UntilDestroyのTarget用
        private GameObject _gameObject = default;

        private PhysicsObject _physicsObject = default;
        public PhysicsObject PhysicsObject => _physicsObject;
        
        //状態
        private ReactiveProperty<GameEnum.PlayerState> _state = new ReactiveProperty<GameEnum.PlayerState>(GameEnum.PlayerState.DEFAULT);
        public IReadOnlyReactiveProperty<GameEnum.PlayerState> State => _state;
        //貯められているジャンプ力。速度の形で溜まっている
        private Vector2 _jumpPower = Vector2.zero;

        //ジャンプ速度
        private float _jumpVelocity = GameConst.Initialize.PlayerJumpVelocity;
        public float JumpVelocity => _jumpVelocity;
        //所持ジャンプ玉
        private int _jumpBall = 0;
        public int JumpBall => _jumpBall;
        //ジャンプ玉最大値
        private int _jumpBallMax = 5;
        public int JumpBallMax => _jumpBallMax;

        //コンストラクタ
        public PlayerData(GameObject gameObject)
        {
            _gameObject = gameObject;
            _state.Value = GameEnum.PlayerState.JUMPING;
            _jumpBall = JumpBallMax;

            _physicsObject = new PhysicsObject();
            _physicsObject.SetCollider(new AABB(new Vector2(-0.5f, -0.5f), new Vector2(0.5f, 0.5f), _physicsObject));
        }

        public bool IsGrounded(StageObject[] objects)
        {
            for(int i = 0; i < objects.Length; i++)
            {
                if(PhysicsObject.Collider.CheckCollision(objects[i].ColliderObject.Collider, out Vector2 contactVector))
                {
                    if(contactVector.y > 0f)
                    {
                        //着地した。
                        var temp = PhysicsObject.Position + contactVector;
                        PhysicsObject.Position = temp;
                        PhysicsObject.Physics.Grounded();
                        //ストンプしていた場合、真上に跳ね上がる
                        if(State.Value == GameEnum.PlayerState.STOMPING)
                        {
                            PhysicsObject.Physics.Velocity = GameConst.StompBoundVecolity;
                        }
                        _state.Value = GameEnum.PlayerState.IDLE;
                        return true;
                    }
                }
            }
            return false;
        }
        //ジャンプを試みる
        public bool TryJump(Vector2 direction)
        {
            if(true)
            {
                //ジャンプ構え状態に
                _state.Value = GameEnum.PlayerState.JUMPSTANDBY;
                _jumpPower = direction.normalized * JumpVelocity;
                _physicsObject.Physics.Type = MyPhysics.BodyType.STATIC;
                Observable.Timer(TimeSpan.FromSeconds(GameConst.JumpStandbySeconds), Scheduler.MainThreadIgnoreTimeScale)
                        .TakeUntilDestroy(_gameObject)
                        .SubscribeWithState(this, static (x, t) =>
                        {
                            //ジャンプする
                            t._physicsObject.Physics.Type = MyPhysics.BodyType.DYNAMIC;
                            t.PhysicsObject.Physics.Velocity = t._jumpPower;
                            t._jumpPower = Vector2.zero;
                            t._state.Value = GameEnum.PlayerState.JUMPING;
                        });
                return true;
            }
            return false;
        }
        //ストンプを試みる
        public bool TryStomp()
        {
            if(State.Value == GameEnum.PlayerState.JUMPING)
            {
                //ストンプ構え状態に
                _state.Value = GameEnum.PlayerState.STOMPSTANDBY;
                _physicsObject.Physics.Type = MyPhysics.BodyType.STATIC;
                Observable.Timer(TimeSpan.FromSeconds(GameConst.JumpStandbySeconds), Scheduler.MainThreadIgnoreTimeScale)
                        .TakeUntilDestroy(_gameObject)
                        .SubscribeWithState(this, static (x, t) =>
                        {
                            //ストンプする
                            t._physicsObject.Physics.Type = MyPhysics.BodyType.DYNAMIC;
                            t.PhysicsObject.Physics.Velocity = GameConst.StompVecolity;
                            t._state.Value = GameEnum.PlayerState.STOMPING;
                        });

                return true;
            }
            return false;
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
    public class StageObject
    {
        public enum ObjectType
        {
            DEFAULT = -1,
            WALL = 0,
            STEP,
        }
        private ObjectType _type = ObjectType.DEFAULT;
        public ObjectType Type => _type;
        private ColliderObject _colliderObject = default;
        public ColliderObject ColliderObject => _colliderObject;
        public StageObject(ObjectType t, Vector2 position, Vector2 scale)
        {
            _type = t;
            _colliderObject = new ColliderObject();
            ColliderObject.Position = position;
            ColliderObject.Scale = scale;
            ColliderObject.SetCollider(new AABB(-scale*0.5f, scale*0.5f, ColliderObject));
        }
    }
}

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

        //エイム中にスローモーションになる時間
        private float _aimSeconds = GameConst.Initialize.AimSeconds;
        public float AimSeconds => _aimSeconds;
        //貯められているジャンプ力。速度の形で溜まっている
        private Vector2 _jumpPower = Vector2.zero;

        //壁ジャンプをした回数
        private int _wallJumpCount = 0;
        private int _wallJumpMax = GameConst.Initialize.PlayerWallJumpMax;

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
            _physicsObject.Position = GameConst.Initialize.PlayerPosition;
            _physicsObject.SetCollider(new AABB(new Vector2(-0.5f, -0.5f), new Vector2(0.5f, 0.5f), _physicsObject));
        }

        public bool IsGrounded(StageObject[] objects)
        {
            for(int i = 0; i < objects.Length; i++)
            {
                if(PhysicsObject.Collider.CheckCollision(objects[i].ColliderObject.Collider, out Vector2 contactVector))
                {
                    if(contactVector.y < 0f)
                    {
                        //上昇中は着地しない
                        if(PhysicsObject.Physics.Velocity.y < 1f)
                        {
                            //着地した。
                            //壁ジャンプの回数リセット
                            _wallJumpCount = 0;
                            var temp = PhysicsObject.Position - contactVector;
                            PhysicsObject.Position = temp;
                            PhysicsObject.Physics.Grounded();
                            //ストンプしていた場合、真上に跳ね上がる
                            if(State.Value == GameEnum.PlayerState.STOMPING)
                            {
                                PhysicsObject.Physics.Velocity = GameConst.StompBoundVelocity;
                            }
                            _state.Value = GameEnum.PlayerState.IDLE;
                        }
                        return true;
                    }
                    else if(contactVector.y > 0f)
                    {
                        if(objects[i].Type == StageObject.ObjectType.WALL)
                        {
                            //天井にぶつかった。
                            var temp = PhysicsObject.Position - contactVector;
                            PhysicsObject.Position = temp;
                            PhysicsObject.Physics.HitCeil();
                        }
                    }
                    else if(contactVector.x != 0f)
                    {
                        if(objects[i].Type == StageObject.ObjectType.WALL)
                        {
                            //壁に当たった
                            var temp = PhysicsObject.Position - contactVector;
                            PhysicsObject.Position = temp;
                            if(_wallJumpCount < _wallJumpMax)
                            {
                                _wallJumpCount++;
                                //壁ジャンプする
                                WallJump(contactVector.x > 0f ? GameConst.RightWallJumpingVelocity : GameConst.LeftWallJumpingVelocity);
                            }
                        }
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
                _jumpPower = direction * JumpVelocity;
                _physicsObject.Physics.Type = MyPhysics.BodyType.STATIC;
                Observable.Timer(TimeSpan.FromSeconds(GameConst.JumpStandbySeconds), Scheduler.MainThreadIgnoreTimeScale)
                        .TakeUntilDestroy(_gameObject)
                        .SubscribeWithState(this, (x, t) =>
                        {
                            //ジャンプする
                            t._physicsObject.Physics.Type = MyPhysics.BodyType.DYNAMIC;
                            t.PhysicsObject.Physics.Velocity = t._jumpPower;
                            t._jumpPower = Vector2.zero;
                            t._state.Value = GameEnum.PlayerState.JUMPING;
                            //壁ジャンプの回数リセット
                            t._wallJumpCount = 0;
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
                        .SubscribeWithState(this, (x, t) =>
                        {
                            //ストンプする
                            t._physicsObject.Physics.Type = MyPhysics.BodyType.DYNAMIC;
                            t.PhysicsObject.Physics.Velocity = GameConst.StompVelocity;
                            t._state.Value = GameEnum.PlayerState.STOMPING;
                        });

                return true;
            }
            return false;
        }
        //壁ジャンプ
        public void WallJump(Vector2 direction)
        {
            _wallJumpCount += 1;
            //ジャンプ構え状態に
            _state.Value = GameEnum.PlayerState.JUMPSTANDBY;
            _jumpPower = direction * GameConst.PlayerWallJumpVelocity;
            _physicsObject.Physics.Type = MyPhysics.BodyType.STATIC;
            Observable.Timer(TimeSpan.FromSeconds(GameConst.JumpStandbySeconds), Scheduler.MainThreadIgnoreTimeScale)
                    .TakeUntilDestroy(_gameObject)
                    .SubscribeWithState(this, (x, t) =>
                    {
                        //ジャンプする
                        t._physicsObject.Physics.Type = MyPhysics.BodyType.DYNAMIC;
                        t.PhysicsObject.Physics.Velocity = t._jumpPower;
                        t._jumpPower = Vector2.zero;
                        t._state.Value = GameEnum.PlayerState.JUMPING;
                    });
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
            _stageObjects = new StageObject[4];
            _stageObjects[0] = new StageObject(StageObject.ObjectType.WALL, new Vector2(0f, -10f), new Vector2(10f, 1f));
            _stageObjects[1] = new StageObject(StageObject.ObjectType.WALL, new Vector2(-5f, 0f), new Vector2(3f, 21f));
            _stageObjects[2] = new StageObject(StageObject.ObjectType.WALL, new Vector2(5f, 0f), new Vector2(3f, 21f));
            _stageObjects[3] = new StageObject(StageObject.ObjectType.STEP, new Vector2(0f, 5f), new Vector2(10f, 1f));
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

    //果物データ
    public class FruitsData
    {
        private FruitsObject[] _fruitsObjects = default;
        public FruitsObject[] FruitsObjects => _fruitsObjects;

        public FruitsData()
        {
            //***Debug仮データ作成
            _fruitsObjects = new FruitsObject[1];
            _fruitsObjects[0] = new FruitsObject(FruitsObject.FruitsType.RED, new Vector2(0f, 3f), 1f);
        }
    }
    public class FruitsObject
    {
        public enum FruitsType
        {
            DEFAULT = -1,
            RED = 0,
            BLUE,
            YELLOW,
        }
        private FruitsType _type = FruitsType.DEFAULT;
        public FruitsType Type => _type;
        private ColliderObject _colliderObject = default;
        public ColliderObject ColliderObject => _colliderObject;
        public FruitsObject(FruitsType t, Vector2 position, float scale)
        {
            _type = t;
            _colliderObject = new ColliderObject();
            ColliderObject.Position = position;
            ColliderObject.Scale = new Vector2(scale, scale);
            ColliderObject.SetCollider(new Circle(Vector2.zero, scale, ColliderObject));
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace mecoinpy.Game
{
    //ゲーム中のデータ
    public class GameData
    {
        //体力
        private IntReactiveProperty _health = new IntReactiveProperty(GameConst.Initialize.Health);
        public IReadOnlyReactiveProperty<int> Health => _health;
        //持っている果物
        private int[] _fruits = new int[(int)FruitsObject.FruitsType.Count];
        public IReadOnlyCollection<int> Fruits => _fruits;
        //余分に取っている果物の数。課題の達成には関係なし。
        private int _bonusFruits = 0;
        //課題
        private int[] _require = new int[(int)FruitsObject.FruitsType.Count];
        public IReadOnlyCollection<int> Require => _require;
        //制限時間最大値
        private float _timeLimitMax = 0f;
        public float TimeLimitMax => _timeLimitMax;
        //課題の状況
        private ReactiveProperty<GameEnum.RequireState> _requireState = new ReactiveProperty<GameEnum.RequireState>(GameEnum.RequireState.DEFAULT);
        public IReadOnlyReactiveProperty<GameEnum.RequireState> RequireState => _requireState;
        //課題をクリアした回数
        private int _countClear = 0;
        public int CountClear => _countClear;

        public GameData()
        {
        }
        //ダメージを受けた
        public bool Damaged()
        {
            return --_health.Value > 0;
        }
        //果物を入手した。課題が進んだら何番目の課題をクリアしたかを返す
        public int GetFruits(FruitsObject.FruitsType type)
        {
            int result = -1;
            _fruits[(int)type]++;
            if(RequireState.Value == GameEnum.RequireState.STILL)
            {
                for(int i = 0; i < _require.Length; i++)
                {
                    if(_require[i] > _fruits[i])
                        break;
                    if(i == _require.Length - 1)
                    {
                        //余分の果物をボーナスに変換する
                        _requireState.Value = GameEnum.RequireState.READY + _bonusFruits;
                        _bonusFruits = 0;
                    }
                }
                //何番目の課題が進んだかを調べる
                if(_fruits[(int)type] <= _require[(int)type])
                {
                    for(int i = 0; i < _require.Length; i++)
                    {
                        if(i < (int)type)
                            result += _require[i];
                        else
                        {
                            result += _fruits[(int)type];
                            break;
                        }
                    }
                }
                //課題を進めない果物だった場合、ボーナスに追加する
                if(result == -1) _bonusFruits++;
            }
            else if(RequireState.Value > GameEnum.RequireState.STILL)
            {
                _requireState.Value++;
            }
            return result;
        }
        //果物をリセットする。
        public void ResetFruits()
        {
            for(int i = 0; i < _fruits.Length; i++)
            {
                _fruits[i] = 0;
            }
            //余分の果物の数をリセット
            _bonusFruits = 0;
        }
        //課題をクリアした
        public void ClearRequire()
        {
            _countClear++;
        }
        //課題を作る
        public void NextRequire()
        {
            //***Debug仮データ作成
            //_require = new int[]{UnityEngine.Random.Range(0,3),UnityEngine.Random.Range(0,3),UnityEngine.Random.Range(0,3)};
            _require = new int[]{1,0,0};
            _timeLimitMax = GameConst.Initialize.TimelimitMax;
            _requireState.Value = GameEnum.RequireState.STILL;
        }
    }
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
        //無敵かどうか
        private bool _invincible = false;

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
        //アイテム類との当たりチェック。触れたか否かのみ
        public bool CheckCollisionWithItem(MyCollider collider)
        {
            return PhysicsObject.Collider.CheckCollision(collider);
        }
        //敵との当たりチェック。上からなら踏めるがそれ以外ならダメージ
        public GameEnum.EnemyCollisionState CheckCollisionWithEnemy(MyCollider collider)
        {
            if(PhysicsObject.Collider.CheckCollision(collider, out Vector2 contactVector))
            {
                if(contactVector.y < 0f)
                {
                    //踏んだ。
                    //壁ジャンプの回数リセット
                    _wallJumpCount = 0;
                    var temp = PhysicsObject.Position - contactVector;
                    PhysicsObject.Position = temp;
                    PhysicsObject.Physics.Grounded();
                    //真上にジャンプ
                    Jump(GameConst.EnemyTreadVelocity);
                    return GameEnum.EnemyCollisionState.TREAD;
                }
                //無敵かどうかを調べる
                else if(!_invincible)
                {
                    //壁ジャンプの回数リセット
                    _wallJumpCount = 0;
                    var temp = PhysicsObject.Position - contactVector;
                    PhysicsObject.Position = temp;
                    PhysicsObject.Physics.Grounded();
                    //無敵に
                    _invincible = true;
                    if(collider.GameObject.Position.x > this.PhysicsObject.Position.x)
                    {
                        //左から当たった
                        //進行方向の反対に飛ばされる
                        Blown(GameConst.EnemyHitLeftVelocity);
                        return GameEnum.EnemyCollisionState.HIT;
                    }
                    else
                    {
                        //右から当たった
                        //進行方向の反対に飛ばされる
                        Blown(GameConst.EnemyHitRightVelocity);
                        return GameEnum.EnemyCollisionState.HIT;
                    }
                }
            }
            return GameEnum.EnemyCollisionState.NOT;
        }
        //接地チェック
        public bool CheckCollisionWithStage(StageObject[] objects)
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
                            return true;
                        }
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
        //ジャンプする
        public void Jump(Vector2 velocity)
        {
            //ジャンプ構え状態に
            _state.Value = GameEnum.PlayerState.JUMPSTANDBY;
            _jumpPower = velocity;
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
        }
        //飛ばされる
        public void Blown(Vector2 velocity)
        {
            //被ダメ状態に
            _state.Value = GameEnum.PlayerState.DAMAGED;
            _jumpPower = velocity;
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
        private List<FruitsObject> _fruitsObjects = default;
        public List<FruitsObject> FruitsObjects => _fruitsObjects;

        public FruitsData()
        {
            //***Debug仮データ作成
            _fruitsObjects = new List<FruitsObject>(5);
            _fruitsObjects.Add(new FruitsObject(FruitsObject.FruitsType.RED, new Vector2(0f, 3f), 1f));
            _fruitsObjects.Add(new FruitsObject(FruitsObject.FruitsType.BLUE, new Vector2(0f, 4f), 1f));
            _fruitsObjects.Add(new FruitsObject(FruitsObject.FruitsType.YELLOW, new Vector2(-1f, 4f), 1f));
            _fruitsObjects.Add(new FruitsObject(FruitsObject.FruitsType.BLUE, new Vector2(1f, 4f), 1f));
            _fruitsObjects.Add(new FruitsObject(FruitsObject.FruitsType.YELLOW, new Vector2(1f, 3f), 1f));
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

            Count
        }
        //オブジェクトの削除に使うID
        private static int _nextId = 0;
        private int _id = _nextId++;
        public int Id => _id;
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
    //敵データ
    public class EnemyData
    {
        private List<EnemyObject> _enemyObjects = default;
        public List<EnemyObject> EnemyObjects => _enemyObjects;

        public EnemyData()
        {
            //***Debug仮データ作成
            _enemyObjects = new List<EnemyObject>(1);
            _enemyObjects.Add(new EnemyObject(EnemyObject.EnemyType.BIRD, new Vector2(-2f, 0f), 1f));
        }
    }
    public class EnemyObject
    {
        public enum EnemyType
        {
            DEFAULT = -1,
            BIRD = 0,

            Count
        }
        //オブジェクトの削除に使うID
        private static int _nextId = 0;
        private int _id = _nextId++;
        public int Id => _id;
        private EnemyType _type = EnemyType.DEFAULT;
        public EnemyType Type => _type;
        private PhysicsObject _physicsObject = default;
        public PhysicsObject PhysicsObject => _physicsObject;
        public EnemyObject(EnemyType t, Vector2 position, float scale)
        {
            _type = t;
            _physicsObject = new PhysicsObject();
            _physicsObject.Position = position;
            _physicsObject.Scale = new Vector3(1f, 0.6f, 1f);
            _physicsObject.SetCollider(new AABB(new Vector2(-0.6f, -0.5f), new Vector2(0.6f, 0.5f), _physicsObject));
        }
    }
}

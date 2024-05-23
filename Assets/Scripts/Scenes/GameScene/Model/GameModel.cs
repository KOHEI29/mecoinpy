using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace mecoinpy.Game
{
    public partial interface IGameModel : IModel
    {
        //ゲームデータ
        GameData GameData{get;}
        //ステージデータ
        StageData StageData{get;}
        //フルーツデータ
        FruitsData FruitsData{get;}
        //敵データ
        EnemyData EnemyData{get;}
        //プレイヤーのジャンプ力（エイムの計算に使用）
        float PlayerJumpVelocity{get;}

        //GameState
        IReadOnlyReactiveProperty<GameEnum.GameState> GameState{get;}
        //タイムスケール
        float TimeScale{get;}
        //エイムのスローモーションの残り時間
        IReadOnlyReactiveProperty<float> AimSlowTimer{get;}
        //プレイヤーObject
        IReadOnlyReactiveProperty<MyGameObject> PlayerGameObject{get;}
        //プレイヤーの状態
        IReadOnlyReactiveProperty<GameEnum.PlayerState> PlayerState{get;}
        //引っ張り始めのスクリーン座標
        Vector2 PullingStartScreenPosition{get;}
        //引っ張っている距離
        IReadOnlyReactiveProperty<Vector2> PullingVector{get;}
        //非表示にしてほしい果物オブジェクト
        IReadOnlyReactiveProperty<int> DisableFruits{get;}
        //入手したフルーツ
        IReadOnlyReactiveProperty<FruitsObject.FruitsType> GetFruits{get;}
        //非表示にしてほしい敵オブジェクト
        IReadOnlyReactiveProperty<int> DisableEnemy{get;}
    }
    public partial class GameModel : IGameModel
    {
        //UntilDestroyのTarget用
        private GameObject _gameObject = default;

        //ゲームデータ
        private GameData _gameData = default;
        public GameData GameData => _gameData;
        //プレイヤーデータ
        private PlayerData _playerData = default;
        //ステージデータ
        private StageData _stageData = default;
        public StageData StageData => _stageData;
        //フルーツデータ
        private FruitsData _fruitsData = default;
        public FruitsData FruitsData => _fruitsData;
        //敵データ
        private EnemyData _enemyData = default;
        public EnemyData EnemyData => _enemyData;

        //プレイヤーのジャンプ力（エイムの計算に使用）
        public float PlayerJumpVelocity => _playerData.JumpVelocity;

        //GameState
        private ReactiveProperty<GameEnum.GameState> _gameState = new ReactiveProperty<GameEnum.GameState>(default);
        public IReadOnlyReactiveProperty<GameEnum.GameState> GameState => _gameState;
        //タイムスケール
        private float _timeScale = 1f;
        public float TimeScale => _timeScale;
        //エイムのスローモーションの残り時間
        private FloatReactiveProperty _aimSlowTimer = new FloatReactiveProperty(0f);
        public IReadOnlyReactiveProperty<float> AimSlowTimer => _aimSlowTimer;
        //GameObject
        private ReactiveProperty<MyGameObject> _playerGameObject = new ReactiveProperty<MyGameObject>(default);
        public IReadOnlyReactiveProperty<MyGameObject> PlayerGameObject => _playerGameObject;
        //プレイヤーの状態
        public IReadOnlyReactiveProperty<GameEnum.PlayerState> PlayerState => _playerData.State;
        //引っ張り始めのスクリーン座標
        private Vector2 _pullingStartScreenPosition = Vector2.zero;
        public Vector2 PullingStartScreenPosition => _pullingStartScreenPosition;
        //引っ張っている距離
        private Vector2ReactiveProperty _pullingVector = new Vector2ReactiveProperty(Vector2.zero);
        public IReadOnlyReactiveProperty<Vector2> PullingVector => _pullingVector;
        //非表示にしてほしい果物オブジェクト
        private IntReactiveProperty _disableFruits = new IntReactiveProperty(-1);
        public IReadOnlyReactiveProperty<int> DisableFruits => _disableFruits;
        //入手したフルーツ
        private ReactiveProperty<FruitsObject.FruitsType> _getFruits = new ReactiveProperty<FruitsObject.FruitsType>(FruitsObject.FruitsType.DEFAULT);
        public IReadOnlyReactiveProperty<FruitsObject.FruitsType> GetFruits => _getFruits;
        //非表示にしてほしい敵オブジェクト
        private IntReactiveProperty _disableEnemy = new IntReactiveProperty(-1);
        public IReadOnlyReactiveProperty<int> DisableEnemy => _disableEnemy;

        internal GameModel(GameObject go)
        {
            _gameObject = go;

            //データ初期化
            _gameData = new GameData();
            _playerData = new PlayerData(_gameObject);
            _stageData = new StageData();
            _fruitsData = new FruitsData();
            _enemyData = new EnemyData();

            _playerGameObject.Value = _playerData.PhysicsObject;

            //毎フレームの処理を開始
            Observable.EveryUpdate()
                    .TakeUntilDestroy(_gameObject)
                    .SubscribeWithState(this, (x, t) => 
                    {
                        //Input処理
                        t.InputUpdate();

                        //スローモーションタイマー
                        if(t.AimSlowTimer.Value > 0f)
                        {
                            t._aimSlowTimer.Value -= Time.deltaTime;
                            if(t._aimSlowTimer.Value <= 0f)
                            {
                                t.DisableSlowMode();
                            }
                        }

                        //力学処理
                        t.PhysicsUpdate();
                    });
        }
        //毎フレーム行う力学的処理
        private void PhysicsUpdate()
        {
            float time = _timeScale * Time.deltaTime;
            //座標更新
            _playerData.PhysicsObject.Physics.Update(time);

            //当たり判定
            //プレイヤーと果物
            for(int i = 0; i < FruitsData.FruitsObjects.Count; i++)
            {
                if(_playerData.CheckCollisionWithItem(FruitsData.FruitsObjects[i].ColliderObject.Collider))
                {
                    //果物に触れた時の処理
                    CollideFruits(i);
                }
            }
            //プレイヤーと敵
            for(int i = 0; i < EnemyData.EnemyObjects.Count; i++)
            {
                var state = _playerData.CheckCollisionWithEnemy(EnemyData.EnemyObjects[i].PhysicsObject.Collider);
                if(state != GameEnum.EnemyCollisionState.NOT)
                {
                    if(state == GameEnum.EnemyCollisionState.TREAD)
                    {
                        //踏んだ時
                        TreadEnemy(i);
                    }
                    else
                    {
                        //踏めなかった時
                        HitEnemy();
                    }
                    //同時に踏んだりしないように
                    break;
                }
            }

            //プレイヤーとステージ
            if(_playerData.CheckCollisionWithStage(StageData.StageObjects))
            {
                //地面に着地した時の処理（壁ジャンプした時は呼ばない）
                Grounded();
            }
            
            //通知
            _playerGameObject.SetValueAndForceNotify(_playerData.PhysicsObject);
        }

        //スローモーションの解除
        private void DisableSlowMode()
        {
            _timeScale = 1f;
            _aimSlowTimer.Value = 0f;
        }
        //地面に着地した時の処理（壁ジャンプした時は呼ばない）
        private void Grounded()
        {
            //必要数を満たしていればジュースを作る
            if(false)
            {

            }
            else
            {
                //果物を失う
                GameData.ResetFruits();
                //通知                
                _getFruits.Value = FruitsObject.FruitsType.DEFAULT;
            }
        }
        //果物に触れた時の処理
        private void CollideFruits(int value)
        {
            //果物を入手
            GameData.GetFruits(FruitsData.FruitsObjects[value].Type);
            //入手通知
            _getFruits.SetValueAndForceNotify(FruitsData.FruitsObjects[value].Type);
            //オブジェクトの削除通知。
            _disableFruits.Value = _fruitsData.FruitsObjects[value].Id;
            //データ削除
            _fruitsData.FruitsObjects.RemoveAt(value);
        }
        //敵を踏んだ時の処理
        private void TreadEnemy(int value)
        {
            //オブジェクトの削除通知。
            _disableEnemy.Value = _enemyData.EnemyObjects[value].Id;
            //データ削除
            _enemyData.EnemyObjects.RemoveAt(value);
        }
        //敵に当たってしまった時の処理
        private void HitEnemy()
        {
            //ダメージを受ける
            if(_gameData.Damaged())
            {
                //生きている
            }
            else
            {
                //ゲームオーバー
            }
        }
    }

    #region GameModelFactory

    public class GameModelFactory : BaseModelFactory<IGameModel>
    {
        private IGameModel _instance;
        public GameModelFactory(GameObject go) : base(go)
        {
        }
        public override IGameModel Create()
        {
            _instance ??= new GameModel(_gameObject);
            return _instance;
        }
    }
    #endregion
}

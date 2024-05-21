using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace mecoinpy.Game
{
    public partial interface IGameModel : IModel
    {
        //ステージデータ
        StageData StageData{get;}
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
    }
    public partial class GameModel : IGameModel
    {
        //UntilDestroyのTarget用
        private GameObject _gameObject = default;
        //プレイヤーデータ
        private PlayerData _playerData = default;
        //ステージデータ
        private StageData _stageData = default;
        public StageData StageData => _stageData;
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

        internal GameModel(GameObject go)
        {
            _gameObject = go;

            //データ初期化
            _playerData = new PlayerData(_gameObject);
            _stageData = new StageData();

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
            //プレイヤーとステージ
            if(_playerData.IsGrounded(StageData.StageObjects))
            {

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

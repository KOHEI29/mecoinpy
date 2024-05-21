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

        //GameObject
        IReadOnlyReactiveProperty<MyGameObject> PlayerGameObject{get;}
        //プレイヤーの状態
        IReadOnlyReactiveProperty<GameEnum.PlayerState> PlayerState{get;}
        //引っ張っている方向
        IReadOnlyReactiveProperty<Vector2> PullingDirection{get;}
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

        //時間の割合
        private float _timeScale = 1f;
        //GameObject
        private ReactiveProperty<MyGameObject> _playerGameObject = new ReactiveProperty<MyGameObject>(default);
        public IReadOnlyReactiveProperty<MyGameObject> PlayerGameObject => _playerGameObject;
        //プレイヤーの状態
        public IReadOnlyReactiveProperty<GameEnum.PlayerState> PlayerState => _playerData.State;
        //ボタンダウンの開始位置
        private Vector2 _mouseStartPosition = Vector2.zero;
        //引っ張っている方向
        private Vector2ReactiveProperty _pullingDirection = new Vector2ReactiveProperty(Vector2.zero);
        public IReadOnlyReactiveProperty<Vector2> PullingDirection => _pullingDirection;

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

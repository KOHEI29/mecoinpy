using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace mecoinpy.Game
{
    public interface IGameModel : IModel
    {
        //ステージデータ
        StageData StageData{get;}
        //GameObject
        IReadOnlyReactiveProperty<MyGameObject> PlayerGameObject{get;}
        //プレイヤーの状態
        IReadOnlyReactiveProperty<GameEnum.PlayerState> PlayerState{get;}
        //引っ張っている方向
        IReadOnlyReactiveProperty<Vector2> PullingDirection{get;}

        //ボタン関連の処理。スワイプとタップの判定などはViewModelでやるべき？
        void OnButtonDown(Vector2 mouse);
        void OnButton(Vector2 mouse);
        void OnButtonUp(Vector2 mouse);
    }
    public class GameModel : IGameModel
    {
        //UntilDestroyのTarget用
        private GameObject _gameObject = default;
        //プレイヤーデータ
        private PlayerData _playerData = default;
        //ステージデータ
        private StageData _stageData = default;
        public StageData StageData => _stageData;

        //時間の割合
        private float _timeScale = 1f;
        //重力加速度
        private float _gAcceleration = GameConst.DefaultGravityAcceleration;
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


        //ボタン関連の処理。スワイプとタップの判定などはViewModelでやるべき？
        public void OnButtonDown(Vector2 mouse)
        {
            _mouseStartPosition = mouse;
        }
        public void OnButton(Vector2 mouse)
        {
            if((_mouseStartPosition - mouse).SqrMagnitude() > GameConst.SwipeThresholdSqr)
            {
                _pullingDirection.Value = _mouseStartPosition - mouse;
            }
            else
            {
                _pullingDirection.Value = Vector2.zero;
            }
        }
        public void OnButtonUp(Vector2 mouse)
        {
            if(PullingDirection.Value.SqrMagnitude() > 0f)
            {
                //ジャンプ
                _playerData.TryJump(PullingDirection.Value.normalized);
            }
            else
            {
                //ストンプ
                Debug.Log("Try Stamp");
                _playerData.TryStomp();
            }
            _pullingDirection.Value = Vector2.zero;
            _mouseStartPosition = default;
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

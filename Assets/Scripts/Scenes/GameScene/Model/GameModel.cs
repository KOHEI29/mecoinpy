using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace mecoinpy.Game
{
    public interface IGameModel : IModel
    {
        //ステージデータ
        StageData StageData{get;}
        //Transform
        IReadOnlyReactiveProperty<MyGameObject> PlayerGameObject{get;}
    }
    public class GameModel : IGameModel
    {
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
        //座標
        private ReactiveProperty<MyGameObject> _playerPosition = new ReactiveProperty<MyGameObject>(default);
        public IReadOnlyReactiveProperty<MyGameObject> PlayerGameObject => _playerPosition;

        private Vector2 _pullingDirection = new Vector2();

        internal GameModel(GameObject go)
        {
            _gameObject = go;

            //データ初期化
            _playerData = new PlayerData();
            _stageData = new StageData();

            _playerPosition.Value = _playerData.PhysicsObject;

            //毎フレームの処理を開始
            Observable.EveryUpdate()
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
            _playerPosition.SetValueAndForceNotify(_playerData.PhysicsObject);
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

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
        //座標
        IReadOnlyReactiveProperty<Vector2> PlayerPosition{get;}
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
        private Vector2ReactiveProperty _playerPosition = new Vector2ReactiveProperty(GameConst.Initialize.PlayerPosition);
        public IReadOnlyReactiveProperty<Vector2> PlayerPosition => _playerPosition;

        private Vector2 _pullingDirection = new Vector2();

        internal GameModel(GameObject go)
        {
            _gameObject = go;

            //データ初期化
            _playerData = new PlayerData();
            _stageData = new StageData();

            _playerPosition = new Vector2ReactiveProperty(GameConst.Initialize.PlayerPosition);

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
            _playerData.Physics.Update(time);
            _playerPosition.Value = _playerData.Physics.Position;
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

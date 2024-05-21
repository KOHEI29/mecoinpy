using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace mecoinpy.Game
{
    public class AimViewModel : BaseViewModel
    {
        private IGameModel _model = default;

        //表示・非表示
        private BoolReactiveProperty _display = new BoolReactiveProperty(false);
        public IReadOnlyReactiveProperty<bool> Display => _display;
        //それぞれのパーツの座標とZ回転
        private Vector2[] _partsPositions = new Vector2[GameConst.AimPartsCount];
        public IReadOnlyCollection<Vector2> PartsPositions => _partsPositions;
        private float[] _partsRotation = new float[GameConst.AimPartsCount];
        public IReadOnlyCollection<float> PartsRotation => _partsRotation;
        //パーツのTransformの更新通知
        private Subject<Unit> _partsTransformObservable = new Subject<Unit>();
        public IObservable<Unit> PartsTransformObservable => _partsTransformObservable;

        internal AimViewModel(GameObject view) : base(view)
        {
            //***Debug
            //本来はSceneManagerなどでしっかりしたGameObjectから作る
            _model = ModelProvider.Get<IGameModel>(new GameModelFactory(_view));

            for(int i = 0; i < GameConst.AimPartsCount; i++)
                _partsPositions[i] = Vector2.zero;

            _model.PullingVector
                .TakeUntilDestroy(_view)
                .SubscribeWithState(this, (x, t) => 
                {
                    if(x == Vector2.zero)
                    {
                        t._display.Value = false;
                    }
                    else
                    {
                        t._display.Value = true;
                        //初速
                        var fV = x.normalized * t._model.PlayerJumpVelocity;
                        for(int i = 0; i < GameConst.AimPartsCount; i++)
                        {
                            var temp = fV * GameConst.AimPartsOffset * i;
                            temp.y -= GameConst.DefaultGravityAcceleration * GameConst.AimPartsOffset * i * GameConst.AimPartsOffset * i * 0.5f;
                            t._partsPositions[i] = t._model.PlayerGameObject.Value.Position + temp;
                            //角度の計算
                            //var v = fV;
                            //v.y -= GameConst.DefaultGravityAcceleration * GameConst.AimPartsOffset * i;
                            //if(temp.y == 0f)
                            //    t._partsRotation[i] = 0f;
                            //else
                            //    t._partsRotation[i] = - v.x / v.y * 45f;
                        }

                        t._partsTransformObservable.OnNext(Unit.Default);
                    }
                });
        }
    }

    public class AimViewModelFactory : BaseViewModelFactory<AimViewModel>{
        private AimViewModel _instance;
        public AimViewModelFactory(GameObject go) : base(go)
        {
        }
        public override AimViewModel Create(){
            if(_instance == null){
                _instance = new AimViewModel(_view);
            }
            return _instance;
        }
    }
}

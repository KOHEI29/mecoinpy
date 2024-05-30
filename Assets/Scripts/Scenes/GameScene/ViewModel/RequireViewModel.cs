using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace mecoinpy.Game
{
    public class RequireViewModel : BaseViewModel
    {
        private IGameModel _model = default;

        //課題
        private ReactiveProperty<int[]> _require = new ReactiveProperty<int[]>(default);
        public IReadOnlyReactiveProperty<int[]> Require => _require;
        //制限時間の割合
        private FloatReactiveProperty _timelimitRatio = new FloatReactiveProperty(1f);
        public IReadOnlyReactiveProperty<float> TimelimitRatio => _timelimitRatio;

        internal RequireViewModel(GameObject view) : base(view)
        {
            //***Debug
            //本来はSceneManagerなどでしっかりしたGameObjectから作る
            _model = ModelProvider.Get<IGameModel>(new GameModelFactory(_view));
            //初期化
            _require.SetValueAndForceNotify(_model.GameData.Require.ToArray());

            _model.NextRequire
                .SubscribeWithState(this, (x, t) => 
                {
                    t._require.SetValueAndForceNotify(t._model.GameData.Require.ToArray());
                });
            _model.TimeLimitTimer
                .SubscribeWithState(this, (x, t) => 
                {
                    t._timelimitRatio.Value = x / t._model.TimeLimitMax;
                });
        }
    }

    public class RequireViewModelFactory : BaseViewModelFactory<RequireViewModel>{
        private RequireViewModel _instance;
        public RequireViewModelFactory(GameObject go) : base(go)
        {
        }
        public override RequireViewModel Create(){
            if(_instance == null){
                _instance = new RequireViewModel(_view);
            }
            return _instance;
        }
    }
}

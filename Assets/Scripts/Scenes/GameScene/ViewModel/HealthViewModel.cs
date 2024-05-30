using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace mecoinpy.Game
{
    public class HealthViewModel : BaseViewModel
    {
        private IGameModel _model = default;

        //体力
        public IReadOnlyReactiveProperty<int> Health => _model.Health;
        
        internal HealthViewModel(GameObject view) : base(view)
        {
            //***Debug
            //本来はSceneManagerなどでしっかりしたGameObjectから作る
            _model = ModelProvider.Get<IGameModel>(new GameModelFactory(_view));
        }
    }

    public class HealthViewModelFactory : BaseViewModelFactory<HealthViewModel>{
        private HealthViewModel _instance;
        public HealthViewModelFactory(GameObject go) : base(go)
        {
        }
        public override HealthViewModel Create(){
            if(_instance == null){
                _instance = new HealthViewModel(_view);
            }
            return _instance;
        }
    }
}

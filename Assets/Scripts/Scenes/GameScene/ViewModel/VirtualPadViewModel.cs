using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace mecoinpy.Game
{
    public class VirtualPadViewModel : BaseViewModel
    {
        private IGameModel _model = default;

        internal VirtualPadViewModel(GameObject view) : base(view)
        {
            //***Debug
            //本来はSceneManagerなどでしっかりしたGameObjectから作る
            _model = ModelProvider.Get<IGameModel>(new GameModelFactory(_view));
        }
    }

    public class VirtualPadViewModelFactory : BaseViewModelFactory<VirtualPadViewModel>{
        private VirtualPadViewModel _instance;
        public VirtualPadViewModelFactory(GameObject go) : base(go)
        {
        }
        public override VirtualPadViewModel Create(){
            if(_instance == null){
                _instance = new VirtualPadViewModel(_view);
            }
            return _instance;
        }
    }
}

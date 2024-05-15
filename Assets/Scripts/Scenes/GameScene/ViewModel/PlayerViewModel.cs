using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace mecoinpy.Game
{
    public class PlayerViewModel : BaseViewModel
    {
        private IGameModel _model = default;

        //座標
        public IReadOnlyReactiveProperty<Vector2> PlayerPosition => _model.PlayerPosition;

        internal PlayerViewModel(GameObject view) : base(view)
        {
            //***Debug
            //本来はSceneManagerなどでしっかりしたGameObjectから作る
            _model = ModelProvider.Get<IGameModel>(new GameModelFactory(_view));
        }
    }

    public class PlayerViewModelFactory : BaseViewModelFactory<PlayerViewModel>{
        private PlayerViewModel _instance;
        public PlayerViewModelFactory(GameObject go) : base(go)
        {
        }
        public override PlayerViewModel Create(){
            if(_instance == null){
                _instance = new PlayerViewModel(_view);
            }
            return _instance;
        }
    }
}

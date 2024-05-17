using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace mecoinpy.Game
{
    public class GameDebugViewModel : BaseViewModel
    {
        private IGameModel _model = default;

        //引っ張っている方向
        public IReadOnlyReactiveProperty<Vector2> PullingDirection => _model.PullingDirection;

        internal GameDebugViewModel(GameObject view) : base(view)
        {
            //***Debug
            //本来はSceneManagerなどでしっかりしたGameObjectから作る
            _model = ModelProvider.Get<IGameModel>(new GameModelFactory(_view));

            //_model.GameDebugGameObject
            //    .Where(x => x != default)
            //    .TakeUntilDestroy(_view)
            //    .SubscribeWithState(this, (x, t) =>
            //    {
            //        t._position.Value = x.Position;
            //    });
        }
    }

    public class GameDebugViewModelFactory : BaseViewModelFactory<GameDebugViewModel>{
        private GameDebugViewModel _instance;
        public GameDebugViewModelFactory(GameObject go) : base(go)
        {
        }
        public override GameDebugViewModel Create(){
            if(_instance == null){
                _instance = new GameDebugViewModel(_view);
            }
            return _instance;
        }
    }
}

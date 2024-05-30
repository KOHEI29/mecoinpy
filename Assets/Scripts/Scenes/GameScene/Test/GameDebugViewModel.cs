using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace mecoinpy.Game
{
    public class GameDebugViewModel : BaseViewModel
    {
        private IGameModel _model = default;

        //引っ張っている方向
        public IReadOnlyReactiveProperty<Vector2> PullingDirection => _model.PullingVector;
        //持っている果物
        private ReactiveProperty<int[]> _fruits = new ReactiveProperty<int[]>(default);
        public IReadOnlyReactiveProperty<int[]> Fruits => _fruits;
        //体力
        public IReadOnlyReactiveProperty<int> Health => _model.Health;

        internal GameDebugViewModel(GameObject view) : base(view)
        {
            //***Debug
            //本来はSceneManagerなどでしっかりしたGameObjectから作る
            _model = ModelProvider.Get<IGameModel>(new GameModelFactory(_view));

            _model.GetFruits
                .TakeUntilDestroy(_view)
                .SubscribeWithState(this, (x, t) =>
                {
                    if(x == FruitsObject.FruitsType.DEFAULT)
                    {
                        t._fruits.Value = default;
                    }
                    else
                    {
                        t._fruits.Value = t._model.GameData.Fruits.ToArray();
                    }
                });
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

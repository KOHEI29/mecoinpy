using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace mecoinpy.Game
{
    public class MonsterViewModel : BaseViewModel
    {
        private IGameModel _model = default;

        //座標
        private FloatReactiveProperty _position = new FloatReactiveProperty(0f);
        public IReadOnlyReactiveProperty<float> MonsterPositionY => _position;

        internal MonsterViewModel(GameObject view) : base(view)
        {
            //***Debug
            //本来はSceneManagerなどでしっかりしたGameObjectから作る
            _model = ModelProvider.Get<IGameModel>(new GameModelFactory(_view));

            _model.PlayerGameObject
                .Where(x => x != default)
                .TakeUntilDestroy(_view)
                .SubscribeWithState(this, (x, t) =>
                {
                    t._position.Value = x.Position.y+GameConst.MonsterOffsetY;
                });
        }
    }

    public class MonsterViewModelFactory : BaseViewModelFactory<MonsterViewModel>{
        private MonsterViewModel _instance;
        public MonsterViewModelFactory(GameObject go) : base(go)
        {
        }
        public override MonsterViewModel Create(){
            if(_instance == null){
                _instance = new MonsterViewModel(_view);
            }
            return _instance;
        }
    }
}

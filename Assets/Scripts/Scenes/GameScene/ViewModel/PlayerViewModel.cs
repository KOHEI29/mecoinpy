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
        private Vector2ReactiveProperty _position = new Vector2ReactiveProperty(Vector2.zero);
        public IReadOnlyReactiveProperty<Vector2> PlayerPosition => _position;
        //ジャンプ中の演出のオンオフ
        private BoolReactiveProperty _jumping = new BoolReactiveProperty(false);
        public IReadOnlyReactiveProperty<bool> Jumping => _jumping;

        internal PlayerViewModel(GameObject view) : base(view)
        {
            //***Debug
            //本来はSceneManagerなどでしっかりしたGameObjectから作る
            _model = ModelProvider.Get<IGameModel>(new GameModelFactory(_view));

            _model.PlayerGameObject
                .Where(x => x != default)
                .TakeUntilDestroy(_view)
                .SubscribeWithState(this, (x, t) =>
                {
                    t._position.Value = x.Position;
                });
            _model.PlayerState
                .Where(x => x != GameEnum.PlayerState.DEFAULT)
                .TakeUntilDestroy(_view)
                .SubscribeWithState(this, (x, t) =>
                {
                    _jumping.Value = x == GameEnum.PlayerState.JUMPING;
                });
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

using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace mecoinpy.Game
{
    public class PossessFruitsViewModel : BaseViewModel
    {
        private IGameModel _model = default;

        //表示座標
        private Vector3ReactiveProperty _position = new Vector3ReactiveProperty(Vector3.zero);
        public IReadOnlyReactiveProperty<Vector3> Position => _position;
        //追加
        public IObservable<FruitsObject.FruitsType> AddFruits => _model.GetFruits.Where(x => x != FruitsObject.FruitsType.DEFAULT);
        //失った時
        private Subject<Unit> _lost = new Subject<Unit>();
        public IObservable<Unit> Lost => _lost;
        //ジュースにした時
        private Subject<Unit> _juiced = new Subject<Unit>();
        public IObservable<Unit> Juiced => _juiced;

        internal PossessFruitsViewModel(GameObject view) : base(view)
        {
            //***Debug
            //本来はSceneManagerなどでしっかりしたGameObjectから作る
            _model = ModelProvider.Get<IGameModel>(new GameModelFactory(_view));

            _model.PlayerGameObject
                .Where(x => x != default)
                .TakeUntilDestroy(_view)
                .SubscribeWithState(this, (x, t) =>
                {
                    t._position.Value = new Vector3(x.Position.x, x.Position.y + GameConst.PossessFruitsOffsetWithPlayer, -1f);
                });
            _model.GetFruits
                .Where(x => x == FruitsObject.FruitsType.DEFAULT)
                .TakeUntilDestroy(_view)
                .SubscribeWithState(this, (x, t) => 
                {
                    _lost.OnNext(Unit.Default);
                });
        }
    }

    public class PossessFruitsViewModelFactory : BaseViewModelFactory<PossessFruitsViewModel>{
        private PossessFruitsViewModel _instance;
        public PossessFruitsViewModelFactory(GameObject go) : base(go)
        {
        }
        public override PossessFruitsViewModel Create(){
            if(_instance == null){
                _instance = new PossessFruitsViewModel(_view);
            }
            return _instance;
        }
    }
}

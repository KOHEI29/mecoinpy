using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace mecoinpy.Game
{
    public class FruitsViewModel : BaseViewModel
    {
        private IGameModel _model = default;

        //表示すべきオブジェクト
        private ReactiveCollection<FruitsObject> _FruitsObjects = new ReactiveCollection<FruitsObject>();
        public IReadOnlyCollection<FruitsObject> FruitsObjects => _FruitsObjects;

        internal FruitsViewModel(GameObject view) : base(view)
        {
            //***Debug
            //本来はSceneManagerなどでしっかりしたGameObjectから作る
            _model = ModelProvider.Get<IGameModel>(new GameModelFactory(_view));

            //初期化
            var objects = _model.FruitsData.FruitsObjects;
            for(int i = 0; i < objects.Length; i++)
            {
                //座標などで表示するオブジェクトを間引く
                //if(true)
                _FruitsObjects.Add(objects[i]);
            }
        }
    }

    public class FruitsViewModelFactory : BaseViewModelFactory<FruitsViewModel>{
        private FruitsViewModel _instance;
        public FruitsViewModelFactory(GameObject go) : base(go)
        {
        }
        public override FruitsViewModel Create(){
            if(_instance == null){
                _instance = new FruitsViewModel(_view);
            }
            return _instance;
        }
    }
}

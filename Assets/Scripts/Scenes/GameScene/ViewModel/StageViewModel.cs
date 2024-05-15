using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace mecoinpy.Game
{
    public class StageViewModel : BaseViewModel
    {
        private IGameModel _model = default;

        //表示すべきオブジェクト
        private ReactiveCollection<StageObject> _stageObjects = new ReactiveCollection<StageObject>();
        public IReadOnlyCollection<StageObject> StageObjects => _stageObjects;

        internal StageViewModel(GameObject view) : base(view)
        {
            //***Debug
            //本来はSceneManagerなどでしっかりしたGameObjectから作る
            _model = ModelProvider.Get<IGameModel>(new GameModelFactory(_view));

            //初期化
            var objects = _model.StageData.StageObjects;
            for(int i = 0; i < objects.Length; i++)
            {
                //座標などで表示するオブジェクトを間引く
                //if(true)
                _stageObjects.Add(objects[i]);
            }
        }
    }

    public class StageViewModelFactory : BaseViewModelFactory<StageViewModel>{
        private StageViewModel _instance;
        public StageViewModelFactory(GameObject go) : base(go)
        {
        }
        public override StageViewModel Create(){
            if(_instance == null){
                _instance = new StageViewModel(_view);
            }
            return _instance;
        }
    }
}

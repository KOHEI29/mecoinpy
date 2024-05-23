using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace mecoinpy.Game
{
    public class EnemyViewModel : BaseViewModel
    {
        private IGameModel _model = default;

        //表示すべきオブジェクト
        private ReactiveCollection<EnemyObject> _enemyObjects = new ReactiveCollection<EnemyObject>();
        public IReadOnlyCollection<EnemyObject> EnemyObjects => _enemyObjects;

        //非表示にすべきオブジェクト
        public IReadOnlyReactiveProperty<int> DisableEnemy => _model.DisableEnemy;

        internal EnemyViewModel(GameObject view) : base(view)
        {
            //***Debug
            //本来はSceneManagerなどでしっかりしたGameObjectから作る
            _model = ModelProvider.Get<IGameModel>(new GameModelFactory(_view));

            //初期化
            var objects = _model.EnemyData.EnemyObjects;
            for(int i = 0; i < objects.Count; i++)
            {
                //座標などで表示するオブジェクトを間引く
                //if(true)
                _enemyObjects.Add(objects[i]);
            }
        }
    }

    public class EnemyViewModelFactory : BaseViewModelFactory<EnemyViewModel>{
        private EnemyViewModel _instance;
        public EnemyViewModelFactory(GameObject go) : base(go)
        {
        }
        public override EnemyViewModel Create(){
            if(_instance == null){
                _instance = new EnemyViewModel(_view);
            }
            return _instance;
        }
    }
}

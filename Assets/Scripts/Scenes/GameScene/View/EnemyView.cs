using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using System;

namespace mecoinpy.Game
{
    public class EnemyView : MonoBehaviour
    {
        //Prefab
        [SerializeField]
        private GameObject _enemyObjectPrefab = default;
        //マテリアル
        [SerializeField]
        private Material[] _enemyMaterials = default;

        //作成したオブジェクト。指定したオブジェクトへの操作のためにDictionaryで保管する。
        private Dictionary<int, GameObject> _enemyObjects = new Dictionary<int, GameObject>(4);
        //オブジェクトプール（非表示などされたらここに入れておく。）
        private Queue<GameObject> _enemyObjectPool = new Queue<GameObject>(4);
        // Start is called before the first frame update
        void Start()
        {
            var viewModel = ViewModelProvider.Get<EnemyViewModel>(new EnemyViewModelFactory(gameObject));

            //初期化
            for(int i = 0; i < viewModel.EnemyObjects.Count; i++)
            {
                var data = viewModel.EnemyObjects.ElementAt(i);
                var go = CreateOrDequeue();
                go.transform.position = data.PhysicsObject.Position;
                go.transform.localScale = new Vector3(data.PhysicsObject.Scale.x, data.PhysicsObject.Scale.y, 1f);
                go.GetComponent<Renderer>().material = _enemyMaterials[(int)data.Type];
                _enemyObjects[data.Id] = go;
            }
            
            //購読
            viewModel.DisableEnemy
                .Where(x => x > -1)
                .TakeUntilDestroy(this)
                .SubscribeWithState(this, (x, t) =>
                {
                    //非表示に
                    if(t._enemyObjects.TryGetValue(x, out GameObject go))
                    {
                        go.SetActive(false);
                        t._enemyObjectPool.Enqueue(go);
                        t._enemyObjects.Remove(x);
                    }
                    else
                    {
                        //非表示にしたいオブジェクトが表示されていない状態。
                        throw new KeyNotFoundException();
                    }
                });
        }
        //プールにあればプールから、なければオブジェクトを作成
        private GameObject CreateOrDequeue()
        {
            GameObject go = default;
            if(_enemyObjectPool.TryDequeue(out go))
            {
                return go;
            }
            else
            {
                go = Instantiate(_enemyObjectPrefab);
                //オブジェクト初期化
                go.transform.SetParent(transform);
                go.SetActive(true);
                return go;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using System;

namespace mecoinpy.Game
{
    public class FruitsView : MonoBehaviour
    {
        //Prefab
        [SerializeField]
        private GameObject _fruitsObjectPrefab = default;
        //マテリアル
        [SerializeField]
        private Material[] _fruitsMaterials = default;

        //作成したオブジェクト。指定したオブジェクトへの操作のためにDictionaryで保管する。
        private Dictionary<int, GameObject> _fruitsObjects = new Dictionary<int, GameObject>(4);
        //オブジェクトプール（非表示などされたらここに入れておく。）
        private Queue<GameObject> _fruitsObjectPool = new Queue<GameObject>(4);
        // Start is called before the first frame update
        void Start()
        {
            var viewModel = ViewModelProvider.Get<FruitsViewModel>(new FruitsViewModelFactory(gameObject));

            //初期化
            for(int i = 0; i < viewModel.FruitsObjects.Count; i++)
            {
                var data = viewModel.FruitsObjects.ElementAt(i);
                var go = CreateOrDequeue();
                go.transform.position = data.ColliderObject.Position;
                go.transform.localScale = new Vector3(data.ColliderObject.Scale.x, data.ColliderObject.Scale.y, 1f);
                go.GetComponent<Renderer>().material = _fruitsMaterials[(int)data.Type];
                _fruitsObjects[data.Id] = go;
            }
            
            //購読
            viewModel.DisableFruits
                .Where(x => x > -1)
                .TakeUntilDestroy(this)
                .SubscribeWithState(this, (x, t) =>
                {
                    //非表示に
                    if(t._fruitsObjects.TryGetValue(x, out GameObject go))
                    {
                        go.SetActive(false);
                        t._fruitsObjectPool.Enqueue(go);
                        t._fruitsObjects.Remove(x);
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
            if(_fruitsObjectPool.TryDequeue(out go))
            {
                return go;
            }
            else
            {
                go = Instantiate(_fruitsObjectPrefab);
                //オブジェクト初期化
                go.transform.SetParent(transform);
                go.SetActive(true);
                return go;
            }
        }
    }
}

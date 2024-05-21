using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace mecoinpy.Game
{
    public class FruitsView : MonoBehaviour
    {
        //Prefab
        [SerializeField]
        private GameObject _FruitsObjectPrefab = default;
        //作成したオブジェクト
        private List<GameObject> _FruitsObjects = new List<GameObject>(4);
        //オブジェクトプール（非表示などされたらここに入れておく。）
        private Queue<GameObject> _FruitsObjectPool = new Queue<GameObject>(4);
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
            }

            //更新（追加でオブジェクトを作る）
        }
        //プールにあればプールから、なければオブジェクトを作成
        private GameObject CreateOrDequeue()
        {
            GameObject go = default;
            if(_FruitsObjectPool.TryDequeue(out go))
            {
                return go;
            }
            else
            {
                go = Instantiate(_FruitsObjectPrefab);
                //オブジェクト初期化
                go.transform.SetParent(transform);
                go.SetActive(true);
                return go;
            }
        }
    }
}

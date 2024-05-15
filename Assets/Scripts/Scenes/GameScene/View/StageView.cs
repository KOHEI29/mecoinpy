using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace mecoinpy.Game
{
    public class StageView : MonoBehaviour
    {
        //Prefab
        [SerializeField]
        private GameObject _stageObjectPrefab = default;
        //作成したオブジェクト
        private List<GameObject> _stageObjects = new List<GameObject>(4);
        //オブジェクトプール（非表示などされたらここに入れておく。）
        private Queue<GameObject> _stageObjectPool = new Queue<GameObject>(4);
        // Start is called before the first frame update
        void Start()
        {
            var viewModel = ViewModelProvider.Get<StageViewModel>(new StageViewModelFactory(gameObject));

            //初期化
            for(int i = 0; i < viewModel.StageObjects.Count; i++)
            {
                var data = viewModel.StageObjects.ElementAt(i);
                var go = CreateOrDequeue();
                go.transform.position = data.Physics.Position;
                go.transform.localScale = new Vector3(data.Scale.x, data.Scale.y, 1f);
            }

            //更新（追加でオブジェクトを作る）
        }
        //プールにあればプールから、なければオブジェクトを作成
        private GameObject CreateOrDequeue()
        {
            GameObject go = default;
            if(_stageObjectPool.TryDequeue(out go))
            {
                return go;
            }
            else
            {
                go = Instantiate(_stageObjectPrefab);
                //オブジェクト初期化
                go.transform.SetParent(transform);
                go.SetActive(true);
                return go;
            }
        }
    }
}

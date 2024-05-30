using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace mecoinpy.Game
{
    //入力処理
    public class HealthView : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] _objects = default;

        // Start is called before the first frame update
        void Start()
        {
            var viewModel = ViewModelProvider.Get<HealthViewModel>(new HealthViewModelFactory(gameObject));

            viewModel.Health
                .TakeUntilDestroy(this)
                .SubscribeWithState(this, (x, t) => 
                {
                    for(int i = 0; i < _objects.Length; i++)
                    {
                        _objects[i].SetActive(x > i);
                    }
                });
        }
    }
}

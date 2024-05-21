using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace mecoinpy.Game
{
    //入力処理
    public class VirtualPadView : MonoBehaviour
    {
        private VirtualPadViewModel _viewModel = default;

        // Start is called before the first frame update
        void Start()
        {
           _viewModel = ViewModelProvider.Get<VirtualPadViewModel>(new VirtualPadViewModelFactory(gameObject));
        }
    }
}

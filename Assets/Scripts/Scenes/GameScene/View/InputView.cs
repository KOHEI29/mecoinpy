using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace mecoinpy.Game
{
    //入力処理
    public class InputView : MonoBehaviour
    {
        private InputViewModel _viewModel = default;

        // Start is called before the first frame update
        void Start()
        {
           _viewModel = ViewModelProvider.Get<InputViewModel>(new InputViewModelFactory(gameObject));
           Observable.EveryUpdate()
                    .TakeUntilDestroy(this)
                    .SubscribeWithState(this, (_, t) =>
                    {
                        t.EveryUpdate();
                    });
        }
        private void EveryUpdate()
        {
            if(Input.GetMouseButtonDown(0))
            {
                //押し始めた
                _viewModel.OnButtonDown(Input.mousePosition);
            }
            if(Input.GetMouseButton(0))
            {
                //押している
                _viewModel.OnButton(Input.mousePosition);
            }
            if(Input.GetMouseButtonUp(0))
            {
                //離した
                _viewModel.OnButtonUp(Input.mousePosition);
            }
        }
    }
}

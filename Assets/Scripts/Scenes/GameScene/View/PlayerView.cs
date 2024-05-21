using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace mecoinpy.Game
{
    public class PlayerView : MonoBehaviour
    {
        private IDisposable _rotationDisposable = default;
        private PlayerViewModel _viewModel = default;
        
        // Start is called before the first frame update
        void Start()
        {
            _viewModel = ViewModelProvider.Get<PlayerViewModel>(new PlayerViewModelFactory(gameObject));

            //座標の書き換え
            _viewModel.PlayerPosition
                .Where(x => x != default)
                .TakeUntilDestroy(this)
                .SubscribeWithState(this, (x, t) => 
                {
                    t.transform.position = x;
                });
            
            //ジャンプ中はメッシュを回転させる
            _viewModel.Jumping
                .TakeUntilDestroy(this)
                .SubscribeWithState(this, (x, t) => 
                {
                    if(x)
                    {
                        t._rotationDisposable = Observable.EveryUpdate()
                                .TakeUntilDestroy(t)
                                .SubscribeWithState(t, (x, tt) => 
                                {
                                    tt.transform.Rotate(0f, 0f, 2f * tt._viewModel.TimeScale);
                                });
                    }
                    else
                    {
                        t._rotationDisposable?.Dispose();
                        t._rotationDisposable = default;
                        t.transform.rotation = Quaternion.identity;
                    }
                });
        }
    }
}

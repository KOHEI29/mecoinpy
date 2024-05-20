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
        
        // Start is called before the first frame update
        void Start()
        {
            var viewModel = ViewModelProvider.Get<PlayerViewModel>(new PlayerViewModelFactory(gameObject));

            //座標の書き換え
            viewModel.PlayerPosition
                .Where(x => x != default)
                .TakeUntilDestroy(this)
                .SubscribeWithState(this, (x, t) => 
                {
                    t.transform.position = x;
                });
            
            //ジャンプ中はメッシュを回転させる
            viewModel.Jumping
                .TakeUntilDestroy(this)
                .SubscribeWithState(this, (x, t) => 
                {
                    if(x)
                    {
                        _rotationDisposable = Observable.EveryUpdate()
                                .TakeUntilDestroy(this)
                                .SubscribeWithState(this, (x, t) => 
                                {
                                    t.transform.Rotate(0f, 0f, 2f);
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

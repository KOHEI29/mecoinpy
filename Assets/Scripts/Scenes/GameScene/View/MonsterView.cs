using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace mecoinpy.Game
{
    public class MonsterView : MonoBehaviour
    {
        private MonsterViewModel _viewModel = default;
        
        // Start is called before the first frame update
        void Start()
        {
            _viewModel = ViewModelProvider.Get<MonsterViewModel>(new MonsterViewModelFactory(gameObject));

            //座標の書き換え
            _viewModel.MonsterPositionY
                .Where(x => x != default)
                .TakeUntilDestroy(this)
                .SubscribeWithState(this, (x, t) => 
                {
                    t.transform.DOMoveY(x, 0.1f)
                                .SetLink(gameObject);
                });
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using TMPro;

namespace mecoinpy.Game
{
    public class GameDebugView : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _text = default;
        [SerializeField]
        private TextMeshProUGUI[] _fruitCount = default;
        [SerializeField]
        private TextMeshProUGUI _health = default;
        [SerializeField]
        private TextMeshProUGUI[] _reqCount = default;
        // Start is called before the first frame update
        void Start()
        {
           var viewModel = ViewModelProvider.Get<GameDebugViewModel>(new GameDebugViewModelFactory(gameObject));

           viewModel.PullingDirection
                .Where(x => x != default)
                .TakeUntilDestroy(this)
                .SubscribeWithState(this, (x, t) => 
                {
                    t._text.SetText("PULL.X:{0}\n\nPULL.Y:{1}", x.x, x.y);
                });
            viewModel.Fruits
                .TakeUntilDestroy(this)
                .SubscribeWithState(this, (x, t) => 
                {
                    if(x == default)
                    {
                        for(int i = 0; i < _fruitCount.Length; i++)
                            t._fruitCount[i].SetText("0");
                    }
                    else
                    {
                        for(int i = 0; i < x.Length; i++)
                            t._fruitCount[i].SetText("{0}", x[i]);
                    }
                });
            viewModel.Health
                .TakeUntilDestroy(this)
                .SubscribeWithState(this, (x, t) => 
                {
                    t._health.SetText("HP:{0}", x);
                });
            viewModel.Require
                .TakeUntilDestroy(this)
                .SubscribeWithState(this, (x, t) =>
                {
                    for(int i = 0; i < x.Length; i++)
                        t._reqCount[i].SetText("{0}", x[i]);
                });
        }
    }
}

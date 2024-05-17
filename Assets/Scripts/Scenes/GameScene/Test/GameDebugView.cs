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
        }
    }
}

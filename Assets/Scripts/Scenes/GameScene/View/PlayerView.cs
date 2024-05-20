using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace mecoinpy.Game
{
    public class PlayerView : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            var viewModel = ViewModelProvider.Get<PlayerViewModel>(new PlayerViewModelFactory(gameObject));

            viewModel.PlayerPosition
                .Where(x => x != default)
                .TakeUntilDestroy(this)
                .SubscribeWithState(this, (x, t) => 
                {
                    t.transform.position = x;
                });
        }
    }
}

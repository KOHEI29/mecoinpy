using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace mecoinpy.Game
{
    public class VirtualPadViewModel : BaseViewModel
    {
        private IGameModel _model = default;

        //リングを表示すべきスクリーン座標
        public Vector2 RingScreenPosition => _model.PullingStartScreenPosition;
        //ボールを表示すべきスクリーン座標
        private Vector2ReactiveProperty _ballCanvasPosition = new Vector2ReactiveProperty(Vector2.zero);
        public IReadOnlyReactiveProperty<Vector2> BallScreenPosition => _ballCanvasPosition;
        //表示するか否か
        private BoolReactiveProperty _display = new BoolReactiveProperty(false);
        public IReadOnlyReactiveProperty<bool> Display => _display;

        internal VirtualPadViewModel(GameObject view) : base(view)
        {
            //***Debug
            //本来はSceneManagerなどでしっかりしたGameObjectから作る
            _model = ModelProvider.Get<IGameModel>(new GameModelFactory(_view));

            _model.GameState
                .TakeUntilDestroy(_view)
                .SubscribeWithState(this, (x, t) => 
                {
                    t._display.Value = x == GameEnum.GameState.AIMING;                    
                });
            _model.PullingVector
                .TakeUntilDestroy(_view)
                .SubscribeWithState(this, (x, t) => 
                {
                    if(t.Display.Value)
                    {
                        t._ballCanvasPosition.Value = t.RingScreenPosition - x;
                    }
                });
        }
    }

    public class VirtualPadViewModelFactory : BaseViewModelFactory<VirtualPadViewModel>{
        private VirtualPadViewModel _instance;
        public VirtualPadViewModelFactory(GameObject go) : base(go)
        {
        }
        public override VirtualPadViewModel Create(){
            if(_instance == null){
                _instance = new VirtualPadViewModel(_view);
            }
            return _instance;
        }
    }
}

using System;
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
        //リングのスケーリング。スローモーションの制限時間を表す
        private Vector2ReactiveProperty _ringScaling = new Vector2ReactiveProperty(Vector2.one);
        public IReadOnlyReactiveProperty<Vector2> RingScaling => _ringScaling;
        //ボールを表示すべきスクリーン座標
        private Vector2ReactiveProperty _ballScreenPosition = new Vector2ReactiveProperty(Vector2.zero);
        public IReadOnlyReactiveProperty<Vector2> BallScreenPosition => _ballScreenPosition;
        //ボールのスケーリングと回転。座標と同じタイミングで更新するので、通知は不要。
        private Vector2 _ballScale = Vector2.one;
        public Vector2 BallScale => _ballScale;
        private Vector2 _ballRotation = Vector2.zero;
        public Vector2 BallRotation => _ballRotation;
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
            _model.AimSlowTimer
                .TakeUntilDestroy(_view)
                .SubscribeWithState(this, (x ,t) => 
                {
                    if(x > 1f)
                        t._ringScaling.Value = Vector2.one;
                    else
                        t._ringScaling.Value = new Vector2(x, x);
                });
            _model.PullingVector
                .TakeUntilDestroy(_view)
                .SubscribeWithState(this, (x, t) => 
                {
                    if(t.Display.Value)
                    {
                        ////通知する前にボールのスケーリングと回転を計算する。
                        //var prev = t.BallScreenPosition.Value;
                        var now = t.RingScreenPosition - x;
                        //var offset = now - prev;
                        //Debug.Log($"X is {offset.x}");
                        //Debug.Log($"Y is {offset.y}");
                        //if(offset.x == 0)
                        //    t._ballScale = new Vector2(0.5f, 2f);
                        //if(offset.y == 0)
                        //    t._ballScale = new Vector2(2f, 0.5f);
                        //else
                        //    t._ballScale = new Vector2(offset.x / offset.y, offset.y / offset.x);

                        t._ballScreenPosition.Value = now;
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

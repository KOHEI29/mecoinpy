using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace mecoinpy.Game
{
    public class RequireViewModel : BaseViewModel
    {
        private IGameModel _model = default;

        //課題
        private ReactiveProperty<int[]> _require = new ReactiveProperty<int[]>(default);
        public IReadOnlyReactiveProperty<int[]> Require => _require;
        //達成した課題のオーダー（画像を灰色に変えるため
        public IReadOnlyReactiveProperty<int> ChangeColorOrder => _model.ClearRequireOrder;
        //制限時間の割合
        private FloatReactiveProperty _timelimitRatio = new FloatReactiveProperty(1f);
        public IReadOnlyReactiveProperty<float> TimelimitRatio => _timelimitRatio;
        //吹き出しのフィルの色
        private ColorReactiveProperty _balloonFillColor = new ColorReactiveProperty(GameConst.RequireBalloonColorStill);
        public IReadOnlyReactiveProperty<Color> BalloonFillColor => _balloonFillColor;
        //吹き出しの右下のテキスト
        private StringReactiveProperty _balloonText = new StringReactiveProperty("");
        public IReadOnlyReactiveProperty<string> BalloonText => _balloonText;

        internal RequireViewModel(GameObject view) : base(view)
        {
            //***Debug
            //本来はSceneManagerなどでしっかりしたGameObjectから作る
            _model = ModelProvider.Get<IGameModel>(new GameModelFactory(_view));
            //初期化
            _require.SetValueAndForceNotify(_model.GameData.Require.ToArray());

            _model.NextRequire
                .TakeUntilDestroy(_view)
                .SubscribeWithState(this, (x, t) => 
                {
                    t._require.SetValueAndForceNotify(t._model.GameData.Require.ToArray());
                });
            _model.TimeLimitTimer
                .TakeUntilDestroy(_view)
                .SubscribeWithState(this, (x, t) => 
                {
                    t._timelimitRatio.Value = 1f - (x / (t._model.TimeLimitMax * GameConst.RequireBalloonVisibleRatio));
                });
            _model.RequireState
                .TakeUntilDestroy(_view)
                .SubscribeWithState(this, (x, t) =>
                {
                    if(x == GameEnum.RequireState.STILL)
                    {
                        t._balloonFillColor.Value = GameConst.RequireBalloonColorStill;
                        t._balloonText.Value = "";
                    }
                    else
                    {
                        t._balloonFillColor.Value = GameConst.RequireBalloonColorReady;
                        if(x == GameEnum.RequireState.READY)
                        {
                            t._balloonText.Value = GameConst.RequireBalloonTextReady;
                        }
                        else
                        {
                            t._balloonText.Value = GameConst.RequireBalloonTextBonus;
                        }
                    }
                });
        }
    }

    public class RequireViewModelFactory : BaseViewModelFactory<RequireViewModel>{
        private RequireViewModel _instance;
        public RequireViewModelFactory(GameObject go) : base(go)
        {
        }
        public override RequireViewModel Create(){
            if(_instance == null){
                _instance = new RequireViewModel(_view);
            }
            return _instance;
        }
    }
}

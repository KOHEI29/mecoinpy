using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace mecoinpy.Game
{
    public class CameraViewModel : BaseViewModel
    {
        private IGameModel _model = default;

        //座標
        private FloatReactiveProperty _position = new FloatReactiveProperty(0f);
        public IReadOnlyReactiveProperty<float> CameraPositionY => _position;

        internal CameraViewModel(GameObject view) : base(view)
        {
            //***Debug
            //本来はSceneManagerなどでしっかりしたGameObjectから作る
            _model = ModelProvider.Get<IGameModel>(new GameModelFactory(_view));

            _model.PlayerGameObject
                .Where(x => x != default)
                .TakeUntilDestroy(_view)
                .SubscribeWithState(this, (x, t) =>
                {
                    t._position.Value = Mathf.Max(GameConst.CameraMinY, x.Position.y+GameConst.CameraOffsetY);
                });
        }
    }

    public class CameraViewModelFactory : BaseViewModelFactory<CameraViewModel>{
        private CameraViewModel _instance;
        public CameraViewModelFactory(GameObject go) : base(go)
        {
        }
        public override CameraViewModel Create(){
            if(_instance == null){
                _instance = new CameraViewModel(_view);
            }
            return _instance;
        }
    }
}

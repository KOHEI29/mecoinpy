using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace mecoinpy.Game
{
    public class InputViewModel : BaseViewModel
    {
        private IGameModel _model = default;

        internal InputViewModel(GameObject view) : base(view)
        {
            //***Debug
            //本来はSceneManagerなどでしっかりしたGameObjectから作る
            _model = ModelProvider.Get<IGameModel>(new GameModelFactory(_view));
        }
        
        //押し始めた時
        public void OnButtonDown(Vector2 mouse)
        {
            _model.OnButtonDown(mouse);
        }
        //押している時
        public void OnButton(Vector2 mouse)
        {
            _model.OnButton(mouse);
        }
        //離した時
        public void OnButtonUp(Vector2 mouse)
        {
            _model.OnButtonUp(mouse);
        }
    }

    public class InputViewModelFactory : BaseViewModelFactory<InputViewModel>{
        private InputViewModel _instance;
        public InputViewModelFactory(GameObject go) : base(go)
        {
        }
        public override InputViewModel Create(){
            if(_instance == null){
                _instance = new InputViewModel(_view);
            }
            return _instance;
        }
    }
}

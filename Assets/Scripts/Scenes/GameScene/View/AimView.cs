using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UniRx;
using UnityEngine;

namespace mecoinpy.Game
{
    public class AimView : MonoBehaviour
    {
        private GameObject _wrapper = default;
        [SerializeField]
        private GameObject _aimPartsPrefab = default;
        private Transform[] _aimParts = new Transform[GameConst.AimPartsCount];
        private IDisposable _animationDisposable = default;
        
        // Start is called before the first frame update
        void Start()
        {
            _wrapper = transform.GetChild(0).gameObject;
            var viewModel = ViewModelProvider.Get<AimViewModel>(new AimViewModelFactory(gameObject));

            for(int i = 0; i < GameConst.AimPartsCount; i++)
            {
                _aimParts[i] = Instantiate(_aimPartsPrefab).transform;
                _aimParts[i].SetParent(_wrapper.transform);
            }

            viewModel.Display.TakeUntilDestroy(this)
                .SubscribeWithState(this, (x, t) => 
                {
                    t._wrapper.SetActive(x);
                });
            viewModel.PartsTransformObservable
                .TakeUntilDestroy(this)
                .SubscribeWithState2(this, viewModel, (_ ,t, vm) =>
                {
                    for(int i = 0; i < GameConst.AimPartsCount; i++)
                    {
                        t._aimParts[i].SetPositionAndRotation
                        (
                            vm.PartsPositions.ElementAt(i),
                            Quaternion.Euler(0f, 0f, vm.PartsRotation.ElementAt(i))
                        );
                    }
                });
        }
    }
}

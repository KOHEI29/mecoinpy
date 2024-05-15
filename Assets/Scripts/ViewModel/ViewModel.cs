using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mecoinpy
{
    public interface IViewModel
    {
    }
    public abstract class BaseViewModel : IViewModel
    {
        protected GameObject _view = default;
        protected BaseViewModel(GameObject go)
        {
            _view = go;
        }
    }

    public abstract class BaseViewModelFactory<T> where T : BaseViewModel
    {
        protected static GameObject _view = default;
        protected BaseViewModelFactory(GameObject go)
        {
            _view = go;
        }
        public abstract T Create();
    }
}
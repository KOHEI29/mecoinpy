using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mecoinpy
{
    public interface IModel
    {
    }

    public abstract class BaseModelFactory<T> where T: IModel
    {
        protected static GameObject _gameObject = default;
        protected BaseModelFactory(GameObject go)
        {
            _gameObject = go;
        }
        public abstract T Create();
    }
}
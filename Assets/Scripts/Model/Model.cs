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
        public abstract T Create();
    }
}
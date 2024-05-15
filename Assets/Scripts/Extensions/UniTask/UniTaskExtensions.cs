using System;
using UnityEngine;
using UnityEngine.Events;


namespace Cysharp.Threading.Tasks
{
    public partial struct UniTask
    {
        public async UniTask WithHandlingErrorAsync(UnityAction action = default) {
            try {
                await this;
            } catch(Exception e) {
                Debug.Log(e.Message);
                action?.Invoke();
            }
        }
        public void WithHandlingError(UnityAction action = default) {
            WithHandlingErrorAsync(action).Forget();
        }
    }
}
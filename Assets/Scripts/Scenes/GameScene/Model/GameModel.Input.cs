using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

namespace mecoinpy.Game
{
    public partial class GameModel : IGameModel
    {
        //毎フレーム行うInput処理
        private void InputUpdate()
        {
            if(Input.GetMouseButtonDown(0))
            {
                //押し始めた
                OnButtonDown(Input.mousePosition);
            }
            if(Input.GetMouseButton(0))
            {
                //押している
                OnButton(Input.mousePosition);
            }
            if(Input.GetMouseButtonUp(0))
            {
                //離した
                OnButtonUp(Input.mousePosition);
            }
        }

        //ボタン関連の処理。
        private void OnButtonDown(Vector2 mouse)
        {
            _mouseStartPosition = mouse;
        }
        private void OnButton(Vector2 mouse)
        {
            if((_mouseStartPosition - mouse).SqrMagnitude() > GameConst.SwipeThresholdSqr)
            {
                _pullingDirection.Value = (_mouseStartPosition - mouse).normalized;
            }
            else
            {
                _pullingDirection.Value = Vector2.zero;
            }
        }
        private void OnButtonUp(Vector2 mouse)
        {
            if(PullingDirection.Value.SqrMagnitude() > 0f)
            {
                //ジャンプ
                _playerData.TryJump(PullingDirection.Value);
            }
            else
            {
                //ストンプ
                Debug.Log("Try Stamp");
                _playerData.TryStomp();
            }
            _pullingDirection.Value = Vector2.zero;
            _mouseStartPosition = default;
        }
    }
}

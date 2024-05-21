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
            _pullingStartScreenPosition = mouse;

            //若干長押しorスワイプ判定な気もするが、一旦即時切り替え
            _gameState.Value = GameEnum.GameState.AIMING;
            _timeScale = GameConst.SlowTimeScale;
            _aimSlowTimer.Value = _playerData.AimSeconds;
        }
        private void OnButton(Vector2 mouse)
        {
            if((_pullingStartScreenPosition - mouse).SqrMagnitude() > GameConst.SwipeThresholdSqr)
            {
                _pullingVector.Value = _pullingStartScreenPosition - mouse;
            }
            else
            {
                _pullingVector.Value = Vector2.zero;
            }
        }
        private void OnButtonUp(Vector2 mouse)
        {
            _gameState.Value = GameEnum.GameState.NORMAL;
            //スローモーションの解除
            DisableSlowMode();

            if(PullingVector.Value != Vector2.zero)
            {
                //ジャンプ
                _playerData.TryJump(PullingVector.Value.normalized);
            }
            else
            {
                //ストンプ
                _playerData.TryStomp();
            }
            _pullingVector.Value = Vector2.zero;
            _pullingStartScreenPosition = default;
        }
    }
}

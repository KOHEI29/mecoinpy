using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mecoinpy.Game
{
    public class GameEnum
    {
        public enum GameState
        {
            DEFAULT = -1,
            NORMAL = 0,     //通常
            AIMING,         //狙い中
            PAUSE,          //一時停止中
            GAMEOVER,       //ゲームオーバー
        }
        public enum PlayerState
        {
            DEFAULT = -1,
            IDLE = 0,       //特に何もしていない
            JUMPSTANDBY,    //ジャンプ構え
            JUMPING,        //ジャンプ中
            STOMPSTANDBY,   //ストンプ構え
            STOMPING,       //ストンプ中
        }
    }
}

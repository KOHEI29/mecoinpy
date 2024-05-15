using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using mecoinpy.Game;

//シーン開始時に必ず呼ぶべき処理を書くところ
//デバグ用になるかもしれないが、見失わないようにまとめておく。
namespace mecoinpy
{
    public class RuntimeInitializer
    {

        private static bool called = false;
        //最初のシーンのロードの前に1度だけ呼ばれる
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            //シーンの切り替えの度に呼ばれてしまうのでフラグで管理
            if(called) return;
            called = true;

            //var task = InitializeLoad(onDestroyToken);
        }

        //データのロードなど
        //private static async UniTask InitializeLoad(CancellationToken token)
        //{
        //    
        //}
    }
}
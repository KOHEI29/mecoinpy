using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace mecoinpy.Game
{
    //入力処理
    public class VirtualPadView : MonoBehaviour
    {
        private VirtualPadViewModel _viewModel = default;

        //スクリーン座標からキャンバス座標への変換に必要。
        [SerializeField]
        private Canvas canvas;
        [SerializeField]
        private RectTransform canvasRectTransform;

        //リング
        [SerializeField]
        private RectTransform _ringTransform = default;
        //中のボール
        [SerializeField]
        private RectTransform _ballTransform = default;

        // Start is called before the first frame update
        void Start()
        {
            _viewModel = ViewModelProvider.Get<VirtualPadViewModel>(new VirtualPadViewModelFactory(gameObject));

            _viewModel.Display
                .TakeUntilDestroy(this)
                .SubscribeWithState(this, (x, t) => 
                {
                    if(x)
                    {
                        //スクリーン座標からキャンバス座標に変換。厳格化するならViewModelに持っていく
                        if(RectTransformUtility.ScreenPointToLocalPointInRectangle(t.canvasRectTransform, t._viewModel.RingScreenPosition, t.canvas.worldCamera, out Vector2 cpos))
                        {
                            t._ringTransform.gameObject.SetActive(true);
                            t._ringTransform.anchoredPosition = cpos;
                            t._ballTransform.gameObject.SetActive(true);
                            t._ballTransform.anchoredPosition = cpos;
                        }
                    }
                    else
                    {
                        t._ringTransform.gameObject.SetActive(false);
                        t._ballTransform.gameObject.SetActive(false);
                    }
                });
            _viewModel.BallScreenPosition
                .Skip(1)
                .TakeUntilDestroy(this)
                .SubscribeWithState(this, (x, t) => 
                {
                    //スクリーン座標からキャンバス座標に変換。厳格化するならViewModelに持っていく
                    if(RectTransformUtility.ScreenPointToLocalPointInRectangle(t.canvasRectTransform, x, t.canvas.worldCamera, out Vector2 cpos))
                    {
                        t._ballTransform.localScale = t._viewModel.BallScale;
                        t._ballTransform.anchoredPosition = cpos;
                    }
                });
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening;
using TMPro;
using System;
using Cysharp.Threading.Tasks;
using Cysharp.Threading;
using DG.Tweening.Core;

namespace mecoinpy
{
    //ボーナスの数字
    public class RequireViewBonusNumber : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _text = default;
        private RectTransform _rt = default;

        //揺らしているSequence
        private Sequence _shakeSequence = default;
        //ウィンクのSequence
        private Sequence _winkSequence = default;

        //表示
        public void Display(int value)
        {
            gameObject.SetActive(true);
            _rt ??= GetComponent<RectTransform>();
            _text.SetText("{0}", value);

            //transform初期化
            _rt.localRotation = Quaternion.Euler(0f, 0f, -15f);
            _rt.localScale = Vector3.one;

            _shakeSequence = DOTween.Sequence();
            _shakeSequence.Append(_rt.DOLocalRotate(new Vector3(0f, 0f, 30f), 1f)
                .SetEase(Ease.InOutCirc));
            _shakeSequence.Append(_rt.DOLocalRotate(new Vector3(0f, 0f, -30f), 1f)
                .SetEase(Ease.InOutCirc));
            _shakeSequence.SetLoops(-1,LoopType.Yoyo);
            _shakeSequence.SetLink(gameObject);
        }
        //数字を増やす
        public void ChangeNumber(int value)
        {
            _rt ??= GetComponent<RectTransform>();
            //ウインクさせる
            _winkSequence = DOTween.Sequence();
            _winkSequence.Append(_rt.DOScaleY(0f, 0.08f)
                .SetEase(Ease.InQuart));
            _winkSequence.AppendCallback(() => {_text.SetText("{0}", value);});
            _winkSequence.Append(_rt.DOScaleY(1f, 0.08f)
                .SetEase(Ease.OutQuart));
            _winkSequence.SetLink(gameObject);
        }
        //非表示
        public void Disable()
        {
            _shakeSequence?.Kill();
            _winkSequence?.Kill();

            gameObject.SetActive(false);
        }
    }
}

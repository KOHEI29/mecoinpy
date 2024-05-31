using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

namespace mecoinpy
{
    //吹き出し
    public class RequireViewBalloon : MonoBehaviour
    {
        private RectTransform _rt = default;
        private Image _image = default;

        //点滅のSequence
        private Sequence _redSequence = default;
        //スケーリングのTween
        private Sequence _scaleSequence = default;

        //ギリギリモードかどうか
        private bool _runningOut = false;

        //時間ギリギリモードにする
        public void RunningOut()
        {
            if(_runningOut) return;
            _runningOut = true;
            _rt ??= GetComponent<RectTransform>();
            _image ??= GetComponent<Image>();
            //点滅シークエンスを開始
            _redSequence = DOTween.Sequence();
            _redSequence.Append(_image.material.DOColor(Color.red, 0f));
            _redSequence.AppendInterval(0.12f);
            _redSequence.Append(_image.material.DOColor(Color.white, 0f));
            _redSequence.AppendInterval(0.12f);
            _redSequence.Append(_image.material.DOColor(Color.red, 0f));
            _redSequence.AppendInterval(0.12f);
            _redSequence.Append(_image.material.DOColor(Color.white, 0f));
            _redSequence.AppendInterval(1f);
            _redSequence.SetLoops(-1);
            _redSequence.SetLink(gameObject);
            //大きくする
            _scaleSequence = DOTween.Sequence();
            _scaleSequence.Append(_rt.DOScale(Vector3.one*1.2f, 1f)
                .SetEase(Ease.OutCirc));
            _scaleSequence.Join(_rt.DOShakePosition(2f, new Vector3(10f, 0f, 0f), 10, 90, false, true)
                .SetEase(Ease.InCirc));
            _scaleSequence.SetLink(gameObject);
        }
        //通常モードに戻す
        public void Normal()
        {
            if(!_runningOut) return;
            _runningOut = false;
            _rt ??= GetComponent<RectTransform>();

            _redSequence?.Kill();
            _scaleSequence?.Kill();

            //色を元に戻す
            _image.material.color = Color.white;
            //サイズをもとに戻す
            _scaleSequence = DOTween.Sequence();
            _scaleSequence.Append(_rt.DOScale(Vector3.one, 0.2f)
                .SetEase(Ease.InCirc))
                .SetLink(gameObject);
        }
    }
}

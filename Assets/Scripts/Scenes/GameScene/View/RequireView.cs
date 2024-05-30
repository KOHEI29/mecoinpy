using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace mecoinpy.Game
{
    public class RequireView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _fruitsParent = default;
        //Prefab
        [SerializeField]
        private Image _fruitsPrefab = default;
        //果物ごとの色。本来ならspriteなどを持っておくことになる
        [SerializeField]
        private Color[] _fruitsColors = default;
        //表示中のオブジェクト
        private List<Image> _fruitsObjects = new List<Image>(8);
        //プール
        private Queue<Image> _fruitsPool = new Queue<Image>();

        //制限時間用の吹き出しのフィル
        [SerializeField]
        private Image _fillBalloon = default;
        //吹き出しの右下に出るテキスト
        [SerializeField]
        private TextMeshProUGUI _balloonText = default;

        //表示状況
        private Dictionary<FruitsObject.FruitsType, int> _fruitsDictionary = new Dictionary<FruitsObject.FruitsType, int>();
        
        // Start is called before the first frame update
        void Start()
        {
            var viewModel = ViewModelProvider.Get<RequireViewModel>(new RequireViewModelFactory(gameObject));

            viewModel.Require
                .Where(x => x != default)
                .TakeUntilDestroy(this)
                .SubscribeWithState(this, (x, t) =>
                {
                    //後片付け
                    for(int i = 0; i < t._fruitsObjects.Count; i++)
                    {
                        t._fruitsObjects[i].gameObject.SetActive(false);
                        t._fruitsPool.Enqueue(t._fruitsObjects[i]);
                    }
                    t._fruitsObjects.Clear();

                    var line = 0;
                    for(int i = 0; i < x.Length; i++)
                    {
                        //1行ずつ表示する
                        if(x[i] > 0)
                        {
                            line++;
                            var firstPosition = new Vector2(-(x[i] - 1) * GameConst.RequireFruitsOffsetX * 0.5f, -GameConst.RequireFruitsOffsetY * line);
                            for(int j = 0; j < x[i]; j++)
                            {
                                var img = t.CreateOrDequeue();
                                t._fruitsObjects.Add(img);
                                img.color = t._fruitsColors[i];
                                img.rectTransform.anchoredPosition = firstPosition;

                                firstPosition.x += GameConst.RequireFruitsOffsetX;
                            }
                        }
                    }
                    //行の数に応じて全体を上下に動かして中央揃えにする。
                    t._fruitsParent.anchoredPosition = new Vector2(0f, GameConst.RequireBalloonBaseY + (line-1) * GameConst.RequireFruitsOffsetY * 0.5f);
                });
            viewModel.TimelimitRatio
                .TakeUntilDestroy(this)
                .SubscribeWithState(this, (x, t) =>
                {
                    t._fillBalloon.fillAmount = x;
                });
            viewModel.BalloonFillColor
                .TakeUntilDestroy(this)
                .SubscribeWithState(this, (x, t) =>
                {
                    t._fillBalloon.color = x;
                });
            viewModel.BalloonText
                .TakeUntilDestroy(this)
                .SubscribeWithState(this, (x, t) =>
                {
                    t._balloonText.SetText(x);
                });
        }

        //イメージを作成か、取り出し
        private Image CreateOrDequeue()
        {
            if(!_fruitsPool.TryDequeue(out Image image))
            {
                image = Instantiate<Image>(_fruitsPrefab);
                image.transform.SetParent(transform, false);
            }
            image.gameObject.SetActive(true);
            image.transform.SetParent(_fruitsParent);
            return image;
        }
    }
}

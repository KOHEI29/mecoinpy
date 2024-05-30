using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace mecoinpy.Game
{
    public class PossessFruitsView : MonoBehaviour
    {
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

        //表示状況
        private Dictionary<FruitsObject.FruitsType, int> _fruitsDictionary = new Dictionary<FruitsObject.FruitsType, int>();
        
        // Start is called before the first frame update
        void Start()
        {
            var viewModel = ViewModelProvider.Get<PossessFruitsViewModel>(new PossessFruitsViewModelFactory(gameObject));

            //座標の書き換え
            viewModel.Position
                .Where(x => x != default)
                .TakeUntilDestroy(this)
                .SubscribeWithState(this, (x, t) => 
                {
                    t.transform.position = x;
                });
            //追加
            viewModel.AddFruits
                .TakeUntilDestroy(this)
                .SubscribeWithState(this, (x, t) =>
                {
                    t.AddFruits(x);
                });

            //失った時
            viewModel.Lost
                .TakeUntilDestroy(this)
                .SubscribeWithState(this, (x, t) =>
                {
                    for(int i = 0; i < t._fruitsObjects.Count; i++)
                    {
                        //落ちるアニメーションをしてから非表示
                        var img = t._fruitsObjects[i];
                        t._fruitsObjects[i].rectTransform.DOLocalMove(t._fruitsObjects[i].rectTransform.anchoredPosition - new Vector2(0f, 1.5f), 0.5f)
                                .SetEase(Ease.InBack, 3f)
                                .SetLink(t.gameObject)
                                .OnComplete(() =>
                                {
                                    img.gameObject.SetActive(false);
                                    t._fruitsPool.Enqueue(img);
                                });
                    }
                    t._fruitsObjects.Clear();
                    t._fruitsDictionary.Clear();
                });

            //ジュースにした時、アニメーションなしで消える
            viewModel.Juiced
                .TakeUntilDestroy(this)
                .SubscribeWithState(this, (x, t) =>
                {
                    for(int i = 0; i < t._fruitsObjects.Count; i++)
                    {
                        t._fruitsObjects[i].gameObject.SetActive(false);
                        t._fruitsPool.Enqueue(t._fruitsObjects[i]);
                    }
                    t._fruitsObjects.Clear();
                    t._fruitsDictionary.Clear();
                });
        }
        //果物を追加
        private void AddFruits(FruitsObject.FruitsType type)
        {
            var img = CreateOrDequeue();
            _fruitsObjects.Add(img);

            //Viewで配列を作成している。厳格化するならViewModelに移動させる。
            if(_fruitsDictionary.ContainsKey(type))
            {
                _fruitsDictionary[type]++;
            }
            else
            {
                _fruitsDictionary.Add(type, 1);
            }
            //表示する。
            var objectCount = 0;
            var posY = 0f;
            for(int i = 0; i < _fruitsDictionary.Count; i++)
            {
                var key = _fruitsDictionary.ElementAt(i).Key;
                var value = _fruitsDictionary.ElementAt(i).Value;
                var startX = - (value - 1) * GameConst.PossessFruitsOffsetX * 0.5f;
                for(int j = 0; j < value; j++)
                {
                    _fruitsObjects[objectCount].rectTransform.anchoredPosition = new Vector2(startX + j * GameConst.PossessFruitsOffsetX, posY);
                    _fruitsObjects[objectCount].color = _fruitsColors[(int)key];
                    objectCount++;
                }
                posY -= GameConst.PossessFruitsOffsetY;
            }


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
            return image;
        }
    }
}

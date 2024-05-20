using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;

namespace mecoinpy.Game
{
    //力学処理
    public class MyPhysics
    {
        private MyGameObject _gameObject = default;
        public MyGameObject GameObject => _gameObject;
        private MyPhysics(){}
        public MyPhysics(MyGameObject go)
        {
            _gameObject = go;
        }
        //質量
        private float _mass = 1f;
        private float Mass => _mass;
        //座標
        public Vector2 Position => _gameObject.Position;
        //速度
        //private Vector2 _velocity = Vector2.zero;
        public Vector2 Velocity{get; set;} = Vector2.zero;
        //加速度
        private Vector2 _acceleration = Vector2.zero;
        public Vector2 Acceleration => _acceleration;
        //加えられている力、Updateでリセットされる
        private Vector2 _force = Vector2.zero;
        public Vector2 Force => _force;
        //揚力
        private float _liftForce = 0f;
        public float LiftForce => _liftForce;
        //BodyType.
        private BodyType _type = BodyType.DEFAULT;
        public BodyType Type
        {
            private get{return _type;}
            set
            {
                _type = value;
                if(_type == BodyType.STATIC)
                {
                    //それまでにかかっていた速度や力をリセット
                    Velocity = Vector2.zero;
                    _force = Vector2.zero;
                }
            }
        }
        //bodytype.kinematic AddPowerされない設定
        public bool IsKinematic => Type == BodyType.KINEMATIC;
        //bodytype.static 動かない設定
        public bool IsStatic => Type == BodyType.STATIC;
        
        //フレーム処理
        public void Update(float time)
        {
            if(IsStatic) return;
            _acceleration = Force / Mass;
            _acceleration.y -= GameConst.DefaultGravityAcceleration - LiftForce;
            _force = Vector2.zero;

            Velocity += Acceleration * time;

            _gameObject.Position += Velocity * time;
        }
        //力を加える
        public void AddForce(Vector2 force)
        {
            if(IsKinematic || IsStatic) return;

            _force += force;
        }
        //着地した。滑ったりバウンドしないゲームなので、Vを0にするのみ
        public void Grounded()
        {
            Velocity = Vector2.zero;
        }
        //頭をぶつけた。跳ね返りたい
        public void HitCeil()
        {            
            Velocity = new Vector2(Velocity.x, -Velocity.y);
        }


        public enum BodyType
        {
            DEFAULT = -1,
            DYNAMIC = 0,
            STATIC,
            KINEMATIC,
        }
    }
}

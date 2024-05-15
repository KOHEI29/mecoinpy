using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mecoinpy.Game
{
    //力学処理とColliderを持つオブジェクトデータ
    public class PhysicsObject
    {
        //力学
        protected MyPhysics _physics = new MyPhysics();
        public MyPhysics Physics => _physics;
        //Collider
        protected MyCollider _collider = new AABB(new Vector2(-0.5f, -0.5f), new Vector2(0.5f, 0.5f));
        public MyCollider Collider = new AABB(new Vector2(-0.5f, -0.5f), new Vector2(0.5f, 0.5f));

        public PhysicsObject(MyPhysics myPhysics, MyCollider myCollider)
        {
            _physics = myPhysics;
            _collider = myCollider;
        }
    }


    //力学処理
    public class MyPhysics
    {
        //質量
        private float _mass = 1f;
        private float Mass => _mass;
        //座標
        private Vector2 _position = Vector2.zero;
        public Vector2 Position => _position;
        //速度
        private Vector2 _velocity = Vector2.zero;
        public Vector2 Velocity => _velocity;
        //加速度
        private Vector2 _acceleration = Vector2.zero;
        public Vector2 Acceleration => _acceleration;
        //加えられている力、Updateでリセットされる
        private Vector2 _force = Vector2.zero;
        public Vector2 Force => _force;
        //揚力
        private float _liftForce = 0f;
        public float LiftForce => _liftForce;
        //動かないオブジェクト
        private bool _isKinematic = false;
        public bool IsKinematic => _isKinematic;
        
        //フレーム処理
        public void Update(float time)
        {
            if(IsKinematic) return;

            _acceleration = Force / Mass;
            _acceleration.y -= GameConst.DefaultGravityAcceleration - LiftForce;
            _force = Vector2.zero;

            _velocity += Acceleration * time;

            _position += Velocity * time;
        }
        //力を加える
        public void AddForce(Vector2 force)
        {
            if(IsKinematic) return;

            _force += force;
        }
        //キネマティックなオブジェクトを作成する。
        public static MyPhysics KinematicObject(Vector2 position)
        {
            return new MyPhysics(){_position = position, _isKinematic = true};
        }
    }
}

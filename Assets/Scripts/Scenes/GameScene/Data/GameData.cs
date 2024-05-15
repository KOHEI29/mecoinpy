using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace mecoinpy.Game
{
    //プレイヤーのデータ
    public class PlayerData
    {
        //力学
        private MyPhysics _physics = new MyPhysics();
        public MyPhysics Physics => _physics;
        //所持ジャンプ玉
        private int _jumpBall = 0;
        public int JumpBall => _jumpBall;
        //ジャンプ玉最大値
        private int _jumpBallMax = 5;
        public int JumpBallMax => _jumpBallMax;

        //コンストラクタ
        public PlayerData()
        {
            _jumpBall = JumpBallMax;
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
        
        //フレーム処理
        public void Update(float time)
        {
            _acceleration = Force / Mass;
            _acceleration.y -= GameConst.DefaultGravityAcceleration - LiftForce;
            _force = Vector2.zero;

            _velocity += Acceleration * time;

            _position += Velocity * time;
        }
        //力を加える
        public void AddForce(Vector2 force)
        {
            _force += force;
        }
    }
}

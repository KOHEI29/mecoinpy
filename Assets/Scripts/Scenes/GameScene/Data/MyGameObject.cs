using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mecoinpy.Game
{
    //GameObject。Transformを持っておく。継承した時にPhysicsやColliderにTransformを渡しづらいので分けた。
    public class MyGameObject
    {
        public Vector2 Position{get; set;} = Vector2.zero;
        public Quaternion Rotation{get; set;} = Quaternion.identity;
        public Vector2 Scale{get; set;} = Vector2.zero;
    }

    //力学処理とColliderを持つオブジェクトデータ
    public class PhysicsObject : MyGameObject, IHavePhysics, IHaveCollider
    {
        //力学
        protected MyPhysics _physics = default;
        public MyPhysics Physics => _physics;
        //Collider
        protected MyCollider _collider = default;
        public MyCollider Collider => _collider;
        public void SetCollider(MyCollider collider)
        {
            _collider = collider;
        }

        public PhysicsObject()
        {
            _physics = new MyPhysics(this);
            _collider = default;
        }
    }

    //Colliderだけを持つオブジェクトデータ。動かない地形などはこれで良い。
    public class ColliderObject : MyGameObject, IHaveCollider
    {
        //Collider
        protected MyCollider _collider = default;
        public MyCollider Collider => _collider;
        public void SetCollider(MyCollider collider)
        {
            _collider = collider;
        }

        public ColliderObject()
        {
            _collider = default;
        }
    }
    public interface IHavePhysics
    {
        MyPhysics Physics{get;}
    }
    public interface IHaveCollider
    {
        MyCollider Collider{get;}
    }
}

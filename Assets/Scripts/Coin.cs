using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Coin : MonoBehaviour
{
    //Для возвращения монеты в пул при сборе
    private Pool _coinPool;

    [SerializeField] private Transform modelTransform;

    [Inject]
    public void Construct(Pool coinPool)
    {
        _coinPool = coinPool;
        Reset(Vector3.zero);
    }
    private void Reset(Vector3 pos)
    {
        transform.position = pos;
    }

    private void Update()
    {
        var pos = modelTransform.position;
        modelTransform.rotation = Quaternion.Euler(-90f, 80f * (0.3f * (pos.x + pos.z) + Time.time),0); 
    }

    private void OnTriggerEnter(Collider other)
    {
        throw new NotImplementedException();
    }

    public class Pool : MonoMemoryPool<Vector3, Coin>
    {
        protected override void Reinitialize(Vector3 pos, Coin item)
        {
            item.Reset(pos);
        }
    }
}

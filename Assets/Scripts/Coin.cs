using System;
using System.Collections;
using System.Collections.Generic;
using Signals;
using UnityEngine;
using Zenject;

public class Coin : MonoBehaviour
{
    //Для возвращения монеты в пул при сборе
    [Inject] private readonly Pool _coinPool;
    [Inject] private readonly SignalBus _signalBus;
    [SerializeField] private Transform modelTransform;
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
        if (other.CompareTag("Player"))
        {
            _signalBus.Fire<PickedUpCollectableSignal>();
            _coinPool.Despawn(this);
        }
    }

    public class Pool : MonoMemoryPool<Vector3, Coin>
    {
        protected override void Reinitialize(Vector3 pos, Coin item)
        {
            item.Reset(pos);
        }
    }
}

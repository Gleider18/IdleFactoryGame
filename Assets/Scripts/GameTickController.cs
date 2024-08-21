using System;
using System.Collections;
using UnityEngine;

public class GameTickController : MonoBehaviour
{
    public static GameTickController Instance { get; private set; }

    public event Action OnTick;

    public float tickInterval = 1f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartCoroutine(TickRoutine());
    }

    private IEnumerator TickRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickInterval);
            OnTick?.Invoke();
        }
    }
}
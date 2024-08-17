using System.Collections.Generic;
using UnityEngine;

public class ConveyorController : MonoBehaviour
{
    [SerializeField] private Transform _startMovePoint;
    [SerializeField] private Transform _endMovePoint;
    [SerializeField] private float _journeyTime = 0.99f;
    
    private List<Part> _partsQueue = new();
    private float _animationTimePassed;
    private FactoryController _nextFactory;

    private void Update()
    {
        if (_partsQueue.Count < 0) return;
        _animationTimePassed += Time.deltaTime;
        float progress = Mathf.Clamp01(_animationTimePassed / _journeyTime);
        
        if (progress >= 1.0f)
        {
            foreach (var currentPart in _partsQueue) OnPartReachedEnd(currentPart);
            _partsQueue.Clear();
            _animationTimePassed = 0;
            return;
        }
        
        foreach (var currentPart in _partsQueue) currentPart.thisPart.transform.position = Vector3.Lerp(_startMovePoint.position, _endMovePoint.position, progress);
    }

    public void ReceivePart(Part part)
    {
        _animationTimePassed = 0f;
        part.thisPart.transform.position = _startMovePoint.position;
        _partsQueue.Add(part);
    }

    private void OnPartReachedEnd(Part part)
    {
        if (_nextFactory != null && _nextFactory.IsBuilt())
        {
            part.thisPart.transform.position = _nextFactory.transform.position;
            _nextFactory.ReceivePart(part);
        }
        else
        {
            GameManager.Instance.AddMoney(part.tPartModel.Price);
            GameManager.Instance.AddExperience(part.tPartModel.Level + 1);
            Destroy(part.thisPart);
        }
    }

    public void SetNextFactory(FactoryController nextFactory) => _nextFactory = nextFactory;
}
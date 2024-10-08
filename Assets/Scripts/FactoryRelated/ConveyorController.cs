using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ConveyorController : MonoBehaviour
{
    [SerializeField] private Transform _startMovePoint;
    [SerializeField] private Transform _endMovePoint;
    [SerializeField] private float _journeyTime = 0.99f;
    
    private FactoryController _nextFactory;

    public void ReceivePart(Part part)
    {
        var partGM = part.thisPart;
        partGM.transform.position = _startMovePoint.position;
        partGM.SetActive(true);
        partGM.transform.DOMove(_endMovePoint.position, _journeyTime).SetEase(Ease.Linear).OnComplete(() => OnPartReachedEnd(part));
    }

    private void OnPartReachedEnd(Part part)
    {
        if (_nextFactory != null && _nextFactory.IsBuilt())
        {
            part.thisPart.transform.position = _nextFactory.transform.position;
            _nextFactory.ReceivePart(part.tPartModel);
        }
        else
        {
            GameManager.Instance.AddMoney(part.tPartModel.Price);
            GameManager.Instance.AddExperience(part.tPartModel.Level + 1);
        }
        PartPool.Instance.ReturnPart(part);
    }

    public void SetNextFactory(FactoryController nextFactory) => _nextFactory = nextFactory;
}
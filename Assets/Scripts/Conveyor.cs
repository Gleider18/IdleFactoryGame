using System.Collections.Generic;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    float journeyTime = 0.99f;
    private List<Part> partsQueue = new();
    public float timeElapsed;

    private void Update()
    {
        if (partsQueue.Count < 0) return;
        timeElapsed += Time.deltaTime;
        float progress = Mathf.Clamp01(timeElapsed / journeyTime);
        
        if (progress >= 1.0f)
        {
            foreach (var currentPart in partsQueue) OnPartReachedEnd(currentPart);
            partsQueue.Clear();
            timeElapsed = 0;
            return;
        }
        
        foreach (var currentPart in partsQueue) currentPart.thisPart.transform.position = Vector3.Lerp(startPoint.position, endPoint.position, progress);
    }

    public void ReceivePart(Part part)
    {
        timeElapsed = 0f;
        part.thisPart.transform.position = startPoint.position;
        partsQueue.Add(part);
    }

    private void OnPartReachedEnd(Part part)
    {
        // Передаем деталь на следующую фабрику
        FactoryController nextFactory = GetComponentInParent<FactoryController>().nextFactory;
        if (nextFactory != null && nextFactory.IsBuilt())
        {
            part.thisPart.transform.position = nextFactory.transform.position;
            nextFactory.ReceivePart(part);
        }
        else
        {
            GameManager.Instance.AddMoney(part.tPartModel.level + 1);
            Destroy(part.thisPart);
        }
    }
}
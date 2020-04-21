using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MaterialRequest
{
    [SerializeField]
    public Materials materials;

    [SerializeField]
    private int stockCount;

    [SerializeField]
    private int maxCount;

    [SerializeField]
    private float progress;
    [SerializeField]
    private float rate;

    private RequestController rc;

    public void UpdateProgress()
    {
        if(progress >= 100)
        {
            if(stockCount < maxCount)
            {
                stockCount++;
                progress = 0;
            }
            else
            {
                progress = 100;
            }
        }

        progress += rate * Time.deltaTime;
    }

    public int GetCount()
    {
        return stockCount;
    }

    public void SetCount(int tmp)
    {
        stockCount = tmp;
    }

    public void SubtractCount(int tmp)
    {
        stockCount -= tmp;
    }

    public int GetMaxCount()
    {
        return maxCount;
    }

    public float GetProgress()
    {
        return progress;
    }
}

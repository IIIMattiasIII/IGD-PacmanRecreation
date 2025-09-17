using System;
using System.Collections.Generic;
using UnityEngine;

public class Tweener : MonoBehaviour
{
    private List<Tween> activeTweens = new();

    void Update()
    {
        if (!(activeTweens?.Count > 0)) return;
        activeTweens.ForEach(activeTween =>
        {
            activeTween.Target.position = (Vector3.Distance(activeTween.EndPos, activeTween.Target.position) > 0.1f) ?
                Vector3.Lerp(
                    activeTween.StartPos,
                    activeTween.EndPos,
                    (Time.time - activeTween.StartTime) / activeTween.Duration
                ) :
                activeTween.EndPos;
        });
        activeTweens.RemoveAll(tw => tw.Target.position == tw.EndPos);
    }

    public bool TweenExists(Transform target)
    {
        foreach (Tween tw in activeTweens)
        {
            if (tw.Target == target)
            {
                return true;
            }
        }
        return false;
    }

    public bool AddTween(Transform targetObj, Vector3 startPos, Vector3 endPos, float duration)
    {
        if (TweenExists(targetObj)) return false;
        activeTweens.Add(new Tween(targetObj, startPos, endPos, Time.time, duration));
        return true;
    }
}

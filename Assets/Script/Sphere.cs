using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class Sphere : MonoBehaviour
{

    public Renderer[] _renderers;
    private Sequence sequence;

    private void Start()
    {
        sequence = DOTween.Sequence();
        sequence.Append(transform.DOShakeScale(.5f, .15f))
                .Append(
                    transform.DORotate(Random.onUnitSphere * Random.Range(90, 360), 2.5f, RotateMode.FastBeyond360)
                   .SetRelative()
                   .SetLoops(-1, LoopType.Yoyo)
                   .SetEase(Ease.OutFlash));
        sequence.Play();
    }

    public void ChangeColor(Color color) => _renderers.ToList().ForEach(rend => rend.material.color = color);


    private void OnDestroy()
    {
        sequence.Kill();
    }

    private void OnDisable()
    {
        sequence.Kill();
    }
}

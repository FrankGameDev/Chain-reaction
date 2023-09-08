using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

public class CancellableMonoBehaviour : MonoBehaviour
{
    public CancellationTokenSource cancellationTokenSource { get; private set; }


    protected void CreateCancellationToken()
    {
        cancellationTokenSource = new CancellationTokenSource();
    }

    protected void CancelOperation()
    {
        cancellationTokenSource.Cancel();
    }

    private void OnDisable()
    {
        CancelOperation();
    }

    private void OnDestroy()
    {
        CancelOperation();
    }
}
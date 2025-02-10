using System.Collections;
using System.Collections.Generic;
using Google.Play.Review;
using Unity.VisualScripting;
using UnityEngine;

public class ReviewManagerController : PersistentSingleton<ReviewManagerController>
{

    private ReviewManager _reviewManager;

    public IEnumerator GetReviewPopup()
    {
        yield return new WaitForSeconds(1f);

        if (_reviewManager == null) _reviewManager = new ReviewManager();

        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            Debug.LogError($"Errore nel recupero del review flow: {requestFlowOperation.Error}");
            Debug.LogWarning("Errore prima della visualizzazione");
            yield break;
        }
        var _playReviewInfo = requestFlowOperation.GetResult();
        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            Debug.LogError($"Errore nell'esecuzione del review popup: {launchFlowOperation.Error}");
            Debug.LogWarning("Errore dopo visualizzazione");
        }
        // The flow has finished. The API does not indicate whether the user
        // reviewed or not, or even whether the review dialog was shown. Thus, no
        // matter the result, we continue our app flow.
        Debug.Log("Review end");
    }
}

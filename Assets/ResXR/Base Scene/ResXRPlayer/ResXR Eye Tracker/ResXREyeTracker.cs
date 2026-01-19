
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResXREyeTracker : MonoBehaviour
{
    public Transform FocusedObject => _focusedObject;
    public Vector3 EyeGazeHitPosition => _eyeGazeHitPosition;
    public Transform RightEye => _rightEye;
    public Transform LeftEye => _leftEye;
    public Vector3 EyePosition => _eyePosition;


    [SerializeField] private Transform _rightEye;
    [SerializeField] private Transform _leftEye;
    private Vector3 _eyePosition;
    private const float EYERAYMAXLENGTH = 100000;
    private const float EYETRACKINGCONFIDENCETHRESHOLD = .5f;
    private Vector3 NOTTRACKINGVECTORVALUE = new Vector3(-1f, -1f, -1f);
    private OVREyeGaze _ovrEyeR;
    private Transform _focusedObject;
    private Vector3 _eyeGazeHitPosition;
    LayerMask _eyeTrackingLayerMask = ~(1 << 7);

    public void Init()
    {
        if (_rightEye.TryGetComponent(out OVREyeGaze er))
        {
            _ovrEyeR = er;
        }

        _focusedObject = null;
        _eyeGazeHitPosition = NOTTRACKINGVECTORVALUE;
    }

    public void UpdateEyeTracker()
    {
        // don't track if there is no OVREye component (enough to check only on one eye).
        if (_ovrEyeR == null) return;

        // don't track on low confidence.
        // Debug.Log(_ovrEyeR.Confidence);
        if (_ovrEyeR.Confidence < EYETRACKINGCONFIDENCETHRESHOLD)
        {
            _focusedObject = null;
            _eyeGazeHitPosition = NOTTRACKINGVECTORVALUE;
            // Debug.Log("EyeTracking confidence value is low. Eyes are not tracked");

            return;
        }


        // We compute a cyclopean (binocular) gaze ray.
        // Origin: midpoint between left and right eye positions.
        // Direction: normalized sum of left and right gaze directions.
        //
        // This avoids lateral parallax introduced by using a single-eye direction
        // from a midpoint origin, and reduces monocular noise.
        // Per-eye data are logged to allow offline recomputation (e.g., vergence-based focus).
        _eyePosition = (_rightEye.position + _leftEye.position) / 2;

        Vector3 eyeForward = (_rightEye.forward + _leftEye.forward).normalized;


        // Fallback: if binocular direction averaging becomes unstable (near-zero magnitude),
        // we revert to a monocular gaze direction.
        // This avoids propagating invalid vectors while preserving a valid gaze ray
        // when only one eye provides reliable tracking.

        if (eyeForward.sqrMagnitude < 1e-6f)
            eyeForward = _rightEye.forward;

        RaycastHit hit;
        if (Physics.Raycast(_eyePosition, eyeForward, out hit, EYERAYMAXLENGTH, _eyeTrackingLayerMask))
        {
            _focusedObject = hit.transform;
            _eyeGazeHitPosition = hit.point;
        }
        else
        {
            _focusedObject = null;
            _eyeGazeHitPosition = NOTTRACKINGVECTORVALUE;
        }

        Debug.DrawRay(_eyePosition, eyeForward * 2f , Color.red);
    }
}
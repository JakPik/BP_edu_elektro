/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: GrabControlUtility
 */

using UnityEngine;
using System;
using System.Collections;
using System.Security.Cryptography.X509Certificates;

public static class GrabControlUtility
{
    private static float animationSpeed = 5f;
    public static void SetGrabbedState(Rigidbody rb, GameObject caller)
    {
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.excludeLayers = 1 << caller.layer;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false;
    }

    public static void SetReleasedState(Rigidbody rb)
    {
        rb.interpolation = RigidbodyInterpolation.None;
        rb.excludeLayers = LayerMask.GetMask("Nothing");
        rb.constraints = RigidbodyConstraints.None;
        rb.useGravity = true;
    }

    public static IEnumerator AnimateRotationCorrection(GameObject caller, Transform targetTransform)
    {
        Rigidbody rigidBody = caller.GetComponent<Rigidbody>();
        Quaternion startRot = rigidBody.rotation;
        float step = 0f;
        rigidBody.constraints = RigidbodyConstraints.None;
        float angle = Quaternion.Angle(rigidBody.rotation, targetTransform.rotation);

        while (angle > 0.1f)
        {
            step += Time.deltaTime;//* animationSpeed;
            step = Mathf.Clamp01(step);

            rigidBody.rotation = Quaternion.Slerp(startRot, targetTransform.rotation, step);
            angle = Quaternion.Angle(rigidBody.rotation, targetTransform.rotation);
            yield return null;
        }
        rigidBody.rotation = targetTransform.rotation;
        rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
    }
}
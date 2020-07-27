using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PIDController 
{

    //Our PID coefficients for tuning the controller
    [Range(0.0f, 1.0f)] [SerializeField] private float errorCoeff = 1;
    [Range(0.0f, 1.0f)] [SerializeField] private float integralCoeff = .0002f;
    [Range(0.0f, 3.0f)] [SerializeField] private float derivativeCoeff = 3;
    [Range(-200.0f, 3.0f)] [SerializeField] private float minimum = -100;
    [Range(0.0f, 1000f)] [SerializeField] private float maximum = 200f;

    float previousError = 0;
    float integral = 0;
   
    public float CalculateForce(float targetHeight, float currentHeight)
    {
        float deltaTime = Time.fixedDeltaTime;
        float error = targetHeight - currentHeight;
        float derivative = (error - previousError) / deltaTime;

        integral = integral + error * deltaTime;
        previousError = error;

        float value = errorCoeff * error + integralCoeff * integral + derivativeCoeff * derivative;
        value = Mathf.Clamp(value, minimum, maximum);
        return value;
    }

}


public class PIDController
{
    public float _Kp;
    public float _Ki;
    public float _Kd;

    

    private float prevError;
    private float P, I, D;

    public PIDController(float kp, float ki, float kd)
    {
        _Kp = kp;
        _Ki = ki;
        _Kd = kd;
    }

    public float GetOutput(float currentError, float dt)
    {
        P = currentError;
        I += P * dt;
        D = (P - prevError) / dt;
        prevError = currentError;

        return P * _Kp + I * _Ki + D * _Kd;
    }
}

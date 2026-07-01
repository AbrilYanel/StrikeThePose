using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxBackgroundObject : MonoBehaviour
{
    private float _speed;
    private float _destroyZ;
    private Camera _camera;
    private bool _billboardToCamera;
    private bool _keepUpright;
    private float _rotationSpeed;
    private Vector3 _spinAxis;

    public void Initialize(
        float speed,
        float destroyZ,
        Camera targetCamera,
        bool billboardToCamera,
        bool keepUpright,
        float randomRotationSpeed = 0f
    )
    {
        _speed = speed;
        _destroyZ = destroyZ;
        _camera = targetCamera;
        _billboardToCamera = billboardToCamera;
        _keepUpright = keepUpright;

        _rotationSpeed = randomRotationSpeed;
        _spinAxis = Vector3.up;

        UpdateBillboard();
    }

    private void Update()
    {
        // Mismo sentido que tus obstáculos: nacen al fondo en Z negativo y avanzan hacia Z positivo.
        transform.position += Vector3.forward * _speed * Time.deltaTime;

        if (_billboardToCamera)
        {
            UpdateBillboard();
        }
        else if (Mathf.Abs(_rotationSpeed) > 0.01f)
        {
            transform.Rotate(_spinAxis, _rotationSpeed * Time.deltaTime, Space.World);
        }

        if (transform.position.z >= _destroyZ)
        {
            Destroy(gameObject);
        }
    }

    private void UpdateBillboard()
    {
        if (_camera == null) return;

        Vector3 direction = transform.position - _camera.transform.position;

        if (_keepUpright)
        {
            direction.y = 0f;
        }

        if (direction.sqrMagnitude < 0.001f) return;

        // Hace que el objeto mire hacia la cámara manteniendo una orientación estable.
        transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
    }
}

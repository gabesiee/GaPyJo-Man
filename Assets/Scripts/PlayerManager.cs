using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour

{
    [Header("Translation & Rotation")]
    [Tooltip("Vitesse de translation en m.s-1")]
    [SerializeField] float m_TranslationSpeed = 0;
    [Tooltip("Vitesse de rotation °.s-1")]
    [SerializeField] float m_RotationSpeed = 0;
    [SerializeField] public Transform camera;

    float yRotation = 0f;

    Rigidbody m_RigidBody;

    private void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        float vInput = Input.GetAxis("Vertical") * m_TranslationSpeed * Time.fixedDeltaTime;
        float hInput = Input.GetAxis("Horizontal") * m_TranslationSpeed * Time.fixedDeltaTime;
        float mouseX = Input.GetAxis("Mouse X") * m_RotationSpeed * Time.fixedDeltaTime;
        float mouseY = Input.GetAxis("Mouse Y");

        //Mouvement
        Vector3 translationVect = vInput * transform.forward;
        Vector3 translationVect2 = hInput * transform.right;
        m_RigidBody.MovePosition(transform.position + translationVect + translationVect2);

        float deltaAngle = mouseX;
        Quaternion qRot = Quaternion.AngleAxis(deltaAngle, transform.up);
        Quaternion qUpright = Quaternion.FromToRotation(transform.up, Vector3.up);

        Quaternion newOrientation = Quaternion.Lerp(transform.rotation, qUpright * transform.rotation, Time.fixedDeltaTime);
        newOrientation = qRot * newOrientation;
        m_RigidBody.MoveRotation(newOrientation);


        yRotation -= mouseY;
        yRotation = Mathf.Clamp(yRotation, -30f, 30f);
        camera.localRotation = Quaternion.Euler(yRotation, 0f, 0f);


    }

        // Update is called once per frame
    void Update()
    {
        
    }

    
}

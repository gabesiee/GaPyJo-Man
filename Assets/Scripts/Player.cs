using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour

{
    [Header("Translation & Rotation")]
    [Tooltip("Vitesse de translation en m.s-1")]
    [SerializeField] float m_TranslationSpeed = 0;
    [Tooltip("Vitesse de rotation °.s-1")]
    [SerializeField] float m_RotationSpeed = 0;

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

        float vInput = Input.GetAxis("Vertical");
        float hInput = Input.GetAxis("Horizontal");
        float mouseX = Input.GetAxis("Mouse X");

        //Mouvement
        Vector3 translationVect = vInput * transform.forward * m_TranslationSpeed * Time.fixedDeltaTime;
        Vector3 translationVect2 = hInput * transform.right * m_TranslationSpeed * Time.fixedDeltaTime;
        m_RigidBody.MovePosition(transform.position + translationVect + translationVect2);

        float deltaAngle = mouseX * m_RotationSpeed * Time.fixedDeltaTime;
        Quaternion qRot = Quaternion.AngleAxis(deltaAngle, transform.up);
        Quaternion qUpright = Quaternion.FromToRotation(transform.up, Vector3.up);

        Quaternion newOrientation = Quaternion.Slerp(transform.rotation, qUpright * transform.rotation, Time.fixedDeltaTime);
        newOrientation = qRot * newOrientation;
        m_RigidBody.MoveRotation(newOrientation);


    }

        // Update is called once per frame
    void Update()
    {
        
    }
}

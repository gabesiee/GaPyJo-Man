using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Ennemy : MonoBehaviour
{

    [SerializeField] GameObject m_Image;

    //Terrain de jeu
    private Vector3[,] mapPath;

    //Position de l'ennemi
    private int[] pos;

    //Réference des déplacements possible du personnage selon son angle de vue
    Dictionary<int, int[,]> rotation_offset = new Dictionary<int, int[,]>()
    {
        {0, new int[3, 2] {
            {0,-1},
            { 1, 0 }, 
            { 0, 1 }
        }},
        {90, new int[3, 2] {
            {1,0}, 
            { 0, 1 }, 
            { -1, 0 }
        }},
        {270, new int[3, 2] {
            {-1,0},
            { 0, -1 },
            { 1, 0 }
        }},
        {180, new int[3, 2] {
            {0,1},
            { -1, 0 },
            { 0,-1 }
        }},
    };
       
    
    bool running = false;
    Rigidbody m_RigidBody;

    void Start() {
        

        mapPath = MapManager.Instance.getMapPath(); // Récupération du terrain
        
        getFirstAvailablePos();                     // Choisi la position de l'ennemi dans une zone libre (à modifier pour l'ajout de mechants)
        transform.position = mapPath[pos[0], pos[1]]; // positionne l'ennemi
        StartCoroutine(pathFindingRoutine());             //Démarrage de la coroutine de déplacement

    }

    /**
     * Choisi la position de l'ennemi dans une zone libre 
     * TODO : à modifier pour l'ajout de mechants (peut etre prendre les coins???)
     */
    private void getFirstAvailablePos() {
        
        for (int i = 0; i < mapPath.GetLength(0); i++)
        {
            for (int j = 0; j < mapPath.GetLength(0); j++)
            {

                if (!mapPath[i, j].Equals(new Vector3(-1, -1, -1))) {
                    pos = new int[2] { i, j };
                    return;
                }
                   
            }
        }

    }


    private void Awake()
    {      

        m_RigidBody = GetComponent<Rigidbody>();
        
    }

    /**
     * Coroutine pricnipale de destion du déplacement
     * 
     */

    IEnumerator pathFindingRoutine() {
        float elapsedTime = 0;
        float waitTime = .1f; // vitesse de deplacement

        //Boucle d'interpolation de la position
        Vector3 currentPos = transform.position;
        while (elapsedTime < waitTime && !running)
        {
            transform.position = Vector3.Lerp(currentPos, mapPath[pos[0],pos[1]], (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;

            //Condition de sortie lorsque l'ennemi s'approche de sa destination
            if ((Mathf.Pow(transform.position.x - mapPath[pos[0], pos[1]].x, 2.0f) < .01))
            {
                running = false;
            }
            yield return null;
        }

        //Fixation des valeurs aaprès l'interpolation
        transform.position = mapPath[pos[0], pos[1]];

        rotateEnnemy();
        StartCoroutine(pathFindingRoutine());
                    
    }

    /**
     * 
     * Gere la rotation et les futurs deplacements dans le terrain 
     */
    void rotateEnnemy() {

        // recupere l'angle de rotation actuel
        int rotation = (int)transform.rotation.eulerAngles.y;

        //Chacune des valeur correspond à une direction (0 : gauche, 1: devant, 2,droite).
        //Ces directions sont relatives à l'angle actuel de l'ennemi
        float[] values = { Random.value, Random.value, Random.value };

        //Recupération de l'offset de l'angle
        var offset = rotation_offset[rotation];

        //Ces trois conditionnelles empeche le choix de direction dans des murs 
        
        //front
        if (isWall(mapPath[pos[0] + offset[1,0], pos[1] + offset[1, 1]])) {
            values[1] = -1;
        }
        //left
        if (isWall(mapPath[pos[0] + offset[0, 0], pos[1] + offset[0, 1]]))
        {
            values[0] = -1;
        }
        //right
        if (isWall(mapPath[pos[0] + offset[2, 0], pos[1] + offset[2, 1]]))
        {
            values[2] = -1;
        }


        //Selection de la valeur aléatoire (plsu grand nombre restant)

        float max = Mathf.Max(values);
        //Cul de sac
        if (max == -1)
        {
            offset = rotation_offset[((int)rotation+180)%360];
            transform.eulerAngles = new Vector3(
                 transform.eulerAngles.x,
                 transform.eulerAngles.y - 180,
                 transform.eulerAngles.z
            );

        }
        //Gauche valeur choisie
        else if (max == values[0])
        {
            pos[0] += offset[0, 0];
            pos[1] += offset[0, 1];

            transform.eulerAngles = new Vector3(
                 transform.eulerAngles.x,
                 (int)transform.eulerAngles.y - 90,
                 transform.eulerAngles.z
            );

        }
        //Droite valeur choisie
        else if (max == values[2])
        {
            pos[0] += offset[2, 0];
            pos[1] += offset[2, 1];

            transform.eulerAngles = new Vector3(
                 transform.eulerAngles.x,
                 (int)transform.eulerAngles.y + 90,
                 transform.eulerAngles.z
            );

        }
        //Continue tout droit
        else if (max == values[1])
        {
            pos[0] += offset[1, 0];
            pos[1] += offset[1, 1];

        } 

    }

   /**
    * 
    * Renvoi vrai si la postion donnée est un mur
    */
    private bool isWall(Vector3 vector3)
    {
        Vector3 test = new Vector3(-1, -1, -1);
        return vector3.Equals(test);
    }


}

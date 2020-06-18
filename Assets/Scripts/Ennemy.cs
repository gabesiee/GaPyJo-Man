using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using SDD.Events;

public class Ennemy : MonoBehaviour
{
    //Terrain de jeu
    private Vector3[,] mapPath;
    private Vector3 huntingposition;
    private bool hunting = false;

    //Position de l'ennemi
    private int[] pos;

    //Réference des déplacements possible du personnage selon sa rotation et son angle de vue
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

    //Références des scripts d'animation
    private AnimatedImage normalAnimationScript; 
    private AnimatedImage huntingAnimationScript;

    //Lumiere de la torche
    private Light light;

    void Start()
    {


        mapPath = MapManager.Instance.getMapPath(); // Récupération du terrain

        // Récupération et initialisation des scripts d'animation
        huntingAnimationScript = GetComponentsInChildren<AnimatedImage>()[0];
        normalAnimationScript = GetComponentsInChildren<AnimatedImage>()[1];
        normalAnimationScript.enabled = true;
        huntingAnimationScript.enabled = false;

        getFirstAvailablePos();                     // Choisi la position de l'ennemi dans une zone libre (à modifier pour l'ajout de mechants)
        transform.position = mapPath[pos[0], pos[1]]; // positionne l'ennemi
        StartCoroutine(pathFindingRoutine());             //Démarrage de la coroutine de déplacement

    }

    /**
     * Choisi la position de l'ennemi dans une zone libre 
     * NOW : Prend la premiere case disponible
     * TODO : à modifier pour l'ajout de mechants (peut etre prendre les coins???)
     */
    private void getFirstAvailablePos()
    {

        for (int i = 0; i < mapPath.GetLength(0); i++)
        {
            for (int j = 0; j < mapPath.GetLength(0); j++)
            {

                if (!mapPath[i, j].Equals(new Vector3(-1, -1, -1)))
                {
                    pos = new int[2] { i, j };
                    return;
                }

            }
        }

    }


    private void Awake()
    {
        light = GetComponentInChildren<Light>();

        m_RigidBody = GetComponent<Rigidbody>();


    }

    /**
     * Coroutine pricnipale de gestion du déplacement
     * 
     */

    IEnumerator pathFindingRoutine()
    {
        float waitTime;
        float elapsedTime = 0;
        if (hunting)
            waitTime = .5f;// vitesse de deplacement en chasse
        else
            waitTime = .7f; // vitesse de deplacement normale

        //Boucle d'interpolation de la prochaine position
        Vector3 currentPos = transform.position;
        while (elapsedTime < waitTime && !running)
        {
            transform.position = Vector3.Lerp(currentPos, mapPath[pos[0], pos[1]], (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;

            //Condition de sortie lorsque l'ennemi s'approche de sa destination
            if ((Mathf.Pow(transform.position.x - mapPath[pos[0], pos[1]].x, 2.0f) < .01))
            {
                running = false;
            }
            yield return null;
        }
        transform.position = mapPath[pos[0], pos[1]];

        if (!hunting)
        {
            light.color = Color.white;
            getNextRandomPosition();
            huntingAnimationScript.enabled = true;
            normalAnimationScript.enabled = false;
        }
        else
        {
            light.color = Color.red;
            huntingAnimationScript.enabled = false;
            normalAnimationScript.enabled = true;
            getNextHuntingPosition();
        }

        StartCoroutine(pathFindingRoutine());

    }

    /**
     *  Selectionne la prochaine position de l'ennemi suivant la postion vue du joueur 
     * 
     */
    private void getNextHuntingPosition()
    {
        //1ere vérification Si l'ennemi a rejoint la position marquée on arrete la chasse
        if (!mapPath[pos[0], pos[1]].Equals(huntingposition))
        {
            //Vecteur entre l'ennemi et la pos marquée
            Vector3 playerVect = huntingposition - transform.position;

            //Calcul des angles entre chaque point cardinal relatif à l'ennemi

            //forward and backward
            float[] anglesZ = {
                Vector3.Angle(playerVect, transform.forward),
                Vector3.Angle(playerVect, -transform.forward)
            };
            //right and left
            float[] anglesX = {
                Vector3.Angle(playerVect, transform.right),
                Vector3.Angle(playerVect, -transform.right),
            };

            int valueZ;
            int angleZ;
            int valueX;
            int angleX;
            if (Mathf.Min(anglesZ) == anglesZ[0])
            {
                valueZ = (int)anglesZ[0];
                angleZ = 0;
            }
            else
            {
                valueZ = (int)anglesZ[1];
                angleZ = 180;
            }

            if (Mathf.Min(anglesX) == anglesX[0])
            {
                valueX = (int)anglesX[0];
                angleX = 90;
            }
            else
            {
                valueX = (int)anglesX[1];
                angleX = 270;
            }
            int rotation = (int)transform.rotation.eulerAngles.y;
            var offset = rotation_offset[(angleZ + rotation) % 360];

            if (isWall(mapPath[pos[0] + offset[1, 0], pos[1] + offset[1, 1]]))
            {

                valueZ = 361;

            }

            offset = rotation_offset[(angleX + rotation) % 360];
            if (isWall(mapPath[pos[0] + offset[1, 0], pos[1] + offset[1, 1]]))
            {

                valueX = 361;

            }

            float min = Mathf.Min(valueX, valueZ);

            if (min == valueZ)
            {
                offset = rotation_offset[(angleZ + rotation) % 360];
                pos[0] += offset[1, 0];
                pos[1] += offset[1, 1];

            }
            else
            {
                offset = rotation_offset[(angleX + rotation) % 360];
                pos[0] += offset[1, 0];
                pos[1] += offset[1, 1];

            }
        }
        else
        {
            hunting = false;
        }
    }

    private Vector3 getNearestHuntingPosition()
    {
        float minDist = 99999;
        Vector3 nearest = new Vector3(-1, -1, -1);
        for (int i = 0; i < mapPath.GetLength(0); i++)
        {
            for (int j = 0; j < mapPath.GetLength(1); j++)
            {
                float tempDist = Vector3.Distance(mapPath[i, j], huntingposition);
                if (tempDist < minDist)
                {
                    minDist = tempDist;
                    nearest = mapPath[i, j];
                }

            }
        }

        return nearest;
    }

    /**
     * 
     * Gere la rotation et les futurs deplacements dans le terrain 
     */
    void getNextRandomPosition()
    {

        // recupere l'angle de rotation actuel
        int rotation = (int)transform.rotation.eulerAngles.y;

        //Chacune des valeur correspond à une direction (0 : gauche, 1: devant, 2,droite).
        //Ces directions sont relatives à l'angle actuel de l'ennemi
        float[] values = { Random.value, Random.value, Random.value };

        //Recupération de l'offset de l'angle
        var offset = rotation_offset[rotation];

        //Ces trois conditionnelles empeche le choix de direction dans des murs 

        //front
        if (isWall(mapPath[pos[0] + offset[1, 0], pos[1] + offset[1, 1]]))
        {
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
            offset = rotation_offset[((int)rotation + 180) % 360];
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

    void FixedUpdate()
    {
        // Masque de layer du joeur et des murs
        int playerMask = 1 << 8;
        int wallsMask = 1 << 9;

        //Recupere le collider du joueur
        Collider[] playerCollider = Physics.OverlapSphere(transform.position, 1000, playerMask);

        //calcul des vecteurs direction et de la distance entre l'ennemi et le joueur
        Vector3 position = new Vector3(transform.position.x, 1, transform.position.z);
        Transform player = playerCollider[0].transform;
        Vector3 direction = (player.position - position).normalized;
        float distance = Vector3.Distance(player.position, position);

        Debug.DrawRay(position, direction * distance, Color.red);
        Debug.DrawRay(position, transform.forward * 10000, Color.yellow);


        RaycastHit hit;
        // Check si le joeur est visible par l'ennemi (à 360deg)
        if (!Physics.Raycast(position, direction, out hit, distance, wallsMask))
        {

            huntingposition = player.position;
            huntingposition = getNearestHuntingPosition();
            hunting = true;

        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.name == "Player")
        {
            EventManager.Instance.Raise(new PlayerHasBeenHitEvent());
        }
    }

}

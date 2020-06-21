using System.Collections;
using UnityEngine;

public class MapManager : Manager<MapManager>
{
    [Header("Map configuration")]
    [SerializeField] private Texture2D[] maps;          // tableau qui contient les différentes maps
    private Texture2D map;                              // variable qui va recevoir la map à générer
    [SerializeField] private GameObject wall;           // l'objet mur
    [SerializeField] private GameObject ground;         // l'objet sol
    [SerializeField] private GameObject book;           // l'objet livre
    [SerializeField] private GameObject ennemy;           // Objet ennemi
    private Vector3[,] mapPath;                         // carte représentant le terrain

    private void Awake()
    {
        base.Awake();
        map = maps[0];                                  // récupération de la map à générer
        Color[] pixels = map.GetPixels();               // extraction des coleurs des pixels dans un tableau

        int mapHeight = map.height;
        int mapWidth = map.width;
        mapPath = new Vector3[mapWidth, mapHeight];


        Vector3[] spawnPositions = new Vector3[pixels.Length];  // tous les spawnpositions possibles (les pixels)
        Vector3 startingSpawnPosition = new Vector3(-Mathf.Round(mapWidth / 2), 0, -Mathf.Round(mapHeight / 2)); // permet de créer la map autour du point de position de départ
        Vector3 currentSpawnPosition = startingSpawnPosition;   // le curseur

        int counter = 0;                                // le compteur qui permet de savoir où on est sur le map pour le déploiement

        for (int z = 0; z < mapHeight; z++)             // la boucle sur les coordonnées Z
        {
            for (int x = 0; x < mapWidth; x++)          // la boucle sur les coordonnées X
            {
                                                         
                mapPath[z, x] =                         // Rempmlissage du terrain par la position si le bloc n'est pas un mur
                    (pixels[counter].Equals(Color.white) ? currentSpawnPosition : new Vector3(-1, -1, -1));
                spawnPositions[counter] = currentSpawnPosition;     //
                counter++;                                          //
                currentSpawnPosition.x++;                           //
            }                                                       // stockage des positions de la map dans le tableau
                                                                    //
            currentSpawnPosition.x = startingSpawnPosition.x;       //
            currentSpawnPosition.z++;                               //
        }

        counter = 0;                                                // reset du compteur
        GameManager.scoreToWin = -1;                                 // initialisation fdu compteur de livre


        foreach (Vector3 pos in spawnPositions)                     // boucle sur les positions
        {
            Color c = pixels[counter];                              // c prend la valeur de la couleur du pixel exploré

            if (c.Equals(Color.black))
            {
                Instantiate(wall, pos, Quaternion.identity);        // s'il est noir, ce sera un mur
            }
            else if (c.Equals(Color.white))                         // s'il est blanc, ce sera un sol + livre
            {
                Instantiate(ground, pos, Quaternion.identity);
                Vector3 bookPos = pos;
                bookPos.y = 0.8f;
                Instantiate(book, bookPos, Quaternion.identity);
                GameManager.scoreToWin++;
            }

            counter++;
        }
    }


    public void setEnnemies(int nbEnnemies)
    {
        for (int i = 0; i < nbEnnemies; i++)
        {
            Instantiate(ennemy, Vector3.zero, Quaternion.identity);
        }
    }



    public Vector3[,] getMapPath() {
        return mapPath;
    }

    protected override IEnumerator InitCoroutine()
    {
        yield break;
    }
}

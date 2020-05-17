using System.Collections;
using System.Collections.Generic;
using Unity.UNetWeaver;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    [Header("Map configuration")]
    [SerializeField] private Texture2D[] maps;          // tableau qui contient les différentes maps
    private Texture2D map;                              // variable qui va recevoir la map à générer
    [SerializeField] private GameObject wall;           // l'objet mur
    [SerializeField] private GameObject ground;         // l'objet sol
    [SerializeField] private GameObject book;           // l'objet livre

    // Start is called before the first frame update
    void Start()
    {
        map = maps[0];                                  // récupération de la map à générer
        Color[] pixels = map.GetPixels();               // extraction des coleurs des pixels dans un tableau

        int mapHeight = map.height;
        int mapWidth = map.width;

        Vector3[] spawnPositions = new Vector3[pixels.Length];  // tous les spawnpositions possibles (les pixels)
        Vector3 startingSpawnPosition = new Vector3(-Mathf.Round(mapWidth / 2), 0, -Mathf.Round(mapHeight / 2)); // permet de créer la map autour du point de position de départ
        Vector3 currentSpawnPosition = startingSpawnPosition;   // le curseur

        int counter = 0;                                // le compteur qui permet de savoir où on est sur le map pour le déploiement

        for (int z = 0; z < mapHeight; z++)             // la boucle sur les coordonnées Z
        {
            for (int x = 0; x < mapWidth; x++)          // la boucle sur les coordonnées X
            {
                spawnPositions[counter] = currentSpawnPosition;     //
                counter++;                                          //
                currentSpawnPosition.x++;                           //
            }                                                       // stockage des positions de la map dans le tableau
                                                                    //
            currentSpawnPosition.x = startingSpawnPosition.x;       //
            currentSpawnPosition.z++;                               //
        }

        counter = 0;                                                // reset du compteur
        
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

            }

            counter++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

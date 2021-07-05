using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

/*public class DoorsGenerator : MonoBehaviour
{
    private List<Node> listOfRooms = new List<Node>();
    private Node mainStructure;
    private List<Vector3Int> possibleDoorVerticalPosition;
    private List<Vector3Int> possibleDoorHorizontalPosition;
    private GameObject doorParent, doorVertical, doorHorizontal;
    

    public DoorsGenerator (List<Node> listOfRooms, Node mainStructure, GameObject doorVertical, GameObject doorHorizontal)
    {
        // confronto lati stanze: x dx sx
        // se un lato di una stanza combacia col perimetro della casa non mette una porta,
        // se un lato di una stanza combacia con un'altra stanza che non è un bagno può mettere una porta, ma deve metterne anche un'altra che non combacia con altre stanze
        // una porta può essere inserita solo c'è spazio sufficiente tra le stanze adiacenti
        // numero di porta per stanza?
        this.listOfRooms = listOfRooms;
        this.mainStructure = mainStructure;
        this.doorHorizontal = doorHorizontal;
        this.doorHorizontal = doorVertical;

        GameObject doorParent = new GameObject("DoorParent");
        doorParent.transform.parent = transform;
        doorParent.transform.position = Vector3.zero;
        possibleDoorVerticalPosition = new List<Vector3Int>();
        possibleDoorHorizontalPosition = new List<Vector3Int>();


    }

    public void Generator(Mesh room, List<Node> listOfRooms)
    {
        for (int i = 0; i < listOfRooms.Count; i++)
        {
            if (room.bounds.center.x != mainStructure.BottomLeftAreaCorner.x)
            {
                Vector3Int middlePoint = CalculateMiddlePoint(room.bounds.center.x, room.bounds.center.y);
                CreateDoor(doorParent, middlePoint, doorHorizontal);
            }
        }
        
    }

    private Vector3Int CalculateMiddlePoint(float v1, float v2)
    {
        float sum = v1 + v2;
        float tempVector = sum / 2;
        Vector3Int v = new Vector3Int((int)tempVector, (int)tempVector, 0);
        return v;
    }

    private void CreateDoor(GameObject doorParent, Vector3Int doorPosition, GameObject doorPrefab)
    {
        Instantiate(doorPrefab, doorPosition, Quaternion.identity, doorParent.transform);
    }
}
*/
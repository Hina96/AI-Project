using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorCreator : MonoBehaviour
{
    private List<Node> listOfRooms = new List<Node>();
    private Node mainStructure;
    private List<Vector3Int> possibleDoorVerticalPosition;
    private List<Vector3Int> possibleDoorHorizontalPosition;
    private GameObject doorParent, doorVertical, doorHorizontal;
    public DoorCreator()
    {
        GameObject doorParent = new GameObject("DoorParent");
        doorParent.transform.parent = transform;
        doorParent.transform.position = Vector3.zero;
        possibleDoorVerticalPosition = new List<Vector3Int>();
        possibleDoorHorizontalPosition = new List<Vector3Int>();

    }

}

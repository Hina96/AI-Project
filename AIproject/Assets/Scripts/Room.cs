using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
   
    private int widthMax;
    private int lengthMax;
    private int typeOfRoom; 

    public Room(int widthMax, int lengthMax, int typeOfRoom)
    {
        this.widthMax = widthMax;
        this.lengthMax = lengthMax;
        this.typeOfRoom = typeOfRoom;
    }

    public int LengthMax { get => lengthMax; set => lengthMax = value; }
    public int WidthMax { get => widthMax; set => lengthMax = value; }
    public int TypeOfRoom { get => typeOfRoom; }
}

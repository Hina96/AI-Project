using UnityEngine;
public class RoomNode : Node
{
    private int typeOfRoom;

    public RoomNode(Vector2Int bottomLeftAreaCorner, Vector2Int topRightAreaCorner, Node parentNode, int index, int typeOfRoom) : base(parentNode)
    {
        // Creation of a rectangle

        this.BottomLeftAreaCorner = bottomLeftAreaCorner;
        this.TopRightAreaCorner = topRightAreaCorner;
        this.BottomRightAreaCorner = new Vector2Int(topRightAreaCorner.x, bottomLeftAreaCorner.y);
        this.TopLeftAreaCorner = new Vector2Int(bottomLeftAreaCorner.x, TopRightAreaCorner.y);
        this.TreeLayerIndex = index;
        this.typeOfRoom = typeOfRoom;
       
    }

    public int Width { get => (int)(TopRightAreaCorner.x - BottomLeftAreaCorner.x); }
    public int Length { get => (int)(TopRightAreaCorner.y - BottomLeftAreaCorner.y); }

    // Parameter that will be used for future implementation

    public int TypeOfRoom { get => typeOfRoom; set => typeOfRoom = value; }
}
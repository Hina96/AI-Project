using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator
{
    private int maxIterations;
    private int roomLengthMin;
    private int roomWidthMin;

    public RoomGenerator(int maxIterations, int roomLengthMin, int roomWidthMin)
    {
        this.maxIterations = maxIterations;
        this.roomLengthMin = roomLengthMin;
        this.roomWidthMin = roomWidthMin;
    }

    public List<RoomNode> GenerateRoomsInGivenSpaces(List<Node> roomSpaces, float roomBottomCornerModifier, float roomTopCornerModifier, int roomOffset, List<Room> roomsRequired )
    {
       List<RoomNode> listToReturn = new List<RoomNode>();
        
        int count = roomsRequired.Count;
        
        try
        {
           
                foreach (var space in roomSpaces)
                {
                    
                   

                    if (count <= 0) break;


                    Vector2Int newBottomLeftPoint = StructureHelper.GenerateBottomLeftCornerBetween(
                    space.BottomLeftAreaCorner, space.TopRightAreaCorner, roomBottomCornerModifier, roomOffset);

                    Vector2Int newTopRightPoint = StructureHelper.GenerateTopRightCornerBetween(
                    space.BottomLeftAreaCorner, space.TopRightAreaCorner, roomTopCornerModifier, roomOffset);

                    space.BottomLeftAreaCorner = newBottomLeftPoint;
                    space.TopRightAreaCorner = newTopRightPoint;
                    space.BottomRightAreaCorner = new Vector2Int(newTopRightPoint.x, newBottomLeftPoint.y);
                    space.TopLeftAreaCorner = new Vector2Int(newBottomLeftPoint.x, newTopRightPoint.y);

                    RoomNode roomSpace = (RoomNode)space;
                    
                   
                    
                    listToReturn.Add(roomSpace);
                       count --;
                     

                  
                }
            
        }
      






        catch (Exception e)
        {
            Debug.LogException(e);
            Debug.Log("WARNING: Change the parameters and try again.");
        }

        if (count != 0)
        {
            
            Debug.Log("It was not possible to insert " + count + " rooms. Try again to generate a new room configuration. \n");
        }
        return listToReturn;

    }
       
            
        
        
    }


   
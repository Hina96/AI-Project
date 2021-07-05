using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class HomeGenerator
{
    
    List<RoomNode> allNodesCollection = new List<RoomNode>();
    

    private int homeWidth;
    private int homeLength;

    public HomeGenerator(int homeWidth, int homeLength)
    {
        this.homeWidth = homeWidth;
        this.homeLength = homeLength;
    }



    public List<Node> CalculateHome(int maxIterations, int roomWidthMin, int roomLengthMin, float roomBottomCornerModifier, float roomTopCornerMidifier, int roomOffset,  List<Room> roomsRequired)
    {
        // Creation of the binary space partitioner

        BinarySpacePartitioner bsp = new BinarySpacePartitioner(homeWidth, homeLength);
        allNodesCollection = bsp.PrepareNodesCollection(maxIterations, roomWidthMin, roomLengthMin);
       
        // Extraction of spaces rapresenting the room, all the children that are childern-less

        List<Node> roomSpaces = StructureHelper.TraverseGraphToExtractLowestLeafes(bsp.RootNode);
   
        RoomGenerator roomGenerator = new RoomGenerator(maxIterations, roomLengthMin, roomWidthMin);
        List<RoomNode> roomList = new List<RoomNode>();
        roomList = roomGenerator.GenerateRoomsInGivenSpaces(roomSpaces, roomBottomCornerModifier, roomTopCornerMidifier, roomOffset, roomsRequired);

        return new List<Node>(roomList).ToList();
    }
}
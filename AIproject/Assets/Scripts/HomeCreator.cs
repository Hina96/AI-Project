using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HomeCreator : MonoBehaviour
{


    [Range(6.0f, 100)]
    public int homeWidth, homeLength;
    [Range(3.0f, 100)]
    public int averageRoomWidth, averageRoomLength;
    public int maxIterations;
    public int numberOfRooms;
    public int numberOfWindows;

    [Range(0.0f, 0.3f)]
    public float roomBottomCornerModifier;
    [Range(0.7f, 1.0f)]
    public float roomTopCornerMidifier;
    [Range(0, 2)]
    public int roomOffset;
    public Material floorMaterial;
    public GameObject wallVertical, wallHorizontal;
    
    List<Vector3Int> possibleWallHorizontalPosition;
    List<Vector3Int> possibleWallVerticalPosition;
    Mesh home;
    bool start;
    List<Mesh> allRooms;
    
    public GameObject  doorVertical, doorHorizontal;
    public GameObject windowVertical, windowHorizontal;


    void Start()
    {
        CreateHome();
    }

    public void CreateHome()
    {
        DestroyAllChildren();
        int roomLength = averageRoomLength;
        int roomWidth = averageRoomWidth;

        if (roomOffset == 1)
        {
            if (averageRoomWidth < 4) roomWidth = 4;
            if (averageRoomLength < 4) roomLength = 4;
        }

        if (roomOffset == 2)
        {
            if (averageRoomWidth < 7) roomWidth = 7;
            if (averageRoomLength < 7) roomLength = 7;
        }
        int nRoom = numberOfRooms;
        if ( ((nRoom) * (((roomLength + roomOffset) + (roomLength* roomTopCornerMidifier)) * ((roomWidth + roomOffset)+  (roomWidth * roomBottomCornerModifier)))) >= ((homeWidth * homeLength)) ) 
        {
            Debug.Log("WARNING: Parameters must be changed. There is no possible configuration. \n ");
            return;

        }

        if (roomLength > homeLength || roomWidth > homeWidth)
        {
            Debug.Log("WARNING: The size of the rooms must be changed. \n");
            return;
        }
      
     
        HomeGenerator generator = new HomeGenerator(homeWidth , homeLength);

        allRooms = new List<Mesh>();
        List<Room> roomsRequired = new List<Room>();
        

        if (numberOfRooms > 0)
        {
            
            for (int i = 0; i < numberOfRooms ; i++)
            {
                
                roomsRequired.Add(new Room(roomWidth,  roomLength, 1));
            }
        }

       
       var listOfRooms = generator.CalculateHome(maxIterations,
            roomWidth,
            roomLength,
            roomBottomCornerModifier,
            roomTopCornerMidifier,
            roomOffset,
            roomsRequired);


        GameObject doorParent = new GameObject("DoorParent");
        doorParent.transform.parent = transform;
        doorParent.transform.position = Vector3.zero;
       

        GameObject windowParent = new GameObject("WindowParent");
        windowParent.transform.parent = transform;
        windowParent.transform.position = Vector3.zero;
        

        GameObject wallParent = new GameObject("WallParent");
        wallParent.transform.parent = transform;
        possibleWallHorizontalPosition = new List<Vector3Int>();
        possibleWallVerticalPosition = new List<Vector3Int>();


        // Creation of the mesh of each room

        for (int i = 0; i < listOfRooms.Count; i++)
        {
            CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner,  doorParent, roomsRequired[i].TypeOfRoom);
            
        }

        // Creation of the walls of each room

        CreateWalls(wallParent);

        // Creation of the mesh of the house, walls, windows and main door

        start = false;
        RoomNode rootNode = new RoomNode(new Vector2Int(0, 0), new Vector2Int(homeWidth  , homeLength ), null, 0, 0);
        Vector2Int topRightAreaCorner = new Vector2Int(rootNode.TopRightAreaCorner.x , rootNode.TopRightAreaCorner.y);
        CreateMesh(rootNode.BottomLeftAreaCorner, topRightAreaCorner, doorParent, -1);
      
        Vector3 mainDoor = MainDoor(doorParent);

        WindowsGenerator(windowParent, mainDoor);

        CreateWalls(wallParent);
        
        
        
       
    }

    private void CreateWalls(GameObject wallParent)
    {
        foreach (var wallPosition in possibleWallHorizontalPosition)
        {
            
            CreateWall(wallParent, wallPosition, wallHorizontal);
            
        }
        foreach (var wallPosition in possibleWallVerticalPosition)
        {
            CreateWall(wallParent, wallPosition, wallVertical);
        }
    }

 

    private void CreateWall(GameObject wallParent, Vector3Int wallPosition, GameObject wallPrefab)
    {
        
        Instantiate(wallPrefab, wallPosition, Quaternion.identity, wallParent.transform);
    }

    private void CreateDoor(GameObject doorParent, Vector3 doorPosition, GameObject doorPrefab)
    {
        Instantiate(doorPrefab, doorPosition, Quaternion.identity, doorParent.transform);
    }

    private void CreateWindow(GameObject windowParent, Vector3 windowPosition, GameObject windowPrefab)
    {
        Instantiate(windowPrefab, windowPosition, Quaternion.identity, windowParent.transform);
    }


    private void CreateMesh(Vector2 bottomLeftCorner, Vector2 topRightCorner, GameObject doorParent, int typeOfRoom)
    {
        Vector3 bottomLeftV = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
        Vector3 bottomRightV = new Vector3(topRightCorner.x, 0, bottomLeftCorner.y);
        Vector3 topLeftV = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
        Vector3 topRightV = new Vector3(topRightCorner.x, 0, topRightCorner.y);
      
        Vector3[] vertices = new Vector3[]
        {
            topLeftV,
            topRightV,
            bottomLeftV,
            bottomRightV
        };

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        int[] triangles = new int[]
        {
            0,
            1,
            2,
            2,
            1,
            3
        };
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        

        GameObject homeFloor = new GameObject("Mesh" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer));

        homeFloor.transform.position = Vector3.zero;
        homeFloor.transform.localScale = Vector3.one;
        homeFloor.GetComponent<MeshFilter>().mesh = mesh;
        homeFloor.GetComponent<MeshRenderer>().material = floorMaterial;
        homeFloor.transform.parent = transform;

        // Mesh of the house is saved

        if (!start)
        {
            home = mesh;
            start = true;
        }

        // For each mesh of the rooms, the method DoorsGenerator is called

        else
        {

            DoorsGenerator(mesh, doorParent);
            allRooms.Add(mesh);
        }

        for (int row = (int)bottomLeftV.x; row < (int)bottomRightV.x ; row++)
        {
          
            
            var wallPosition = new Vector3(row, 0, bottomLeftV.z );
            
            AddWallPositionToList(wallPosition, possibleWallHorizontalPosition, typeOfRoom);
            

        }
       
       
        for (int row = (int)topLeftV.x; row < (int)topRightCorner.x; row++)
        {
            var wallPosition = new Vector3(row, 0, topRightV.z);
            AddWallPositionToList(wallPosition, possibleWallHorizontalPosition, typeOfRoom);
        }
        for (int col = (int)bottomLeftV.z; col < (int)topLeftV.z; col++)
        {
            var wallPosition = new Vector3(bottomLeftV.x, 0, col);
            AddWallPositionToList(wallPosition, possibleWallVerticalPosition, typeOfRoom );
        }
        for (int col = (int)bottomRightV.z; col < (int)topRightV.z; col++)
        {
            var wallPosition = new Vector3(bottomRightV.x, 0, col);
            AddWallPositionToList(wallPosition, possibleWallVerticalPosition, typeOfRoom);
        }
    }

   

    
    private void AddWallPositionToList(Vector3 wallPosition, List<Vector3Int> wallList, int typeOfRoom)
    {
        Vector3Int point = Vector3Int.CeilToInt(wallPosition);
       
        wallList.Add(point);
    }

    public Vector3 MainDoor (GameObject doorParent)
    {
        List<Vector3> roomVertices = RoomPosition();
        
        Vector3[] homeVertices = home.vertices;
        Vector3 position = new Vector3();
        bool mainDoor = false;
        int edge = Random.Range(0, 4);
        List<int> exceptions = new List<int>();

        while (!mainDoor)
        {
            if (edge == 0 && !mainDoor) // Top edge
            {
                for (int i = 2; i < homeVertices[1].x - 2; i++)
                {
                    
                    Vector3 pos = new Vector3(i, 1, homeVertices[1].z);
                    Vector3 space = new Vector3(i, 1, homeVertices[1].z - 1);
                    if (!roomVertices.Contains(pos) && !roomVertices.Contains(space))
                    {
                        position = new Vector3(pos.x, 0, pos.z);
                        CreateDoor(doorParent, position, doorHorizontal);
                        mainDoor = true;
                        break;
                    }
                }
            }
            exceptions.Clear();
            exceptions.Add(0);
            edge = randomIntExcept(0, 4, exceptions);

            if (edge == 1 && !mainDoor) // Right edge
            {
                for (int i = 2; i < homeVertices[3].z - 2; i++)
                {

                    Vector3 pos = new Vector3(homeVertices[1].x, 1, i);
                    Vector3 space = new Vector3(homeVertices[1].x - 1, 1, i);
                    if (!roomVertices.Contains(pos) && !roomVertices.Contains(space))
                    {
                        position = new Vector3(pos.x, 0, pos.z);
                        CreateDoor(doorParent, position, doorVertical);
                        mainDoor = true;
                        break;
                    }
                }
            }
            exceptions.Clear();
            exceptions.Add(1);
            edge = randomIntExcept(0, 4, exceptions);

            if (edge == 2 && !mainDoor) // Bottom edge
            {
                for (int i = 2; i < homeVertices[3].x -2; i++)
                {

                    Vector3 pos= new Vector3(i, 1, homeVertices[2].z);
                    Vector3 space = new Vector3(i, 1, homeVertices[2].z + 1);
                    
                    if (!roomVertices.Contains(pos) && !roomVertices.Contains(space))
                    {
                        position = new Vector3(pos.x, 0, pos.z);
                        CreateDoor(doorParent, position, doorHorizontal);
                        mainDoor = true;
                        break;
                    }
                }
            }

            if (edge == 3 && !mainDoor) // Left edge
            {
                for (int i = 2; i < homeVertices[0].z - 2; i++)
                {

                   Vector3 pos = new Vector3(homeVertices[2].x, 1, i);
                    Vector3 space = new Vector3(homeVertices[2].x + 1 , 1, i);
                    if (!roomVertices.Contains(pos) && !roomVertices.Contains(space))
                    {
                        position = new Vector3(pos.x, 0, pos.z);
                        CreateDoor(doorParent, position, doorVertical);
                        mainDoor = true;
                        break;
                    }
                }
            }
            exceptions.Clear();
            exceptions.Add(3);
            edge = randomIntExcept(0, 4, exceptions);
        }
        
        return position;
     }
    public void DoorsGenerator(Mesh room, GameObject doorParent)
    {

        bool doorInserted = false;
        int edge = Random.Range(0,4);
        List<int> exceptions = new List<int>();
        
        Vector3[] roomVertices = room.vertices;
        Vector3[] homeVertices = home.vertices;

        List<Vector3> doorPosition = new List<Vector3>();
        bool checkPosition;
        doorInserted = false;
        checkPosition = false;

       

        while (!doorInserted)
         {

             checkPosition = true;

             if (edge == 0) // Top edge 
             {

                 Vector3 middlePoint = CalculateMiddlePoint(roomVertices[0], roomVertices[1]);
                
               
                for (int i = 0; i < homeVertices[1].x; i++)
                 {
                     if (!checkPosition)
                     {
                         break;
                     }
                     for (int j = 0; j < roomVertices[1].x; j++)
                     {
                         float p1 = homeVertices[0].z;
                         float p2 = roomVertices[0].z;

                        if ((i == j && p1 == p2))
                        {

                            exceptions.Add(0);
                            edge = randomIntExcept(0, 4, exceptions);
                            checkPosition = false;
                            break;

                        }
                        for (float k = 0; k < 1; k += 0.1f)
                        {
                            Vector3 nextPoint = new Vector3(middlePoint.x + k, middlePoint.y, middlePoint.z);
                            Vector3 previousPoint = new Vector3(middlePoint.x - k, middlePoint.y, middlePoint.z);
                            if ( doorPosition.Contains(nextPoint) || doorPosition.Contains(previousPoint))
                            {
                                exceptions.Add(0);
                                edge = randomIntExcept(0, 4, exceptions);
                                checkPosition = false;
                                break;
                            }
                        }
                        
                     }
                 }


                 if (checkPosition)
                 {

                     CreateDoor(doorParent, middlePoint, doorHorizontal);
                     doorPosition.Add(middlePoint);
                     doorInserted = true;
                 }




             }

             if (edge == 1) // Right edge
             {
                 Vector3 middlePoint = CalculateMiddlePoint(roomVertices[1], roomVertices[3]);
               
                
                for (int i = 0; i < homeVertices[1].z; i++)
                 {
                     if (!checkPosition)
                     {
                         break;
                     }
                     for (int j = 0; j < roomVertices[1].z; j++)
                     {
                         float p1 = homeVertices[3].x;
                         float p2 = roomVertices[3].x;
                         if ((i == j && p1 == p2) )
                         {
                             exceptions.Add(1);
                             edge = randomIntExcept(0, 4, exceptions);
                             checkPosition = false;
                             break;

                         }
                        for (float k = 0; k < 1; k += 0.1f)
                        {
                            Vector3 nextPoint = new Vector3(middlePoint.x, middlePoint.y, middlePoint.z + k);
                            Vector3 previousPoint = new Vector3(middlePoint.x , middlePoint.y, middlePoint.z - k);
                            if (doorPosition.Contains(nextPoint) || doorPosition.Contains(previousPoint))
                            {
                                exceptions.Add(0);
                                edge = randomIntExcept(0, 4, exceptions);
                                checkPosition = false;
                                break;
                            }
                        }
                    }
                 }

                 if (checkPosition)
                 {

                     CreateDoor(doorParent, middlePoint, doorVertical);
                     doorPosition.Add(middlePoint);
                     doorInserted = true;
                 }

             }


             if (edge == 2) // Bottom edge
             {
                 Vector3 middlePoint = CalculateMiddlePoint(roomVertices[2], roomVertices[3]);
              
                for (int i = 0; i < homeVertices[3].x; i++)
                 {
                     if (!checkPosition)
                     {
                         break;
                     }
                     for (int j = 0; j < roomVertices[3].x; j++)
                     {
                         float p1 = homeVertices[2].z;
                         float p2 = roomVertices[2].z;
                         if ((i == j && p1 == p2) )
                         {
                             exceptions.Add(2);
                             edge = randomIntExcept(0, 4, exceptions);
                             checkPosition = false;
                             break;

                         }
                        for (float k = 0; k < 1; k += 0.1f)
                        {
                            Vector3 nextPoint = new Vector3(middlePoint.x + k, middlePoint.y, middlePoint.z);
                            Vector3 previousPoint = new Vector3(middlePoint.x - k, middlePoint.y, middlePoint.z);
                            if (doorPosition.Contains(middlePoint) || doorPosition.Contains(nextPoint) || doorPosition.Contains(previousPoint))
                            {
                                exceptions.Add(0);
                                edge = randomIntExcept(0, 4, exceptions);
                                checkPosition = false;
                                break;
                            }
                        }
                    }
                 }
                 if (checkPosition)
                 {

                     CreateDoor(doorParent, middlePoint, doorHorizontal);
                     doorPosition.Add(middlePoint);
                     doorInserted = true;
                 }

             }
             if (edge == 3) // Left edge
                 {
                 Vector3 middlePoint = CalculateMiddlePoint(roomVertices[0], roomVertices[2]);
             
                for (int i = 0; i < homeVertices[0].z; i++)
                     {
                         if (!checkPosition)
                         {
                             break;
                         }
                         for (int j = 0; j < roomVertices[0].z; j++)
                         {
                             float p1 = homeVertices[2].x;
                             float p2 = roomVertices[2].x;
                             if ((i == j && p1 == p2) )
                             {
                                 exceptions.Add(3);
                                 edge = randomIntExcept(0, 4, exceptions);
                                 checkPosition = false;
                                 break;

                             }
                        for (float k = 0; k < 1; k+= 0.1f)
                        {
                            Vector3 nextPoint = new Vector3(middlePoint.x , middlePoint.y, middlePoint.z + k);
                            Vector3 previousPoint = new Vector3(middlePoint.x , middlePoint.y, middlePoint.z - k);
                            if (doorPosition.Contains(nextPoint) || doorPosition.Contains(previousPoint))
                            {
                                exceptions.Add(0);
                                edge = randomIntExcept(0, 4, exceptions);
                                checkPosition = false;
                                break;
                            }
                        }
                    }
                     }

                     if (checkPosition)
                     {

                         CreateDoor(doorParent, middlePoint, doorVertical);
                     doorPosition.Add(middlePoint);
                         doorInserted = true;
                     }
                 }


         }
    }

    public void WindowsGenerator(GameObject windowParent, Vector3 mainDoor)
    {
        Vector3[] homeVertices = home.vertices;
       
        bool checkPosition = false;
        int num = numberOfWindows;

        foreach (Mesh mesh in allRooms)
        {

            Vector3[] roomVertices = mesh.vertices;

            for (int i = 0; i < homeVertices[1].x; i++) // Top edge
            {
                if (checkPosition) break;

                for (int j = 0; j < roomVertices[1].x; j++)
                {
                    float p1 = homeVertices[0].z;
                    float p2 = roomVertices[0].z;
                        
                        if (i == j && p1 == p2  && num > 0 )
                    {
                            Vector3 middlePoint = CalculateMiddlePoint(roomVertices[0], roomVertices[1]);
                        Vector3 point = new Vector3(middlePoint.x, 1, middlePoint.z);
                        
                        CreateWindow(windowParent, point, windowHorizontal);
                        
                        num--;
                        checkPosition = true;
                        break;
                    }

                }
            }
            checkPosition = false;
            for (int i = 0; i < homeVertices[3].x; i++) // Bottom edge
            {
                if (checkPosition) break;
                for (int j = 0; j < roomVertices[3].x; j++)
                {
                    float p1 = homeVertices[2].z;
                    float p2 = roomVertices[2].z;
                        
                        if (i == j && p1 == p2 && num > 0 )
                    {

                            Vector3 middlePoint = CalculateMiddlePoint(roomVertices[2], roomVertices[3]);
                        Vector3 point = new Vector3(middlePoint.x, 1, middlePoint.z);
                        CreateWindow(windowParent, point, windowHorizontal);
                        
                        num--;
                        checkPosition = true;
                        break;
                    }

                }
            }
            checkPosition = false;
            for (int i = 0; i < homeVertices[0].z; i++) // Left edge
            {
                if (checkPosition) break;

                for (int j = 0; j < roomVertices[0].z; j++)
                {
                    float p1 = homeVertices[2].x;
                    float p2 = roomVertices[2].x;
                       
                        if (i == j && p1 == p2 && num > 0)
                    {
                            Vector3 middlePoint = CalculateMiddlePoint(roomVertices[0], roomVertices[2]);
                        Vector3 point = new Vector3(middlePoint.x, 1, middlePoint.z);
                        CreateWindow(windowParent, point, windowVertical);
                       
                        num--;
                        checkPosition = true;
                        break;
                    }

                }
            }
            checkPosition = false;
            for (int i = 0; i < homeVertices[1].z; i++) // Right edge
            {

                if (checkPosition) break;
                for (int j = 0; j < roomVertices[1].z; j++)
                {
                    float p1 = homeVertices[3].x;
                    float p2 = roomVertices[3].x;
                       
                        if (i == j && p1 == p2 && num > 0 )
                    {
                            Vector3 middlePoint = CalculateMiddlePoint(roomVertices[1], roomVertices[3]);
                        Vector3 point = new Vector3(middlePoint.x, 1, middlePoint.z);
                            CreateWindow(windowParent, point, windowVertical);
                        
                        num--;
                        checkPosition = true;
                        break;
                    }

                }
            }



        }

            checkPosition = false;
           

        if (num > 0)
        {
            MoreWindows(num, windowParent, mainDoor);
        }
    }

    public void MoreWindows(int windows, GameObject windowParent, Vector3 mainDoor)
    {

        int edge = Random.Range(0, 4);
        
        Vector3[] homeVertices = home.vertices;
        List<Vector3Int> windowsPosition = new List<Vector3Int>();
       
        List<int> exceptions = new List<int>();
        List<Vector3> roomPosition = new List<Vector3>();
        Vector3Int window = new Vector3Int();
        roomPosition = RoomPosition();
       
        int offsetWidth = 0;
        int offsetLength = 0;
        int count = 0;
        int numberWindows = windows;
        int length = 0;
        int width = 0;

        while (count < homeLength ){
            length += 2;
            count += 4;
            numberWindows--;
            if (numberWindows <= 0 || numberWindows <= windows/2) break;
        }
       
        count = 0;
        while (count < homeWidth)
        {
            width += 2;
            count += 4;
             numberWindows--;
            if (numberWindows <= 0) break;
        }
        
        offsetLength = homeLength / length;
        offsetWidth = homeWidth / width;
        if (offsetLength == 1) {  offsetLength = homeLength / (length/2); }
        if (offsetWidth == 1) { offsetWidth = homeWidth / (width / 2); }
        

        int maxWindows = MaxWindows(windows);
        int w = 0;
        
        List<int> stop = new List<int>();
        

         for (int k = 0; k <= maxWindows; k++) {
            

            if (edge == 0) // Top edge
                {
                int middle = ((int)homeVertices[1].x / 2) - offsetWidth;
                int end = (int)homeVertices[1].x - offsetWidth;
                for (int i = middle; i <= end; i += offsetWidth)
                {



                    if (w >= maxWindows) break;
                    if (windows == 0) return;
                    window = new Vector3Int((int)i, 1, (int)homeVertices[1].z);
                    if (!roomPosition.Contains(window) && !windowsPosition.Contains(window) && (window.x != mainDoor.x ))
                    {


                        CreateWindow(windowParent, window, windowHorizontal);
                        windowsPosition.Add(window);


                        windows--;
                        w++;

                        break;
                    }
                    if (i + offsetWidth >= end)
                    {
                        
                        for (int j = offsetWidth; j <= middle - offsetWidth; j += offsetWidth)
                        {
                            if (w >= maxWindows) break;
                            if (windows == 0) return;
                            window = new Vector3Int((int)j, 1, (int)homeVertices[1].z);

                            if (!roomPosition.Contains(window) && !windowsPosition.Contains(window) && (window.x != mainDoor.x ))
                            {


                                CreateWindow(windowParent, window, windowHorizontal);
                                windowsPosition.Add(window);


                                windows--;
                                w++;

                            }
                        }

                    }
                }
               exceptions.Clear();
                exceptions.Add(0);
                edge = randomIntExcept(0, 4, exceptions);
                
            }
                
                
                if (edge == 1) // Right edge
                {
                int end = (int)homeVertices[1].z - offsetLength;
                int middle = ((int)homeVertices[1].z / 2) - offsetLength;
                for (int i = middle; i <= end; i += offsetLength)
                    {
                      
                            if (windows == 0 ) return;
                    if (w >= maxWindows) break;
                    window = new Vector3Int((int)homeVertices[1].x , 1,  (int) i );
                            if ( !roomPosition.Contains(window) && !windowsPosition.Contains(window) && (window.z != mainDoor.z ))
                            {
                                
                                CreateWindow(windowParent, window, windowVertical);
                                
                                windowsPosition.Add(window);
                        
                        windows--;
                        w++;
                       
                        break;
                            }
                    if (i + offsetLength >= end)
                    {

                        for (int j = offsetLength; j <= middle - offsetLength; j += offsetLength)
                        {
                            if (w >= maxWindows) break;
                            if (windows == 0) return;
                            window = new Vector3Int((int)homeVertices[1].x, 1, (int)j);
                            
                            if (!roomPosition.Contains(window) && !windowsPosition.Contains(window) && (window.z != mainDoor.z ))
                            {


                                CreateWindow(windowParent, window, windowVertical);
                                windowsPosition.Add(window);


                                windows--;
                                w++;

                            }
                        }

                    }
                }


                exceptions.Clear(); 
                exceptions.Add(1);
               
                edge = randomIntExcept(0, 4, exceptions);
            }
                
                if (edge == 2) // Bottom edge
                {
                int end = (int)homeVertices[3].x - offsetWidth;
                int middle = ((int)homeVertices[3].x / 2) - offsetWidth;
                for (int i = middle; i <= end ; i += offsetWidth)
                    {
                        
                        
                            if (windows == 0 ) return;
                    if (w >= maxWindows) break;
                     window = new Vector3Int((int)i, 1, (int)homeVertices[3].z);
                   
                    
                            if (!roomPosition.Contains(window) && !windowsPosition.Contains(window)&& (window.x != mainDoor.x ))
                            {
                                
                                
                                CreateWindow(windowParent, window, windowHorizontal);
                        
                                windowsPosition.Add(window);
                        
                        windows--;
                        w++;
                      
                        break;
                            }

                    if (i + offsetWidth >= end)
                    {

                        for (int j = offsetWidth; j <= middle - offsetWidth; j += offsetWidth)
                        {
                            if (w >= maxWindows) break;
                            if (windows == 0) return;
                            window = new Vector3Int((int)j, 1, (int)homeVertices[3].z);
                            
                            if (!roomPosition.Contains(window) && !windowsPosition.Contains(window) && (window.x != mainDoor.x ))
                            {


                                CreateWindow(windowParent, window, windowHorizontal);
                                windowsPosition.Add(window);


                                windows--;
                                w++;

                            }
                        }

                    }
                }

                exceptions.Clear();
                exceptions.Add(2);
                
                edge = randomIntExcept(0, 4, exceptions);
            }
                

                if (edge == 3) // Left edge
                {
                int end =(int) homeVertices[0].z - offsetLength;
                int middle = ((int)homeVertices[0].z / 2) - offsetLength;
                    for (int i = middle; i <= end; i += offsetLength)
                    {
                       
                            if (windows == 0) return;
                    if (w >= maxWindows) break;
                   window = new Vector3Int((int)homeVertices[0].x, 1, (int) i );
                    
                    if (  !roomPosition.Contains(window) && !windowsPosition.Contains(window) &&  (window.z != mainDoor.z ))
                        
                            {

                        
                        CreateWindow(windowParent, window, windowVertical);
                               
                                windowsPosition.Add(window);
                       
                        windows--;
                        w++;
                        
                       
                        break;
                            }
                    if (i + offsetLength >= end)
                    {

                        for (int j = offsetLength; j <= middle - offsetLength; j += offsetLength)
                        {
                            
                            if (w >= maxWindows) break;
                            if (windows == 0) return;
                            window = new Vector3Int((int)homeVertices[0].x, 1, (int) j);
                            
                            if (!roomPosition.Contains(window) && !windowsPosition.Contains(window) && (window.z != mainDoor.z ))
                            {


                                CreateWindow(windowParent, window, windowVertical);
                                windowsPosition.Add(window);


                                windows--;
                                w++;

                            }
                        }

                    }
                }

                 exceptions.Clear();
                exceptions.Add(3);
                
                    edge = randomIntExcept(0, 4, exceptions);
            }


           
            }


        if (windows > 0) Debug.Log("It was not possible to insert " + windows + " windows due to lack of space. \n");

    }

    private int MaxWindows(int windows)
    {
        Vector3[] homeVertices = home.vertices;
        List<Vector3> roomPosition = new List<Vector3>();
        
        int maxWindows = 0;
        int count = 0;
        int offsetLength = 0;
        int offsetWidth = 0;
        int width = 0;
        int length = 0;
        int numberWindows = windows;


        while (count < homeLength)
        {
            length += 2;
            count += 4;
            numberWindows--;
            if (numberWindows <= 0 || numberWindows <= windows / 2) break;
        }
        
        count = 0;
        while (count < homeWidth)
        {
            width += 2;
            count += 4;
            numberWindows--;
            if (numberWindows <= 0) break;
        }
       

        offsetLength = homeLength / length;
        offsetWidth = homeWidth / width;
        if (offsetLength == 1) { offsetLength = homeLength / 4; }
        if (offsetLength == 1) { offsetWidth = homeWidth / 4; }

        int middle = ((int)homeVertices[1].x / 2) - offsetWidth;
        int end = (int)homeVertices[1].x - offsetWidth;

        for (int i = middle; i <= end; i += offsetWidth)
        {



            Vector3Int window = new Vector3Int((int)i, 1, (int)homeVertices[1].z);
            if (!roomPosition.Contains(window))
            {

                maxWindows++;
            }


        }


        for (int i = offsetWidth; i <= middle - offsetWidth; i += offsetWidth)
        {



            Vector3Int window = new Vector3Int((int)i, 1, (int)homeVertices[1].z);
            if (!roomPosition.Contains(window))
            {


                maxWindows++;
            }


        }

        middle = ((int)homeVertices[1].z / 2) - offsetLength;
        end = (int)homeVertices[1].z - offsetLength;

        for (int i = middle; i <= end; i += offsetLength)
        {


            Vector3Int window = new Vector3Int((int)homeVertices[1].x, 1, (int)i);
            if (!roomPosition.Contains(window))
            {
                maxWindows++;
            }

        }

        for (int i = offsetLength; i <= middle - offsetLength; i += offsetLength)
        {


            Vector3Int window = new Vector3Int((int)homeVertices[1].x, 1, (int)i);
            if (!roomPosition.Contains(window))
            {
                maxWindows++;
            }

        }

        middle = ((int)homeVertices[3].x / 2) - offsetWidth;
        end = (int)homeVertices[3].x - offsetWidth;

        for (int i = middle; i <= end ; i += offsetWidth)
                {


                    
                    Vector3Int window = new Vector3Int((int)i, 1, (int)homeVertices[3].z);
                    if (!roomPosition.Contains(window))
            {
                maxWindows++;
            }

                }


        for (int i = offsetWidth; i <= middle - offsetWidth; i += offsetWidth)
        {



            Vector3Int window = new Vector3Int((int)i, 1, (int)homeVertices[3].z);
            if (!roomPosition.Contains(window))
            {
                maxWindows++;
            }

        }


        middle = ((int)homeVertices[0].z / 2) - offsetLength;
        end = (int)homeVertices[0].z - offsetLength;


        for (int i = middle; i <= end; i += offsetLength)
                {

                    ;
                    Vector3Int window = new Vector3Int((int)homeVertices[0].x, 1, (int)i);
            if (!roomPosition.Contains(window))
            {
                maxWindows++;
            }

        }

        for (int i = offsetLength; i <= middle - offsetLength; i += offsetLength)
        {

            ;
            Vector3Int window = new Vector3Int((int)homeVertices[0].x, 1, (int)i);
            if (!roomPosition.Contains(window))
            {
                maxWindows++;
            }

        }

        return maxWindows;

        }
            

        public List<Vector3> RoomPosition()
{
        List<Vector3> roomPosition = new List<Vector3>();
        
        foreach (Mesh mesh in allRooms)
    {
       
        Vector3[] roomVertices = mesh.vertices;
        
         for (int i = (int)roomVertices[0].x; i <= roomVertices[1].x; i++)
            {
                Vector3 pos = new Vector3(i, 1, (int)roomVertices[0].z);
                roomPosition.Add(pos);


          }


        for (int i = (int)roomVertices[2].x; i <= (int)roomVertices[3].x; i++)
            {

                Vector3 pos = new Vector3(i, 1, (int)roomVertices[2].z);
                roomPosition.Add(pos);
             
            }

        for (int i = (int)roomVertices[3].z; i <= (int)roomVertices[1].z; i++)
            {

                Vector3 pos = new Vector3((int)roomVertices[3].x, 1, i);
                roomPosition.Add(pos);

            }

        for (int i = (int)roomVertices[2].z; i <= (int)roomVertices[0].z; i++)
            {

                Vector3 pos = new Vector3((int)roomVertices[2].x, 1, i);
                roomPosition.Add(pos);

            }
        }

        return roomPosition;
    }


    private Vector3 CalculateMiddlePoint(Vector3 v1, Vector3 v2)
    {
        Vector3 sum = v1 + v2;
        Vector3 tempVector = sum / 2;
       
        return new Vector3(tempVector.x + 0.1f, 0, tempVector.z +0.1f);


    }

   
    public int randomIntExcept(int min, int max, List<int> except)
    {
        int result = Random.Range(min, max);
        for(int i = 0; i < except.Count ; i++)
        {
            if (result == except[i])
            {
                result = Random.Range(min, max);
                i = 0;
            }
        }
        
        return result;
    }

    private void DestroyAllChildren()
    {
        while(transform.childCount != 0)
        {
            foreach(Transform item in transform)
            {
                DestroyImmediate(item.gameObject);
            }
        }
    }
}

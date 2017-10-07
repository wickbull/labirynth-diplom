using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Maze : MonoBehaviour {

    [Serializable]
    public class Cell
    {
        public bool visited;
        public GameObject north; //1
        public GameObject east;  //2
        public GameObject west;  //3
        public GameObject south; //4
    }


    public GameObject wall;
    public float wallLength = 1.0f;
    public int xSize = 5;
    public int ySize = 5;
    private Vector3 initialPos;
    private GameObject wallHolder;
    private Cell[] cells;
    //public Cell[] cells;
    //public int currentCell = 0;
    private int currentCell = 0;
    private int totalCells;
    private int visitedCells = 0;
    private bool startedBuilding = false;
    private int currentNeighbour = 0;
    private List<int> lastCells;
    private int backingUp = 0;
    private int wallToBreak = 0;


    // Use this for initialization
    void Start ()
    {
        CreateWalls ();
	}

    void CreateWalls ()
    {
        wallHolder = new GameObject();
        wallHolder.name = "Maze";


        initialPos = new Vector3((-xSize / 2) + wallLength/2,0.0f,(-ySize / 2 ) + wallLength/2);
        Vector3 myPos = initialPos;
        GameObject tempWall;

        //створює 6 стін по 1 комірці (матриця для осі х)
        for(int i = 0; i < ySize; i++)
        {
            for(int j = 0; j <= xSize; j++)
            {
                myPos = new Vector3(initialPos.x + (j * wallLength) - wallLength / 2, 0.0f, initialPos.z + (i * wallLength) - wallLength / 2);
                tempWall = Instantiate(wall, myPos, Quaternion.identity) as GameObject;
                tempWall.transform.parent = wallHolder.transform;
            }
        }

        //створює 6 стін по 1 комірці (матриця для осі у)
        for(int i = 0; i <= ySize; i++)
        {
            for(int j = 0; j < xSize; j++)
            {
                myPos = new Vector3(initialPos.x + (j * wallLength), 0.0f, initialPos.z + (i * wallLength) - wallLength);
                tempWall = Instantiate(wall, myPos, Quaternion.Euler(0.0f,90.0f,0.0f)) as GameObject;
                tempWall.transform.parent = wallHolder.transform;
            }
        }

        CreateCells ();

    }

    void CreateCells()
    {
        lastCells = new List<int>();
        lastCells.Clear();
        totalCells = xSize * ySize;
        GameObject[] allWalls;
        int children = wallHolder.transform.childCount;
        allWalls = new GameObject[children];
        cells = new Cell[xSize * ySize];
        int eastWestProcess = 0; // з якої ячейки начинає(точніше координати)
        int childProcess = 0;
        int termCount = 0; //перехід на слідуючий ряд


        //викор всіх дочірніх
        for (int i=0; i < children; i++)
        {
            allWalls[i] = wallHolder.transform.GetChild(i).gameObject;
        }

        //прикріпити стіну до ячейки
        for (int cellProcess = 0; cellProcess < cells.Length; cellProcess++)
        {
            cells[cellProcess] = new Cell();
            cells[cellProcess].east = allWalls[eastWestProcess];
            cells[cellProcess].south = allWalls[childProcess + (xSize + 1) * ySize];
            if (termCount == xSize)
            {
                eastWestProcess += 2;
                termCount = 0;
            }
            else
                eastWestProcess++;

            termCount++;
            childProcess++;

            cells[cellProcess].west = allWalls[eastWestProcess];
            cells[cellProcess].north = allWalls[(childProcess + (xSize + 1) * ySize) + xSize - 1];
        }
        CreateMaze();

    }

    void CreateMaze () //генерація уже самого лабіринта
    {
        //if(visitedCells < totalCells)
        while(visitedCells < totalCells)
        {
            if (startedBuilding)
            {
                GiveMeNeighbour();
                if(cells[currentNeighbour].visited == false && cells[currentCell].visited == true)
                {
                    BreakWall(); //ріже стіни
                    cells[currentNeighbour].visited = true;
                    visitedCells++;
                    lastCells.Add(currentCell);
                    currentCell = currentNeighbour;
                    if(lastCells.Count > 0)
                    {
                        backingUp = lastCells.Count - 1;
                    }
                }
            }
            else
            {
                currentCell = UnityEngine.Random.Range(0, totalCells); //unity енджин рендом під сумнівом, провіряю!, в ідеалі повинно бути чисто рандом!
                cells[currentCell].visited = true;
                visitedCells++;
                startedBuilding = true;
            }

            Debug.Log("Finished");
            //Invoke("СreateMaze",0.0f);
        }
    }

    void BreakWall () // ріже стіни
    {
        switch (wallToBreak)
        {
            case 1 : Destroy(cells[currentCell].north); break; // інструмент ломання стіни по напрямку
            case 2 : Destroy(cells[currentCell].east); break;
            case 3 : Destroy(cells[currentCell].west); break;
            case 4 : Destroy(cells[currentCell].south); break;
        }
    }

    void GiveMeNeighbour ()
    {
        int length = 0;
        int[] neighbours = new int[4];
        int[] connectingWall = new int[4];
        int check = 0;
        check = ((currentCell + 1) / xSize);
        check -= 1;
        check *= xSize;
        check += xSize;
        //------------------------------------------------------//
        //west(права ячейка)

        if (currentCell + 1 < totalCells && (currentCell + 1) != check)
        {
            if (cells[currentCell + 1].visited == false)
            {
                neighbours[length] = currentCell + 1;
                connectingWall[length] = 3;
                length++;
            }
        }

        //east(ліва ячейка)

        if (currentCell - 1 >= 0 && currentCell != check)
        {
            if (cells[currentCell - 1].visited == false)
            {
                neighbours[length] = currentCell - 1;
                connectingWall[length] = 2;
                length++;
            }
        }

        //north(верхня ячейка)

        if (currentCell + xSize < totalCells)
        {
            if (cells[currentCell + xSize].visited == false)
            {
                neighbours[length] = currentCell + xSize;
                connectingWall[length] = 1;
                length++;
            }
        }

        //south(нижня ячейка)

        if (currentCell - xSize >= 0)
        {
            if (cells[currentCell - xSize].visited == false)
            {
                neighbours[length] = currentCell - xSize;
                connectingWall[length] = 4;
                length++;
            }
        }
        //------------------------------------------------------//

        //for (int i = 0; i < length; i++)
        //Debug.Log(neighbours[i]); // можна проглянути лог ячейки(які сусідні ячейки у вибраної ячейки)

        if (length != 0) // перевірка при генерації
        {
            int theChosenOne = UnityEngine.Random.Range(0, length); // unityengine!!!
            currentNeighbour = neighbours[theChosenOne];
            wallToBreak = connectingWall[theChosenOne];
        }
        else
        {
            if(backingUp > 0)
            {
                currentCell = lastCells[backingUp];
                backingUp--;
            }
        }
        
    }

    // Update is called once per frame
    void Update ()
    {
	
	}
}

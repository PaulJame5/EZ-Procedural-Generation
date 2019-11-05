using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceduralLevelGenerator : MonoBehaviour
{
    public GameObject[] arriveFromSouth;
    public GameObject[] arriveFromNorth;
    public GameObject[] arriveFromEast;
    public GameObject[] arriveFromWest;

    [Space(10)]
    public GameObject[] roomsThatGoLeft;
    public GameObject[] roomsThatGoRight;
    public GameObject[] roomsThatGoTop;
    public GameObject[] roomsThatGoBottom;

    public GameObject deadEndNorth, deadEndEast, deadEndWest, deadEndSouth;

    [Space(10)]
    public GameObject[] startRoom;
    public GameObject[] end;

    private List<int> lastMoveX = new List<int>();
    private List<int> lastMoveY = new List<int>();

    public int width = 5, height = 5;

    //public GameObject[][] map;

    public Direction direction;
    private Direction arrivedFrom;

    private int x = 0, y = 0;

    public int seed = 42;

    GameObject[,] map;

    public enum Direction
    {
        NORTH,
        SOUTH,
        EAST,
        WEST
    }

    public GameObject gameObj;
    // Start is called before the first frame update
    void Start()
    {
        //lastMoveX.Add(5);
        //lastMoveX.Add(6);
        //lastMoveX.Add(7);
        //for(int i = 0; i < lastMoveX.Count; i++)
        //{
        //    Debug.Log(lastMoveX[i]);
        //}

        //lastMoveX.RemoveAt(lastMoveX.Count - 1);

        //for (int i = 0; i < lastMoveX.Count; i++)
        //{
        //    Debug.Log(lastMoveX[i]);
        //}




        map = new GameObject[width, height];
        for (y = 0; y < height; y++)
        {
            for (x = 0; x < width; x++)
            {
                map[x, y] = null;
            }

        }
        //set seed
        Random.InitState(seed);

        for (y = 0; y < height; y++)
        {
            for (x = 0; x < width; x++)
            {
                if (x == 0 && y == 0)
                {
                    int size = startRoom.Length;
                    GameObject go = Instantiate(startRoom[Random.Range(0, size)], new Vector2(transform.position.x + (2.56f * x), transform.position.y + (2.56f * y)), Quaternion.identity);

                    map[x, y] = go;
                    Debug.Log(go.name);
                    Debug.Log(map[x, y].name);
                    lastMoveX.Add(x);
                    lastMoveY.Add(y);
                    if (go.GetComponent<CanGo>().canGoUp && go.GetComponent<CanGo>().canGoRight)
                    {
                        float rand = Random.Range(0.0f, 1.0f);
                        if (rand > .5f)
                        {
                            direction = Direction.EAST;
                        }
                        else
                        {
                            direction = Direction.WEST;
                        }
                    }
                    else if (go.GetComponent<CanGo>().canGoUp)
                    {
                        direction = Direction.NORTH;
                    }
                    else
                    {
                        direction = Direction.EAST;
                    }

                }
                else if (x == width - 1 && y == height - 1)
                {
                    if (direction == Direction.NORTH)
                    {
                        GameObject go = Instantiate(deadEndNorth, new Vector2(transform.position.x + (2.56f * x), transform.position.y + (2.56f * y)), Quaternion.identity);

                        map[x, y] = go;

                    }
                    else
                    {
                        GameObject go = Instantiate(deadEndEast, new Vector2(transform.position.x + (2.56f * x), transform.position.y + (2.56f * y)), Quaternion.identity);

                        map[x, y] = go;

                    }
                    lastMoveX.Add(x);
                    lastMoveY.Add(y);
                }
                else
                {
                    Generate(x, y);
                }
                // Generate

            }// end x
        } // End Loop

        FindUnconnectedRooms();
    }

    void FindUnconnectedRooms()
    {
        int[] isolatedRoomX;
        int[] isolatedRoomY;
        int found = 0;
        for (y = 0; y < height; y++)
        {
            for (x = 0; x < width; x++)
            {
                if (map[x, y] != null)
                {
                    bool isolated = true;
                    // Left Object
                    if (x != 0 && map[x - 1, y] != null)
                    {
                        if (map[x, y].GetComponent<CanGo>().canGoLeft && map[x - 1, y].GetComponent<CanGo>().canGoRight)
                        {
                            isolated = false;
                        }
                    }
                    // Right

                    if (x != width - 1 && map[x + 1, y] != null)
                    {
                        if (map[x, y].GetComponent<CanGo>().canGoRight && map[x + 1, y].GetComponent<CanGo>().canGoLeft)
                        {
                            isolated = false;
                        }
                    }
                    // Top
                    if (y != height - 1 && map[x, y + 1] != null)
                    {
                        if (map[x, y].GetComponent<CanGo>().canGoUp && map[x, y + 1].GetComponent<CanGo>().canGoDown)
                        {
                            isolated = false;
                        }
                    }
                    // Bottom
                    if (y != 0 && map[x, y - 1] != null)
                    {
                        if (map[x, y].GetComponent<CanGo>().canGoDown && map[x, y - 1].GetComponent<CanGo>().canGoUp)
                        {
                            isolated = false;
                        }
                    }
                    if (isolated)
                    {
                        found++;
                    }




                }

            }
        }
        Debug.Log("There is " + found + " isolated rooms");
    }

    void TidyUpMazeExits()
    {
        for (y = 0; y < height; y++)
        {
            for (x = 0; x < width; x++)
            {
                bool hasExit = false;
                bool loopedOnce = false;
                while (hasExit == false)
                {
                    if (!loopedOnce)
                    {
                        // left
                        if (map[x, y].GetComponent<CanGo>().canGoLeft)
                        {
                            if (x != 0)
                            {
                                if (map[x - 1, y].GetComponent<CanGo>().canGoRight)
                                {
                                    hasExit = true;
                                }
                            }

                        }
                        // right
                        if (map[x, y].GetComponent<CanGo>().canGoRight)
                        {
                            if (x != width - 1)
                            {
                                if (map[x + 1, y].GetComponent<CanGo>().canGoLeft)
                                {
                                    hasExit = true;
                                }
                            }

                        }
                        // up
                        if (map[x, y].GetComponent<CanGo>().canGoUp)
                        {
                            if (y != height - 1)
                            {
                                if (map[x, y + 1].GetComponent<CanGo>().canGoRight)
                                {
                                    hasExit = true;
                                }
                            }

                        }
                        // down
                        if (map[x, y].GetComponent<CanGo>().canGoLeft)
                        {
                            if (y != 0)
                            {
                                if (map[x, y - 1].GetComponent<CanGo>().canGoDown)
                                {
                                    hasExit = true;
                                }
                            }

                        }
                    }
                    else
                    {

                        // left
                        if (map[x, y].GetComponent<CanGo>().canGoLeft)
                        {


                        }
                        // right
                        if (map[x, y].GetComponent<CanGo>().canGoRight)
                        {


                        }
                        // up
                        if (map[x, y].GetComponent<CanGo>().canGoUp)
                        {


                        }
                        // down
                        if (map[x, y].GetComponent<CanGo>().canGoLeft)
                        {


                        }
                        break;
                    }
                    loopedOnce = true;
                }
            }

        }

    }


    private void Generate(int t_x, int t_y)
    {
        switch (direction)
        {
            case (Direction.WEST):
                if (t_x > 0)
                {

                    CreateRoomToLeft(t_x, t_y);
                }
                else
                {
                    x--;
                }
                break;

            case (Direction.SOUTH):
                if (y != 0)
                {
                    CreateRoomToBottom(t_x, t_y);
                }
                else
                {
                    x--;
                }

                break;

            case (Direction.NORTH):
                if (t_y < height)
                {
                    CreateRoomToTop(t_x, t_y);
                }
                else
                {
                    x--;
                }
                break;

            case (Direction.EAST):
                if (x < width - 1)
                {
                    CreateRoomToRight(t_x, t_y);
                }
                break;



        }//end switch direction
    }

    void CreateRoomToLeft(int t_x, int t_y)
    {
        int size = roomsThatGoLeft.Length;
        GameObject go = Instantiate(roomsThatGoLeft[Random.Range(0, size)], new Vector2(transform.position.x + (2.56f * (t_x)), transform.position.y + (2.56f * t_y)), Quaternion.identity);
        bool isOK = false;

        if (go.GetComponent<CanGo>().canGoRight && x == width - 1)
        {
            Destroy(go);
            while (!isOK)
            {
                int pos = Random.Range(0, size);
                if (!roomsThatGoLeft[pos].GetComponent<CanGo>().canGoRight)
                {
                    go = Instantiate(roomsThatGoLeft[pos], new Vector2(transform.position.x + (2.56f * (t_x)), transform.position.y + (2.56f * t_y)), Quaternion.identity);
                    isOK = true;
                }
            }
        }
        else if ((go.GetComponent<CanGo>().canGoDown && y == 0))
        {
            Destroy(go);
            while (!isOK)
            {
                int pos = Random.Range(0, size);
                if (!roomsThatGoLeft[pos].GetComponent<CanGo>().canGoDown)
                {
                    go = Instantiate(roomsThatGoLeft[pos], new Vector2(transform.position.x + (2.56f * (t_x)), transform.position.y + (2.56f * t_y)), Quaternion.identity);
                    isOK = true;
                }
            }
        }
        else if ((go.GetComponent<CanGo>().canGoUp && y == height - 1))
        {
            Destroy(go);
            while (!isOK)
            {
                int pos = Random.Range(0, size);
                if (!roomsThatGoLeft[pos].GetComponent<CanGo>().canGoUp)
                {
                    go = Instantiate(roomsThatGoLeft[pos], new Vector2(transform.position.x + (2.56f * (t_x)), transform.position.y + (2.56f * t_y)), Quaternion.identity);
                    isOK = true;
                }
            }
        }
        map[x, y] = go;
        lastMoveX.Add(x);
        lastMoveY.Add(y);

        arrivedFrom = direction;
        x -= 2;

    }
    void CreateRoomToRight(int t_x, int t_y)
    {
        int size = roomsThatGoRight.Length;
        GameObject go = Instantiate(roomsThatGoRight[Random.Range(0, size)], new Vector2(transform.position.x + (2.56f * t_x), transform.position.y + (2.56f * t_y)), Quaternion.identity);
        bool isOK = false;

        if (go.GetComponent<CanGo>().canGoLeft && x == 0)
        {
            Destroy(go);
            while (!isOK)
            {
                int pos = Random.Range(0, size);
                if (!roomsThatGoRight[pos].GetComponent<CanGo>().canGoLeft)
                {
                    go = Instantiate(roomsThatGoRight[pos], new Vector2(transform.position.x + (2.56f * (t_x)), transform.position.y + (2.56f * t_y)), Quaternion.identity);
                    isOK = true;
                }
            }
        }
        else if ((go.GetComponent<CanGo>().canGoDown && y == 0))
        {
            Destroy(go);
            while (!isOK)
            {
                int pos = Random.Range(0, size);
                if (!roomsThatGoRight[pos].GetComponent<CanGo>().canGoDown)
                {
                    go = Instantiate(roomsThatGoRight[pos], new Vector2(transform.position.x + (2.56f * (t_x)), transform.position.y + (2.56f * t_y)), Quaternion.identity);
                    isOK = true;
                }
            }
        }
        else if ((go.GetComponent<CanGo>().canGoUp && y == height - 1))
        {
            Destroy(go);
            while (!isOK)
            {
                int pos = Random.Range(0, size);
                if (!roomsThatGoRight[pos].GetComponent<CanGo>().canGoUp)
                {
                    go = Instantiate(roomsThatGoLeft[pos], new Vector2(transform.position.x + (2.56f * (t_x)), transform.position.y + (2.56f * t_y)), Quaternion.identity);
                    isOK = true;
                }
            }
        }
        map[x, y] = go;
        lastMoveX.Add(x);
        lastMoveY.Add(y);

        arrivedFrom = direction;

    }
    void CreateRoomToTop(int t_x, int t_y)
    {
        int size = roomsThatGoTop.Length;
        GameObject go = Instantiate(roomsThatGoTop[Random.Range(0, size)], new Vector2(transform.position.x + (2.56f * t_x), transform.position.y + (2.56f * t_y)), Quaternion.identity);
        bool isOK = false;

        if (go.GetComponent<CanGo>().canGoRight && x == width - 1)
        {
            Destroy(go);
            while (!isOK)
            {
                int pos = Random.Range(0, size);
                if (!roomsThatGoTop[pos].GetComponent<CanGo>().canGoRight)
                {
                    go = Instantiate(roomsThatGoTop[pos], new Vector2(transform.position.x + (2.56f * (t_x)), transform.position.y + (2.56f * t_y)), Quaternion.identity);
                    isOK = true;
                }
            }
        }
        else if ((go.GetComponent<CanGo>().canGoLeft && x == 0))
        {
            Destroy(go);
            while (!isOK)
            {
                int pos = Random.Range(0, size);
                if (!roomsThatGoTop[pos].GetComponent<CanGo>().canGoLeft)
                {
                    go = Instantiate(roomsThatGoTop[pos], new Vector2(transform.position.x + (2.56f * (t_x)), transform.position.y + (2.56f * t_y)), Quaternion.identity);
                    isOK = true;
                }
            }
        }
        else if ((go.GetComponent<CanGo>().canGoDown && y == 0))
        {
            Destroy(go);
            while (!isOK)
            {
                int pos = Random.Range(0, size);
                if (!roomsThatGoTop[pos].GetComponent<CanGo>().canGoDown)
                {
                    go = Instantiate(roomsThatGoTop[pos], new Vector2(transform.position.x + (2.56f * (t_x)), transform.position.y + (2.56f * t_y)), Quaternion.identity);
                    isOK = true;
                }
            }
        }

        map[x, y] = go;
        lastMoveX.Add(x);
        lastMoveY.Add(y);

        arrivedFrom = direction;
        x--;
        y++;

    }
    void CreateRoomToBottom(int t_x, int t_y)
    {
        int size = roomsThatGoBottom.Length;
        GameObject go = Instantiate(roomsThatGoBottom[Random.Range(0, size)], new Vector2(transform.position.x + (2.56f * t_x), transform.position.y + (2.56f * t_y)), Quaternion.identity);
        bool isOK = false;

        if (go.GetComponent<CanGo>().canGoRight && x == width - 1)
        {
            Destroy(go);
            while (!isOK)
            {
                int pos = Random.Range(0, size);
                if (!roomsThatGoBottom[pos].GetComponent<CanGo>().canGoRight)
                {
                    go = Instantiate(roomsThatGoBottom[pos], new Vector2(transform.position.x + (2.56f * (t_x)), transform.position.y + (2.56f * t_y)), Quaternion.identity);
                    isOK = true;
                }
            }
        }
        else if ((go.GetComponent<CanGo>().canGoLeft && x == 0))
        {
            Destroy(go);
            while (!isOK)
            {
                int pos = Random.Range(0, size);
                if (!roomsThatGoBottom[pos].GetComponent<CanGo>().canGoDown)
                {
                    go = Instantiate(roomsThatGoBottom[pos], new Vector2(transform.position.x + (2.56f * (t_x)), transform.position.y + (2.56f * t_y)), Quaternion.identity);
                    isOK = true;
                }
            }
        }
        else if ((go.GetComponent<CanGo>().canGoUp && y == height - 1))
        {
            Destroy(go);
            while (!isOK)
            {
                int pos = Random.Range(0, size);
                if (!roomsThatGoBottom[pos].GetComponent<CanGo>().canGoUp)
                {
                    go = Instantiate(roomsThatGoBottom[pos], new Vector2(transform.position.x + (2.56f * (t_x)), transform.position.y + (2.56f * t_y)), Quaternion.identity);
                    isOK = true;
                }
            }
        }
        map[x, y] = go;
        lastMoveX.Add(x);
        lastMoveY.Add(y);

        arrivedFrom = direction;
        y--;
        x--;


    }

    
}

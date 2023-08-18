using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerationScript : MonoBehaviour
{
    private GameManager manager;
    public ChunkScript chunk;
    public ChunkScript[,] chunks;
    private ChunkScript startingChunk;

    public int width;
    public int height;

    [SerializeField]
    private float maxDensity;
    [SerializeField]
    private float minDensity;
    private float density;
    private float numGenerated;

    // Start is called before the first frame update
    void Start()
    {
        numGenerated = 0;
        manager = GameObject.FindObjectOfType<GameManager>();
        chunks = new ChunkScript[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                ChunkScript newChunk = Instantiate(chunk);
                newChunk.transform.position = new Vector3(2 * newChunk.getWidth() * i, 2 * newChunk.getHeight() * j, 0);
                newChunk.xCoord = i;
                newChunk.yCoord = j;
                chunks[i, j] = newChunk;
            }
        }
        generateStartingChunk();
        generateTerrain();
        generateChunkStructures();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void generateStartingChunk()
    {
        startingChunk = chunks[10, 10];
        startingChunk.discovered = true;
    }

    void generateTerrain()
    {
        Stack<ChunkScript> s = new Stack<ChunkScript>();
        Queue<ChunkScript> q = new Queue<ChunkScript>();
        q.Enqueue(startingChunk);
        s.Push(startingChunk);
        while (s.Count > 0)
        {
            density = Mathf.Sqrt(Mathf.Max(s.Count - 5, 0)) * (0.5f * (maxDensity + minDensity) - 0.5f * (maxDensity - minDensity) * Mathf.Cos(numGenerated * Mathf.PI));
            //ChunkScript nextChunk = q.Dequeue();
            ChunkScript nextChunk = s.Pop();
            int x = nextChunk.xCoord;
            int y = nextChunk.yCoord;
            ChunkScript leftChunk = null;
            ChunkScript rightChunk = null;
            ChunkScript upChunk = null;
            ChunkScript downChunk = null;

            if (x - 1 >= 0)
            {
                leftChunk = chunks[x - 1, y];
            }
            if (y - 1 >= 0)
            {
                downChunk = chunks[x, y - 1];
            }
            if (x + 1 < width)
            {
                rightChunk = chunks[x + 1, y];
            }
            if (y + 1 < height)
            {
                upChunk = chunks[x, y + 1];
            }

            if (x == 0 || y == 0 || x == width - 1 || y == height - 1)
            {
                nextChunk.setFiller(true);
            }
            else
            {
                int fillerChance = 0;
                float floorChance = density;
                float leftWallChance = density;
                float rightWallChance = density;
                float ceilingChance = density;

                if (x == 1)
                {
                    leftWallChance = maxDensity;
                }
                if (x == width - 1)
                {
                    rightWallChance = maxDensity;
                }
                if (y == 1)
                {
                    floorChance = maxDensity;
                }
                if (y == height - 1)
                {
                    ceilingChance = maxDensity;
                }

                if (leftChunk != null)
                {
                    if (leftChunk.discovered) 
                    {//Left
                        if (leftChunk.getRightWall())
                        {
                            nextChunk.setFiller(true);
                        }
                        else
                        {
                            leftWallChance = 0;
                        }
                        if (leftChunk.getFiller())
                        {
                            fillerChance++;
                            leftWallChance = maxDensity;
                        }
                    }
                }
                if (downChunk != null)
                {
                    if (downChunk.discovered)
                    { //Down
                        if (downChunk.getCeiling())
                        {
                            nextChunk.setFiller(true);
                        }
                        else
                        {
                            floorChance = 0;
                        }
                        if (downChunk.getFiller())
                        {
                            fillerChance++;
                            floorChance = maxDensity;
                        }
                    }
                }
                if (rightChunk != null)
                {
                    if (rightChunk.discovered) 
                    {//Right
                        if (rightChunk.getLeftWall())
                        {
                            nextChunk.setFiller(true);
                        }
                        else
                        {
                            rightWallChance = 0;
                        }
                        if (rightChunk.getFiller())
                        {
                            fillerChance++;
                            rightWallChance = maxDensity;
                        }
                    }
                }
                if (upChunk)
                {
                    if (upChunk.discovered) 
                    { //Up
                        if (upChunk.getFloor())
                        {
                            nextChunk.setFiller(true);
                        }
                        else
                        {
                            ceilingChance = 0;
                        }
                        if (upChunk.getFiller())
                        {
                            fillerChance++;
                            ceilingChance = maxDensity;
                        }
                    }
                }
                if (Random.Range(0, 4) < fillerChance || nextChunk.getFiller())
                {
                    nextChunk.setFiller(true);
                    if (leftChunk != null && leftChunk.getGenerated() && !leftChunk.getFiller() && !leftChunk.getRightWall()) 
                    {
                        leftChunk.setRightWall(true);
                        leftChunk.generateRightWall();
                    }
                    if (rightChunk != null && rightChunk.getGenerated() && !rightChunk.getFiller() && !rightChunk.getLeftWall())
                    {
                        rightChunk.setLeftWall(true);
                        rightChunk.generateLeftWall();
                    }
                    if (upChunk != null && upChunk.getGenerated() && !upChunk.getFiller() && !upChunk.getFloor())
                    {
                        upChunk.setFloor(true);
                        upChunk.generateGround();
                    }
                    if (downChunk != null && downChunk.getGenerated() && !downChunk.getFiller() && !downChunk.getCeiling())
                    {
                        downChunk.setCeiling(true);
                        downChunk.generateCeiling();
                    }
                } else
                {
                    nextChunk.setFloor(Random.Range(0, maxDensity) < floorChance);
                    nextChunk.setLeftWall(Random.Range(0, maxDensity) < leftWallChance);
                    nextChunk.setRightWall(Random.Range(0, maxDensity) < rightWallChance);
                    nextChunk.setCeiling(Random.Range(0, maxDensity) < ceilingChance);
                    int i = 0;
                    while (i < 100 && nextChunk.getFloor() && nextChunk.getLeftWall() && nextChunk.getRightWall() && nextChunk.getCeiling())
                    {
                        nextChunk.setFloor(Random.Range(0, maxDensity) < floorChance);
                        nextChunk.setLeftWall(Random.Range(0, maxDensity) < leftWallChance);
                        nextChunk.setRightWall(Random.Range(0, maxDensity) < rightWallChance);
                        nextChunk.setCeiling(Random.Range(0, maxDensity) < ceilingChance);
                        i++;
                    }

                    Debug.Log(x + " " + y + ": " + floorChance + " " + leftWallChance + " " + rightWallChance + " " + ceilingChance);

                    if (leftChunk != null && !leftChunk.discovered)
                    {
                        q.Enqueue(leftChunk);
                        s.Push(leftChunk);
                        leftChunk.discovered = true;
                    }
                    if (rightChunk != null && !rightChunk.discovered)
                    {
                        q.Enqueue(rightChunk);
                        s.Push(rightChunk);
                        rightChunk.discovered = true;
                    }
                    if (upChunk != null && !upChunk.discovered)
                    {
                        q.Enqueue(upChunk);
                        s.Push(upChunk);
                        upChunk.discovered = true;
                    }
                    if (downChunk != null && !downChunk.discovered)
                    {
                        q.Enqueue(downChunk);
                        s.Push(downChunk);
                        downChunk.discovered = true;
                    }
                }
            }
            nextChunk.generateChunk();
            numGenerated++;
        }
    }

    void generateChunkStructures()
    {
        for (int i = 1; i < width - 1; i++)
        {
            for (int j = 1; j < height - 1; j++)
            {
                int x = chunks[i, j].xCoord;
                int y = chunks[i, j].yCoord;
                ChunkScript leftChunk = null;
                ChunkScript rightChunk = null;
                ChunkScript upChunk = null;
                ChunkScript downChunk = null;

                if (x - 1 >= 0)
                {
                    leftChunk = chunks[x - 1, y];
                }
                if (y - 1 >= 0)
                {
                    downChunk = chunks[x, y - 1];
                }
                if (x + 1 < width)
                {
                    rightChunk = chunks[x + 1, y];
                }
                if (y + 1 < height)
                {
                    upChunk = chunks[x, y + 1];
                }

                List<string> coreTypes = new List<string>();
                coreTypes.Add("Empty");
                coreTypes.Add("Platform");
                coreTypes.Add("Column");
                coreTypes.Add("Columns");
                coreTypes.Add("Island");

                if (chunks[i, j].getFloor())
                {
                    coreTypes.Add("Pillar");
                    if (!chunks[i, j].getLeftWall() || !chunks[i, j].getRightWall())
                    {
                        coreTypes.Add("Highground");
                    }
                    if (!chunks[i, j].getCeiling() && chunks[i, j].getLeftWall() && !chunks[i, j].getRightWall())
                    {
                        coreTypes.Add("BigPillar");
                    }
                } else
                {
                    if (!chunks[i, j].getCeiling() && !chunks[i, j].getLeftWall() && !chunks[i, j].getRightWall())
                    {
                        coreTypes.Add("BigIsland");
                    }
                }

                chunks[i, j].coreType = coreTypes[Random.Range(0, coreTypes.Count)];
                chunks[i, j].generateStructures();
            }
        }
    }
}

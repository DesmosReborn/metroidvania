using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkScript : MonoBehaviour
{
    private GameManager manager;
    private TerrainGenerationScript terrain;

    public GameObject highLight1;

    public GameObject leftWall1;
    public GameObject rightWall1;
    public GameObject ground1;
    public GameObject ground2;
    public GameObject ceiling1;
    public GameObject ceiling2;
    public GameObject filler1;
    public GameObject platform1;
    public GameObject platform2;
    public GameObject ceilingPlatform;
    public GameObject leftWallPlatform;
    public GameObject rightWallPlatform;
    public GameObject smallPlatform1;
    public GameObject smallPlatform2;
    public GameObject column1;
    public GameObject column2;
    public GameObject island1;
    public GameObject island2;
    public GameObject bigIsland1;
    public GameObject bigIsland2;
    public GameObject pillar1;
    public GameObject bigPillar1;
    public GameObject highGround1;
    public GameObject groundHBox;

    public GameObject gbg1;
    public GameObject gbg2;
    public GameObject gbg3;
    public GameObject cbg1;
    public GameObject wbg1;
    public GameObject wbg2;

    private int width = 10;
    private int height = 10;
    public int xCoord;
    public int yCoord;

    public bool discovered = false;
    public bool isFiller = false;
    [SerializeField]
    private bool hasLeftWall = false;
    [SerializeField]
    private bool hasRightWall = false;
    [SerializeField]
    private bool hasFloor = false;
    [SerializeField]
    private bool hasCeiling = false;
    private bool generated = false;

    private PlayerScript player;
    private float playerJumpDistance;
    private float playerJumpHeight;

    public int[,] filled = new int[9, 9];

    private float[] platformX = new float[] { -9.74f, -7.24f, -4.74f, -2.24f, 0.26f, 2.76f, 5.26f, 7.76f, 10.26f };
    private float[] platformY = new float[] { -8.74f, -6.24f, -3.74f, -1.24f, 1.26f, 3.76f, 6.26f, 8.76f, 11.26f };

    private float[] columnX = new float[] { -6.25f, -3.75f, -1.25f, 1.25f, 3.76f, 6.25f, 8.76f, 11.25f, 13.76f };
    private float[] columnY = new float[] { -11f, -7.5f, -5.25f, -3.5f, -1f, 1.5f, 2.75f, 5.5f, 9f };

    private float columnLeft = 1.51f;
    private float columnRight = 6.01f;

    private float pillarXCenter = -1f;
    private float pillarYCenter = -8.25f;

    private float bigPillarXCenter = -2.25f;
    private float bigPillarYCenter = -6.5f;

    private float islandXCenter = 0.39f;
    private float islandYCenter = 3.47f;

    private float bigIslandXCenter = 0.84f;
    private float bigIslandYCenter = 6.94f;

    private float highgroundXCenter = 0.42f;
    private float highgroundYCenter = -1.82f;

    [SerializeField]
    public string coreType;

    public bool getFiller()
    {
        return isFiller;
    }

    public bool getLeftWall()
    {
        return hasLeftWall;
    }

    public bool getRightWall()
    {
        return hasRightWall;
    }

    public bool getFloor()
    {
        return hasFloor;
    }

    public bool getCeiling()
    {
        return hasCeiling;
    }

    public int getWidth()
    {
        return width;
    }

    public int getHeight()
    {
        return height;
    }

    public bool getGenerated()
    {
        return generated;
    }

    public void setFiller(bool active)
    {
        isFiller = active;
    }

    public void setLeftWall(bool active)
    {
        hasLeftWall = active;
    }

    public void setRightWall(bool active)
    {
        hasRightWall = active;
    }

    public void setFloor(bool active)
    {
        hasFloor = active;
    }

    public void setCeiling(bool active)
    {
        hasCeiling = active;
    }

    void Awake()
    {
        gbg1.SetActive(false);
        gbg2.SetActive(false);
        gbg3.SetActive(false);
        cbg1.SetActive(false);
        wbg1.SetActive(false);
        wbg2.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                filled[i, j] = 0;
            }
        }
        manager = GameObject.FindObjectOfType<GameManager>();
        player = GameObject.FindObjectOfType<PlayerScript>();
        terrain = GameObject.FindObjectOfType<TerrainGenerationScript>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void generateChunk()
    {
        generateFiller();
        generateGround();
        generateLeftWall();
        generateRightWall();
        generateCeiling();
        generated = true;
    }

    public void generateFiller()
    {
        if (isFiller)
        {
            Vector2 position = new Vector2(-width + 2, -height + 4);
            while (position.y <= height - 1)
            {
                position.x = -width + 2;
                while (position.x <= width - 2)
                {
                    placeFiller(position.x, position.y);
                    position.x += 3;
                }
                placeFiller(width - 2, position.y);
                position.y += 4;
            }
            position.x = -width + 2;
            while (position.x <= width - 2)
            {
                placeFiller(position.x, height - 1);
                position.x += 3;
            }
            placeFiller(width - 2, height - 1);
        }
    }

    public void generateGround()
    {
        if (hasFloor && !isFiller)
        {
            Vector2 position = new Vector2(-width + 2, -height + 3);
            while (position.x <= width - 2)
            {
                if (position.y >= -height + 2)
                {
                    filled[(int)(position.x + 8) / 2, 1] = 1;
                }
                placeGround(position.x, Mathf.Clamp(position.y, -height + 2, -height + 4));
                position.x += Random.Range(1f, 2f);
                //float random = Random.Range(-0.5f, 0.5f);
                //position.y += random;

            }
            placeGround(width - 2, Mathf.Clamp(position.y, -height + 2, -height + 4));

            for (int i = 0; i < 9; i++)
            {
                filled[i, 0] = 2;
            }
        }
    }

    public void generateLeftWall()
    {
        if (hasLeftWall && !isFiller)
        {
            Vector2 position = new Vector2(-width, -height + 2);
            while (position.y <= height - 2)
            {
                if (position.x <= -width + 1.5f)
                {
                    filled[1, (int)(position.y + 8) / 2] = 1;
                }
                placeLeftWall(Mathf.Clamp(position.x, -width - 1.5f, -width + 1.5f), position.y);
                position.x += Random.Range(-1f, 1f);
                position.y += Random.Range(2f, 4f);

            }
            placeLeftWall(Mathf.Clamp(position.x, -width - 1.5f, -width + 1.5f), height - 2);

            for (int i = 0; i < 9; i++)
            {
                filled[0, i] = 2;
            }
        }
    }

    public void generateRightWall()
    {
        if (hasRightWall && !isFiller)
        {
            Vector2 position = new Vector2(width, -height + 2);
            while (position.y <= height - 2)
            {
                if (position.x >= width - 1.5f)
                {
                    filled[7, (int)(position.y + 8) / 2] = 1;
                }
                placeRightWall(Mathf.Clamp(position.x, width - 1.5f, width + 1.5f), position.y);
                position.x += Random.Range(-0.5f, 0.5f);
                position.y += Random.Range(2f, 4f);

            }
            placeRightWall(Mathf.Clamp(position.x, width - 1.5f, width + 1.5f), height - 2);
            
            for (int i = 0; i < 9; i++)
            {
                filled[8, i] = 2;
            }
        }
    }

    public void generateCeiling()
    {
        if (hasCeiling && !isFiller)
        {
            Vector2 position = new Vector2(-width + 2, height + Random.Range(-0.5f, 0.5f));
            while (position.x <= width - 2)
            {
                if (position.y <= height - 1.5f)
                {
                    filled[(int)(position.x + 8) / 2, 7] = 1;
                }
                placeCeiling(position.x, Mathf.Clamp(position.y, height - 1.5f, height + 1.5f));
                position.x += Random.Range(2f, 3f);
                position.y += Random.Range(-1f, 1f);

            }
            placeCeiling(width - 2, Mathf.Clamp(position.y, height - 1.5f, height + 1.5f));

            for (int i = 0; i < 9; i++)
            {
                filled[i, 8] = 2;
            }
        }
    }

    public void generateStructures()
    {
        if (generated && !isFiller) {
            generateCore();
            generateAuxiliaries();
            generateBGStructures();
        }
    }

    public void generateCore()
    {
        if (coreType.Equals("Platform"))
        {
            placePlatform(4, 4);
        } else if (coreType.Equals("Column"))
        {
            placeColumn(4, 4);
        } else if (coreType.Equals("Columns"))
        {
            placeTwoColumns();
        } else if (coreType.Equals("Island"))
        {
            placeIsland(islandXCenter, islandYCenter);
        } else if (coreType.Equals("Pillar"))
        {
            placePillar(pillarXCenter, pillarYCenter);
        } else if (coreType.Equals("Highground"))
        {
            placeHighground(highgroundXCenter, highgroundYCenter);
        } else if (coreType.Equals("BigPillar"))
        {
            placeBigPillar(bigPillarXCenter, bigPillarYCenter);
        } else if (coreType.Equals("BigIsland"))
        {
            placeBigIsland(bigIslandXCenter, bigIslandYCenter);
        }
    }

    public void generateAuxiliaries()
    {
        List<Vector2Int> pairs = new List<Vector2Int>();
        for (int i = 1; i < 8; i++)
        {
            for (int j = 1; j < 8; j++)
            {
                pairs.Add(new Vector2Int(i, j));
            }
        }

        while (pairs.Count > 0)
        {
            int idx = Random.Range(0, pairs.Count);
            Vector2Int coord = pairs[idx];
            pairs.RemoveAt(idx);

            int strucType = Random.Range(0, 2);
            if (strucType == 0)
            {
                if (!placeColumn(coord.x, coord.y))
                {
                    if (Random.Range(0, 3) <= 1)
                    {
                        if (!placePlatform(coord.x, coord.y))
                        {
                            placeSmallPlatform(coord.x, coord.y);
                        }
                    } else
                    {
                        placeSmallPlatform(coord.x, coord.y);
                    }
                }
            } else
            {
                if (Random.Range(0, 3) <= 1)
                {
                    if (!placePlatform(coord.x, coord.y))
                    {
                        placeSmallPlatform(coord.x, coord.y);
                    }
                }
                else
                {
                    placeSmallPlatform(coord.x, coord.y);
                }
            }
        }

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (filled[i, j] == 1)
                {
                    //GameObject highlight = Instantiate(highLight1, this.transform);
                    //highlight.transform.position += new Vector3(i * 2.5f - 10f, j * 2.5f - 10f, 0);
                } else if (filled[i, j] == 2)
                {
                    //GameObject highlight = Instantiate(highLight1, this.transform);
                    //highlight.transform.position += new Vector3(i * 2.5f - 10f, j * 2.5f - 10f, 0);
                    //highlight.GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
        }
    }

    public void generateBGStructures()
    {
        if (hasCeiling)
        {
            if (Random.Range(0, 3) <= 1)
            {
                cbg1.SetActive(true);
            }
        }


        if (hasLeftWall && hasRightWall)
        {
            if (Random.Range(0, 2) <= 1)
            {
                if (Random.Range(0, 2) <= 1)
                {
                    wbg1.SetActive(true);
                }
                if (Random.Range(0, 2) <= 1)
                {
                    wbg2.SetActive(true);
                }
            }
        }

        if (hasFloor)
        {
            if (Random.Range(0, 3) <= 1)
            {
                if (Random.Range(0, 3) <= 1)
                {
                    gbg1.SetActive(true);
                }
                if (Random.Range(0, 3) <= 1)
                {
                    gbg2.SetActive(true);
                }
                if (Random.Range(0, 3) <= 1)
                {
                    gbg3.SetActive(true);
                }
            }
        }
    }

    void placeFiller(float x, float y)
    {
        GameObject filler = Instantiate(filler1, this.transform);
        filler.transform.position += new Vector3(x, y, 0);
    }

    void placeGround(float x, float y)
    {
        int oneOrTwo = Random.Range(0, 2);
        if (oneOrTwo == 0)
        {
            GameObject ground = Instantiate(ground1, this.transform);
            ground.transform.position += new Vector3(x, y, 0);
        }
        else
        {
            GameObject ground = Instantiate(ground2, this.transform);
            ground.transform.position += new Vector3(x, y, 0);
        }
    }

    void placeLeftWall(float x, float y)
    {
        GameObject wall = Instantiate(leftWall1, this.transform);
        wall.transform.position += new Vector3(x, y, 0);
    }

    void placeRightWall(float x, float y)
    {
        GameObject wall = Instantiate(rightWall1, this.transform);
        wall.transform.position += new Vector3(x, y, 0);
    }

    void placeCeiling(float x, float y)
    {
        int oneOrTwo = Random.Range(0, 2);
        if (oneOrTwo == 0)
        {
            GameObject ceiling = Instantiate(ceiling1, this.transform);
            ceiling.transform.position += new Vector3(x, y, 0);
        }
        else
        {
            GameObject ceiling = Instantiate(ceiling2, this.transform);
            ceiling.transform.position += new Vector3(x, y, 0);
        }
    }

    bool placePlatform(int x, int y)
    {
        bool open = true;
        if (filled[x, y] > 0 || filled[x + 1, y] == 2 || filled[x - 1, y] == 2 || filled[x, y + 1] == 2 || filled[x, y - 1] == 2)
        {
            open = false;
        }
        if (open)
        {
            if (hasCeiling && x >= 1 && x <= 7 && y >= 5)
            {
                GameObject platform = Instantiate(ceilingPlatform, this.transform);
                platform.transform.position += new Vector3(platformX[x] + 0.5f, platformY[y] + 0.5f, 0);
            }
            else if (hasLeftWall || hasRightWall)
            {
                if (hasLeftWall && x <= 2 && y <= 7 && y >= 1)
                {
                    GameObject platform = Instantiate(leftWallPlatform, this.transform);
                    platform.transform.position += new Vector3(platformX[x] + 0.4f, platformY[y] + 0.4f, 0);
                }
                else if (hasRightWall && x >= 6 && y <= 7 && y >= 1)
                {
                    GameObject platform = Instantiate(rightWallPlatform, this.transform);
                    platform.transform.position += new Vector3(platformX[x] + 0.4f, platformY[y] + 0.4f, 0);
                }
            }
            else
            {
                int oneOrTwo = Random.Range(0, 2);
                if (oneOrTwo == 0)
                {
                    GameObject platform = Instantiate(platform1, this.transform);
                    platform.transform.position += new Vector3(platformX[x], platformY[y], 0);
                }
                else
                {
                    GameObject platform = Instantiate(platform2, this.transform);
                    platform.transform.position += new Vector3(platformX[x], platformY[y], 0);
                }
            }
            filled[x, y] = 2;
            filled[x - 1, y] = 1;
            filled[x + 1, y] = 1;
            filled[x, y - 1] = 1;
            filled[x, y + 1] = 1;
            return true;
        }
        return false;
    }

    void placeIsland(float x, float y)
    {
        int oneOrTwo = Random.Range(0, 2);
        if (oneOrTwo == 0)
        {
            GameObject island = Instantiate(island1, this.transform);
            island.transform.position += new Vector3(x, y, 0);
        }
        else
        {
            GameObject island = Instantiate(island2, this.transform);
            island.transform.position += new Vector3(x, y, 0);
        }
        for (int i = 2; i <= 6; i++)
        {
            filled[i, 4] = 2;
        }
        filled[1, 4] = 1;
        filled[7, 4] = 1;
        filled[2, 5] = 1;
        filled[6, 5] = 1;
        for (int i = 3; i <= 5; i++)
        {
            filled[i, 5] = 2;
            filled[i, 6] = 1;
            filled[i, 3] = 1;
        }
    }

    void placeBigIsland(float x, float y)
    {
        int oneOrTwo = Random.Range(0, 2);
        if (oneOrTwo == 0)
        {
            GameObject bigIsland = Instantiate(bigIsland1, this.transform);
            bigIsland.transform.position += new Vector3(x, y, 0);
        }
        else
        {
            GameObject bigIsland = Instantiate(bigIsland2, this.transform);
            bigIsland.transform.position += new Vector3(x, y, 0);
        }

        for (int i = 0; i <= 8; i++)
        {
            filled[i, 4] = 2;
            filled[i, 5] = 2;
            filled[i, 3] = 1;
        }
        for (int i = 3; i <= 5; i++)
        {
            filled[i, 6] = 2;
        }
        for (int i = 3; i <= 5; i++)
        {
            filled[i, 7] = 1;
        }
    }

    bool placeColumn(int x, int y)
    {
        bool open = true;
        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (filled[i, j] == 2)
                {
                    open = false;
                }
                if (i == x && filled[i, j] > 0)
                {
                    open = false;
                }
            }
        }
        if (open)
        {
            int oneOrTwo = Random.Range(0, 2);
            if (oneOrTwo == 0)
            {
                GameObject column = Instantiate(column1, this.transform);
                column.transform.position += new Vector3(columnX[x], columnY[y], 0);
            }
            else
            {
                GameObject column = Instantiate(column2, this.transform);
                column.transform.position += new Vector3(columnX[x], columnY[y], 0);
            }
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (i == x)
                    {
                        filled[i, j] = 2;
                    } else
                    {
                        filled[i, j] = 1;
                    }
                }
            }
            return true;
        }
        return false;
    }

    void placeTwoColumns()
    {
        int oneOrTwo = Random.Range(0, 2);
        if (oneOrTwo == 0)
        {
            GameObject column = Instantiate(column1, this.transform);
            column.transform.position += new Vector3(columnLeft, columnY[4], 0);
        }
        else
        {
            GameObject column = Instantiate(column2, this.transform);
            column.transform.position += new Vector3(columnLeft, columnY[4], 0);
        }

        int oneOrTwo2 = Random.Range(0, 2);
        if (oneOrTwo2 == 0)
        {
            GameObject column = Instantiate(column1, this.transform);
            column.transform.position += new Vector3(columnRight, columnY[4], 0);
        }
        else
        {
            GameObject column = Instantiate(column2, this.transform);
            column.transform.position += new Vector3(columnRight, columnY[4], 0);
        }

        for (int i = 2; i <= 6; i++)
        {
            for (int j = 3; j <= 5; j++)
            {
                if (i >= 3 && i <= 5)
                {
                    filled[i, j] = 2;
                }
                else
                {
                    filled[i, j] = 1;
                }
            }
        }
    }

    void placeSmallPlatform(int x, int y)
    {
        if (!(filled[x, y] > 0 || filled[x + 1, y] == 2 || filled[x - 1, y] == 2 || filled[x, y + 1] == 2 || filled[x, y - 1] == 2 || y < 2))
        {
            filled[x, y] = 2;
            filled[x - 1, y] = 1;
            filled[x + 1, y] = 1;
            filled[x, y + 1] = 1;
            int oneOrTwo = Random.Range(0, 2);
            if (oneOrTwo == 0)
            {
                GameObject smallPlatform = Instantiate(smallPlatform1, this.transform);
                smallPlatform.transform.position += new Vector3(platformX[x], platformY[y], 0);
            }
            else
            {
                GameObject smallPlatform = Instantiate(smallPlatform2, this.transform);
                smallPlatform.transform.position += new Vector3(platformX[x], platformY[y], 0);
            }
        }
    }

    void placeHighground(float x, float y)
    {
        GameObject highground = Instantiate(highGround1, this.transform);
        highground.transform.position += new Vector3(x, y, 0);
        for (int i = 0; i <= 8; i++)
        {
            filled[i, 4] = 2;
            filled[i, 3] = 1;
            filled[i, 5] = 1;
        }
    }

    void placePillar(float x, float y)
    {
        GameObject pillar = Instantiate(pillar1, this.transform);
        pillar.transform.position += new Vector3(x, y, 0);
            
        for (int i = 3; i <= 5; i++)
        {
            for (int j = 0; j <= 4; j++)
            {
                filled[i, j] = 2;
            }
        }
        for (int i = 3; i <=5; i++)
        {
            filled[i, 5] = 1;
        }
    }

    void placeBigPillar(float x, float y)
    {
        GameObject bigPillar = Instantiate(bigPillar1, this.transform);
        bigPillar.transform.position += new Vector3(x, y, 0);

        for (int i = 1; i <= 7; i++)
        {
            for (int j = 0; j <= 8; j++)
            {
                filled[i, j] = 2;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Random = UnityEngine.Random;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace SA
// Avoids naming conflicts
{
    public class GameManager : MonoBehaviour
    {

        //Creating variables
        //height and width of the map
        public int maxHeight = 15;
        public int maxWidth = 17;
        public continueGame timemode;
        public Transform entryContainer;
        public Transform entryTemplate;
        public List<Transform> HighScoreEntryTransformList;
        public bool Timemode;
        //Colour of the background
        public Color colour1;
        public Color colour2;
        public Sprite SnakeHead;


        public Sprite apple;
        public Sprite tailAdd;

        public Text currentScoreText;
        public Text HighscoreText;
        public Transform cameraHolder;        //adding the player on the map
        public Color player;
        public float currentTime = 0f;
        public int startingTime = 120;

        [SerializeField] Text countdownText;



        //public Saving SavingScore;


        //Adding gameObject for the player
        GameObject playerObj;
        GameObject appleObj;
        GameObject tailParent;

        Node playerNode;
        Node appleNode;
        Node previousPlayerNode;
        GameObject mapObject;
        SpriteRenderer mapRenderer;
        Sprite playerSprite;
        Node[,] grid;
        List<Node> availableNodes = new List<Node>();
        List<SpecialNode> tail = new List<SpecialNode>();
        public string Usernames;
        bool up, left, right, down;
        public int currentScore;
        public int highScore;
        public bool isGameOver;
        public bool isWinner;
        public bool isFirstInput;
        public float moveRate = 0.5f;
        float timer;
        Direction curDirection;
        public Login login;//setting the function inside this script

        public enum Direction
        {
            up, down, left, right
        }
        public UnityEvent onStart;
        public UnityEvent onGameOver;
        public UnityEvent firstInput;
        public UnityEvent onScore;
        public UnityEvent onWinner;
        public string file = "PlayerInfo.txt";
        public GameManager data;
        #region intialisation


        void Start()
        {

            onStart.Invoke();
            currentTime = startingTime;
            
            
        }
        public void StartNewGame()
        {
            ClearReferences();
            CreateMap();
            PlacePlayer();
            PlaceCamera();
            CreateApple();
            curDirection = Direction.right;
            isGameOver = false;
            currentScore = 0;
            UpdateScore();

        }
        public void ClearReferences()
        {
            //removing all the objects to be able to display the Game over screen.
            if (mapObject != null)
                Destroy(mapObject);
            if (playerObj != null)
                Destroy(playerObj);
            if (appleObj != null)
                Destroy(appleObj);
            foreach (var t in tail)
            {
                if (t.obj != null)
                    Destroy(t.obj);
            }
            tail.Clear();
            availableNodes.Clear();
            grid = null;

        }



        void CreateMap()
        //Creating the tiled background
        {
            mapObject = new GameObject("map");
            mapRenderer = mapObject.AddComponent<SpriteRenderer>();
            grid = new Node[maxWidth, maxHeight];

            Texture2D txt = new Texture2D(maxWidth, maxHeight);
            //Creating the tiled background
            for (int x = 0; x < maxWidth; x++)
            {
                for (int y = 0; y < maxHeight; y++)
                {
                    Vector3 tp = Vector3.zero;
                    tp.x = x;
                    tp.y = y;
                    Node n = new Node()
                    {
                        x = x,
                        y = y,
                        worldPosition = tp
                    };
                    grid[x, y] = n;

                    availableNodes.Add(n);

                    if (x % 2 != 0)
                    {
                        if (y % 2 != 0)
                        {
                            txt.SetPixel(x, y, colour1);
                        }
                        else
                        {
                            txt.SetPixel(x, y, colour2);
                        }
                    }
                    else
                    {
                        if (y % 2 != 0)
                        {
                            txt.SetPixel(x, y, colour2);
                        }
                        else
                        {
                            txt.SetPixel(x, y, colour1);
                        }


                    }

                }
            }
            //Making proper rectangles
            txt.filterMode = FilterMode.Point;
            //Applying sprites
            txt.Apply();
            Rect rect = new Rect(0, 0, maxWidth, maxHeight);
            Sprite sprite = Sprite.Create(txt, rect, Vector2.zero, 1, 0, SpriteMeshType.FullRect);
            mapRenderer.sprite = sprite;
        }
        void PlacePlayer()
        {

            playerObj = new GameObject("player");
            SpriteRenderer playerRender = playerObj.AddComponent<SpriteRenderer>();
            playerSprite = SnakeHead;
            playerRender.sprite = playerSprite;
            playerRender.sortingOrder = 1;
            playerNode = GetNode(3, 3);

            PlacePlayerObject(playerObj, playerNode.worldPosition);
            tailParent = new GameObject("tailParent");


        }
        void PlaceCamera()
        {
            //Finding middle position of the snake
            Node n = GetNode(maxWidth / 2, maxHeight / 2);
            cameraHolder.position = n.worldPosition;
        }
        void CreateApple()
        {
            appleObj = new GameObject("apple");
            SpriteRenderer appleRenderer = appleObj.AddComponent<SpriteRenderer>();
            appleRenderer.sprite = apple;
            appleRenderer.sortingOrder = 1;
            RandomlyPlaceApple();
        }

        #endregion
        #region Update

        void Update()
        {


            if (isGameOver)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    onStart.Invoke();
                    startingTime = 120;
                    currentTime = 120f;

                }
                return;
            }
            if (isWinner)
            {

            }





            GetInput();
            if (isFirstInput)
            {

                setDirection();
                timer += Time.deltaTime;
                if (timer > moveRate)
                {
                    timer = 0;
                    MovePlayer();
                }
                currentTime -= 1 * Time.deltaTime;
                countdownText.text = currentTime.ToString("0");
                if (currentTime <= 0)
                {
                currentTime = 0;
                onGameOver.Invoke();

                }
            }
            else
            {
                if (up || down || left || right)
                {
                    isFirstInput = true;
                    firstInput.Invoke();

                }
            }

        }

        public void GetInput()
        {
            //Controls
            up = Input.GetButtonDown("up");
            down = Input.GetButtonDown("down");
            left = Input.GetButtonDown("left");
            right = Input.GetButtonDown("right");

        }
        void setDirection()
        {
            if (up)
            {
                if (!isOpposite(Direction.up))
                    curDirection = Direction.up;
            }
            else if (down)
            {
                if (!isOpposite(Direction.down))
                    curDirection = Direction.down;

            }
            else if (left)
            {
                if (!isOpposite(Direction.left))
                    curDirection = Direction.left;

            }
            else if (right)
            {
                if (!isOpposite(Direction.right))
                    curDirection = Direction.right;

            }


        }


        public void MovePlayer()
        {

            int x = 0;
            int y = 0;
            //Adds 1 or -1 to the coordinates depending on which button is pressed
            switch (curDirection)
            {
                case Direction.up:
                    y = 1;
                    break;
                case Direction.down:
                    y = -1;
                    break;
                case Direction.left:
                    x = -1;
                    break;
                case Direction.right:
                    x = 1;
                    break;

            }
            Node targetNode = GetNode(playerNode.x + x, playerNode.y + y);
            if (targetNode == null)
            {
                onGameOver.Invoke();

            }
            else
            {
                if(currentScore >= startingTime/4)
                {
                onWinner.Invoke();


                }

                if (isTailNode(targetNode))
                {
                    //gameover
                    onGameOver.Invoke();

                }
                else
                {
                    bool isScore = false;

                    if (targetNode == appleNode)
                    {
                        isScore = true;
                        RandomlyPlaceApple();
                    }
                    Node previousNode = playerNode;
                    availableNodes.Add(previousNode);

                    if (isScore)
                    {
                        tail.Add(CreateTailNode(previousNode.x, previousNode.y));
                        availableNodes.Remove(previousNode);
                    }
                    MoveTail();

                    PlacePlayerObject(playerObj, targetNode.worldPosition);
                    playerNode = targetNode;

                    availableNodes.Remove(playerNode);
                    if (isScore)
                    {

                        currentScore++;
                        if (currentScore > highScore)
                        {
                            highScore = currentScore;

                        }
                        onScore.Invoke();

                        if (availableNodes.Count > 0)
                        {

                            RandomlyPlaceApple();
                        }
                        else
                        {
                            //you won
                        }
                    }
                }




            }
        }
        void MoveTail()
        {
            Node prevNode = null;
            for (int i = 0; i < tail.Count; i++)
            {
                SpecialNode p = tail[i];
                availableNodes.Add(p.node);
                if (i == 0)
                {
                    prevNode = p.node;
                    p.node = playerNode;
                }
                else
                {
                    Node prev = p.node;
                    p.node = prevNode;
                    prevNode = prev;


                }
                availableNodes.Remove(p.node);
                PlacePlayerObject(p.obj, p.node.worldPosition);

            }
        }
        #endregion
        #region Utilities

        public void Winner()
        {
            isWinner = true;
            isFirstInput = false;

        }
        public void GameOver()
        {
            isGameOver = true;
            isFirstInput = false;

        }
        public void UpdateScore()
        {
            currentScoreText.text = currentScore.ToString();
            HighscoreText.text = highScore.ToString();

        }
        bool isOpposite(Direction d)
        {
            switch (d)
            {
                default:
                case Direction.up:
                    if (curDirection == Direction.down)
                        return true;
                    else
                        return false;
                case Direction.down:
                    if (curDirection == Direction.up)
                        return true;
                    else
                        return false;
                case Direction.left:
                    if (curDirection == Direction.right)
                        return true;
                    else
                        return false;
                case Direction.right:
                    if (curDirection == Direction.left)
                        return true;
                    else
                        return false;
            }

        }
        bool isTailNode(Node n)
        {
            for (int i = 0; i < tail.Count; i++)
            {
                if (tail[i].node == n)
                {
                    return true;
                }
            }
            return false;
        }
        void PlacePlayerObject(GameObject obj, Vector3 pos)
        {
            pos += Vector3.one * .5f;
            obj.transform.position = pos;
        }
        void RandomlyPlaceApple()
        {
            int ran = Random.Range(0, availableNodes.Count);
            Node n = availableNodes[ran];
            PlacePlayerObject(appleObj, n.worldPosition);
            appleNode = n;
        }

        Node GetNode(int x, int y)
        {


            //Making sure the snake is not out of bounds
            if (x < 0 || x > maxWidth - 1 || y < 0 || y > maxHeight - 1)
                return null;
            return grid[x, y];

        }
        SpecialNode CreateTailNode(int x, int y)
        {
            SpecialNode s = new SpecialNode();
            s.node = GetNode(x, y);
            s.obj = new GameObject();
            s.obj.transform.parent = tailParent.transform;
            s.obj.transform.position = s.node.worldPosition;
            SpriteRenderer r = s.obj.AddComponent<SpriteRenderer>();
            r.sprite = tailAdd;
            r.sortingOrder = 1;
            return s;

        }
        Sprite CreateSprite(Color targetcolour)
        {
            //Snake poistion and colour
            Texture2D txt = new Texture2D(1, 1);
            txt.SetPixel(0, 0, targetcolour);
            txt.Apply();
            txt.filterMode = FilterMode.Point;
            Rect rect = new Rect(0, 0, 1, 1);
            return Sprite.Create(txt, rect, Vector2.one * .5f, 1, 0, SpriteMeshType.FullRect);




        }
        public void Scene1()
        {
            SceneManager.LoadScene("gameMenuScene");
        }


    }
}


    #endregion

    




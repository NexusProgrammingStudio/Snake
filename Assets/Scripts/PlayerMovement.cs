using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public KeyCode Left;
    public KeyCode Right;
    public KeyCode Up;
    public KeyCode Down;

    public enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    private enum State
    {
        Alive,
        Dead
    }

    private State state;
    public string PlayerName;
    public Direction gridMoveDirection;
    public Vector2Int gridPosition;
    private Vector2Int player1Position;
    private Vector2Int player2Position;
    private float gridMoveTimer;
    private float gridMoveTimerMax;
    private LevelGrid levelGrid;
    private int snakeBodySize;
    private List<SnakeMovePosition> snakeMovePositionList;
    private List<SnakeBodyPart> PlayerName_snakeBodyPartList;

    public void Setup(LevelGrid levelGrid)
    {
        this.levelGrid = levelGrid;
    }

    private void Awake()
    {
        //gridPosition = new Vector2Int(10, 10);
        gridMoveTimerMax = .2f;
        gridMoveTimer = gridMoveTimerMax;
        //gridMoveDirection = Direction.Right;

        snakeMovePositionList = new List<SnakeMovePosition>();
        snakeBodySize = 0;

        PlayerName_snakeBodyPartList = new List<SnakeBodyPart>();

        state = State.Alive;
    }

    private void Update()
    {
        switch (state)
        {
            case State.Alive:
                HandleInput();
                HandleGridMovement();
                break;
            case State.Dead:
                GameHandler.SnakeDied();
                break;
        }
    }

    private void HandleInput()
    {
        if (Input.GetKey(Up))
        {
            if (gridMoveDirection != Direction.Down)
            {
                gridMoveDirection = Direction.Up;
            }
        }
        if (Input.GetKey(Down))
        {
            if (gridMoveDirection != Direction.Up)
            {
                gridMoveDirection = Direction.Down;
            }
        }
        if (Input.GetKey(Left))
        {
            if (gridMoveDirection != Direction.Right)
            {
                gridMoveDirection = Direction.Left;
            }
        }
        if (Input.GetKey(Right))
        {
            if (gridMoveDirection != Direction.Left)
            {
                gridMoveDirection = Direction.Right;
            }
        }
    }

    private void HandleGridMovement()
    {
        gridMoveTimer += Time.deltaTime;
        if (gridMoveTimer >= gridMoveTimerMax)
        {
            gridMoveTimer -= gridMoveTimerMax;

            //SoundManager.PlaySound(SoundManager.Sound.SnakeMove);
            SnakeMovePosition previousSnakeMovePosition = null;
            if (snakeMovePositionList.Count > 0)
            {
                previousSnakeMovePosition = snakeMovePositionList[0];
            }

            SnakeMovePosition snakeMovePosition = new SnakeMovePosition(previousSnakeMovePosition, gridPosition, gridMoveDirection);
            snakeMovePositionList.Insert(0, snakeMovePosition);

            Vector2Int gridMoveDirectionVector;
            switch (gridMoveDirection)
            {
                default:
                case Direction.Right: gridMoveDirectionVector = new Vector2Int(+1, 0); break;
                case Direction.Left: gridMoveDirectionVector = new Vector2Int(-1, 0); break;
                case Direction.Up: gridMoveDirectionVector = new Vector2Int(0, +1); break;
                case Direction.Down: gridMoveDirectionVector = new Vector2Int(0, -1); break;
            }

            gridPosition += gridMoveDirectionVector;
            GameObject p1 = GameObject.Find("Snake 1");
            GameObject p2 = GameObject.Find("Snake 2");
            player1Position = new Vector2Int((int)p1.transform.position.x, (int)p1.transform.position.y);
            player2Position = new Vector2Int((int)p2.transform.position.x, (int)p2.transform.position.y);
            gridPosition = levelGrid.ValidateGridPosition(gridPosition);

            bool snakeAteFood = levelGrid.TrySnakeEatFood(gridPosition);
            if (snakeAteFood)
            {
                // Snake ate food, grow body
                snakeBodySize++;
                CreateSnakeBodyPart();
                SoundManager.PlaySound(SoundManager.Sound.SnakeEat);
            }

            bool snakeAteBadFood = levelGrid.TrySnakeEatBadFood(gridPosition);
            if (snakeAteBadFood)
            {
                // Snake ate Bad food, Srink body
                Debug.Log(snakeBodySize);

                if(snakeBodySize >= 1)
                {
                    DeleteSnakeBodyPart();
                    snakeBodySize--;
                }
                else 
                { 
                    snakeBodySize = 0;
                    Debug.Log(PlayerName + "'s Snake Dead...");
                    state = State.Dead;
                    GameHandler.SnakeDied();
                    SoundManager.PlaySound(SoundManager.Sound.SnakeDie);
                }
                
                SoundManager.PlaySound(SoundManager.Sound.SnakeEat);
            }

            if (snakeMovePositionList.Count >= snakeBodySize + 1)
            {
                snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
            }

            UpdateSnakeBodyParts();

            foreach (SnakeBodyPart snakeBodyPart in PlayerName_snakeBodyPartList)
            {
                Vector2Int snakeBodyPartGridPosition = snakeBodyPart.GetGridPosition();
                if (gridPosition == snakeBodyPartGridPosition)
                {
                    // Game Over!
                    //CMDebug.TextPopup("DEAD!", transform.position);
                    state = State.Dead;
                    GameHandler.SnakeDied();
                    SoundManager.PlaySound(SoundManager.Sound.SnakeDie);
                }
            }
            if(PlayerName == "Snake_1")
            {
                foreach(SnakeBodyPart snakeBodyPart in PlayerName_snakeBodyPartList)
            {
                    Vector2Int snakeBodyPartGridPosition = snakeBodyPart.GetGridPosition();
                    if (player2Position == snakeBodyPartGridPosition)
                    {
                        // Game Over!
                        //CMDebug.TextPopup("DEAD!", transform.position);
                        state = State.Dead;
                        GameHandler.SnakeDied();
                        SoundManager.PlaySound(SoundManager.Sound.SnakeDie);
                    }
                }
            }
            if (PlayerName == "Snake_2")
            {
                foreach (SnakeBodyPart snakeBodyPart in PlayerName_snakeBodyPartList)
                {
                    Vector2Int snakeBodyPartGridPosition = snakeBodyPart.GetGridPosition();
                    if (player1Position == snakeBodyPartGridPosition)
                    {
                        // Game Over!
                        //CMDebug.TextPopup("DEAD!", transform.position);
                        state = State.Dead;
                        GameHandler.SnakeDied();
                        SoundManager.PlaySound(SoundManager.Sound.SnakeDie);
                    }
                }
            }                

            if (player1Position == player2Position)
            {
                state = State.Dead;
                GameHandler.SnakeDied();
                SoundManager.PlaySound(SoundManager.Sound.SnakeDie);
                Debug.Log("Player 1 Position :" + player1Position + " Player 2 Position : " + player2Position);
            }

            transform.position = new Vector3(gridPosition.x, gridPosition.y);
            transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirectionVector) - 90);
        }
    }

    private void CreateSnakeBodyPart()
    {
        PlayerName_snakeBodyPartList.Add(new SnakeBodyPart(PlayerName_snakeBodyPartList.Count, PlayerName));
    }

    private void DeleteSnakeBodyPart()
    {
        PlayerName_snakeBodyPartList.RemoveAt((PlayerName_snakeBodyPartList.Count) - 1);
        string name = PlayerName + "_SnakeBody_" + PlayerName_snakeBodyPartList.Count;
        GameObject snakeBodyGameObject = GameObject.Find(name);
        Destroy(snakeBodyGameObject);
    }

        private void UpdateSnakeBodyParts()
    {
        for (int i = 0; i < PlayerName_snakeBodyPartList.Count; i++)
        {
            PlayerName_snakeBodyPartList[i].SetSnakeMovePosition(snakeMovePositionList[i]);
        }
    }


    private float GetAngleFromVector(Vector2Int dir)
    {
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }

    // Return the full list of positions occupied by the snake: Head + Body
    public List<Vector2Int> GetFullSnakeGridPositionList()
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int>() { gridPosition };
        foreach (SnakeMovePosition snakeMovePosition in snakeMovePositionList)
        {
            gridPositionList.Add(snakeMovePosition.GetGridPosition());
        }
        return gridPositionList;
    }




    /*
     * Handles a Single Snake Body Part
     * */
    private class SnakeBodyPart
    {

        private SnakeMovePosition snakeMovePosition;
        private Transform transform;

        public SnakeBodyPart(int bodyIndex, string name)
        {
            GameObject snakeBodyGameObject = new GameObject(name + "_SnakeBody_" + bodyIndex, typeof(SpriteRenderer));
            if(name == "Snake_1")
            {
                snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.snake1BodySprite;
            }
            else
            {
                snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.snake2BodySprite;
            }            
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder = -1 - bodyIndex;
            transform = snakeBodyGameObject.transform;
        }

        public void SetSnakeMovePosition(SnakeMovePosition snakeMovePosition)
        {
            this.snakeMovePosition = snakeMovePosition;

            transform.position = new Vector3(snakeMovePosition.GetGridPosition().x, snakeMovePosition.GetGridPosition().y);

            float angle;
            switch (snakeMovePosition.GetDirection())
            {
                default:
                case Direction.Up: // Currently going Up
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = 0;
                            break;
                        case Direction.Left: // Previously was going Left
                            angle = 0 + 45;
                            transform.position += new Vector3(.2f, .2f);
                            break;
                        case Direction.Right: // Previously was going Right
                            angle = 0 - 45;
                            transform.position += new Vector3(-.2f, .2f);
                            break;
                    }
                    break;
                case Direction.Down: // Currently going Down
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = 180;
                            break;
                        case Direction.Left: // Previously was going Left
                            angle = 180 - 45;
                            transform.position += new Vector3(.2f, -.2f);
                            break;
                        case Direction.Right: // Previously was going Right
                            angle = 180 + 45;
                            transform.position += new Vector3(-.2f, -.2f);
                            break;
                    }
                    break;
                case Direction.Left: // Currently going to the Left
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = +90;
                            break;
                        case Direction.Down: // Previously was going Down
                            angle = 180 - 45;
                            transform.position += new Vector3(-.2f, .2f);
                            break;
                        case Direction.Up: // Previously was going Up
                            angle = 45;
                            transform.position += new Vector3(-.2f, -.2f);
                            break;
                    }
                    break;
                case Direction.Right: // Currently going to the Right
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = -90;
                            break;
                        case Direction.Down: // Previously was going Down
                            angle = 180 + 45;
                            transform.position += new Vector3(.2f, .2f);
                            break;
                        case Direction.Up: // Previously was going Up
                            angle = -45;
                            transform.position += new Vector3(.2f, -.2f);
                            break;
                    }
                    break;
            }

            transform.eulerAngles = new Vector3(0, 0, angle);
        }

        public Vector2Int GetGridPosition()
        {
            return snakeMovePosition.GetGridPosition();
        }
    }



    /*
     * Handles one Move Position from the Snake
     * */
    private class SnakeMovePosition
    {

        private SnakeMovePosition previousSnakeMovePosition;
        private Vector2Int gridPosition;
        private Direction direction;

        public SnakeMovePosition(SnakeMovePosition previousSnakeMovePosition, Vector2Int gridPosition, Direction direction)
        {
            this.previousSnakeMovePosition = previousSnakeMovePosition;
            this.gridPosition = gridPosition;
            this.direction = direction;
        }

        public Vector2Int GetGridPosition()
        {
            return gridPosition;
        }

        public Direction GetDirection()
        {
            return direction;
        }

        public Direction GetPreviousDirection()
        {
            if (previousSnakeMovePosition == null)
            {
                return Direction.Right;
            }
            else
            {
                return previousSnakeMovePosition.direction;
            }
        }

    }
}

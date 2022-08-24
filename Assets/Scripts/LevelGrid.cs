using UnityEngine;

public class LevelGrid {

    private Vector2Int foodGridPosition;
    private GameObject foodGameObject;
    private Vector2Int badfoodGridPosition;
    private GameObject badfoodGameObject;
    private int width;
    private int height;
    private PlayerMovement snake;

    public LevelGrid(int width, int height) {
        this.width = width;
        this.height = height;
    }

    public void Setup(PlayerMovement snake) {
        this.snake = snake;

        SpawnFood();
        SpawnBadFood();
    }

    private void SpawnFood() {
        do {
            foodGridPosition = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
            if (badfoodGridPosition == foodGridPosition)
            {
                Object.Destroy(foodGameObject);
                SpawnFood();
            }
        } while (snake.GetFullSnakeGridPositionList().IndexOf(foodGridPosition) != -1);

        foodGameObject = new GameObject("Food", typeof(SpriteRenderer));
        foodGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.foodSprite;
        foodGameObject.transform.position = new Vector3(foodGridPosition.x, foodGridPosition.y);
    }

    private void SpawnBadFood()
    {
        do
        {
            badfoodGridPosition = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
            if (foodGridPosition == badfoodGridPosition)
            {
                Object.Destroy(badfoodGameObject);
                SpawnBadFood();
            }
        } while (snake.GetFullSnakeGridPositionList().IndexOf(badfoodGridPosition) != -1);

        badfoodGameObject = new GameObject("BadFood", typeof(SpriteRenderer));
        badfoodGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.badfoodSprite;
        badfoodGameObject.transform.position = new Vector3(badfoodGridPosition.x, badfoodGridPosition.y);
    }

    public bool TrySnakeEatFood(Vector2Int snakeGridPosition) {
        if (snakeGridPosition == foodGridPosition) {
            Object.Destroy(foodGameObject);
            SpawnFood();
            Score.AddScore();
            return true;
        } else {
            return false;
        }
    }

    public bool TrySnakeEatBadFood(Vector2Int snakeGridPosition)
    {
        if (snakeGridPosition == badfoodGridPosition)
        {
            Object.Destroy(badfoodGameObject);
            SpawnBadFood();
            Score.ReduceScore();
            return true;
        }
        else
        {
            return false;
        }
    }

    public Vector2Int ValidateGridPosition(Vector2Int gridPosition) {
        if (gridPosition.x < 0) {
            gridPosition.x = width - 1;
        }
        if (gridPosition.x > width - 1) {
            gridPosition.x = 0;
        }
        if (gridPosition.y < 0) {
            gridPosition.y = height - 1;
        }
        if (gridPosition.y > height - 1) {
            gridPosition.y = 0;
        }
        return gridPosition;
    }
}

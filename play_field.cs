using Godot;
using System;
using System.Collections.Generic;

public partial class play_field : Node2D
{
	const int _screenWidth = 32;
	public static int screenWidth {  get { return _screenWidth; } }
	//const int _screenHeight = 16;
	const int _screenHeight = 32;
	public static int screenHeight { get { return _screenHeight; } }

	public static int score = 5;
	public static bool isNotGameOver = true;
	public static string currentlMovementDirectio = "RIGHT";
	public static Random random = new Random();
	public static Color UIcolor = Colors.Aqua;
	public class SnakeBody
	{
	    public int horizontalPosition { get; set; }
	    public int verticalPosition { get; set; }
	    public Color snakeHeadeColor { get; set; }
	}

	public static List<int> horizontalPositionSnakeList = new List<int>();
	public static List<int> verticalPositionSnakeList = new List<int>();

	SnakeBody snakePosition;

	public static int horizontalBerrySpawnPosition;
	public static int verticalBerrySpawnPosition;
	public static double time;
	
	public override void _Ready()
	{
		snakePosition = new SnakeBody();
		SnakeLogic.initializeSnake(snakePosition);
        SnakeLogic.chooseBerryPozition();
        time = 0;
	}

    public override void _Draw()
    {
		var viewport = GetViewportRect();
		var viewportSize = viewport.Size;

		float tileHeight = viewport.Size.Y / _screenHeight;
		float tileWidth = viewport.Size.X / _screenWidth;

		// clear screen
		DrawRect(viewport, Colors.Black);

        // draw playfield
		var leftWall = new Rect2(0, 0, tileWidth, viewportSize.Y);
		DrawRect(leftWall, UIcolor);
		var topWall = new Rect2(0, 0, viewportSize.X, tileHeight);
		DrawRect(topWall, UIcolor);
		var rightWall = new Rect2(viewportSize.X - tileWidth, 0, tileWidth, viewportSize.Y);
		DrawRect(rightWall, UIcolor);
		var bottomWall = new Rect2(0, viewportSize.Y - tileHeight, viewportSize.X, tileHeight);
		DrawRect(bottomWall, UIcolor);
		
		// draw food
		var foodTile = new Rect2(horizontalBerrySpawnPosition * tileWidth, verticalBerrySpawnPosition * tileHeight, tileWidth, tileHeight);
		DrawRect(foodTile, Colors.Orange);

		// draw snake
		for(int i = 0; i < verticalPositionSnakeList.Count; i++) {
			var snakeTile = new Rect2(horizontalPositionSnakeList[i] * tileWidth, verticalPositionSnakeList[i] * tileHeight, tileWidth, tileHeight);
			DrawRect(snakeTile, Colors.Green);
		}

		var headTile = new Rect2(snakePosition.horizontalPosition * tileWidth, snakePosition.verticalPosition * tileHeight, tileWidth, tileHeight);
		DrawRect(headTile, snakePosition.snakeHeadeColor);

		if(!isNotGameOver) {
			DrawString(ThemeDB.FallbackFont, new Vector2(0, screenHeight / 2 * tileHeight - 8), "GAME OVER", HorizontalAlignment.Center, viewportSize.X);
			DrawString(ThemeDB.FallbackFont, new Vector2(0, screenHeight / 2 * tileHeight + 8), String.Format("Score: {0}", score), HorizontalAlignment.Center, viewportSize.X);
		}
    }

    public override void _Process(double delta)
	{
		time += delta;
		SnakeLogic.selectSnakeDirection();
		if(time < 0.5) return;


		if (isNotGameOver)
		{
    		SnakeLogic.isSnakeOutsideBorder(snakePosition);

    		if (SnakeLogic.isSnakeOnBarry(snakePosition))
    		{
    		    score++;
    		    SnakeLogic.chooseBerryPozition();
    		}
    		SnakeLogic.isSnakeIntersecting(snakePosition);
    		if (isNotGameOver == false)
    		{
				QueueRedraw();	
				return;
    		}
    		SnakeLogic.addSnakeBodyPosition(snakePosition);
    		SnakeLogic.moveSnake(snakePosition);
			QueueRedraw();	
		}
		time = 0;
	}
}

    static class SnakeLogic
    {
        public static void chooseBerryPozition()
        {
            int spawnBerryMargin = 1;
            play_field.horizontalBerrySpawnPosition = play_field.random.Next(spawnBerryMargin, play_field.screenWidth - spawnBerryMargin);
            play_field.verticalBerrySpawnPosition = play_field.random.Next(spawnBerryMargin, play_field.screenHeight - spawnBerryMargin);
        }


        public static void initializeSnake(play_field.SnakeBody snakePosition)
        {
            snakePosition.horizontalPosition = play_field.screenWidth / 2;
            snakePosition.verticalPosition = play_field.screenHeight / 2;
            snakePosition.snakeHeadeColor = Colors.Red;
        }


        public static void selectSnakeDirection()
        {                
			if (Input.IsActionPressed("up")) {
				play_field.currentlMovementDirectio = "UP";
			}

			if (Input.IsActionPressed("down")) {
				play_field.currentlMovementDirectio = "DOWN";
			}

			if (Input.IsActionPressed("left")) {
				play_field.currentlMovementDirectio = "LEFT";
			}

			if (Input.IsActionPressed("right")) {
				play_field.currentlMovementDirectio = "RIGHT";
			}
        }

        public static void moveSnake(play_field.SnakeBody snakePosition)
        {
            switch (play_field.currentlMovementDirectio)
            {
                case "UP":
                    snakePosition.verticalPosition--;
                    break;
                case "DOWN":
                    snakePosition.verticalPosition++;
                    break;
                case "LEFT":
                    snakePosition.horizontalPosition--;
                    break;
                case "RIGHT":
                    snakePosition.horizontalPosition++;
                    break;
            }
            if (play_field.horizontalPositionSnakeList.Count > play_field.score)
            {
                play_field.horizontalPositionSnakeList.RemoveAt(0);
                play_field.verticalPositionSnakeList.RemoveAt(0);
            }
        }
        public static bool isSnakeOutsideBorder(play_field.SnakeBody snakePosition)
        {
            if (snakePosition.horizontalPosition == play_field.screenWidth - 1 || snakePosition.horizontalPosition == 0
                || snakePosition.verticalPosition == play_field.screenHeight - 1 || snakePosition.verticalPosition == 0)
            {
                play_field.isNotGameOver = false;
                return false;
            }
            else
            {
                play_field.isNotGameOver = true;
                return true;
            }
        }

        public static bool isSnakeIntersecting(play_field.SnakeBody snakePosition)
        {
            for (int currentSnakeSquare = 0; currentSnakeSquare < play_field.horizontalPositionSnakeList.Count; currentSnakeSquare++)
            {
                if (SnakeLogic.isSnakeTouchingItSelf(snakePosition, currentSnakeSquare))
                {
                    play_field.isNotGameOver = false;
                    return true;
                }
            }
            return false;
        }

        public static bool isSnakeOnBarry(play_field.SnakeBody snakePosition)
        {
            return play_field.horizontalBerrySpawnPosition == snakePosition.horizontalPosition && play_field.verticalBerrySpawnPosition == snakePosition.verticalPosition;
        }

        public static bool isSnakeTouchingItSelf(play_field.SnakeBody snakePosition, int i)
        {
            return play_field.horizontalPositionSnakeList[i] == snakePosition.horizontalPosition && play_field.verticalPositionSnakeList[i] == snakePosition.verticalPosition;
        }

		public static void addSnakeBodyPosition(play_field.SnakeBody snakePosition)
        {
            play_field.horizontalPositionSnakeList.Add(snakePosition.horizontalPosition);
            play_field.verticalPositionSnakeList.Add(snakePosition.verticalPosition);
        }

    }

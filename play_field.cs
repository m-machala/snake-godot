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
	public static Color UIcolor = Colors.Green;
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

	public static DateTime currentTime;
	public static DateTime updateTime;
	
	public override void _Ready()
	{
		snakePosition = new SnakeBody();
		SnakeLogic.initializeSnake(snakePosition);
        SnakeLogic.chooseBerryPozition();
        SnakeLogic.startTimer();

		/*horizontalPositionSnakeList.Add(5);
		verticalPositionSnakeList.Add(5);
		horizontalPositionSnakeList.Add(6);
		verticalPositionSnakeList.Add(5);
		horizontalPositionSnakeList.Add(7);
		verticalPositionSnakeList.Add(5);*/
	}

    public override void _Draw()
    {
		Viewport viewport = GetViewport();

		var viewportRect = viewport.GetVisibleRect();
		float tileHeight = viewportRect.Size.Y / _screenHeight;
		float tileWidth = viewportRect.Size.X / _screenWidth;

		// clear screen
		DrawRect(viewportRect, Colors.Black);

        // draw playfield
		var wallColor = Colors.White;
		var leftWall = new Rect2(0, 0, tileWidth, viewportRect.Size.Y);
		DrawRect(leftWall, wallColor);
		var topWall = new Rect2(0, 0, viewportRect.Size.X, tileHeight);
		DrawRect(topWall, wallColor);
		var rightWall = new Rect2(viewportRect.Size.X - tileWidth, 0, tileWidth, viewportRect.Size.Y);
		DrawRect(rightWall, wallColor);
		var bottomWall = new Rect2(0, viewportRect.Size.Y - tileHeight, viewportRect.Size.X, tileHeight);
		DrawRect(bottomWall, wallColor);

		// draw snake
		for(int i = 0; i < verticalPositionSnakeList.Count; i++) {
			var snakeTile = new Rect2(horizontalPositionSnakeList[i] * tileWidth, verticalPositionSnakeList[i] * tileHeight, tileWidth, tileHeight);
			DrawRect(snakeTile, Colors.Green);
		}

		var headTile = new Rect2(snakePosition.horizontalPosition * tileWidth, snakePosition.verticalPosition * tileHeight, tileWidth, tileHeight);
		DrawRect(headTile, snakePosition.snakeHeadeColor);

		// draw food
    }

    public override void _Process(double delta)
	{
		if (isNotGameOver)
		{
    		SnakeLogic.isSnakeOutsideBorder(snakePosition);

    		//Console.ForegroundColor = UIcolor;
    		if (SnakeLogic.isSnakeOnBarry(snakePosition))
    		{
    		    score++;
    		    SnakeLogic.chooseBerryPozition();
    		}
    		SnakeLogic.isSnakeIntersecting(snakePosition);
    		if (isNotGameOver == false)
    		{
    		    //SnakeUI.drawGameOverText();
				return;
    		}
    		SnakeLogic.selectSnakeDirection();
    		SnakeLogic.addSnakeBodyPosition(snakePosition);
    		SnakeLogic.moveSnake(snakePosition);
			QueueRedraw();	
		}
	}
}

    static class SnakeLogic
    {
        public static void startTimer()
        {
            play_field.currentTime = DateTime.Now;
            play_field.updateTime = DateTime.Now;
        }

        public static void chooseBerryPozition()
        {
            int spawnBerryMargin = 2;
            play_field.horizontalBerrySpawnPosition = play_field.random.Next(0, play_field.screenWidth - spawnBerryMargin);
            play_field.verticalBerrySpawnPosition = play_field.random.Next(0, play_field.screenHeight - spawnBerryMargin);
        }


        public static void initializeSnake(play_field.SnakeBody snakePosition)
        {
            snakePosition.horizontalPosition = play_field.screenWidth / 2;
            snakePosition.verticalPosition = play_field.screenHeight / 2;
            snakePosition.snakeHeadeColor = Colors.Red;
        }


        public static void selectSnakeDirection()
        {
            play_field.currentTime = DateTime.Now;
            while (true)
            {
                play_field.updateTime = DateTime.Now;
                if (play_field.updateTime.Subtract(play_field.currentTime).TotalMilliseconds > 500) { break; }
                /*if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo pressedKey = Console.ReadKey(true);
                    //Console.WriteLine(pressedKey.Key.ToString());
                    if (pressedKey.Key.Equals(ConsoleKey.UpArrow))
                    {
                        play_field.currentlMovementDirectio = "UP";
                    }
                    if (pressedKey.Key.Equals(ConsoleKey.DownArrow))
                    {
                        play_field.currentlMovementDirectio = "DOWN";
                    }
                    if (pressedKey.Key.Equals(ConsoleKey.LeftArrow))
                    {
                        play_field.currentlMovementDirectio = "LEFT";
                    }
                    if (pressedKey.Key.Equals(ConsoleKey.RightArrow))
                    {
                        play_field.currentlMovementDirectio = "RIGHT";
                    }
                }*/
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
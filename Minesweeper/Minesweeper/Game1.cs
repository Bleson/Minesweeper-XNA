using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Minesweeper
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Variables
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        int uiHeight = 50; //pixels

        #region Texture variables
        SpriteFont font;
        Texture2D blankTexture;
        Texture2D clicked1Texture;
        Texture2D clicked2Texture;
        Texture2D clicked3Texture;
        Texture2D clicked4Texture;
        Texture2D clicked5Texture;
        Texture2D clicked6Texture;
        Texture2D clicked7Texture;
        Texture2D clicked8Texture;
        Texture2D mineTexture;
        Texture2D unclickedTexture;
        Texture2D flagTexture;
        
        int tileSize;
        #endregion

        #region Text variables
        float textScale = 0.5f;
        int textPaddings = 20;
        Color winTextColor = Color.Red;
        Color generalTextColor = Color.Black;

        string textToPlayer = "";
        string winStreakText = "Streak: ";
        string minesLeftText = "Mines: ";

        Vector2 winTextPosition;
        Vector2 winStreakTextPosition;
        Vector2 minesLeftTextPosition;
        #endregion

        #region Round
        float nextRoundTimer = 0f; //seconds
        float timeToNextRound = 3f; //seconds
        bool gameOver = false;
        int winStreak = 0;
        int minesLeft;
        #endregion

        #region Grid settings
        Square[,] grid;
        int width = 25;
        int height = 25;
        int mines = 80;
        #endregion

        bool mouse1Down = false;
        bool mouse2Down = false;
        int squareUnderMouseX = 0;
        int squareUnderMouseY = 0;
        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            InitializeGrid();
            this.IsMouseVisible = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("TextFont");
            blankTexture = Content.Load<Texture2D>("blank");
            clicked1Texture = Content.Load<Texture2D>("clicked_1");
            clicked2Texture = Content.Load<Texture2D>("clicked_2");
            clicked3Texture = Content.Load<Texture2D>("clicked_3");
            clicked4Texture = Content.Load<Texture2D>("clicked_4");
            clicked5Texture = Content.Load<Texture2D>("clicked_5");
            clicked6Texture = Content.Load<Texture2D>("clicked_6");
            clicked7Texture = Content.Load<Texture2D>("clicked_7");
            clicked8Texture = Content.Load<Texture2D>("clicked_8");
            mineTexture = Content.Load<Texture2D>("mine");
            unclickedTexture = Content.Load<Texture2D>("unclicked");
            flagTexture = Content.Load<Texture2D>("flagged");
            tileSize = blankTexture.Width;

            SetScreenSize();
            winTextPosition = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2,
                uiHeight / 2);
            winStreakTextPosition = new Vector2(textPaddings, uiHeight / 2);
            minesLeftTextPosition = new Vector2(graphics.GraphicsDevice.Viewport.Width - textPaddings, uiHeight / 2);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            if (gameOver)
            {
                nextRoundTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (nextRoundTimer > timeToNextRound)
                {
                    InitializeGrid();
                }
            }
            else
            {
                #region Mouse1 input
                //Mouse is clicked
                if (Mouse.GetState().LeftButton == ButtonState.Pressed && mouse1Down == false)
                {
                    mouse1Down = true;
                    squareUnderMouseX = Mouse.GetState().X / tileSize;
                    squareUnderMouseY = (Mouse.GetState().Y - uiHeight) / tileSize;
                }
                //Mouse is released
                else if (Mouse.GetState().LeftButton == ButtonState.Released && mouse1Down == true)
                {
                    int squareUnderMouseXRelease = Mouse.GetState().X / tileSize;
                    int squareUnderMouseYRelease = (Mouse.GetState().Y - uiHeight) / tileSize;

                    if (squareUnderMouseX == squareUnderMouseXRelease && squareUnderMouseY == squareUnderMouseYRelease)
                    {
                        OpenClickedSquare();
                        CheckForWin();
                    }
                    mouse1Down = false;
                }
                else if (Mouse.GetState().LeftButton == ButtonState.Released)
                {
                    mouse1Down = false;
                }
                #endregion

                #region Mouse2 input
                //Mouse2 is pressed
                if (Mouse.GetState().RightButton == ButtonState.Pressed && mouse2Down == false)
                {
                    mouse2Down = true;
                    squareUnderMouseX = Mouse.GetState().X / tileSize;
                    squareUnderMouseY = (Mouse.GetState().Y - uiHeight) / tileSize;
                }
                //Mouse2 is released
                else if (Mouse.GetState().RightButton == ButtonState.Released && mouse2Down == true)
                {
                    int squareUnderMouseXRelease = Mouse.GetState().X / tileSize;
                    int squareUnderMouseYRelease = (Mouse.GetState().Y - uiHeight) / tileSize;

                    if (squareUnderMouseX == squareUnderMouseXRelease && squareUnderMouseY == squareUnderMouseYRelease)
                    {
                        FlagClickedSquare();
                    }
                    mouse2Down = false;
                }
                else if (Mouse.GetState().RightButton == ButtonState.Released)
                {
                    mouse2Down = false;
                }
                #endregion

                //Debugging
                //if (Keyboard.GetState().IsKeyDown(Keys.Enter) && !gameOver)
                //{
                //    OpenAll();
                //}
            }
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGray);
            spriteBatch.Begin();

            #region Draw grid
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Vector2 renderPosition = new Vector2(x * blankTexture.Height, y * blankTexture.Width + uiHeight);

                    if (grid[x, y].isFlagged)
                    {
                        spriteBatch.Draw(flagTexture, renderPosition, Color.White);
                    }
                    else if (!grid[x, y].isClicked)
                    {
                        spriteBatch.Draw(unclickedTexture, renderPosition, Color.White);
                    }
                    else if (grid[x, y].isMine)
                    {
                        spriteBatch.Draw(mineTexture, renderPosition, Color.White);
                    }
                    else
                    {
                        switch (grid[x, y].minesNearby)
                        {
                            case 1:
                                spriteBatch.Draw(clicked1Texture, renderPosition, Color.White);
                                break;
                            case 2:
                                spriteBatch.Draw(clicked2Texture, renderPosition, Color.White);
                                break;
                            case 3:
                                spriteBatch.Draw(clicked3Texture, renderPosition, Color.White);
                                break;
                            case 4:
                                spriteBatch.Draw(clicked4Texture, renderPosition, Color.White);
                                break;
                            case 5:
                                spriteBatch.Draw(clicked5Texture, renderPosition, Color.White);
                                break;
                            case 6:
                                spriteBatch.Draw(clicked6Texture, renderPosition, Color.White);
                                break;
                            case 7:
                                spriteBatch.Draw(clicked7Texture, renderPosition, Color.White);
                                break;
                            case 8:
                                spriteBatch.Draw(clicked8Texture, renderPosition, Color.White);
                                break;
                            default:
                                spriteBatch.Draw(blankTexture, renderPosition, Color.White);
                                break;
                        }
                    }
                }
            }
            #endregion

            //Draw text
            Vector2 fontOrigin = font.MeasureString(textToPlayer) / 2;
            if (textToPlayer != "")
            {
                spriteBatch.DrawString(font, textToPlayer, winTextPosition, winTextColor, 0, fontOrigin, textScale, SpriteEffects.None, 0.5f);
            }
            //Winstreak
            fontOrigin = new Vector2(0, font.MeasureString(winStreakText + winStreak.ToString()).Y / 2);
            spriteBatch.DrawString(font, winStreakText + winStreak.ToString(), winStreakTextPosition, generalTextColor, 0, fontOrigin, textScale, SpriteEffects.None, 0.5f);
            //Mines left
            fontOrigin = new Vector2(font.MeasureString(minesLeftText + minesLeft.ToString()).X, font.MeasureString(minesLeft.ToString()).Y / 2);
            spriteBatch.DrawString(font, minesLeftText + minesLeft.ToString(), minesLeftTextPosition, generalTextColor, 0, fontOrigin, textScale, SpriteEffects.None, 0.5f);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void SetScreenSize()
        {
            graphics.PreferredBackBufferWidth = width * tileSize;
            graphics.PreferredBackBufferHeight = height * tileSize + uiHeight;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            Window.Title = "Minesweeper";
        }

        #region Winning & Losing

        private void CheckForWin()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (!grid[x, y].isClicked && !grid[x, y].isMine)
                    {
                        return;
                    }
                }
            };
            Win();
        }

        private void Win()
        {
            textToPlayer = "You win!";
            winTextColor = Color.Green;
            gameOver = true;
            nextRoundTimer = 0f;
            winStreak++;
        }

        private void Lose()
        {
            
            textToPlayer = "Game Over";
            winTextColor = Color.Red;
            gameOver = true;
            nextRoundTimer = 0f;
            winStreak = 0;
            OpenAllMines();
        }

        #endregion

        #region Using squares/tiles
        private void FlagClickedSquare()
        {
            int mouseX = Mouse.GetState().X;
            int mouseY = Mouse.GetState().Y - uiHeight;

            if (mouseY <= 0 && mouseX <= 0)
            {
                return;
            }

            if (grid[mouseX / tileSize, mouseY / tileSize].isFlagged)
            {
                minesLeft++;
            }
            else
            {
                minesLeft--;
            }

            grid[mouseX / tileSize, mouseY / tileSize].Flag();
        }

        private void OpenClickedSquare()
        {
            int mouseX = Mouse.GetState().X;
            int mouseY = Mouse.GetState().Y - uiHeight;

            OpenAndFlood((mouseX / tileSize), mouseY / tileSize);
        }

        void OpenAndFlood(int xIndex, int yIndex)
        {
            //Prevent flooding going out of bounds and flooding to tiles that are already filled.
            if (xIndex >= width || yIndex >= height || xIndex < 0 || yIndex < 0)
            {
                return;
            }
            else if (grid[xIndex, yIndex].isClicked || grid[xIndex, yIndex].isFlagged)
            {
                return;
            }
            else if (grid[xIndex, yIndex].isMine)
            {
                grid[xIndex, yIndex].Open();
                Lose();
                return;
            }

            //Open and begin flooding
            grid[xIndex, yIndex].Open();
            if (grid[xIndex, yIndex].minesNearby == 0)
            {
                if (yIndex != 0)
                {
                    OpenAndFlood(xIndex, yIndex - 1);
                    if (xIndex != 0)
                    {
                        OpenAndFlood(xIndex - 1, yIndex - 1);
                    }
                    if (xIndex != width)
                    {
                        OpenAndFlood(xIndex + 1, yIndex - 1);
                    }
                }
                if (yIndex != height)
                {
                    OpenAndFlood(xIndex, yIndex + 1);
                    if (xIndex != 0)
                    {
                        OpenAndFlood(xIndex - 1, yIndex + 1);
                    }
                    if (xIndex != width)
                    {
                        OpenAndFlood(xIndex + 1, yIndex + 1);
                    }
                }
                if (xIndex != 0)
                {
                    OpenAndFlood(xIndex - 1, yIndex);
                }
                if (xIndex != height)
                {
                    OpenAndFlood(xIndex + 1, yIndex);
                }
            }
        }

        void OpenAllMines()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (grid[x, y].isMine)
                    {
                        grid[x, y].Open();
                    }
                }
            }
        }

        void OpenAll()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    grid[x, y].Open();
                }
            }
        }
        #endregion

        #region Grid preparing
        void InitializeGrid()
        {
            textToPlayer = "";
            gameOver = false;
            CreateBlanks();
            RandomizeMines(mines);
            minesLeft = mines;
        }

        void CreateBlanks()
        {
            grid = new Square[width, height];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    grid[x, y] = new Square();
                }
            }
        }

        void RandomizeMines(int amountOfMines)
        {
            Random rng = new Random();
            for (int i = 0; i < amountOfMines; i++)
            {
                int x = rng.Next(width);
                int y = rng.Next(height);
                if (grid[x, y].isMine)
                {
                    i--; //Randomizes one more time
                }
                else
                {
                    grid[x, y].AddMine();
                    AddNumbersToSurroundingSquares(x , y);
                }
            }
        }

        void AddNumbersToSurroundingSquares(int xLocation, int yLocation)
        {
            //Add numbers to squares above
            if (yLocation > 0)
            {
                grid[xLocation, yLocation - 1].minesNearby++;
                if (xLocation > 0)
                {
                    grid[xLocation - 1, yLocation - 1].minesNearby++;
                }
                if (xLocation < width - 1)
                {
                    grid[xLocation + 1, yLocation - 1].minesNearby++;
                }
            }
            //Add numbers to squares below
            if (yLocation < height - 1)
            {
                grid[xLocation, yLocation + 1].minesNearby++;
                if (xLocation > 0)
                {
                    grid[xLocation - 1, yLocation + 1].minesNearby++;
                }
                if (xLocation < width - 1)
                {
                    grid[xLocation + 1, yLocation + 1].minesNearby++;
                }
            }
            if (xLocation > 0)
            {
                grid[xLocation - 1, yLocation].minesNearby++;
            }
            if (xLocation < height - 1)
            {
                grid[xLocation + 1, yLocation].minesNearby++;
            }
        }
        #endregion
    }
}

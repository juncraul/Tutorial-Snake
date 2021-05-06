using System;
using System.Drawing;
using System.Windows.Forms;

namespace Tutorial_Snake
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Timer timer;
        int sizeMatrix;
        int[,] matrix;
        SnakeDirection snakeDirection;
        Point headPosition;
        int lastSegment;
        Random random;

        enum MatrixObject
        {
            Food = -1,
            Spike = -2
        }

        enum SnakeDirection
        {
            Up, 
            Right,
            Down,
            Left
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            random = new Random();
            timer = new Timer();
            timer.Interval = 200;
            timer.Start();
            timer.Tick += Timer_Tick;
            sizeMatrix = 20;
            matrix = new int[sizeMatrix, sizeMatrix];

            Initialize();
        }

        private void Initialize()
        {
            for (int i = 0; i < sizeMatrix; i++)
            {
                for (int j = 0; j < sizeMatrix; j++)
                {
                    matrix[i, j] = 0;
                }
            }
            GenerateFood();
            headPosition = new Point(5, 5);
            matrix[5, 5] = 1;
            matrix[6, 5] = 2;
            matrix[7, 5] = 3;
            lastSegment = 3;
            snakeDirection = SnakeDirection.Left;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            GameLogic();
            Draw();
        }

        private void Draw()
        {
            using (Bitmap bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.FillRectangle(Brushes.Gray, 0, 0, pictureBox1.Width, pictureBox1.Height);

                    SizeF sizeCell = new SizeF((float)pictureBox1.Width / sizeMatrix, (float)pictureBox1.Height / sizeMatrix);

                    for (int i = 0; i < sizeMatrix; i++)
                    {
                        for (int j = 0; j < sizeMatrix; j++)
                        {
                            if (matrix[i, j] == 0)
                            {
                                graphics.FillRectangle(Brushes.White, i * sizeCell.Width + 1, j * sizeCell.Height + 1, sizeCell.Width - 2, sizeCell.Height - 2);
                            }
                            else if (matrix[i, j] == (int)MatrixObject.Food)
                            {
                                graphics.FillRectangle(Brushes.Red, i * sizeCell.Width + 1, j * sizeCell.Height + 1, sizeCell.Width - 2, sizeCell.Height - 2);
                            }
                            else
                            {
                                graphics.FillRectangle(Brushes.Blue, i * sizeCell.Width + 1, j * sizeCell.Height + 1, sizeCell.Width - 2, sizeCell.Height - 2);
                            }
                        }
                    }

                    if (pictureBox1.Image != null)
                    {
                        pictureBox1.Image.Dispose();
                    }

                    pictureBox1.Image = (Image)bitmap.Clone();
                }
            }
        }

        private void GameLogic()
        {
            Point walkPosition;
            switch (snakeDirection)
            {
                case SnakeDirection.Up:
                    walkPosition = new Point(headPosition.X, headPosition.Y - 1);
                    break;
                case SnakeDirection.Right:
                    walkPosition = new Point(headPosition.X + 1, headPosition.Y);
                    break;
                case SnakeDirection.Down:
                    walkPosition = new Point(headPosition.X, headPosition.Y + 1);
                    break;
                case SnakeDirection.Left:
                    walkPosition = new Point(headPosition.X - 1, headPosition.Y);
                    break;
                default:
                    throw new Exception("It is not possible for the snake to not have a direction");
            }
            if (walkPosition.X < 0 || walkPosition.Y < 0 || walkPosition.X == sizeMatrix || walkPosition.Y == sizeMatrix || matrix[walkPosition.X, walkPosition.Y] > 0)
            {
                Initialize();
                return;
            }
            if (matrix[walkPosition.X, walkPosition.Y] == (int)MatrixObject.Food)
            {
                lastSegment++;
                GenerateFood();
            }
            matrix[walkPosition.X, walkPosition.Y] = 1;
            matrix[headPosition.X, headPosition.Y]++;
            
            for (int i = 0; i < sizeMatrix; i++)
            {
                for (int j = 0; j < sizeMatrix; j++)
                {
                    if(headPosition.X == i && headPosition.Y == j)
                    {
                        continue;
                    }
                    if (matrix[i, j] == lastSegment)
                    {
                        matrix[i, j] = 0;
                    }
                    else if (matrix[i, j] > 1)
                    {
                        matrix[i, j]++;
                    }
                }
            }

            headPosition = walkPosition;
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch(e.KeyChar)
            {
                case 'w':
                    if (snakeDirection != SnakeDirection.Down)
                        snakeDirection = SnakeDirection.Up;
                    break;
                case 'd':
                    if (snakeDirection != SnakeDirection.Left)
                        snakeDirection = SnakeDirection.Right;
                    break;
                case 's':
                    if (snakeDirection != SnakeDirection.Up)
                        snakeDirection = SnakeDirection.Down;
                    break;
                case 'a':
                    if (snakeDirection != SnakeDirection.Right)
                        snakeDirection = SnakeDirection.Left;
                    break;
            }
        }

        private void GenerateFood()
        {
            Point foodPosition;

            do
            {
                foodPosition = new Point(random.Next() % sizeMatrix, random.Next() % sizeMatrix);
            } while (matrix[foodPosition.X, foodPosition.Y] != 0);
            //} while (matrix[foodPosition.X, foodPosition.Y] != 0 || foodPosition.X == 0 || foodPosition.Y == 0 || foodPosition.X == sizeMatrix - 1 || foodPosition.Y == sizeMatrix - 1); // Do not generate food on edge, good for easy game

            matrix[foodPosition.X, foodPosition.Y] = (int)MatrixObject.Food;
        }
    }
}

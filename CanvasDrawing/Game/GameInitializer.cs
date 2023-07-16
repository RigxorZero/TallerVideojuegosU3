using CanvasDrawing.UtalEngine2D_2023_1;
using System.Windows.Forms;
using System.Drawing;

namespace CanvasDrawing.Game
{
    public static class GameInitializer //Dibuja el tablero
    {
        public static Image ballSprite { get; private set; }
        public static void InitializeGame(Form form)
        {
            Image piso = Properties.Resources.Piso;
            Image muro = Properties.Resources.Muro;
            ballSprite = Properties.Resources.BouncingBall;

            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 11; j++)
                {
                    if (j == 0 || j == 10) // Bloque horizontal inferior
                    {
                        Wall wall = new Wall(muro, new Vector2(50, 50), i * 50 + 25, j * 50 + 25);
                        wall.rigidbody.isStatic = true;
                    }
                    else
                    {
                        new BackgroundElement(piso, new Vector2(50, 50), i * 50 + 25, j * 50 + 25);
                    }
                }
            }

            // Obtén las dimensiones de la cámara
            int cameraWidth = GameEngine.MainCamera.xSize;
            int cameraHeight = GameEngine.MainCamera.ySize;

            // Calcula la posición inicial en el centro de la pantalla
            float ballX = cameraWidth / 2f;
            float ballY = cameraHeight / 2f;

            // Crea una instancia de Ball en el centro de la pantalla
            new Ball(2, ballSprite, new Vector2(16, 16), ballX, ballY);

            GameEngine.InitEngine(form);
        }
    }
}



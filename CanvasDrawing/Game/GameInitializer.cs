using CanvasDrawing.UtalEngine2D_2023_1;
using System.Windows.Forms;
using System.Drawing;

namespace CanvasDrawing.Game
{
    public static class GameInitializer //Dibuja el tablero
    {
        
        public static void InitializeGame(Form form)
        {
            Image piso = Properties.Resources.Piso;
            Image muro = Properties.Resources.Muro;
            

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

            GameEngine.InitEngine(form);
        }
    }
}



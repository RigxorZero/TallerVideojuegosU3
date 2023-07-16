
using CanvasDrawing.Game;
using CanvasDrawing.UtalEngine2D_2023_1.Physics;
using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows.Forms;

namespace CanvasDrawing.UtalEngine2D_2023_1
{
    public static class GameEngine
    {
        //Formulario, loop y camara
        private static Form engineDrawForm;
        private static Thread gameLoopThread = null;
        public static Camera MainCamera = new Camera();
        //Manejo de pantallas
        public static bool playerLost = false; // Variable para indicar si el jugador ha perdido
        private static GameInicio gameInicio;
        private static GameOverScreen gameOverScreen;
        //Clase HUD
        public static HealthBar healthBar;
        //Creacion de personaje y enemigos
        public static Image jugador2 { get; private set; }
        public static Image jugador { get; private set; }
        public static Player player { get; private set; }
        public static Player2 player2 { get; private set; }

        private static float timeScale = 1f;
        public static float TimeScale
        {
            get { return timeScale; }
            set { timeScale = value; }
        }

        //Clases destroy
        public static void Destroy(GameObject go)
        {
            GameObjectManager.AllDeadGameObjects.Add(go);
        }
        public static void Destroy(UtalText utalText)
        {
            GameObjectManager.AllDeadText.Add(utalText);
        }
        public static void Destroy(EmptyUpdatable empty)
        {
            GameObjectManager.AllDeadEmptyUpdatables.Add(empty);
        }
        //Inicio del GameEngine
        public static void InitEngine(Form engineDrawForm)
        {
            //Asigna el formulario
            GameEngine.engineDrawForm = engineDrawForm;
            //Establece el loop
            gameLoopThread = new Thread(GameLoop);
            //Suscribe al formulario en las funciones de InputManager
            engineDrawForm.KeyPress += new KeyPressEventHandler(InputManager.KeyPressHandler);
            engineDrawForm.KeyDown += new KeyEventHandler(InputManager.KeyDownHandler);
            engineDrawForm.KeyUp += new KeyEventHandler(InputManager.KeyUpHandler);
            //Asigna el tamaño del form al doble que el de la camara
            engineDrawForm.Height = MainCamera.ySize;
            engineDrawForm.Width = MainCamera.xSize;
            //Asigna imagenes a jugador y NPC
            jugador = Properties.Resources.BouncingBallBar;
            //Gestión de spawn de player
            Random random = new Random();
            player = Player.GetInstance(2, jugador, new Vector2(32, 64), 50, 300);
            player2 = Player2.GetInstance(2, jugador, new Vector2(32, 64), 950, 300);
            //Crea instancias de otras pantallas
            gameInicio = new GameInicio(engineDrawForm);
            gameOverScreen = new GameOverScreen(engineDrawForm);
            //Muestra pantalla de inicio
            gameInicio.Show();
            //Comienza el loop
            gameLoopThread.Start(); 
        }
        public static void Start()
        {
            /*  MUESTRA PANTALLA DE INICIO  */
            gameInicio.Show();
        }
        private static void GameLoop()
        {
            while (!engineDrawForm.IsDisposed)
            {
                Thread.Sleep(1000 / 120);
                try
                {
                    engineDrawForm.Refresh();
                }
                catch
                {
                    engineDrawForm.Invalidate();
                }

                //Actualiza tiempo, objetos y fisicas
                Time.UpdateDeltaTime();
                GameObjectManager.Update();
                PhysicsEngine.Update();

                //Verifica si perdio el jugador
                if (playerLost)
                {
                    gameOverScreen.Show();
                    break; // Salir del bucle de juego
                }
                //Actualiza entradas del jugador
                InputManager.Update();

                //Si no se ha perdido actualiza deadUpdate
                if (!gameInicio.IsActive && !playerLost)
                {
                    GameObjectManager.DeadUpdate();
                }
            }
        }
        // Enfoque de la cámara
        public static Vector2 WorldToCameraPos(Vector2 pos)
        {
            float minX = MainCamera.Position.x;
            float maxX = MainCamera.Position.x + (1920 - 2 * 50) * MainCamera.scale - MainCamera.xSize;
            float minY = MainCamera.Position.y;
            float maxY = MainCamera.Position.y + (1080 - 2 * 50) * MainCamera.scale - MainCamera.ySize;

            float clampedX = Math.Max(minX, Math.Min(pos.x, maxX));
            float clampedY = Math.Max(minY, Math.Min(pos.y, maxY));

            return new Vector2((clampedX - MainCamera.Position.x) / MainCamera.scale + MainCamera.xSize,
                               (clampedY - MainCamera.Position.y) / MainCamera.scale + MainCamera.ySize);
        }


        //En caso de redimensionar la pantalla
        public static void Paint(Object sender, PaintEventArgs e)
        {
            int newXSize = engineDrawForm.Width;
            int newYSize = engineDrawForm.Height;
            bool changed = false;
            if (e.Graphics.ClipBounds.Width < MainCamera.xSize)
            {
                newXSize = MainCamera.xSize + (MainCamera.xSize - (int)e.Graphics.ClipBounds.Width);
                changed = true;
            }
            if (e.Graphics.ClipBounds.Height < MainCamera.ySize)
            {
                newYSize = MainCamera.ySize + (MainCamera.ySize - (int)e.Graphics.ClipBounds.Height);
                changed = true;
            }
            if (e.Graphics.ClipBounds.Height > MainCamera.ySize)
            {
                newYSize = engineDrawForm.Height - ((int)e.Graphics.ClipBounds.Height - MainCamera.ySize);
                changed = true;
            }
            if (e.Graphics.ClipBounds.Width > MainCamera.xSize)
            {
                newXSize = engineDrawForm.Width - ((int)e.Graphics.ClipBounds.Width - MainCamera.xSize);
                changed = true;
            }
            if (changed)
            {
                engineDrawForm.Size = new Size(newXSize, newYSize);
            }
            Draw(e.Graphics);
        }
        //Dibuja todos los objetos con GameObjectManager
        private static void Draw(Graphics graphics)
        {
            for (int i = 0; i < GameObjectManager.AllGameObjects.Count; i++)
            {
                GameObject go = GameObjectManager.AllGameObjects[i];
                go.Draw(graphics, MainCamera);
            }
            //Dibuja los textos de UtalText
            for (int i = 0; i < GameObjectManager.AllText.Count; i++)
            {
                UtalText utext = GameObjectManager.AllText[i];
                utext.DrawString(graphics);
            }
        }
    }
}


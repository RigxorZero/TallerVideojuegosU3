﻿
using CanvasDrawing.Game;
using CanvasDrawing.UtalEngine2D_2023_1.Physics;
using System;
using System.Drawing;
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
        //Manejo spawn enemigos
        private static Random random = new Random();
        private static float timeSinceLastMovNPC = 0f;
        private static float timeSinceLastPerseguidorNPC = 0f;
        private static float timeSinceLastEstaticoNPC = 0f;

        private const float MovNPCGenerationInterval = 10f;
        private const float PerseguidorNPCGenerationInterval = 15f;
        private const float EstaticoNPCGenerationInterval = 20f;
        //Clase HUD
        public static HealthBar healthBar;
        //Creacion de personaje y enemigos
        public static Image NPC { get; private set; }
        public static Image jugador { get; private set; }
        public static Player player { get; private set; }
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
            engineDrawForm.Height = MainCamera.ySize * 2;
            engineDrawForm.Width = MainCamera.xSize * 2;
            //Asigna imagenes a jugador y NPC
            jugador = Properties.Resources._1_south1;
            NPC = Properties.Resources._3_south1;
            //Gestión de spawn de player
            Random random = new Random();
            player = Player.GetInstance(2, jugador, new Vector2(40, 48), 100, 100);
            player.currentLifes = 3;
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

                // Aumenta el tiempo de generación de cada tipo de NPC
                timeSinceLastMovNPC += Time.deltaTime;
                timeSinceLastPerseguidorNPC += Time.deltaTime;
                timeSinceLastEstaticoNPC += Time.deltaTime;

                //Verifica si perdio el jugador
                if (playerLost)
                {
                    gameOverScreen.Show();
                    break; // Salir del bucle de juego
                }
                //Actualiza entradas del jugador
                InputManager.Update();
                // Genera NPC de tipo EnemigoMov cada 10 segundos
                if (timeSinceLastMovNPC >= MovNPCGenerationInterval)
                {
                    GenerateEnemigoMov();
                    timeSinceLastMovNPC = 0f;
                }

                // Genera NPC de tipo EnemigoPerseguidor cada 15 segundos
                if (timeSinceLastPerseguidorNPC >= PerseguidorNPCGenerationInterval)
                {
                    GenerateEnemigoPerseguidor();
                    timeSinceLastPerseguidorNPC = 0f;
                }

                // Genera NPC de tipo EnemigoEstatico cada 20 segundos
                if (timeSinceLastEstaticoNPC >= EstaticoNPCGenerationInterval)
                {
                    GenerateEnemigoEstatico();
                    timeSinceLastEstaticoNPC = 0f;
                }

                //Si no se ha perdido actualiza deadUpdate
                if (!gameInicio.IsActive && !playerLost)
                {
                    GameObjectManager.DeadUpdate();
                }
            }
        }
        //Enfoque de camara 
        public static Vector2 WorldToCameraPos(Vector2 pos)
        {
            float minX = MainCamera.Position.x + MainCamera.xSize * 0.5f;
            float maxX = MainCamera.Position.x + (1920 - 2 * 50) * MainCamera.scale - MainCamera.xSize * 0.5f;
            float minY = MainCamera.Position.y + MainCamera.ySize * 0.5f;
            float maxY = MainCamera.Position.y + (1080 - 2 * 50) * MainCamera.scale - MainCamera.ySize * 0.5f;

            float clampedX = Math.Max(minX, Math.Min(pos.x, maxX));
            float clampedY = Math.Max(minY, Math.Min(pos.y, maxY));

            return new Vector2((clampedX - MainCamera.Position.x) / MainCamera.scale + MainCamera.xSize * 0.5f,
                               (clampedY - MainCamera.Position.y) / MainCamera.scale + MainCamera.ySize * 0.5f);
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
                //En caso de ser una bala, sino dibuja normal
                if (go is Bullet)
                {
                    Bullet bullet = (Bullet)go;
                    bullet.DrawBullet(graphics, MainCamera);
                }
                else
                {
                    go.Draw(graphics, MainCamera);
                }
            }
            //Dibuja los textos de UtalText
            for (int i = 0; i < GameObjectManager.AllText.Count; i++)
            {
                UtalText utext = GameObjectManager.AllText[i];
                utext.DrawString(graphics);
            }
            //Dibuja el HUD
            healthBar.Draw(graphics);
        }
        // Método para generar un npc
        private static void GenerateEnemigoMov()
        {
            Vector2 npcPosition = GetRandomPositionWithoutCollision();
            Console.WriteLine("Creado EnemigoMov en" + npcPosition.ToString());
            // Crea el EnemigoMov en la posición generada
            new EnemigoMov(2, NPC, new Vector2(40, 48), player, npcPosition.x, npcPosition.y);
        }

        private static void GenerateEnemigoPerseguidor()
        {
            Vector2 npcPosition = GetRandomPositionWithoutCollision();
            Console.WriteLine("Creado EnemigoPerseguidor en" + npcPosition.ToString());
            // Crea el EnemigoPerseguidor en la posición generada
            new EnemigoPerseguidor(2, NPC, new Vector2(40, 48), player, npcPosition.x, npcPosition.y);
        }

        private static void GenerateEnemigoEstatico()
        {
            Vector2 npcPosition = GetRandomPositionWithoutCollision();
            Console.WriteLine("Creado EnemigoEstatico en" + npcPosition.ToString());
            // Crea el EnemigoEstatico en la posición generada
            new EnemigoEstatico(2, NPC, new Vector2(40, 48), player, npcPosition.x, npcPosition.y);
        }

        // Crea una posición aleatoria que no colisione con nada
        private static Vector2 GetRandomPositionWithoutCollision()
        {
            Vector2 position;

            do
            {
                // Genera una posición aleatoria dentro de los límites del mundo
                int x = random.Next(0, 1900 * 2);
                int y = random.Next(0, 1060 * 2);
                position = new Vector2(x, y);
            }
            while (HasCollisionWithSolidSurface(position));

            return position;
        }
        //Verifica que no haya colisión en la posición creada
        private static bool HasCollisionWithSolidSurface(Vector2 position)
        {
            CollisionDetector collisionDetector = new CollisionDetector();

            foreach (GameObject go in GameObjectManager.AllGameObjects)
            {
                if (go is Wall)
                {
                    foreach (Collider collider in go.rigidbody.colliders)
                    {
                        if (collisionDetector.DetectCollisionWithPoint(collider, position))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}


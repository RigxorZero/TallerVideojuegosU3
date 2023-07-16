using CanvasDrawing.UtalEngine2D_2023_1;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CanvasDrawing.Game
{
    public class Player2 : Frame
    {
        public Image sprite;
        public int lifes = 3;
        public int currentLifes;
        public static int score;
        private static Player2 instance; // Instancia única de Player
        public Vector2 Direction { get; set; }

        public Player2(float speed, Image newSprite, Vector2 newSize, float x = 0, float y = 0) : base(speed, newSprite, newSize, x, y)
        {

        }
        //Posicion del jugador
        public Vector2 GetPosition()
        {
            return transform.position;
        }

        public override void OnCollisionEnter(GameObject other)
        {
        }
        //Consigue instancia player
        public static Player2 GetInstance(float speed, Image newSprite, Vector2 newSize, float x = 0, float y = 0)
        {
            if (instance == null)
            {
                instance = new Player2(speed, newSprite, newSize, x, y);
            }
            return instance;
        }
        //Actualiza al player
        public override void Update()
        {
            Vector2 auxLastPos = transform.position;
            bool moved = false;
            float moveSpeed = Speed * 100; // Velocidad de movimiento del jugador

            if (InputManager.GetKey(Keys.Up))
            {
                if (transform.position.y - (moveSpeed * Time.deltaTime) > 0)
                {
                    transform.position.y -= moveSpeed * Time.deltaTime;
                    moved = true;
                }
            }
            else if (InputManager.GetKey(Keys.Down))
            {
                if (transform.position.y + (moveSpeed * Time.deltaTime) < 1080 * 2)
                {
                    transform.position.y += moveSpeed * Time.deltaTime;
                    moved = true;
                }
            }
            if (moved)
            {
                lastPos = auxLastPos;
            }
        }
        //Dibuja al jugador
        public override void Draw(Graphics graphics, Camera camera)
        {
            base.Draw(graphics, camera);
        }
    }
}


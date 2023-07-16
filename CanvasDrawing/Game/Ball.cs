using CanvasDrawing.UtalEngine2D_2023_1;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasDrawing.Game
{
    public class Ball : Frame
    {
        private Vector2 velocity;  // Velocidad de la esfera
        private Vector2 newSize;   // Tamaño de la esfera

        public Ball(float Speed, Image newsprite, Vector2 newSize, float x = 0, float y = 0) : base(Speed, newsprite, newSize, x, y)
        {
            this.newSize = newSize;
            velocity = new Vector2(Speed, Speed);  // Inicializa la velocidad de la esfera
        }

        public override void Update()
        {
            // Mueve la esfera según su velocidad
            transform.position += velocity * 50 * Time.deltaTime;
            Console.WriteLine(velocity);
        }

        public override void OnCollisionEnter(GameObject other)
        {
            if (other is Wall)
            {
                Wall wall = (Wall)other;
                float wallTop = wall.transform.position.y;
                float wallBottom = wall.transform.position.y + wall.spriteRenderer.Size.y;

                if (velocity.y > 0 && transform.position.y <= wallTop) // Colisión en la parte superior del muro
                {
                    velocity.y = -velocity.y; // Invierte la velocidad en el eje Y
                }
                else if (velocity.y < 0 && transform.position.y <= wallBottom) // Colisión en la parte inferior del muro
                {
                    velocity.y = -velocity.y; // Invierte la velocidad en el eje Y
                }
            }

            else if (other is Ball)
            {
                // Colisión con otro objeto Ball
                // Implementa aquí la lógica de colisión con otra Ball
                // ...
            }
            else if (other is Player2)
            {
                Player2 player = (Player2)other;
                float playerCenter = player.transform.position.y + player.spriteRenderer.Size.y / 2;

                // Invertir la dirección horizontal de la pelota
                velocity.x = -velocity.x;

                // Ajustar la dirección vertical de la pelota en función de la posición de colisión en el jugador
                if (transform.position.y < playerCenter)
                {
                    velocity.y = -Math.Abs(velocity.y); // Rebote hacia abajo
                }
                else
                {
                    velocity.y = Math.Abs(velocity.y); // Rebote hacia arriba
                }

                // Aumentar la velocidad de la pelota (opcional)
                velocity *= 1.1f; // Multiplicar la velocidad actual por un factor de incremento
            }
            else if (other is Player)
            {
                Player player = (Player)other;
                float playerCenter = player.transform.position.y + player.spriteRenderer.Size.y / 2;

                // Invertir la dirección horizontal de la pelota
                velocity.x = -velocity.x;

                // Ajustar la dirección vertical de la pelota en función de la posición de colisión en el jugador
                if (transform.position.y < playerCenter)
                {
                    velocity.y = -Math.Abs(velocity.y); // Rebote hacia abajo
                }
                else
                {
                    velocity.y = Math.Abs(velocity.y); // Rebote hacia arriba
                }

                // Aumentar la velocidad de la pelota (opcional)
                velocity *= 1.1f; // Multiplicar la velocidad actual por un factor de incremento
            }
        }

    }

}



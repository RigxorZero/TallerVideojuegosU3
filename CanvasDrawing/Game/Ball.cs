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
                // Colisión con un objeto Wall
                // Implementa aquí la lógica de colisión con un Wall

                Wall wall = (Wall)other;
                float wallTop = wall.transform.position.y;
                float wallBottom = wall.transform.position.y + wall.spriteRenderer.Size.y;

                if (velocity.y > 0 && transform.position.y + newSize.y >= wallTop && transform.position.y <= wallBottom) // Colisión por la parte inferior
                {
                    transform.position = new Vector2(transform.position.x, wallTop - newSize.y); // Ajusta la posición de la esfera
                    velocity.y = -velocity.y; // Invierte la velocidad en el eje Y
                }
                else if (velocity.y < 0 && transform.position.y <= wallBottom && transform.position.y + newSize.y >= wallTop) // Colisión por la parte superior
                {
                    transform.position = new Vector2(transform.position.x, wallBottom); // Ajusta la posición de la esfera
                    velocity.y = -velocity.y; // Invierte la velocidad en el eje Y
                }
            }



            else if (other is Ball)
            {
                // Colisión con otro objeto Ball
                // Implementa aquí la lógica de colisión con otra Ball
                // ...
            }
            else if (other is Player)
            {
                // Colisión con un objeto Player
                // Implementa aquí la lógica de colisión con un Player
                // ...
            }
        }

    }

}



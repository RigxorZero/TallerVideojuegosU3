using CanvasDrawing.UtalEngine2D_2023_1;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CanvasDrawing.Game
{
    public class Ball : Frame
    {
        private Vector2 velocity;  // Velocidad de la esfera
        private Vector2 newSize;   // Tamaño de la esfera
        public static Image ballSprite { get; private set; }
        private bool shouldCreateNewBall = false;
        private int additionalBallsToCreate = 2;
        private float additionalBallDelay = 1f; // Tiempo en segundos entre la creación de cada pelota adicional
        private float elapsedTime = 0f; // Tiempo acumulado desde la destrucción de la pelota original




        public Ball(float Speed, float y1, Image newsprite, Vector2 newSize, float x = 0, float y = 0) : base(Speed, newsprite, newSize, x, y)
        {
            this.newSize = newSize;
            velocity = new Vector2(Speed, Speed);  // Inicializa la velocidad de la esfera

        }

        public Ball(Vector2 initialVelocity, Image newsprite, Vector2 newSize, float x = 0, float y = 0) : base(0, newsprite, newSize, x, y)
        {
            this.newSize = newSize;
            velocity = initialVelocity; // Inicializa la velocidad de la esfera con el vector proporcionado

            // Calcula la magnitud del vector de velocidad
            float magnitude = (float)Math.Sqrt(velocity.x * velocity.x + velocity.y * velocity.y);

            // Normaliza el vector de velocidad
            velocity.x /= magnitude;
            velocity.y /= magnitude;
        }


        public override void Update()
        {
            // Mueve la esfera según su velocidad
            transform.position += velocity * 50 * Time.deltaTime;

            // Obtén las dimensiones de la cámara
            int cameraWidth = GameEngine.MainCamera.xSize;

            // Incrementa el tiempo acumulado con el deltaTime
            elapsedTime += Time.deltaTime;

            // Comprueba si la pelota ha salido del límite de la pantalla por la izquierda
            if (transform.position.x < 0)
            {
                // Imprime un mensaje indicando que la pelota ha salido por el límite izquierdo
                Console.WriteLine("La pelota ha salido por el límite izquierdo.");

                // Establece shouldCreateNewBall como true para indicar que se debe crear una nueva pelota en el próximo ciclo de actualización
                shouldCreateNewBall = true;
            }
            // Comprueba si la pelota ha salido del límite de la pantalla por la derecha
            else if (transform.position.x > cameraWidth)
            {
                // Imprime un mensaje indicando que la pelota ha salido por el límite derecho
                Console.WriteLine("La pelota ha salido por el límite derecho.");

                // Establece shouldCreateNewBall como true para indicar que se debe crear una nueva pelota en el próximo ciclo de actualización
                shouldCreateNewBall = true;
            }

            // Comprueba si se debe crear una nueva pelota
            if (shouldCreateNewBall)
            {
                // Comprueba si ha pasado el tiempo suficiente para crear una pelota adicional
                if (elapsedTime >= additionalBallDelay)
                {
                    // Llama al método LaunchBall para crear una nueva pelota en el centro de la pantalla
                    LaunchBall();

                    // Reinicia el tiempo acumulado
                    elapsedTime = 0f;

                    // Reduce el número total de pelotas adicionales restantes
                    additionalBallsToCreate--;

                    // Comprueba si se han creado todas las pelotas adicionales
                    if (additionalBallsToCreate <= 0)
                    {
                        // Reinicia la variable shouldCreateNewBall para indicar que no se deben crear más pelotas adicionales
                        shouldCreateNewBall = false;
                        GameEngine.Destroy(this);
                    }
                }
            }

            Console.WriteLine(additionalBallsToCreate);
            Console.WriteLine(elapsedTime);
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
                // Aumentar la velocidad de la pelota
                velocity *= 1.1f; // Multiplicar la velocidad actual por un factor de incremento
            }

            else if (other is Ball)
            {
                Ball otherBall = (Ball)other;

                // Calcula el vector de dirección entre las dos bolas
                Vector2 collisionDirection = otherBall.transform.position - transform.position;

                // Normaliza el vector de dirección
                collisionDirection.Normalize();

                // Calcula las velocidades proyectadas en la dirección de la colisión
                float thisVelocityProjection = velocity.x * collisionDirection.x + velocity.y * collisionDirection.y;
                float otherVelocityProjection = otherBall.velocity.x * collisionDirection.x + otherBall.velocity.y * collisionDirection.y;

                // Calcula las masas inversas de las bolas
                float thisInverseMass = 1f / this.rigidbody.mass;
                float otherInverseMass = 1f / otherBall.rigidbody.mass;

                // Calcula las nuevas velocidades después de la colisión (aplicando la fórmula de colisión elástica teniendo en cuenta las masas)
                Vector2 thisNewVelocity = (collisionDirection * (otherInverseMass * otherVelocityProjection) * 2f) + (collisionDirection * (thisVelocityProjection * (thisInverseMass - otherInverseMass)));
                Vector2 otherNewVelocity = (collisionDirection * (otherVelocityProjection * (otherInverseMass - thisInverseMass))) + (collisionDirection * (2f * thisInverseMass * thisVelocityProjection));

                // Actualiza las velocidades de las bolas
                velocity = thisNewVelocity;
                otherBall.velocity = otherNewVelocity;
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

                // Aumentar la velocidad de la pelota
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

                // Aumentar la velocidad de la pelota
                velocity *= 1.1f; // Multiplicar la velocidad actual por un factor de incremento
            }
        }

        // Función para generar una dirección inicial aleatoria evitando ángulos específicos
        public Vector2 GenerateRandomDirection()
        {
            // Crea una instancia de la clase Random
            Random random = new Random();

            // Genera un ángulo aleatorio entre 0 y 360 grados
            float angle = (float)(random.NextDouble() * 360f);

            // Verifica si el ángulo generado está dentro de los rangos a evitar
            while ((angle >= 80f && angle <= 100f) || (angle >= 260f && angle <= 280f))
            {
                // Genera un nuevo ángulo aleatorio
                angle = (float)(random.NextDouble() * 360f);
            }

            // Convierte el ángulo a radianes
            float angleRadians = MathHelper.ToRadians(angle);

            // Calcula las componentes x e y de la dirección inicial
            float directionX = (float)Math.Cos(angleRadians);
            float directionY = (float)Math.Sin(angleRadians);

            // Retorna el vector de dirección inicial
            return new Vector2(directionX, directionY);
        }

        // Función para lanzar la pelota desde el centro de la pantalla en una dirección aleatoria evitando ángulos específicos
        public void LaunchBall()
        {
            // Crea una instancia de Ball en el centro de la pantalla
            ballSprite = Properties.Resources.BouncingBall;
            // Obtén las dimensiones de la cámara
            int cameraWidth = GameEngine.MainCamera.xSize;
            int cameraHeight = GameEngine.MainCamera.ySize;

            // Calcula la posición inicial en el centro de la pantalla
            float ballX = cameraWidth / 2f;
            float ballY = cameraHeight / 2f;

            // Genera una dirección inicial aleatoria evitando los ángulos específicos
            Vector2 initialVelocity = GenerateRandomDirection();

            // Crea una nueva instancia de la pelota en la posición inicial con la dirección inicial generada
            new Ball(initialVelocity, ballSprite, new Vector2(16, 16), ballX, ballY);
        }


    }
}



using CanvasDrawing.UtalEngine2D_2023_1;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace CanvasDrawing.Game
{
    public class GameInicio //Pantalla de inicio
    {
        private readonly Form form;
        private readonly SynchronizationContext synchronizationContext;
        private Button exitButton, startButton;
        private bool isInitialized;
        public static Camera MainCamera = new Camera();
        public bool IsActive{private set; get;}
        public static Image ballSprite { get; private set; }

        public GameInicio(Form engineDrawForm)
        {
            form = engineDrawForm;
            synchronizationContext = SynchronizationContext.Current;
            engineDrawForm.Height = MainCamera.ySize;
            engineDrawForm.Width = MainCamera.xSize;
        }
        public void InitializeGameInicio()
        {
            if (isInitialized)
                return;

            // Desactiva temporalmente AutoSizeMode del formulario
            form.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            // Agrega el botón de exit
            exitButton = new Button
            {
                Text = "EXIT",
                Size = new Size(100, 30),
                BackColor = SystemColors.Control
        };
            exitButton.Click += (buttonSender, buttonArgs) => ExitGame();
            form.Controls.Add(exitButton);

            // Agrega el botón de start
            startButton = new Button
            {
                Text = "START",
                Size = new Size(100, 30),
                BackColor = SystemColors.Control
        };
            startButton.Click += (buttonSender, buttonArgs) => StartGame();
            form.Controls.Add(startButton);

            // Restablece AutoSizeMode del formulario
            //form.SizeChanged += StaticSize;
            form.AutoSize = false;

            isInitialized = true;
            
            form.Paint -= GameEngine.Paint;
            form.Paint += Form_Paint;
            form.Refresh();
            //form.Paint -= Form_Paint;
        }
        public void Show()
        {
            synchronizationContext.Send(state => DrawGameInicio(), null);
        }
        private void DrawGameInicio()
        {
            InitializeGameInicio();
            // Invalida el formulario para disparar el evento Paint
        }
        private void Form_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;

            // Dibuja la imagen "Inicio"
           // Image InicioImage  = Properties.Resources._1298880;
            Rectangle destinationRectangle = new Rectangle(0, 0, form.Width, form.Height);
            //graphics.DrawImage(InicioImage, destinationRectangle);

            // Dibuja nombre del juego
            string gamenombre = "Ghost escape";
            Font font = new Font("Arial", 20, FontStyle.Bold);
            SizeF textSize = graphics.MeasureString(gamenombre, font);
            PointF textPosition = new PointF(form.Width / 2 - textSize.Width / 2, form.Height / 2 - textSize.Height / 2);

            // Dibuja nombres de creadores
            string gamecreadores = "Antonia Donoso y Hector Villalobos";
            Font font2 = new Font("Arial", 8, FontStyle.Bold);
            SizeF textSize2 = graphics.MeasureString(gamecreadores, font2);
            PointF textPosition2 = new PointF(form.Width / 6 - textSize2.Width / 2, form.Height / 18 - textSize2.Height / 2);

            // Dibuja el fondo del texto
            RectangleF textBackgroundRect = new RectangleF(textPosition.X, textPosition.Y, textSize.Width, textSize.Height);
            graphics.FillRectangle(Brushes.MediumPurple, textBackgroundRect);

            // Dibuja el texto
            graphics.DrawString(gamenombre, font, Brushes.Black, textPosition);
            graphics.DrawString(gamecreadores, font2, Brushes.Black, textPosition2);

            // Posiciona el botón de exit
            exitButton.Location = new Point((form.Width - exitButton.Width) / 2, (int)(textBackgroundRect.Bottom + 100));

            // Posiciona el botón de start
            startButton.Location = new Point((form.Width - exitButton.Width) / 2, (int)(textBackgroundRect.Bottom + 20));

            startButton.Update();
            exitButton.Update();
        }

        private void ExitGame()
        {
            Application.Exit();
        }
        private void StartGame()
        {
            // Crea una instancia del formulario del GameInitializer
            // Inicializa el juego
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

            // Crea una instancia de Ball en el centro de la pantalla
            ballSprite = Properties.Resources.BouncingBall;
            // Obtén las dimensiones de la cámara
            int cameraWidth = GameEngine.MainCamera.xSize;
            int cameraHeight = GameEngine.MainCamera.ySize;

            // Calcula la posición inicial en el centro de la pantalla
            float ballX = cameraWidth / 2f;
            float ballY = cameraHeight / 2f;
            Vector2 initialVelocity = new Vector2(directionX, directionY);
            new Ball(initialVelocity, Properties.Resources.BouncingBall, new Vector2(16, 16), ballX, ballY);


            form.Paint -= Form_Paint;
            IsActive = false;
            form.Controls.Remove(startButton);
            form.Controls.Remove(exitButton);
            form.Paint += GameEngine.Paint;
            //GameInitializer.InitializeGame(form);

            // Muestra el formulario del GameInitializer
        }
    }
}

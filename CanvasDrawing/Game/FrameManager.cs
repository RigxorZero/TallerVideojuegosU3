using CanvasDrawing.UtalEngine2D_2023_1;
using System.Collections.Generic;

namespace CanvasDrawing.Game
{
    public class FrameManager: EmptyUpdatable
    {
        public static List<Frame> AllFrames = new List<Frame>();
        public static int selectedIndex;
        public static FrameManager Instance;
        public override void Update()
        {
            // Mantener la cámara estática en su posición y tamaño
            GameEngine.MainCamera.Position = new Vector2(0, 0);  // Ajusta la posición de la cámara según tus necesidades
            GameEngine.MainCamera.scale = 1;  // Ajusta la escala de la cámara según tus necesidades
        }
    }
}

﻿
using System.Drawing;

namespace CanvasDrawing.UtalEngine2D_2023_1.Physics
{ 
    public abstract class Collider
    {
        public Rigidbody rigidbody;
        public bool isSolid = true;

        public Collider(Rigidbody rigidbody)
        {
            this.rigidbody = rigidbody;
        }
        //public abstract bool CheckCollision(Collider other);

        public abstract void DrawCollider(Graphics graphics, Camera camera);
    }
}


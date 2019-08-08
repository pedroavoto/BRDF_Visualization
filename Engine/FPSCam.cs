using Engine;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Point = System.Drawing.Point;
using Timer = Engine.Timer;
using SharpDX.DirectInput;

namespace Engine
{
    public class FPSCam : ICameraInterface
    {

       private float leftrightRot;
       private float updownRot;


		#region ICameraInterface Members

		public Vector3 Position	{ get; private set; }

		public Vector3 Forward	{ get; private set;	}

		public Vector3 UpVector	{ get; private set;	}

		public Matrix ViewMatrix { get; private set; }

		public Matrix ProjectionMatrix { get; private set; }

		#endregion

        public FPSCam(int windowWidth, int windowHeight)
        {
			//Dados para a proj
			float viewAngle = MathUtil.PiOverFour;
			float aspectRatio = (float)windowWidth / (float)windowHeight;
			float nearPlane = 0.01f;
			float farPlane = 1000.0f;
			this.ProjectionMatrix = Matrix.PerspectiveFovLH(viewAngle, aspectRatio, nearPlane, farPlane);

			//Dados para a View
			this.leftrightRot = 0.0f;
			this.updownRot = 0.0f;
			this.Position = new Vector3(0, 0, 10);
			this.UpdateViewMatrix();
		}


        public void Update()
        {
            if(InputHandler.Get().LockedMouse)
            {
                UpdateMouse();
                UpdateKeyboard();
            }
           
		}

        private void UpdateMouse()
        {
            float rotationSpeed = 0.001f;
			MouseState currentMouseState = InputHandler.Get().MouseState;

			float xDifference = currentMouseState.X;
			float yDifference = currentMouseState.Y;
			this.leftrightRot += rotationSpeed * xDifference;
			this.updownRot -= rotationSpeed * yDifference;
			this.UpdateViewMatrix();
		}

        private void UpdateKeyboard()
        {
			KeyboardState keyState = InputHandler.Get().KeyboardState;
			if (keyState.IsPressed(Key.Up))
                this.AddToCameraPosition(new Vector3(0, 0, -1));
            if (keyState.IsPressed(Key.Down))
                this.AddToCameraPosition(new Vector3(0, 0, 1));
            if (keyState.IsPressed(Key.Right))
                this.AddToCameraPosition(new Vector3(-1, 0, 0));
            if (keyState.IsPressed(Key.Left))
                this.AddToCameraPosition(new Vector3(1, 0, 0));
        }

        private void UpdateViewMatrix()
        {

			Matrix3x3 cameraRotation = Matrix3x3.RotationX(updownRot) * Matrix3x3.RotationY(leftrightRot);
            Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
            Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);

            Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
            this.Forward = this.Position + cameraRotatedTarget; //tem que somar pois eu quero que ele olhe pra frente na posição atual
            

            this.UpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotation);

            this.ViewMatrix = Matrix.LookAtLH(this.Position, this.Forward, this.UpVector);
        }

        private void AddToCameraPosition(Vector3 vectorToAdd)
        {
            float moveSpeed = 0.02f;
            Matrix3x3 cameraRotation = Matrix3x3.RotationX(this.updownRot) * Matrix3x3.RotationY(this.leftrightRot);
            Vector3 rotatedVector = Vector3.Transform(vectorToAdd, cameraRotation);
            this.Position += moveSpeed * rotatedVector;
            this.UpdateViewMatrix();
       }


    }


}

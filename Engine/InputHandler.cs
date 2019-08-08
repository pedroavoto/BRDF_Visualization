using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Engine
{
	public class InputHandler
	{
		private static InputHandler instance = null;

		public static InputHandler Get()
		{
			if (InputHandler.instance == null)
			{
				InputHandler.instance = new InputHandler();
			}
			return InputHandler.instance;
		}

		private DirectInput directInput;
		private Mouse mouse;
		private Keyboard keyboard;
        private bool[] previousFramePressedKeysMap;

		public KeyboardState KeyboardState { get; private set; }
		public MouseState MouseState { get; private set; }

        public bool LockedMouse { get; private set; }

		private MouseState test = new MouseState();
		private InputHandler()
		{
			this.directInput = new DirectInput();
			this.mouse = new Mouse(this.directInput);
			this.keyboard = new Keyboard(this.directInput);
            Cursor.Hide();
            this.SetLocation(30, 30);
			this.mouse.Acquire();
			this.keyboard.Acquire();
			this.MouseState = this.mouse.GetCurrentState();
			this.KeyboardState = this.keyboard.GetCurrentState();
            this.previousFramePressedKeysMap = new bool[237];
            this.LockedMouse = true;
		}

		public void Update()
		{
			this.MouseState = this.mouse.GetCurrentState();
			this.KeyboardState = this.keyboard.GetCurrentState();

            if (this.LockedMouse && this.WasToggled(Key.Escape))
            {
                Cursor.Show();
                this.LockedMouse = false;
            }
            else if (!this.LockedMouse && this.WasToggled(Key.Escape))
            {
                Cursor.Hide();
                this.LockedMouse = true;
            }

            if (this.LockedMouse)
            {
                this.SetLocation(30, 30);
            }

            this.UpdatePreviousPressedMap();

		}

        public void UpdatePreviousPressedMap()
        {
            for (int i = 0; i < previousFramePressedKeysMap.Length; i++)
            {
                previousFramePressedKeysMap[i] = false;
            }

            foreach (Key pressedKey in this.KeyboardState.PressedKeys)
            {
                this.previousFramePressedKeysMap[(int)pressedKey] = true;
            }
        }

        public bool WasToggled(Key key)
        {
            if(this.KeyboardState.IsPressed(key) && !this.previousFramePressedKeysMap[(int)key])
            {
                return true;
            }
            else return false;            
        }

		 [DllImport("user32.dll")] 
         static extern bool SetCursorPos(int X, int Y); 

         private void SetLocation(int x, int y)
         { 
             SetCursorPos(x, y); 
         }


}
}

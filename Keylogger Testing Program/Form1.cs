using KeyBoardListener.ListenManager;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KeyBoardListener
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        public TextBox LastCreatedTextBox = null;

        private void Form1_Load(object sender, EventArgs e)
        {
            TextBox OneText = new TextBox(); //Declare a textbox
            OneText.Name = "KeyLog";//set name
            OneText.BackColor = Color.Black;//set background colour
            OneText.ForeColor = Color.White;//set font color
            OneText.Multiline = true; //multiline
            OneText.Width = this.Width - 10;
            OneText.Height = this.Height - 10;
            OneText.Left = 10 / 2;//left margin = left top right bottom
            OneText.Top = OneText.Left;
            LastCreatedTextBox = OneText;

            this.Controls.Add(OneText);
            KeyBoardHook.InstallHook(OneKeyProcess);//Install hook when press key

            GC.Collect();//Empty working set 

        }

        
        //Hook structure
        private void OneKeyProcess(HookStruct OneStruct, out bool Handle)
        {
            Handle = false;
            Keys OneKey = (Keys)OneStruct.vkCode;//Keys is the enum of the Csharp Windows.Form, is the packet of Ascii

            LastCreatedTextBox.Invoke(new Action(() => // new Action(()=>{}) Lambda expressions. Delegate is the packet of Action & Func
            {
                LastCreatedTextBox.Text += OneKey.ToString() + "\r\n" + DateTime.Now.ToString();//Enum.ToString Method (textbox)
                //+= = LastCreatedTextBox.Text=LastCreatedTextBox.Text+

                LastCreatedTextBox.SelectionStart = LastCreatedTextBox.Text.Length;//set cursor starting position

                LastCreatedTextBox.ScrollToCaret();//scroll to the cursor position

                //for the textbox easily control by scroll function
            }));


        }
    }
}

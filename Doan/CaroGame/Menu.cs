using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaroGame
{
    public partial class Menu : Form
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void btn_return_Click(object sender, EventArgs e)
        {
            this.Hide();
            var main = Application.OpenForms.OfType<Form1>().FirstOrDefault();
            if (main != null)
            {
                main.Show();
            }
            else
            {
                var newMain = new Form1();
                newMain.Show();
            }
        }

        private void btn_newgame_Click(object sender, EventArgs e)
        {
            var Openform = Application.OpenForms.OfType<Form1>().ToList();
                foreach (var f in Openform)
                {
                    f.Close();
                }
                this.Close();
            var form = new Form1();
            form.Show();
        }

        private void btn_pause_Click(object sender, EventArgs e)
        {
            
        }
    }
}

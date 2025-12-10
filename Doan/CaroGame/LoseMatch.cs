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
    public partial class LoseMatch : Form
    {
        public event Action loseRematch;
        public event Action loseExit;
        public LoseMatch()
        {
            InitializeComponent();
        }

        private void btnRematch_Click(object sender, EventArgs e)
        {
            loseRematch?.Invoke();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            loseExit?.Invoke();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace CaroGame
{
    public partial class Manager : Component
    {
        public Manager()
        {
            InitializeComponent();
        }

        public bool Undo()
        {
            return false;
        }
        
    }
}

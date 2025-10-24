namespace CaroGame
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }



        private void btnMenu_Click(object sender, EventArgs e)
        {
            var menuForm = new Menu();
            menuForm.Show();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            ActiveForm.Close();
            var DashBoard = new Dashboard();
            DashBoard.Show();
        }
    }
}

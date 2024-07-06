using System;
using System.Windows.Forms;

namespace ElectronicsInventory
{
    public partial class ElectronicsInventory : Form
    {
        public ElectronicsInventory()
        {
            InitializeComponent();
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void phonesButton_Click(object sender, EventArgs e)
        {
            Form straightTalkPhones = new PhoneForm();
            straightTalkPhones.ShowDialog();
        }
    }
}

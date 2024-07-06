using System;
using System.Windows.Forms;

namespace ElectronicsInventory
{
    public partial class WarningForm : Form
    {
        #region Fields and Constructors

        private bool _proceed;

        public bool Proceed
        {
            get
            {
                return _proceed;
            }
            set
            {
                _proceed = value;
            }
        }

        #endregion

        #region Methods

        #region Initializing Form

        public WarningForm(string message)
        {
            InitializeComponent();

            // Configure warningForm
            warningLabel.Text = message;
            MaximizeBox = false;
            MinimizeBox = false;
            ControlBox = false;
        }

        #endregion

        #region Click EventHandlers

        // Does not execute remaining code on parent form
        private void noButton_Click(object sender, EventArgs e)
        {
            _proceed = false;
            Close();
        }

        // Executes remaining code on parent form
        private void yesButton_Click(object sender, EventArgs e)
        {
            _proceed = true;
            Close();
        }

        #endregion

        #endregion
    }
}
using System;
using System.Windows.Forms;

namespace ElectronicsInventory
{
    public partial class UpdateForm : Form
    {
        // Fields
        private string _component;
        private string _newValue;
        private Phone _phone;

        // Constructors
        public string Component
        {
            get
            {
                return _component;
            }
            set
            {
                _component = value;
            }
        }

        public string NewValue
        {
            get
            {
                return _newValue;
            }
            set
            {
                _newValue = value;
            }
        }

        public Phone Phone
        {
            get
            {
                return _phone;
            }
            set
            {
                _phone = value;
            }
        }
        
        // Methods
        public UpdateForm(string component, Phone phone)
        {
            InitializeComponent();

            // Set fields to parameter values
            Component = component;
            Phone = phone;

            // Form setup
            Text = "Update " + component;
            oldPromptLabel.Text = "Old " + component;
            newPromptLabel.Text = "New " + component;
            MaximizeBox = false;
            MinimizeBox = false;
            
            // Determine which property is being changed
            if (Component == "IMEI")
            {
                oldLabel.Text = phone.IMEI.ToString();
            }
            else if (Component == "Price")
            {
                oldLabel.Text = phone.Price.ToString("c");
            }

        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            if (newTextBox.Text != "")
            {
                // Updating IMEI
                if (Component == "IMEI")
                {
                    if (long.TryParse(newTextBox.Text, out long result))
                    {
                        if (newTextBox.Text.Length == Phone.IMEI_LENGTH)
                        {
                            // Updates value and closes the form
                            NewValue = newTextBox.Text;
                            Close();
                        }
                        else
                        {
                            MessageBox.Show("IMEI must be " + Phone.IMEI_LENGTH + " digits.", "Error");
                        }
                    }
                    else
                    {
                        MessageBox.Show("IMEI must be a number.", "Error");
                    }
                }

                // Updating price
                else if (Component == "Price")
                {
                    if (decimal.TryParse(newTextBox.Text, out decimal result))
                    {
                        // Updates value and closes the form
                        NewValue = newTextBox.Text;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Price must be a number.", "Error");
                    }
                }
            }
            else
            {
                MessageBox.Show("New value cannot be blank.", "Error");
            }
        }
    }
}
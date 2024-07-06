using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ElectronicsInventory
{
    public partial class ListForm : Form
    {
        #region Fields

        // Used to open the form
        readonly string network;
        readonly bool all;

        // Determine which network and get appropriate file
        FileInfo fi;

        // Parallel lists (match model, brand, and UPC)
        // Brands and Networks used to list all respective items
        readonly List<string> phoneNames = new List<string>();
        readonly List<string> phoneBrands = new List<string>();
        readonly List<long> upcs = new List<long>();
        readonly List<string> brands = new List<string>();
        readonly List<string> networks = new List<string>();

        // List for network phones
        readonly List<string> networkPhoneNames = new List<string>();

        // Used to track amount for each phone
        // Combines repeated items in phones list
        readonly List<string> phoneCounts = new List<string>();

        #endregion

        #region Methods

        #region Click EventHandlers

        // Exits the form
        private void exitButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region Initializing Form

        public ListForm(string _network, bool _all)
        {
            InitializeComponent();
            network = _network;
            all = _all;
            MaximizeBox = false;

            // Use LoadData() method from previous form to load same data
            // Rather than duplicating the code
            PhoneForm phoneForm = new PhoneForm();
            phoneForm.LoadData(phoneNames, phoneBrands, upcs, brands, networks);
        }

        private void ListForm_Load(object sender, EventArgs e)
        {
            // Get network inventory file
            fi = new FileInfo(@"files\Network_Inventories.txt");

            // Verify user request (single network or total inventory)
            if (all == false)
            {
                GetPhones(network);
                networkLabel.Text = network;
            }
            else
            {
                // Get phones for all networks
                foreach (string str in networks)
                {
                    GetPhones(str);
                }

                networkLabel.Text = "All Networks";
            }

            ListPhones();

            // Adjust size of form
            string[] lines = inventoryLabel.Text.Split(new char[] { '\n' });
            int totalLines = lines.Length;
            int newHeight = totalLines * 15;

            inventoryLabel.Size = new Size(214, newHeight);

            Size = new Size(254, newHeight + 140);

            exitButton.Location = new Point(12, newHeight + 65);
        }

        #endregion

        #region Listing Phones

        // Lists phones on inventoryLabel
        void ListPhones()
        {
            // Used to list brands for each phone in network inventory
            // Parallel to phones list
            List<string> phoneBrands2 = new List<string>();

            // Used to list brands for each phone in phoneCounts
            // Parallel to phoneCounts
            // Contents of this list will be added to inventoryLabel.Text
            List<string> brandPhoneCounts = new List<string>();

            BuildPhoneCounts(networkPhoneNames);

            // Sort brands alphabetically
            brands.Sort();

            // Build inventoryLabel.Text contents, brand by brand
            foreach (string str1 in brands)
            {
                // Sort phones by brand and build phoneBrands2
                foreach (string str2 in networkPhoneNames)
                {
                    // Used to hold the brand for str2 phone
                    string brand = "";

                    // This loop finds the brand for the phone in inventory
                    for (int i = 0; i < phoneNames.Count; i++)
                    {
                        // If phone name is found in phoneNames
                        if (phoneNames[i] == str2)
                        {
                            // Get phone's brand from phoneBrands
                            brand = phoneBrands[i];
                            break;
                        }
                    }
                    if (brand != "")
                    {
                        // Build phoneBrands2
                        phoneBrands2.Add(brand);
                    }
                }

                // Add title if brand has phones in inventory
                if (phoneBrands2.Count != 0)
                {
                    // Build title line
                    string title = "===" + str1 + "===\n";

                    // Add title line if not already present
                    if (!inventoryLabel.Text.Contains(title))
                    {
                        if (inventoryLabel.Text == "")
                        {
                            // Add first title
                            inventoryLabel.Text = title;
                        }
                        else
                        {
                            // Add empty line and title for next brand on list
                            inventoryLabel.Text += "\n" + title;
                        }
                    }
                }

                // Build brandPhoneCounts
                foreach (string str2 in phoneCounts)
                {
                    // Find phone name that is in str2
                    for (int i = 0; i < phoneNames.Count; i++)
                    {
                        // If phone name matches and brand matches item in brands
                        // Add phoneCounts string to brandPhoneCounts
                        if (str2.Contains(phoneNames[i]) && str1 == phoneBrands[i])
                        {
                            brandPhoneCounts.Add(str2);
                            break;
                        }
                    }
                }

                // Organize all phones in descending order
                // (phones with highest quantity will be shown first)
                brandPhoneCounts.Sort();
                brandPhoneCounts.Reverse();

                // Add brandPhoneCounts items to inventoryLabel.Text
                foreach (string str2 in brandPhoneCounts)
                {
                    if (!inventoryLabel.Text.Contains(str2))
                    {
                        inventoryLabel.Text += str2;
                    }
                }


                // Clear brandPhoneCounts for next brand
                brandPhoneCounts.Clear();
            }

            // Add total
            int total = networkPhoneNames.Count;
            inventoryLabel.Text += "\n\n" + "Total: " + total;

            // Clear text if no phones in inventory
            if (networkPhoneNames.Count == 0)
            {
                inventoryLabel.Text = "No phones in system\n\n";
            }
        }

        // Builds phoneCounts lists (used in ListPhones)
        void BuildPhoneCounts(List<string> phones)
        {
            // Build phoneCounts
            foreach (string str in phones)
            {
                // Find all occurrences of str in list
                int count = phones.FindAll(c => c == str).Count;

                // Concatenate new string to add to phoneCounts
                string newStr = count.ToString() + "x " + str + "\n";

                // Add new string
                phoneCounts.Add(newStr);
            }
        }

        #endregion

        #region Loading Phones

        // Gets phones from network inventory retrieved in GetFile
        void GetPhones(string networkInv)
        {
            // Create variables to read the contents
            FileStream fs = fi.Open(FileMode.Open, FileAccess.ReadWrite);
            StreamReader sr = new StreamReader(fs);
            string line = sr.ReadLine();

            // Reach specified network inventory
            while (line != "|---" + networkInv.ToUpper() + "---|")
            {
                line = sr.ReadLine();
            }

            // Skips through "---Phone Inventory---"
            sr.ReadLine();

            // Add phones until end of inventory is reached
            while (line != "|End of Inventory|")
            {
                line = sr.ReadLine();

                if (line != "|End of Inventory|")
                {
                    // Add phone to list
                    networkPhoneNames.Add(line);

                    // Skips brand, price, IMEI, and divider lines
                    sr.ReadLine(); sr.ReadLine(); sr.ReadLine(); sr.ReadLine();
                }
            }

            // Close StreamReader and FileStream
            sr.Close(); fs.Close();
        }

        #endregion

        #endregion
    }
}
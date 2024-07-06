using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ElectronicsInventory
{
    public partial class PhoneForm : Form
    {
        #region Fields

        // Phones in system
        readonly List<Phone> phoneList = new List<Phone>();

        // Parallel lists (match model, brand, and UPC)
        readonly List<string> phoneNames = new List<string>();
        readonly List<string> phoneBrands = new List<string>();
        readonly List<long> upcs = new List<long>();

        // List of brands
        readonly List<string> brands = new List<string>();

        // List of networks
        readonly List<string> networks = new List<string>();

        // If false, an error has occurred and the form can't load
        bool fatalError = false;

        // Info for inventory.txt file
        FileInfo fi;

        // Tracks network (defaults to first option, Straight Talk)
        // Value is changed with the radio buttons at the top
        string network = "Straight Talk";

        // Notifications textBox
        readonly TextBox notifTextBox = new TextBox();

        // Tracks notifications
        int notifs = 0;

        // Search results list
        readonly List<Phone> results = new List<Phone>();

        // Tracks current phone being shown
        int curItem = 0;

        // Tracks if user is on "Enter" or "Search" tab
        bool search = false;

        #region Backup Contents
        // Backup UPC_Organizer.txt contents
        public const string BACKUP_TEXT =
            "-----Apple-----\r\niPhone 11\r\n217093817731\r\niPhone 12\r\n217093816951\r\niPhone 13" +
            "\r\n217093816931\r\niPhone 14\r\n217093815591\r\niPhone 15\r\n217093818881\r\n" +

            "-----Motorola-----\r\nMoto G 5G\r\n342190002960\r\nMoto G Play\r\n342190015550\r\n" +
            "Moto G Power\r\n342190009880\r\nMoto G Pure\r\n342190010002\r\nMoto G Stylus\r\n" +
            "342190004510\r\n" +

            "-----Google-----\r\nPixel 4\r\n547102938214\r\nPixel 5\r\n547102932994\r\n" +
            "Pixel 6a\r\n547102935124\r\nPixel 7\r\n547102940114\r\nPixel 8\r\n547102939524\r\n" +

            "-----Samsung-----\r\nGalaxy A53\r\n392111087550\r\nGalaxy A54\r\n392111089420\r\n" +
            "Galaxy A23\r\n392111091100\r\nGalaxy A13\r\n392111095820\r\nGalaxy A03s\r\n392111095710\r\n" +
            "***End of UPCs***\n\n\n***Networks***\nStraight Talk\nVerizon\nTotal by Verizon\nAT&T" +
            "\nTracfone\nUnlocked\n***End of Networks***";
        #endregion

        // Model search term (used in two methods)
        string modelSearchTerm = "";
        #endregion

        #region Methods

        #region Displaying Phones

        // Display information for current phone
        void DisplayInfo(List<Phone> list)
        {
            if (list.Count > 0)
            {
                // Display information for current item
                numberLabel.Text = (curItem + 1).ToString() + " of " + list.Count.ToString();
                nameLabel.Text = list[curItem].Name;
                brandLabel.Text = list[curItem].Brand;
                priceLabel.Text = list[curItem].Price.ToString("c");
                imeiLabel.Text = list[curItem].IMEI.ToString();

                // Get phone's UPC (not stored in Phone class)
                for (int i = 0; i < phoneNames.Count; i++)
                {
                    if (nameLabel.Text == phoneNames[i])
                    {
                        upcLabel.Text = upcs[i].ToString();
                    }
                }

                // Check if two same phones have different prices
                List<Tuple<string, decimal>> namesPrices = new List<Tuple<string, decimal>>();

                foreach (Phone phone in list)
                {
                    namesPrices.Add(new Tuple<string, decimal>(phone.Name, phone.Price));
                }

                // Sort list alphabetically
                namesPrices.Sort((x, y) => x.Item1.CompareTo(y.Item1));

                for (int i = 1; i < namesPrices.Count; i++)
                {
                    Tuple<string, decimal> phone1 = namesPrices[i - 1];
                    Tuple<string, decimal> phone2 = namesPrices[i];

                    // Same phones, different prices
                    if (phone1.Item1 == phone2.Item1 && phone1.Item2 != phone2.Item2)
                    {
                        // Notification message
                        string notif = "The " + network + " inventory has two "
                            + phone1.Item1 + " phones that have different prices. One is "
                            + phone1.Item2.ToString("c") + " and the other is "
                            + phone2.Item2.ToString("c") + ".";

                        // Add notification if not already present
                        AddNotification(notif);
                    }
                }

            }
            else
            {
                // No phones that match search term
                if (search == true)
                {
                    numberLabel.Text = "No results";
                }

                // No phones in system
                else
                {
                    numberLabel.Text = "No phones in system";
                }

                // Clear all labels
                nameLabel.Text = ""; brandLabel.Text = ""; priceLabel.Text = "";
                imeiLabel.Text = ""; upcLabel.Text = "";
            }

            if (list.Count > 0)
            {
                // Toggle buttons; clicking these on an empty list would result in an
                // unhandled exception.
                firstButton.Enabled = true;
                previousButton.Enabled = true;
                nextButton.Enabled = true;
                lastButton.Enabled = true;
                deleteButton.Enabled = true;

                // Toggle clear buttons to prevent FileInfo error
                clearButton.Enabled = true;
            }
            else
            {
                firstButton.Enabled = false;
                previousButton.Enabled = false;
                nextButton.Enabled = false;
                lastButton.Enabled = false;
                deleteButton.Enabled = false;
                clearButton.Enabled = false;
            }
        }

        // Adds phones in selected model for either modelListBox or modelSearchListBox
        // Depending on isSearch
        void ListBrandPhones(string brand, bool isSearch)
        {
            if (isSearch == true)
            {
                // Clear previous results
                modelSearchListBox.Items.Clear();

                for (int i = 0; i < phoneNames.Count; i++)
                {
                    // Add phone models under specified brand
                    if (phoneBrands[i] == brand)
                    {
                        // Checks if phone model is already listed
                        if (!modelSearchListBox.Items.Contains(phoneNames[i]))
                        {
                            modelSearchListBox.Items.Add(phoneNames[i]);
                        }
                    }
                }

                // Sort alphabetically
                SortListBox(modelSearchListBox);
            }
            else
            {
                // Clear previous results
                modelListBox.Items.Clear();

                for (int i = 0; i < phoneNames.Count; i++)
                {
                    // Add phone models under specified brand
                    if (phoneBrands[i] == brand)
                    {
                        // Checks if phone model is already listed
                        if (!modelListBox.Items.Contains(phoneNames[i]))
                        {
                            modelListBox.Items.Add(phoneNames[i]);
                        }
                    }
                }

                // Sort alphabetically
                SortListBox(modelListBox);
            }
        }

        #endregion

        #region Initializing Form

        public PhoneForm()
        {
            InitializeComponent();
        }

        private void PhoneForm_Load(object sender, EventArgs e)
        {
            // Upload data from UPC_Organizer.txt
            LoadData(phoneNames, phoneBrands, upcs, brands, networks);

            if (fatalError == false)
            {
                // Set FileInfo to Network_Inventories.txt
                fi = new FileInfo(@"files\Network_Inventories.txt");

                // Get phone inventory from Straight Talk text file (default option)
                // Cycle through all inventories for potential problems
                foreach (string str in networks)
                {
                    // Get network name
                    network = str;

                    // Build network's phone list
                    BuildPhoneList();
                    DisplayInfo(phoneList);
                }

                // Reset to default
                network = "Straight Talk";
                networkListBox.SelectedIndex = 0;
                BuildPhoneList();

                // Build brand list boxes
                LoadBrandListBoxes(brands, brandListBox);
                LoadBrandListBoxes(brands, brandSearchListBox);

                // Build network list box
                LoadNetworkListBoxes(networks, networkListBox);

                // Displays first result
                curItem = 0;

                // Display info for phones
                DisplayInfo(phoneList);

                // Compare UPC_Organizer.txt to backup
                CompareToBackup();
            }
            else
            {
                Close();
            }
        }

        // Updates Notifications tab in middle section
        void AddNotification(string notif)
        {
            // Add textBox to notifTabPage if not already there
            if (!notifTabPage.Controls.Contains(notifTextBox))
            {
                notifTabPage.Controls.Add(notifTextBox);
            }

            // Textbox Configuration
            // Set properties to make it act like a label
            notifTextBox.Visible = true;
            notifTextBox.Multiline = true;
            notifTextBox.ReadOnly = true;
            notifTextBox.BorderStyle = BorderStyle.None;
            notifTextBox.WordWrap = true;

            // Set location and size
            notifTextBox.Location = new Point(3, 3);
            notifTextBox.Size = new Size(352, 174);

            // Add notification if not already present
            if (!notifTextBox.Text.Contains(notif))
            {
                // Increase notification counter
                notifs++;

                // Add notification to page (universal newline required, \n didn't work on textbox
                notifTextBox.Text += notifs.ToString() + ". " + notif + Environment.NewLine
                    + Environment.NewLine;
            }

            // Update title of notifications tab after all setup processes
            notifTabPage.Text = "Notifications (" + notifs.ToString() + ")";
        }

        // Updates status strip on bottom of form
        // Used with MouseEnter and MouseLeave EventHandlers
        void UpdateStatus(string status)
        {
            statusLabel.Text = status;
        }

        #endregion

        #region Loading Data

        // Reads inventory text file and adds phones to phoneList
        void BuildPhoneList()
        {
            // Clear previous results
            phoneList.Clear();

            // Create variables to read the contents
            FileStream fs = fi.Open(FileMode.Open, FileAccess.ReadWrite);
            StreamReader sr = new StreamReader(fs);

            string str = sr.ReadLine();

            // Reach network inventory
            while (str != "|---" + network.ToUpper() + "---|")
            {
                str = sr.ReadLine();
            }

            // Skips "---Phone Inventory---" line
            str = sr.ReadLine();

            if (str != null && !str.Contains("*****"))       // Check if network's inventory is empty
            {
                try
                {
                    while (sr.Peek() != '*' && sr.Peek() != '|')     // Check for end of inventory or end of file
                    {
                        string phoneName = sr.ReadLine();
                        string phoneBrand = sr.ReadLine();
                        decimal phonePrice = decimal.Parse(sr.ReadLine());
                        phoneList.Add(new Phone(phoneName, phoneBrand, phonePrice));
                        phoneList[curItem].IMEI = long.Parse(sr.ReadLine());
                        curItem++;
                        sr.ReadLine();          // Move past "=====" divider
                    }

                    curItem = 0;                // Display info for first phone

                    // Close StreamReader and FileStream
                    sr.Close(); sr.Dispose(); fs.Close();
                }
                // Reset PhoneInventory.txt (CLEARS INVENTORY)
                // Activates when an error occurs in loading inventory
                catch
                {
                    sr.Close(); fs.Close();
                    RepairInventory(false);
                }
            }
            else
            {
                sr.Close(); fs.Close();
                RepairInventory(false);
            }
        }

        // Compares UPC_Organizer.txt to backup phones
        // If they match, restoreButton will be disabled. If not, the option will be available.
        void CompareToBackup()
        {
            try
            {
                // Get UPC_Organizer.txt
                FileInfo fi2 = new FileInfo(@"files/UPC_Organizer.txt");

                string curContent = File.ReadAllText(fi2.FullName);

                // Test if file content is the same as the default data
                if (curContent == BACKUP_TEXT)
                {
                    // They are the same
                    restoreButton.Enabled = false;
                }
                else
                {
                    // They are different
                    restoreButton.Enabled = true;
                }
            }
            catch
            {
                MessageBox.Show("Failed to compare to backup file.", "Error");
            }
        }

        // Builds brand lists on all forms
        public void LoadBrandListBoxes(List<string> brandList, ListBox listBox)
        {
            // Clear previous results (if applicable)
            listBox.Items.Clear();

            // Add each brand to all list boxes that list brands
            foreach (string str in brandList)
            {
                listBox.Items.Add(str);
            }

            // Sort items in list box
            SortListBox(listBox);

            // If no brands or phones are in system
            if (brandList.Count == 0)
            {
                // Show message encouraging user to add phones
                listBox.Items.Add("Add some phones");
                listBox.Items.Add("to get started!");
                listBox.Enabled = false;

                // Disable enter button
                enterButton.Enabled = false;
            }
            else
            {
                // Enable controls if previously disabled
                enterButton.Enabled = true;
                listBox.Enabled = true;
            }
        }

        // Gets phone & upc data from UPC_Organizer.txt (UPCs do not change with network)
        // Also builds parallel brand list for phones and list of all brands in system
        // Used when first loading all forms that show phone data
        public void LoadData(List<string> names, List<string> phoneBrands,
            List<long> upcs, List<string> brandList, List<string> networkList)
        {
            try
            {
                // Has to be declared and initialized before fi2 to prevent error
                List<string> lines = File.ReadAllLines(@"files\UPC_Organizer.txt").ToList();

                // Get file and create variables to read the contents
                FileInfo fi2 = new FileInfo(@"files\UPC_Organizer.txt");

                FileStream fs = fi2.Open(FileMode.Open, FileAccess.Read);

                StreamReader sr = new StreamReader(fs);

                // Used to verify that "***End of UPCs***", "***Networks***", and
                // "***End of Networks***" are present

                bool hasUPC = false;
                bool hasNetworks = false;
                bool hasNetworksEnd = false;

                // Check for essential markers in UPC_Organizer.txt
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i].Contains("***End of UPCs***"))
                    {
                        hasUPC = true;

                        if (lines[i] != "***End of UPCs***")
                        {
                            string notif = "\"***End of UPCs***\" line in UPC_Organizer.txt" +
                                " has some unnecessary text.";

                            AddNotification(notif);
                        }
                    }
                    if (lines[i].Contains("***Networks***"))
                    {
                        hasNetworks = true;

                        if (lines[i] != "***Networks***")
                        {
                            string notif = "\"***Networks***\" line in UPC_Organizer.txt" +
                                " has some unnecessary text.";

                            AddNotification(notif);
                        }
                    }
                    if (lines[i].Contains("***End of Networks***"))
                    {
                        hasNetworksEnd = true;

                        if (lines[i] != "***End of Networks***")
                        {
                            string notif = "\"***End of Networks***\" line in UPC_Organizer.txt" +
                                " has some unnecessary text.";

                            AddNotification(notif);
                        }
                    }
                }

                if (hasUPC == true && hasNetworks == true && hasNetworksEnd == true)
                {
                    string line;                    // Reads line
                    string brand = "";              // Initialized to prevent unassigned variable

                    // Error detector
                    int occurrences = 0;

                    // This loop adds phones to the appropriate lists
                    // and ends when the StreamReader reaches the end
                    // of UPC_Organizer.txt
                    while (sr.Peek() != -1)
                    {
                        // Reads line
                        line = sr.ReadLine();

                        if (!line.Contains("***End of UPCs***"))
                        {
                            // If a space is accidentally entered, this code will jump over it.
                            if (line == "")
                            {
                                // An unexpected case is an error occurrence
                                occurrences++;

                                line = sr.ReadLine();

                                // Skip more lines if there are multiple unexpected spaces in one area
                                while (line == "")
                                {
                                    line = sr.ReadLine();
                                    occurrences++;
                                }
                            }

                            // Declare variables used to write data
                            string phone;
                            string upc;

                            // If next line is brand name
                            if (line.Contains("-----"))
                            {
                                // Removes the dashes from the brand name
                                brand = line.Trim('-');

                                brandList.Add(brand);

                                line = null;
                            }

                            // Get new phone information (if a brand name was just read,
                            // line will be set to null, prompting the StreamReader to read the
                            // next line. If a previous phone was just read, the StreamReader is
                            // already sitting on the next phone name and will just read that.
                            if (line == null)
                            {
                                phone = sr.ReadLine();

                                // Test if phone is blank
                                while (phone == "")
                                {
                                    occurrences++;
                                    phone = sr.ReadLine();
                                }
                            }
                            else
                            {
                                phone = line;
                            }

                            upc = sr.ReadLine();

                            // Test if UPC is a number
                            while (!long.TryParse(upc, out long upc2))
                            {
                                occurrences++;
                                upc = sr.ReadLine();
                            }

                            // Test if UPC is 12 digits
                            if (upc.Length != 12)
                            {
                                // Notification message and fix error
                                string notif;

                                // Add zeros if UPC is less than 12 digits
                                if (upc.Length < 12)
                                {
                                    notif = phone + "'s UPC is less than 11 digits. This has been " +
                                        "fixed for now, but this might cause potential problems.";

                                    int extraDigits = 12 - upc.Length;

                                    string extraZeros = "";

                                    // Add all necessary digits
                                    for (int i = 0; i < extraDigits; i++)
                                    {
                                        extraZeros += "0";
                                    }

                                    upc += extraZeros;
                                }
                                else
                                {
                                    notif = phone + "'s UPC is more than 11 digits. Extra digits have been " +
                                        "removed, but this might cause potential problems.";

                                    // Trim extra digits off and update UPC
                                    string newUPC = upc.Substring(0, Math.Min(upc.Length, 11));

                                    upc = newUPC;
                                }

                                // Add notification
                                AddNotification(notif);
                            }

                            // Add phone name and UPC to respective lists
                            names.Add(phone);
                            phoneBrands.Add(brand);
                            upcs.Add(long.Parse(upc));
                        }

                        // Network list
                        else
                        {
                            // Reach Networks section
                            while (!line.Contains("***Networks***"))
                            {
                                // Skip "***Networks***" line
                                line = sr.ReadLine();
                            }

                            // Get first network
                            line = sr.ReadLine();

                            // Build Networks list
                            while (!line.Contains("***End of Networks***"))
                            {
                                networkList.Add(line);
                                networkListBox.Items.Add(line);
                                line = sr.ReadLine();
                            }
                        }
                    }

                    // Notification if errors were spotted (like unexpected spaces)
                    if (occurrences > 0)
                    {
                        // String to hold notification message
                        string notif;

                        if (occurrences == 1)
                        {
                            notif = "An unexpected space was detected in the UPC_Organizer file.";
                        }
                        else
                        {
                            notif = occurrences.ToString() + " unexpected spaces were detected " +
                                "in the UPC_Organizer file.";
                        }

                        // Add notification if not already present
                        AddNotification(notif);
                    }

                    // Sort brands alphabetically
                    brandList.Sort();
                }
                else
                {
                    // Notify user of which crucial line is missing
                    if (hasUPC == false)
                    {
                        MessageBox.Show("UPC_Organizer.txt does not contain \"***End of UPCs***\"." +
                            " Unable to load phone information.", "Fatal Error");
                    }
                    else if (hasNetworks == false)
                    {
                        MessageBox.Show("UPC_Organizer.txt does not contain \"***Networks***\"." +
                            " Unable to load phone information.", "Fatal Error");
                    }
                    else if (hasNetworksEnd == false)
                    {
                        MessageBox.Show("UPC_Organizer.txt does not contain \"***End of Networks***\"." +
                            " Unable to load phone information.", "Fatal Error");
                    }

                    fatalError = true;
                }

                // Close StreamReader and FileStream
                sr.Close(); sr.Dispose(); fs.Close();
            }
            catch
            {
                string message = "There was an error retrieving the phone names and UPCs."
                    + " Do you want to restore the UPC file to default configurations?";

                WarningForm warningForm = new WarningForm(message);

                if (warningForm.Proceed == true)
                {
                    // Restore UPC_Organizer.txt default phones
                    object sender = null;
                    EventArgs e = new EventArgs();

                    restoreButton_Click(sender, e);
                }
            }
        }

        // Builds network lists on all forms
        public void LoadNetworkListBoxes(List<string> networkList, ListBox listBox)
        {
            // Clear previous results (if applicable)
            listBox.Items.Clear();

            // Add each brand to all list boxes that list brands
            foreach (string str in networkList)
            {
                listBox.Items.Add(str);
            }

            // Sort items in list box
            SortListBox(listBox);

            // If nothing is in the system
            if (networkList.Count == 0)
            {
                // Show message encouraging user to add phones
                listBox.Items.Add("Add some phones");
                listBox.Items.Add("to get started!");
                listBox.Enabled = false;

                // Disable enter button
                enterButton.Enabled = false;
            }
            else
            {
                // Enable controls if previously disabled
                enterButton.Enabled = true;
                listBox.Enabled = true;
            }

        }

        // Refreshes the form (used when UPC_Organizer.txt is edited)
        void RefreshForm(object sender, EventArgs e)
        {
            // Reset curItem
            curItem = 0;

            // Clear list boxes
            modelListBox.Items.Clear(); modelSearchListBox.Items.Clear();

            // Clear lists
            phoneNames.Clear(); phoneBrands.Clear(); upcs.Clear();
            phoneList.Clear(); brands.Clear(); networks.Clear();

            // Refresh form
            PhoneForm_Load(sender, e);
        }

        // Sorts items in listBox parameter alphabetically
        public void SortListBox(ListBox listBox)
        {
            // Sort items alphabetically
            List<string> items = new List<string>();
            foreach (var item in listBox.Items)
            {
                items.Add(item.ToString());
            }

            // Sort items and clear original list
            items.Sort();
            listBox.Items.Clear();

            // Add sorted items back into ListBox
            foreach (var item in items)
            {
                listBox.Items.Add(item);
            }
        }

        #endregion

        #region Write Data

        // If an inventory error is spotted, rewrites file
        // Also used when intentionally clearing inventory
        void RepairInventory(bool clear)
        {
            // Used if an inventory file has been edited to where it's unreadable by the program.
            // StreamReader will check for "---Phone Inventory---" on first line. If it is not there,
            // file will be restored to a readable state.

            // Get file contents and phone count
            List<string> lines = File.ReadAllLines(fi.FullName).ToList();
            List<string> removeStrings = new List<string>();

            // Search for network inventory
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i] == "|---" + network.ToUpper() + "---|")
                {
                    // Get lines to remove
                    // The "+ 2" skips the two title lines
                    for (int j = i + 2; j < lines.Count; j++)
                    {
                        if (lines[j] != "|End of Inventory|")
                        {
                            removeStrings.Add(lines[j]);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                // Exit loop if inventory was found
                if (removeStrings.Count > 0)
                {
                    break;
                }
            }

            // Add variable to find network again (fix later)
            bool found = false;

            // Remove selected lines from list
            foreach (string str in removeStrings)
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i] == "|---" + network.ToUpper() + "---|")
                    {
                        found = true;
                    }

                    if (lines[i] == str && found == true)
                    {
                        lines.RemoveAt(i);
                        break;
                    }
                }

                // Reset boolean for next iteration
                if (found == true)
                {
                    found = false;
                }
            }

            // Rewrite file contents
            File.WriteAllLines(fi.FullName, lines.ToArray());

            // Verify if this method was activated intentionally or not
            if (clear == false)
            {
                MessageBox.Show("An error occurred while trying to load phone inventory.", "Inventory Not Found");
            }
            else
            {
                // Only accessible through clear buttons. A warning pops up letting the user know
                // this process cannot be undone. This code will execute if the user clicks YES.
                // If the user clicks NO, RepairInventory will not execute.
                phoneList.Clear();
                results.Clear();
                DisplayInfo(phoneList);
            }
        }

        // Writes new phones into network inventory
        void WriteToFile(Phone phone)
        {
            // Get file contents
            List<string> lines = File.ReadAllLines(fi.FullName).ToList();

            // Find network inventory
            for (int i = 0; i < lines.Count; i++)
            {
                if (i > 0)      // Prevents index out of range error
                {
                    // Add phone to inventory
                    if (lines[i - 1] == "|---" + network.ToUpper() + "---|")
                    {
                        lines.Insert(i + 1, phone.Name);
                        lines.Insert(i + 2, phone.Brand);
                        lines.Insert(i + 3, phone.Price.ToString());
                        lines.Insert(i + 4, phone.IMEI.ToString());
                        lines.Insert(i + 5, "===========================");
                    }

                    // Rewrite file contents
                    File.WriteAllLines(fi.FullName, lines);
                }
            }
        }

        #endregion

        #region EventHandlers

        #region Enter Tab

        private void enterButton_Click(object sender, EventArgs e)
        {
            // Declare variables
            string brand;
            string model;
            decimal cost;

            // Toggle enter mode
            search = false;

            if (modelListBox.SelectedItem != null)
            {
                if (decimal.TryParse(costTextBox.Text, out cost))
                {
                    if (cost >= 0)
                    {
                        bool addPhone = true;
                        string message;

                        // Triggers warning form if price is outside of a certain range
                        if (cost < 20 || cost > 2500)
                        {
                            if (cost < 20)
                            {
                                message = "WARNING: Price is unusually low for a phone. Do you want to proceed?";
                            }
                            else
                            {
                                message = "WARNING: Price is unusually high for a phone. Do you want to proceed?";
                            }
                            WarningForm warningForm = new WarningForm(message);
                            warningForm.ShowDialog();

                            if (warningForm.Proceed == true)
                            {
                                addPhone = true;
                            }
                            else
                            {
                                addPhone = false;
                                costTextBox.Focus();
                                costTextBox.SelectAll();
                            }
                        }

                        // Code runs if price is within range or user chooses to proceed
                        if (addPhone == true)
                        {
                            brand = brandListBox.SelectedItem.ToString();
                            model = modelListBox.SelectedItem.ToString();

                            // Add new phone
                            phoneList.Add(new Phone(model, brand, cost));

                            // Reset menu
                            brandListBox.SelectedIndex = 0;
                            brandListBox_SelectedIndexChanged(sender, e);
                            costTextBox.Clear();
                            modelTextBox.Clear();
                            brandListBox.Focus();

                            curItem = phoneList.Count - 1;
                            WriteToFile(phoneList[curItem]);
                            DisplayInfo(phoneList);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Enter a positive price.", "Error");
                    }
                }
                else
                {
                    MessageBox.Show("Enter a valid price.", "Error");
                }
            }
            else
            {
                if (brandListBox.SelectedItem == null)
                {
                    MessageBox.Show("Select a brand.", "Error");
                }
                else
                {
                    MessageBox.Show("Select a phone model.", "Error");
                }
            }
        }

        private void brandListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Clear previous results
            modelListBox.Items.Clear();
            modelTextBox.Clear();

            // Verify an item has been selected
            if (brandListBox.SelectedIndex != -1)
            {
                ListBrandPhones(brandListBox.SelectedItem.ToString(), false);
            }
            else
            {
                modelListBox.Items.Clear();
            }
        }

        private void modelListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check if phone already exists in inventory.
            // If it does, automatically add the price to costTextBox
            string selPhone = modelListBox.SelectedItem.ToString();

            bool firstEntry = true;

            for (int i = 0; i < phoneList.Count; i++)
            {
                if (phoneList[i].Name == selPhone)
                {
                    costTextBox.Text = phoneList[i].Price.ToString();
                    firstEntry = false;
                    break;
                }
            }

            if (firstEntry == true)
            {
                costTextBox.Clear();
            }
        }

        private void modelTextBox_TextChanged(object sender, EventArgs e)
        {
            // modelListBox acts as a search bar when using text box
            if (modelListBox.Items.Count > 0)
            {
                if (modelTextBox.Text != "")
                {
                    List<string> items = new List<string>();

                    // Add listbox items to list
                    foreach (var item in modelListBox.Items)
                    {
                        items.Add(item.ToString());
                    }

                    // Clear listbox items
                    modelListBox.Items.Clear();

                    // Add search results to modelListBox
                    foreach (string str in items)
                    {
                        if (str.Contains(modelTextBox.Text))
                        {
                            modelListBox.Items.Add(str);
                        }
                    }

                    // Sort search results alphabetically
                    SortListBox(modelListBox);
                }
            }
        }

        #endregion

        #region Search Tab

        private void brandSearchListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Clear previous result
            modelListBox.Items.Clear();

            // Verify an item has been selected
            if (brandSearchListBox.SelectedIndex != -1)
            {
                ListBrandPhones(brandSearchListBox.SelectedItem.ToString(), true);
            }
            else
            {
                modelListBox.Items.Clear();
            }
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            // Enter "Search" mode
            search = true;

            // Clear previous search result (if applicable)
            results.Clear();

            // Verify that a brand and model were selected
            if (brandSearchListBox.SelectedIndex != -1 && modelSearchListBox.SelectedIndex != -1)
            {
                // Assign variables to search terms
                modelSearchTerm = modelSearchListBox.SelectedItem.ToString();

                foreach (Phone phone in phoneList)
                {
                    // Check for phone model in inventory
                    if (phone.Name == modelSearchTerm)
                    {
                        results.Add(phone);
                    }
                }

                curItem = 0;            // Show information for first phone
                DisplayInfo(results);
            }
            else
            {
                if (brandSearchListBox.SelectedIndex == -1)
                {
                    MessageBox.Show("Select a brand.", "Error");
                }
                else
                {
                    MessageBox.Show("Select a model.", "Error");
                }
            }
        }

        private void searchIMEIButton_Click(object sender, EventArgs e)
        {
            // Declare variables
            long imei;

            // Enter "Search" mode
            search = true;

            // Clear previous search result (if applicable)
            results.Clear();

            if (long.TryParse(imeiTextBox.Text, out imei))
            {
                foreach (Phone phone in phoneList)
                {
                    // Check for IMEI in inventory
                    if (phone.IMEI.ToString().Contains(imeiTextBox.Text))
                    {
                        results.Add(phone);
                    }
                }

                curItem = 0;            // Show information for first phone
                DisplayInfo(results);
            }
            else
            {
                if (imeiTextBox.Text == null)
                {
                    MessageBox.Show("Enter an IMEI.", "Error");
                }
                else
                {
                    MessageBox.Show("Enter a valid IMEI.", "Error");
                }
            }
        }

        private void upcSearchButton_Click(object sender, EventArgs e)
        {
            // Declare variables
            long upc;

            // Enter "Search" mode
            search = true;

            // Clear previous search result (if applicable)
            results.Clear();

            if (long.TryParse(upcTextBox.Text, out upc))
            {
                for (int i = 0; i < upcs.Count; i++)
                {
                    if (upcs[i].ToString().Contains(upcTextBox.Text.ToString()))
                    {
                        modelSearchTerm = phoneNames[i];

                        foreach (Phone phone in phoneList)
                        {
                            // Check for phone model in inventory
                            if (phone.Name == modelSearchTerm)
                            {
                                results.Add(phone);
                            }
                        }
                    }
                }

                curItem = 0;            // Show information for first phone
                DisplayInfo(results);
            }
            else
            {
                if (imeiTextBox.Text == null)
                {
                    MessageBox.Show("Enter a UPC.", "Error");
                }
                else
                {
                    MessageBox.Show("Enter a valid UPC.", "Error");
                }
            }
        }

        private void upcTextBox_TextChanged(object sender, EventArgs e)
        {
            // Enter "Search" mode
            search = true;

            // Clear previous search result (if applicable)
            upcResultLabel.Text = "";

            try
            {
                long searchTerm = long.Parse(upcTextBox.Text);
                List<string> searchResults = new List<string>();

                for (int i = 0; i < upcs.Count; i++)
                {
                    if (upcs[i].ToString().Contains(upcTextBox.Text.ToString()))
                    {
                        searchResults.Add(phoneNames[i].ToString());
                    }
                }

                for (int i = 0; i < searchResults.Count; i++)
                {
                    if (searchResults.Count - i > 1)
                    {
                        upcResultLabel.Text += searchResults[i].ToString() + ", ";
                    }
                    else
                    {
                        upcResultLabel.Text += searchResults[i].ToString();
                    }
                }
            }
            catch
            {

            }
        }

        #endregion

        #region Inventory Management Tab

        // Tracks warningForm.Proceed (only used in one instance but in two methods)
        bool proceed = false;

        // Clear all network inventories (Clear All Inventories)
        private void clearAllButton_Click(object sender, EventArgs e)
        {
            // Create warningForm
            string warning = "WARNING: This cannot be undone. Do you want to proceed?";
            WarningForm warningForm = new WarningForm(warning);

            // Displayed if this code was not accessed via RestoreButton_Click
            if (proceed == false)
            {
                warningForm.ShowDialog();
            }

            // warningForm.Proceed == true when clearAllButton was clicked
            // proceed == true when RestoreButton was clicked
            if (warningForm.Proceed == true || proceed == true)
            {
                // Rewrite inventories to be blank
                List<string> lines = new List<string>();

                // Cycle through all networks
                foreach (string network in networks)
                {
                    // Add necessary inventory lines
                    lines.Add("|---" + network.ToUpper() + "---|");
                    lines.Add("---Phone Inventory---");
                    lines.Add("|End of Inventory|");
                    lines.Add("***************************");
                    lines.Add("");
                }

                // Rewrite file contents
                File.WriteAllLines(fi.FullName, lines.ToArray());

                // Default network
                networkListBox.SelectedIndex = 0;
            }
        }

        // Clear inventory of selected network (Clear Network Inventory)
        private void clearButton_Click(object sender, EventArgs e)
        {
            // Display warning
            string warning = "WARNING: This cannot be undone. Do you want to proceed?";
            WarningForm warningForm = new WarningForm(warning);
            warningForm.ShowDialog();

            if (warningForm.Proceed == true)
            {
                RepairInventory(true);
            }
        }

        // Open modelsMgmtForm (Manage Phone Models)
        private void manageModelsButton_Click(object sender, EventArgs e)
        {
            ModelsMgmtForm modelsMgmtForm = new ModelsMgmtForm();
            modelsMgmtForm.ShowDialog();

            RefreshForm(sender, e);
        }

        // Restore default phone names and UPCs (Restore Default Phone Models)
        private void restoreButton_Click(object sender, EventArgs e)
        {
            // Display warning
            string warning = "WARNING: This will also clear all inventories and cannot be undone. " +
                "Do you want to proceed?";
            WarningForm warningForm = new WarningForm(warning);
            warningForm.ShowDialog();

            if (warningForm.Proceed == true)
            {
                // Change value of proceed boolean for clearAllButton_Click
                // Prevents warningForm from appearing twice
                proceed = true;

                // Clear all inventories
                clearAllButton_Click(sender, e);

                // Get UPC_Organizer.txt
                FileInfo fi2 = new FileInfo(@"files\UPC_Organizer.txt");

                // Restore backup text to file
                File.WriteAllText(fi2.FullName, BACKUP_TEXT);

                // Refresh form to show changes
                RefreshForm(sender, e);
            }
        }

        // List phones in all network inventories (List All Phones)
        private void totalButton_Click(object sender, EventArgs e)
        {
            string str = network;
            
            network = "all";
            Form ListForm = new ListForm(network, true);
            ListForm.ShowDialog();

            // Reset form to first network in listbox
            // Prevents "all" from being used elsewhere
            network = str;

            int index = networkListBox.Items.IndexOf(str);
            networkListBox.SelectedIndex = index;

            networkListBox_SelectedIndexChanged(sender, e);
        }

        // Copy network inventory file and place it in user's Downloads folder (Export Network Inventory)
        private void backUpInventoryButton_Click(object sender, EventArgs e)
        {
            // String for destinationFilePath
            string fileName = "Phone_Inventory.txt";

            // Create file path variables for copying the file
            string sourceFilePath = fi.FullName;
            string destinationFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";
            string destinationFilePath = Path.Combine(destinationFolderPath, fileName);

            try
            {
                // Copy the file to the downloads folder
                File.Copy(sourceFilePath, destinationFilePath, true);
                MessageBox.Show("File successfully copied to your downloads folder.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region Menu Buttons

        // Delete phone currently displayed (Delete Phone)
        private void deleteButton_Click(object sender, EventArgs e)
        {
            // Get file contents
            string fileContents = File.ReadAllText(fi.FullName);

            // Concatenate text to find in PhoneInventory
            string targetTxt =
                 phoneList[curItem].Name + "\r\n" +
                 phoneList[curItem].Brand + "\r\n" +
                 phoneList[curItem].Price.ToString() + "\r\n" +
                 phoneList[curItem].IMEI.ToString() + "\r\n" +
                 "===========================" + "\r\n";

            // Update fileContents if phone is found
            if (fileContents.Contains(targetTxt))
            {
                fileContents = fileContents.Replace(targetTxt, string.Empty);
            }
            else
            {
                MessageBox.Show("Phone not found in inventory.", "Error");
            }

            // Rewrite file contents
            File.WriteAllText(fi.FullName, fileContents);

            // Update phoneList
            if (phoneList.Count > 1)
            {
                phoneList.RemoveAt(curItem);
                curItem = 0;                // Form will display info for first phone in list
            }
            else if (phoneList.Count == 1)
            {
                phoneList.Clear();
            }

            // Display updated phoneList
            DisplayInfo(phoneList);
        }

        // Exits the form
        private void exitButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        // Displays info for first phone in bottom section
        private void firstButton_Click(object sender, EventArgs e)
        {
            curItem = 0;

            if (search == false)
            {
                DisplayInfo(phoneList);
            }
            else
            {
                DisplayInfo(results);
            }
        }

        // Updates currently displayed phone's IMEI number
        private void imeiLabel_Click(object sender, EventArgs e)
        {
            if (imeiLabel.Text != "")
            {
                UpdateForm updateForm = new UpdateForm("IMEI", phoneList[curItem]);
                updateForm.ShowDialog();

                if (updateForm.NewValue != null)
                {
                    // Concatenate text to find in PhoneInventory
                    // targetTxt1 is old properties, targetTxt2 is new properties
                    string targetTxt1 =
                         phoneList[curItem].Name + "\r\n" +
                         phoneList[curItem].Brand + "\r\n" +
                         phoneList[curItem].Price.ToString() + "\r\n" +
                         phoneList[curItem].IMEI.ToString() + "\r\n" +
                         "===========================" + "\r\n";

                    // Get old price before assigning new one
                    // NewValue is validated on updateForm
                    long oldIMEI = phoneList[curItem].IMEI;
                    phoneList[curItem].IMEI = long.Parse(updateForm.NewValue);

                    // Get file contents
                    string fileContents = File.ReadAllText(fi.FullName);

                    // Update fileContents if phone is in inventory
                    if (fileContents.Contains(targetTxt1))
                    {
                        string targetTxt2 = targetTxt1.Replace(oldIMEI.ToString(), updateForm.NewValue);
                        fileContents = fileContents.Replace(targetTxt1, targetTxt2);
                    }
                    else
                    {
                        MessageBox.Show("Phone not found in inventory.", "Error");
                    }

                    // Rewrite inventory
                    File.WriteAllText(fi.FullName, fileContents);

                    // Refresh form with new information
                    RefreshForm(sender, e);
                }
            }
        }

        // Displays info for last phone in bottom section
        private void lastButton_Click(object sender, EventArgs e)
        {
            if (search == false)
            {
                curItem = phoneList.IndexOf(phoneList.Last());
                DisplayInfo(phoneList);
            }
            else
            {
                curItem = results.IndexOf(results.Last());
                DisplayInfo(results);
            }
        }

        // Lists phones in selected network's inventory
        private void listButton_Click(object sender, EventArgs e)
        {
            Form ListForm = new ListForm(network, false);
            ListForm.ShowDialog();
        }

        // Displays info for next phone in bottom section
        private void nextButton_Click(object sender, EventArgs e)
        {
            if (search == false)
            {
                if (curItem < phoneList.Count - 1)
                {
                    curItem++;
                    DisplayInfo(phoneList);
                }
            }
            else
            {
                if (curItem < results.Count - 1)
                {
                    curItem++;
                    DisplayInfo(results);
                }
            }
        }

        // Displays info for previous phone in bottom section
        private void previousButton_Click(object sender, EventArgs e)
        {
            if (curItem > 0)
            {
                curItem--;

                if (search == false)
                {
                    DisplayInfo(phoneList);
                }
                else
                {
                    DisplayInfo(results);
                }
            }
        }

        // Updates currently displayed phone's price
        private void priceLabel_Click(object sender, EventArgs e)
        {
            if (priceLabel.Text != "")
            {
                UpdateForm updateForm = new UpdateForm("Price", phoneList[curItem]);
                updateForm.ShowDialog();

                if (updateForm.NewValue != null)
                {
                    // Concatenate text to find in PhoneInventory
                    // targetTxt1 is old properties, targetTxt2 is new properties
                    string targetTxt1 =
                         phoneList[curItem].Name + "\r\n" +
                         phoneList[curItem].Brand + "\r\n" +
                         phoneList[curItem].Price.ToString() + "\r\n" +
                         phoneList[curItem].IMEI.ToString() + "\r\n" +
                         "===========================" + "\r\n";

                    // Get old price before assigning new one
                    // NewValue is validated on updateForm
                    decimal oldPrice = phoneList[curItem].Price;
                    phoneList[curItem].Price = decimal.Parse(updateForm.NewValue);

                    // Get file contents
                    string fileContents = File.ReadAllText(fi.FullName);

                    // Update fileContents if phone is in inventory
                    if (fileContents.Contains(targetTxt1))
                    {
                        string targetTxt2 = targetTxt1.Replace(oldPrice.ToString(), updateForm.NewValue);
                        fileContents = fileContents.Replace(targetTxt1, targetTxt2);
                    }
                    else
                    {
                        MessageBox.Show("Phone not found in inventory.", "Error");
                    }

                    // Rewrite inventory
                    File.WriteAllText(fi.FullName, fileContents);

                    // Refresh form with new information
                    RefreshForm(sender, e);
                }
            }
        }

        #endregion

        #region MouseEnter & MouseLeave EventHandlers

        private void backUpInventoryButton_MouseEnter(object sender, EventArgs e)
        {
            UpdateStatus("Creates a copy of " + network + " inventory in your Downloads folder.");
        }

        private void backUpInventoryButton_MouseLeave(object sender, EventArgs e)
        {
            UpdateStatus("");
        }

        private void clearAllButton_MouseEnter(object sender, EventArgs e)
        {
            UpdateStatus("Clears inventories of all networks.");
        }

        private void clearAllButton_MouseLeave(object sender, EventArgs e)
        {
            UpdateStatus("");
        }

        private void clearButton_MouseEnter(object sender, EventArgs e)
        {
            UpdateStatus("Clears " + network + " inventory.");
        }

        private void clearButton_MouseLeave(object sender, EventArgs e)
        {
            UpdateStatus("");
        }

        private void deleteButton_MouseEnter(object sender, EventArgs e)
        {
            UpdateStatus("Deletes currently displayed phone.");
        }

        private void deleteButton_MouseLeave(object sender, EventArgs e)
        {
            UpdateStatus("");
        }

        private void firstButton_MouseEnter(object sender, EventArgs e)
        {
            UpdateStatus("Shows first phone in " + network + " inventory.");
        }

        private void firstButton_MouseLeave(object sender, EventArgs e)
        {
            UpdateStatus("");
        }

        private void lastButton_MouseEnter(object sender, EventArgs e)
        {
            UpdateStatus("Shows last phone in " + network + " inventory.");
        }

        private void lastButton_MouseLeave(object sender, EventArgs e)
        {
            UpdateStatus("");
        }

        private void listButton_MouseEnter(object sender, EventArgs e)
        {
            if (phoneList.Count > 0)
            {
                UpdateStatus("Lists all phones in " + network + " inventory.");
            }
            else
            {
                UpdateStatus("Lists all phones in " + network + " inventory (if there were any).");
            }
        }

        private void listButton_MouseLeave(object sender, EventArgs e)
        {
            UpdateStatus("");
        }

        private void manageModelsButton_MouseEnter(object sender, EventArgs e)
        {
            UpdateStatus("Opens inventory management form.");
        }

        private void manageModelsButton_MouseLeave(object sender, EventArgs e)
        {
            UpdateStatus("");
        }

        private void nextButton_MouseEnter(object sender, EventArgs e)
        {
            UpdateStatus("Shows next phone in " + network + " inventory.");
        }

        private void nextButton_MouseLeave(object sender, EventArgs e)
        {
            UpdateStatus("");
        }

        private void previousButton_MouseEnter(object sender, EventArgs e)
        {
            UpdateStatus("Shows previous phone in " + network + " inventory.");
        }

        private void previousButton_MouseLeave(object sender, EventArgs e)
        {
            UpdateStatus("");
        }

        private void restoreButton_MouseEnter(object sender, EventArgs e)
        {
            UpdateStatus("Clears inventories and resets list of available models to default.");
        }

        private void restoreButton_MouseLeave(object sender, EventArgs e)
        {
            UpdateStatus("");
        }

        private void totalButton_MouseEnter(object sender, EventArgs e)
        {
            UpdateStatus("Lists all phones in all networks.");
        }

        private void totalButton_MouseLeave(object sender, EventArgs e)
        {
            UpdateStatus("");
        }

        #endregion

        private void networkListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            network = networkListBox.SelectedItem.ToString();
            curItem = 0;
            BuildPhoneList();
            DisplayInfo(phoneList);
            search = false;
        }

        #endregion

        #endregion
    }
}
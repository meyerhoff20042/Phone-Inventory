using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ElectronicsInventory
{
    public partial class ModelsMgmtForm : Form
    {
        #region Fields

        // Parallel lists (match model, brand, and UPC)
        readonly List<string> phoneNames = new List<string>();
        readonly List<string> phoneBrands = new List<string>();
        readonly List<long> upcs = new List<long>();
        readonly List<string> networks = new List<string>();

        // List of brands
        readonly List<string> brands = new List<string>();

        // FileInfo variable
        FileInfo fi;

        // PhoneForm (used to access public methods)
        readonly PhoneForm phoneForm = new PhoneForm();

        // TextBoxes for "Add Model", "Add Brand", and "Add Network"
        TextBox modelListTextBox = new TextBox();
        TextBox brandListTextBox = new TextBox();
        TextBox networkListTextBox = new TextBox();

        #endregion

        #region Methods

        #region Initializing Form

        public ModelsMgmtForm()
        {
            InitializeComponent();
        }

        private void ModelsMgmtForm_Load(object sender, EventArgs e)
        {
            // Build lists
            phoneForm.LoadData(phoneNames, phoneBrands, upcs, brands, networks);

            // Clear listTextBoxes if previously loaded
            modelListTextBox.Clear();
            brandListTextBox.Clear();
            networkListTextBox.Clear();

            // Load brands into list boxes and labels
            phoneForm.LoadBrandListBoxes(brands, addModelBrandListBox);
            phoneForm.LoadBrandListBoxes(brands, updateModelBrandListBox);
            phoneForm.LoadBrandListBoxes(brands, deleteModelBrandListBox);
            phoneForm.LoadBrandListBoxes(brands, updateBrandListBox);
            phoneForm.LoadBrandListBoxes(brands, deleteBrandListBox);

            // Load networks into list boxes and labels
            phoneForm.LoadNetworkListBoxes(networks, updateNetworkListBox);
            phoneForm.LoadNetworkListBoxes(networks, deleteNetworkListBox);

            // Configure brandListTextBox
            addBrandTabPage.Controls.Add(brandListTextBox);
            ConfigureTextBox(brandListTextBox);

            foreach (string str in brands)
            {
                brandListTextBox.Text += str + Environment.NewLine;
            }

            if (brands.Count == 0)
            {
                brandListTextBox.Text = "Add some phones to get started!";
            }

            // Configure networkListTextBox
            addNetworkTabPage.Controls.Add(networkListTextBox);
            ConfigureTextBox(networkListTextBox);

            foreach (string str in networks)
            {
                networkListTextBox.Text += str + Environment.NewLine;
            }

            if (networks.Count == 0)
            {
                networkListTextBox.Text = "Add some networks to get started!";
            }
        }

        void RefreshForm(object sender, EventArgs e)
        {
            // Clear list boxes
            addModelBrandListBox.Items.Clear(); updateModelBrandListBox.Items.Clear();
            deleteModelBrandListBox.Items.Clear(); updateBrandListBox.Items.Clear();
            deleteBrandListBox.Items.Clear();

            updateModelListBox.Items.Clear(); deleteModelListBox.Items.Clear();
            updateNetworkListBox.Items.Clear(); deleteNetworkListBox.Items.Clear();

            // Clear lists
            phoneNames.Clear(); phoneBrands.Clear(); upcs.Clear();
            brands.Clear(); networks.Clear();

            // Restart form
            ModelsMgmtForm_Load(sender, e);
        }

        #endregion

        #region Listing Phones

        // Checks if new name is already in use in system
        bool CheckForMatches(string newName)
        {
            bool isMatch = false;

            foreach (string str in phoneNames)
            {
                if (newName == str)
                {
                    isMatch = true;
                }
            }

            foreach (string str in phoneBrands)
            {
                if (newName == str)
                {
                    isMatch = true;
                }
            }

            foreach (string str in networks)
            {
                if (newName == str)
                {
                    isMatch = true;
                }
            }

            return isMatch;
        }

        // Configures read-only text boxes
        // modelListTextBox, brandListTextBox, and networkListTextBox
        void ConfigureTextBox(System.Windows.Forms.TextBox textBox)
        {
            // Text
            textBox.Multiline = true;
            textBox.Font = new Font("Courier New", 8, FontStyle.Bold);
            textBox.WordWrap = true;
            textBox.ReadOnly = true;

            // Visual
            textBox.Visible = true;
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Location = new Point(293, 29);
            textBox.Size = new Size(151, 227);
        }

        // Loads phones in the selected brand on modelListLabel in "Add Model" tab
        // As this lists both phone names and UPCs in a label it requires a special sorting
        // algorithm and cannot use SortListBoxes
        void ListAddPhones(string brand)
        {
            // Configure modelListTextBox
            addModelTabPage.Controls.Add(modelListTextBox);
            ConfigureTextBox(modelListTextBox);

            // Clear previous results
            modelListTextBox.Text = "";

            // Lists for phones and UPCs
            List<Tuple<string, long>> brandPhones = new List<Tuple<string, long>>();

            // Check each phone in the system that is under the specified brand
            for (int i = 0; i < phoneNames.Count; i++)
            {
                // Add phone models under specified brand
                if (phoneBrands[i] == brand)
                {
                    // Check if phone model is already listed
                    if (!modelListTextBox.Text.Contains(phoneNames[i] + "\n"))
                    {
                        // Add to list that will be sorted
                        brandPhones.Add(new Tuple<string, long>(phoneNames[i], upcs[i]));
                    }
                }
            }

            // Sort list alphabetically while keeping them parallel
            brandPhones.Sort();

            // Add sorted lists to modelListLabel
            for (int i = 0; i < brandPhones.Count; i++)
            {
                modelListTextBox.Text += brandPhones[i].Item1 + Environment.NewLine;
                modelListTextBox.Text += "     " + brandPhones[i].Item2 + Environment.NewLine;
            }
        }

        // Loads phones in the selected brand on separate ListBox
        // modelListBox1 and modelListBox2
        void ListSearchPhones(string brand, ListBox listBox)
        {
            // Clear previous results
            listBox.Items.Clear();

            for (int i = 0; i < phoneNames.Count; i++)
            {
                // Add phone models under specified brand
                if (phoneBrands[i] == brand)
                {
                    // Check if phone model is already listed
                    if (!listBox.Items.Contains(phoneNames[i]))
                    {
                        listBox.Items.Add(phoneNames[i]);
                    }
                }
            }

            // Sort items alphabetically
            phoneForm.SortListBox(listBox);
        }

        #endregion

        #region Click EventHandlers
        // Ordered by tab page instead of alphabetically like other regions

        // (1 "Add Model" tab)
        // Enters new model created by user
        private void enterModelButton_Click(object sender, EventArgs e)
        {
            // Declare variables
            string modelBrand;
            string modelName;
            long modelUPC;

            if (brandLabel.Text != "" && newModelNameTextBox.Text != "" && newModelUPCTextBox.Text != "")
            {
                modelBrand = brandLabel.Text;
                modelName = newModelNameTextBox.Text;

                bool isMatch = CheckForMatches(modelName);

                if (isMatch == false)
                {
                    if (long.TryParse(newModelUPCTextBox.Text, out modelUPC))
                    {
                        if (newModelUPCTextBox.Text.Length == Phone.UPC_LENGTH)
                        {
                            // Get file and create variables to read the contents
                            var fileName = @"files\UPC_Organizer.txt";
                            List<string> lines = File.ReadAllLines(fileName).ToList();

                            FileInfo fi = new FileInfo(fileName);
                            FileStream fs = fi.Open(FileMode.Open, FileAccess.ReadWrite);
                            StreamReader sr = new StreamReader(fs);

                            string brandLine = "-----" + modelBrand + "-----";
                            string line;
                            int i = 1;              // Loop counter

                            // Find line with brand name
                            while ((line = sr.ReadLine()) != null)
                            {
                                if (line == brandLine)
                                {
                                    break;
                                }
                                i++;
                            }

                            // Close StreamReader and FileStream to free up text file
                            sr.Close(); fs.Close();

                            // Insert new model and UPC at appropriate spot
                            lines.Insert(i, modelName); i++;
                            lines.Insert(i, modelUPC.ToString());

                            // Rewrite UPC_Organizer.txt with new phone data
                            File.WriteAllLines(fi.FullName, lines.ToArray());

                            // Clear text boxes and change focus to brandListBox1
                            newModelNameTextBox.Clear();
                            newModelUPCTextBox.Clear();
                            modelListTextBox.Text = "";
                            if (addModelBrandListBox.Items.Count > 0)
                            {
                                addModelBrandListBox.SelectedIndex = 0;
                                addModelBrandListBox.Focus();
                            }

                            // Refreshes form with updated info
                            RefreshForm(sender, e);
                        }
                        else
                        {
                            MessageBox.Show("UPC must be a " + Phone.UPC_LENGTH.ToString() +
                                "-digit number.", "Error");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Enter a valid UPC.", "Error");
                    }
                }
                else
                {
                    MessageBox.Show("Name is already in use.", "Error");
                }
            }
            else
            {
                if (brandLabel.Text == "")
                {
                    MessageBox.Show("Select a brand.", "Error");
                }
                else if (newModelNameTextBox.Text == "")
                {
                    MessageBox.Show("Add a model name.", "Error");
                }
                else
                {
                    MessageBox.Show("Add a UPC.", "Error");
                }
            }
        }


        // (2 "Update Model" tab)
        // Updates selected model information
        private void updateModelButton_Click(object sender, EventArgs e)
        {
            // Declare variables
            string modelName;
            long oldModelUPC = 0;
            long newModelUPC;

            if (brandLabel2.Text != "" && updateModelNameTextBox.Text != "" && updateModelUPCTextBox.Text != "")
            {
                modelName = updateModelNameTextBox.Text;

                if (long.TryParse(updateModelUPCTextBox.Text, out newModelUPC))
                {
                    if (updateModelUPCTextBox.Text.Length == Phone.UPC_LENGTH)
                    {
                        if (updateModelListBox.SelectedIndex != -1)
                        {
                            for (int j = 0; j < phoneNames.Count; j++)
                            {
                                if (phoneNames[j] == updateModelListBox.SelectedItem.ToString())
                                {
                                    oldModelUPC = upcs[j];
                                    break;
                                }
                            }

                            if (modelName != updateModelListBox.SelectedItem.ToString()
                                || newModelUPC != oldModelUPC)
                            {
                                // Get file and create variables to read the contents
                                var fileName = @"files\UPC_Organizer.txt";
                                List<string> lines = File.ReadAllLines(fileName).ToList();

                                FileInfo fi = new FileInfo(@"files\UPC_Organizer.txt");
                                FileStream fs = fi.Open(FileMode.Open, FileAccess.ReadWrite);
                                StreamReader sr = new StreamReader(fs);

                                string oldModelName = updateModelListBox.SelectedItem.ToString();
                                string newModelName = updateModelNameTextBox.Text;
                                string line;
                                int i = 0;              // Loop counter

                                // Find line with brand name
                                while ((line = sr.ReadLine()) != null)
                                {
                                    if (line == oldModelName)
                                    {
                                        // Remove old phone name and UPC
                                        lines.RemoveAt(i);
                                        lines.RemoveAt(i);
                                        break;
                                    }
                                    i++;
                                }

                                // Close StreamReader and FileStream to free up text file
                                sr.Close(); fs.Close();

                                // Insert renamed model and UPC at appropriate spot
                                lines.Insert(i, newModelName); i++;
                                lines.Insert(i, newModelUPC.ToString());

                                // Rewrite UPC_Organizer.txt with new phone data
                                File.WriteAllLines(fi.FullName, lines.ToArray());

                                // Clear text boxes and change focus to brandListBox2
                                updateModelNameTextBox.Clear();
                                updateModelUPCTextBox.Clear();
                                updateModelBrandListBox.SelectedIndex = 0;
                                updateModelListBox.SelectedIndex = 0;
                                updateModelBrandListBox.Focus();

                                RefreshForm(sender, e);
                            }
                            else
                            {
                                MessageBox.Show("To update a phone model, change either the name or UPC.", "Error");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Select a model on the right. To add a new model, use the \"Add Model\" tab.", "Error");
                        }
                    }
                    else
                    {
                        MessageBox.Show("UPC must be a " + Phone.UPC_LENGTH.ToString() +
                           "-digit number.", "Error");
                    }
                }
                else
                {
                    MessageBox.Show("Enter a valid UPC.", "Error");
                }
            }
            else
            {
                if (brandLabel2.Text == "")
                {
                    MessageBox.Show("Select a brand.", "Error");
                }
                else if (updateModelNameTextBox.Text == "")
                {
                    MessageBox.Show("Add a model name.", "Error");
                }
                else
                {
                    MessageBox.Show("Add a UPC.", "Error");
                }
            }
        }


        // (3 "Delete Model" tab)
        // Deletes selected phone model
        private void deleteModelButton_Click(object sender, EventArgs e)
        {
            if (deleteModelLabel.Text != "")
            {
                if (deleteModelListBox.Items.Count > 1)
                {
                    string name = deleteModelLabel.Text;

                    // Execute this code on each network
                    foreach (string network in networks)
                    {
                        // Get network inventory
                        fi = new FileInfo(@"files\Network_Inventories.txt");

                        // Create list to hold lines
                        List<string> lines = File.ReadAllLines(fi.FullName).ToList();

                        // Check if inventory has phone
                        if (lines.Contains(name))
                        {
                            // Find line with phone name
                            for (int i = 0; i < lines.Count; i++)
                            {
                                while (lines[i] == name)
                                {
                                    // Execute when name is found
                                    if (lines[i] == name)
                                    {
                                        // Delete model and related information
                                        for (int j = 0; j < 5; j++)
                                        {
                                            lines.RemoveAt(i);
                                        }
                                    }

                                    // End loop if no more phones are in inventory
                                    if (lines.Count <= 1)
                                    {
                                        break;
                                    }
                                }

                                // Check if inventory has another of the same phone
                                List<string> lineCheck = File.ReadAllLines(fi.FullName).ToList();

                                if (!lineCheck.Contains(name))
                                {
                                    break;
                                }
                            }
                        }

                        // Write updated inventory list to file
                        File.WriteAllLines(fi.FullName, lines.ToArray());
                    }

                    // Get UPC_Organizer.txt
                    FileInfo fi2 = new FileInfo(@"files\UPC_Organizer.txt");

                    // Create list to hold lines
                    List<string> lines2 = File.ReadAllLines(fi2.FullName).ToList();

                    // Find line with phone name
                    for (int i = 0; i < lines2.Count; i++)
                    {
                        // Execute when name is found
                        if (lines2[i] == name)
                        {
                            // Remove phone model and UPC
                            lines2.RemoveAt(i);
                            lines2.RemoveAt(i);
                        }
                    }

                    // Write updated inventory list to file
                    File.WriteAllLines(fi2.FullName, lines2.ToArray());

                    RefreshForm(sender, e);
                }
                else
                {
                    MessageBox.Show("The " + deleteModelLabel.Text
                        + " is the only model under the " +
                        brandLabel.Text + " brand. Remove the brand instead.", "Error");
                }
            }
            else
            {
                MessageBox.Show("Select a model.", "Error");
            }
        }


        // (4 "Add Brand" tab)
        // Enters new brand created by user
        private void enterBrandButton_Click(object sender, EventArgs e)
        {
            if (newBrandTextBox.Text != "")
            {
                if (modelNameTextBox.Text != "")
                {
                    if (modelUPCTextBox.Text != "")
                    {
                        if (modelUPCTextBox.Text.Length == Phone.UPC_LENGTH)
                        {
                            // Variable to hold new brand name
                            string brand = newBrandTextBox.Text;
                            string model = modelNameTextBox.Text;
                            long upc = long.Parse(modelUPCTextBox.Text);

                            bool isMatchBrand = CheckForMatches(brand);
                            bool isMatchModel = CheckForMatches(model);

                            if (isMatchBrand == false && isMatchModel == false)
                            {
                                // Get file and create variables to read the contents
                                // Index is where new information will be inserted
                                var fileName = @"files\UPC_Organizer.txt";
                                List<string> lines = File.ReadAllLines(fileName).ToList();
                                int index = -1;

                                FileInfo fi = new FileInfo(fileName);
                                FileStream fs = fi.Open(FileMode.Open, FileAccess.ReadWrite);
                                StreamReader sr = new StreamReader(fs);

                                for (int i = 0; i < lines.Count; i++)
                                {
                                    if (lines[i] == "***End of UPCs***")
                                    {
                                        index = i;
                                        break;
                                    }
                                }

                                if (index != -1)
                                {
                                    // Add brand to UPC_Organizer.txt
                                    string brandLine = "-----" + brand + "-----";
                                    lines.Insert(index, brandLine);

                                    // Close StreamReader and FileStream to free up text file
                                    sr.Close(); fs.Close();

                                    // Rewrite UPC_Organizer.txt with new phone data
                                    File.WriteAllLines(fi.FullName, lines.ToArray());

                                    // Move information to Add Model tab to prevent errors
                                    brandLabel.Text = brand;
                                    newModelNameTextBox.Text = model;
                                    newModelUPCTextBox.Text = upc.ToString();

                                    // Add new phone model
                                    enterModelButton_Click(sender, e);

                                    // Clear text boxes and change focus to New Brand Name
                                    newBrandTextBox.Clear();
                                    modelNameTextBox.Clear();
                                    modelUPCTextBox.Clear();
                                    newBrandTextBox.Focus();
                                }
                                else
                                {
                                    MessageBox.Show("Unable to add new brand. Check UPC_Organizer.txt" +
                                        " for \"***End of UPCs***\" line.", "Error");
                                }
                            }
                            else
                            {
                                if (isMatchBrand == true)
                                {
                                    MessageBox.Show("Brand name is already in use.", "Error");
                                }
                                else
                                {
                                    MessageBox.Show("Model name is already in use.", "Error");
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("UPC must be a " + Phone.UPC_LENGTH.ToString() +
                                "-digit number.", "Error");
                        }
                    }
                    else
                    {
                        MessageBox.Show("Enter a new model UPC. This is required when adding a new brand.", "Error");
                    }
                }
                else
                {
                    MessageBox.Show("Enter a new model name. This is required when adding a new brand.", "Error");
                }
            }
            else
            {
                MessageBox.Show("Enter a brand name.", "Error");
            }
        }


        // (5 "Update Brand" tab)
        // Updates selected brand information
        private void updateBrandButton_Click(object sender, EventArgs e)
        {
            if (updateBrandListBox.SelectedIndex != -1)
            {
                if (updateBrandNameTextBox.Text != "")
                {
                    // Variable to hold new brand name
                    string brand = updateBrandNameTextBox.Text;

                    if (brand != updateBrandListBox.SelectedItem.ToString())
                    {
                        // Get file and create variables to read the contents
                        var fileName = @"files\UPC_Organizer.txt";
                        List<string> lines = File.ReadAllLines(fileName).ToList();

                        FileInfo fi = new FileInfo(fileName);
                        FileStream fs = fi.Open(FileMode.Open, FileAccess.ReadWrite);
                        StreamReader sr = new StreamReader(fs);

                        string oldBrandName = updateBrandListBox.SelectedItem.ToString();
                        string newBrandName = updateBrandNameTextBox.Text;
                        string line;
                        int i = 0;              // Loop counter

                        // Find line with brand name
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.Contains(oldBrandName))
                            {
                                // Create new brand line
                                line = "-----" + newBrandName + "-----";
                                break;
                            }
                            i++;
                        }

                        // Replace old brand name with new one
                        for (int j = 0; j < lines.Count; j++)
                        {
                            if (lines[j].Contains(oldBrandName))
                            {
                                lines[j] = line;
                                break;
                            }
                        }

                        // Close StreamReader and FileStream to free up text file
                        sr.Close(); fs.Close();

                        // Rewrite UPC_Organizer.txt with new phone data
                        File.WriteAllLines(fi.FullName, lines.ToArray());

                        // Clear text box
                        updateBrandNameTextBox.Clear();
                        updateBrandListBox.SelectedIndex = 0;

                        RefreshForm(sender, e);
                    }
                    else
                    {
                        MessageBox.Show("New name must be different from old name.", "Error");
                    }
                }
                else
                {
                    MessageBox.Show("Enter a new brand name.", "Error");
                }
            }
            else
            {
                MessageBox.Show("Select a brand on the right.", "Error");
            }
        }


        // (6 "Delete Brand" tab)
        // Deletes selected phone brand
        private void deleteBrandButton_Click(object sender, EventArgs e)
        {
            if (deleteBrandLabel.Text != "")
            {
                //if (brandListBox5.Items.Count > 1)
                //{
                string brand = deleteBrandLabel.Text;

                // Execute this code on each network
                foreach (string network in networks)
                {
                    // Get network inventory
                    fi = new FileInfo(@"files\Network_Inventories.txt");

                    // Create list to hold lines
                    List<string> lines = File.ReadAllLines(fi.FullName).ToList();

                    // Current line
                    string line;

                    // Find line with brand name
                    for (int i = 0; i < lines.Count; i++)
                    {
                        line = lines[i];

                        if (line == brand)
                        {
                            i -= 1;
                            lines.RemoveAt(i);
                            lines.RemoveAt(i);
                            lines.RemoveAt(i);
                            lines.RemoveAt(i);
                            lines.RemoveAt(i);
                        }

                        // Check if file contains any more phones under the deleted brand
                        List<string> linesCheck = File.ReadAllLines(fi.FullName).ToList();

                        if (!linesCheck.Contains(brand))
                        {
                            break;
                        }
                    }

                    // Write updated inventory list to file
                    File.WriteAllLines(fi.FullName, lines.ToArray());
                }

                // Get UPC_Organizer.txt
                FileInfo fi2 = new FileInfo(@"files\UPC_Organizer.txt");

                // Create list to hold lines
                List<string> lines2 = File.ReadAllLines(fi2.FullName).ToList();

                // Find line with brand name
                for (int i = 0; i < lines2.Count; i++)
                {
                    // Current line
                    string line = lines2[i];

                    // Execute when name is found
                    if (lines2[i].Contains(brand))
                    {
                        // Remove brand name
                        lines2.RemoveAt(i);

                        // Remove all lines until next brand is reached
                        while (!lines2[i].Contains("---"))
                        {
                            lines2.RemoveAt(i);
                            if (i == lines2.Count)
                            {
                                break;
                            }
                        }

                        break;
                    }
                }

                // Write updated inventory list to file
                File.WriteAllLines(fi2.FullName, lines2.ToArray());

                deleteBrandLabel.Text = "";

                RefreshForm(sender, e);
            }
            else
            {
                MessageBox.Show("Select a brand.", "Error");
            }
        }


        // (7 "Add Network" tab)
        // Enters new network created by user
        private void enterNetworkButton_Click(object sender, EventArgs e)
        {
            if (newNetworkTextBox.Text != "")
            {
                // Get new network name
                string networkName = newNetworkTextBox.Text;

                bool isMatch = CheckForMatches(networkName);

                if (isMatch == false)
                {
                    // Update NetworkInventories.txt
                    FileInfo fi = new FileInfo(@"files\Network_Inventories.txt");

                    // Get file contents
                    List<string> lines1 = File.ReadAllLines(fi.FullName).ToList();

                    // Add network inventory lines
                    lines1.Add("");
                    lines1.Add("|---" + networkName.ToUpper() + "---|");
                    lines1.Add("---Phone Inventory---");
                    lines1.Add("|End of Inventory|");
                    lines1.Add("***************************");

                    // Rewrite file contents
                    File.WriteAllLines(fi.FullName, lines1.ToArray());

                    // Update UPC_Organizer.txt
                    fi = new FileInfo(@"files\UPC_Organizer.txt");

                    // Update network list on UPC_Organizer.txt
                    // Get file contents
                    List<string> lines2 = File.ReadAllLines(fi.FullName).ToList();

                    // Add network to list of networks at bottom
                    lines2[lines2.Count - 1] = networkName;

                    // Add "***End of Networks***" to end of document
                    lines2.Add("***End of Networks***");

                    // Rewrite file contents
                    File.WriteAllLines(fi.FullName, lines2.ToArray());

                    // Reset textbox
                    newNetworkTextBox.Clear();
                    newNetworkTextBox.Focus();
                }
                else
                {
                    MessageBox.Show("Network name is already in use.", "Error");
                }
            }
            else
            {
                MessageBox.Show("Enter a network name.", "Error");
            }
        }


        // (8 "Update Network" tab)
        // Updates selected network information
        private void updateNetworkButton_Click(object sender, EventArgs e)
        {
            if (updateNetworkListBox.SelectedIndex != -1)
            {
                if (updateNetworkNameTextBox.Text != "")
                {
                    // Variable to hold new network name
                    string network = updateNetworkNameTextBox.Text;

                    if (network != updateNetworkListBox.SelectedItem.ToString())
                    {
                        // Get file and create variables to read the contents
                        var fileName = @"files\UPC_Organizer.txt";
                        List<string> lines1 = File.ReadAllLines(fileName).ToList();

                        FileInfo fi = new FileInfo(fileName);
                        FileStream fs = fi.Open(FileMode.Open, FileAccess.ReadWrite);
                        StreamReader sr = new StreamReader(fs);

                        string oldNetworkName = updateNetworkListBox.SelectedItem.ToString();
                        string newNetworkName = updateNetworkNameTextBox.Text;
                        string line;
                        int i = 0;              // Loop counter

                        // Find line with network name
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.Contains(oldNetworkName))
                            {
                                // Create new network line
                                line = newNetworkName;
                                break;
                            }
                            i++;
                        }

                        // Replace old network name with new one
                        for (int j = 0; j < lines1.Count; j++)
                        {
                            if (lines1[j].Contains(oldNetworkName))
                            {
                                lines1[j] = line;
                                break;
                            }
                        }
                        // Close StreamReader and FileStream to free up text file
                        sr.Close(); fs.Close();

                        // Rewrite UPC_Organizer.txt with new phone data
                        File.WriteAllLines(fi.FullName, lines1.ToArray());

                        // Update Network_Inventories.txt
                        fi = new FileInfo(@"files\Network_Inventories.txt");

                        List<string> lines2 = File.ReadAllLines(fi.FullName).ToList();

                        for (int j = 0; j < lines2.Count; j++)
                        {
                            string str = lines2[j];

                            if (str.Contains(oldNetworkName.ToUpper()))
                            {
                                string newStr = "|---" + newNetworkName.ToUpper() + "---|";
                                lines2[j] = newStr;
                                break;
                            }
                        }

                        // Rewrite file content
                        File.WriteAllLines(fi.FullName, lines2.ToArray());

                        // Clear text box
                        updateNetworkNameTextBox.Clear();
                        updateNetworkListBox.SelectedIndex = 0;

                        RefreshForm(sender, e);
                    }
                    else
                    {
                        MessageBox.Show("New name must be different from old name.", "Error");
                    }
                }
                else
                {
                    MessageBox.Show("Enter a new network name.", "Error");
                }
            }
            else
            {
                MessageBox.Show("Select a network on the right.", "Error");
            }
        }


        // (9 "Delete Network" tab)
        // Deletes selected phone network
        private void deleteNetworkButton_Click(object sender, EventArgs e)
        {
            if (deleteNetworkLabel.Text != "")
            {
                //if (networkListBox5.Items.Count > 1)
                //{
                string network = deleteNetworkLabel.Text;

                // Get UPC_Organizer.txt
                fi = new FileInfo(@"files\UPC_Organizer.txt");

                // Create list to hold lines
                List<string> lines1 = File.ReadAllLines(fi.FullName).ToList();

                // Find line with network name
                for (int i = 0; i < lines1.Count; i++)
                {
                    if (lines1[i] == network)
                    {
                        lines1.RemoveAt(i);
                    }
                }

                // Write updated inventory list to file
                File.WriteAllLines(fi.FullName, lines1.ToArray());

                // Remove network inventory
                fi = new FileInfo(@"files\Network_Inventories.txt");

                // Get file contents and phone count
                List<string> lines2 = File.ReadAllLines(fi.FullName).ToList();
                List<string> removeStrings = new List<string>();

                // Search for network inventory
                for (int i = 0; i < lines2.Count; i++)
                {
                    if (lines2[i] == "|---" + network.ToUpper() + "---|")
                    {
                        // Get lines to remove
                        while (lines2[i] != "***************************")
                        {
                            removeStrings.Add(lines2[i]);
                            i++;
                        }

                        // Remove "*" divider and extra line (if applicable)
                        removeStrings.Add(lines2[i + 1]);
                        if (i + 1 > lines2.Count)
                        {
                            removeStrings.Add(lines2[i + 2]);
                        }
                        break;
                    }

                    // Exit loop if inventory was found
                    if (removeStrings.Count > 0)
                    {
                        break;
                    }
                }

                // Remove selected lines from list
                bool found = false;

                foreach (string str in removeStrings)
                {
                    for (int i = 0; i < lines2.Count; i++)
                    {
                        if (lines2[i] == "|---" + network.ToUpper() + "---|")
                        {
                            found = true;
                        }

                        if (lines2[i] == str && found == true)
                        {
                            lines2.RemoveAt(i);
                            break;
                        }
                    }

                    // Reset boolean for next iteration
                    if (found == true)
                    {
                        found = false;
                    }
                }

                // Rewrite file content
                File.WriteAllLines(fi.FullName, lines2.ToArray());

                deleteNetworkLabel.Text = "";

                RefreshForm(sender, e);
            }
            else
            {
                MessageBox.Show("Select a network.", "Error");
            }
        }


        // Exits the form
        private void exitButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region SelectedIndexChanged

        // 1 "Add Model" tab
        private void addModelBrandListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (addModelBrandListBox.SelectedIndex != -1)
            {
                ListAddPhones(addModelBrandListBox.SelectedItem.ToString());
                brandLabel.Text = addModelBrandListBox.SelectedItem.ToString();
            }
        }


        // 2 "Update Model" tab brands
        private void updateModelBrandListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updateModelBrandListBox.SelectedIndex != -1)
            {
                ListSearchPhones(updateModelBrandListBox.SelectedItem.ToString(), updateModelListBox);
                brandLabel2.Text = updateModelBrandListBox.SelectedItem.ToString();

                // Clear previous phone data (if applicable)
                updateModelNameTextBox.Clear();
                updateModelUPCTextBox.Clear();
            }
        }

        // 2 "Update Model" tab models
        private void updateModelListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updateModelListBox.SelectedIndex != -1)
            {
                string selection = updateModelListBox.SelectedItem.ToString();
                int index = 0;

                for (int i = 0; i < phoneNames.Count; i++)
                {
                    if (selection == phoneNames[i].ToString())
                    {
                        index = i;
                        break;
                    }
                }

                // Get phone UPC and display info
                long upc = upcs[index];

                updateModelNameTextBox.Text = updateModelListBox.SelectedItem.ToString();
                updateModelUPCTextBox.Text = upc.ToString();
            }
        }


        // 3 "Delete Model" tab brands
        private void deleteModelBrandListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (deleteModelBrandListBox.SelectedIndex != -1)
            {
                ListSearchPhones(deleteModelBrandListBox.SelectedItem.ToString(), deleteModelListBox);
                deleteModelBrandLabel.Text = deleteModelBrandListBox.SelectedItem.ToString();

                // Clear previous phone data (if applicable)
                deleteModelLabel.Text = "";
                deleteUPCLabel.Text = "";
            }
        }

        // 3 "Delete Model" tab models
        private void deleteModelListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (deleteModelListBox.SelectedIndex != -1)
            {
                string selection = deleteModelListBox.SelectedItem.ToString();
                int index = 0;

                for (int i = 0; i < phoneNames.Count; i++)
                {
                    if (selection == phoneNames[i].ToString())
                    {
                        index = i;
                        break;
                    }
                }

                // Get phone UPC and display info
                long upc = upcs[index];

                deleteModelLabel.Text = deleteModelListBox.SelectedItem.ToString();
                deleteUPCLabel.Text = upc.ToString();
            }
        }


        // 5 "Update Brand" tab
        private void updateBrandListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updateBrandListBox.SelectedIndex != -1)
            {
                updateBrandNameTextBox.Text = updateBrandListBox.SelectedItem.ToString();
            }
        }


        // 6 "Delete Brand" tab
        private void deleteBrandListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (deleteBrandListBox.SelectedIndex != -1)
            {
                deleteBrandLabel.Text = deleteBrandListBox.SelectedItem.ToString();
            }
        }


        // 8 "Update Network" tab
        private void updateNetworkListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (updateNetworkListBox.SelectedIndex != -1)
            {
                updateNetworkNameTextBox.Text = updateNetworkListBox.SelectedItem.ToString();
            }
        }


        // 9 "Delete Network" tab
        private void deleteNetworkListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (deleteNetworkListBox.SelectedIndex != -1)
            {
                deleteNetworkLabel.Text = deleteNetworkListBox.SelectedItem.ToString();
            }
        }

        #endregion

        #endregion
    }
}
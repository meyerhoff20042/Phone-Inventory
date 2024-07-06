using System;

namespace ElectronicsInventory
{
    public class Phone
    {
        // Fields
        private string _name;
        private string _brand;
        private decimal _price;
        public long IMEI;               // IMEI needs to be public so phone's IMEI is
                                        // not regenerated each time info is loaded on the form

        public const int IMEI_LENGTH = 15;
        public const int UPC_LENGTH = 12;

        #region Properties
        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }

        public string Brand
        {
            get
            {
                return _brand;
            }
            set
            {
                _brand = value;
            }
        }

        public decimal Price
        {
            get
            {
                return _price;
            }
            set
            {
                _price = value;
            }
        }

        public long _imei(long IMEI)
        {
            // Random IMEI generator
            Random rand = new Random();

            // IMEI must be 15 digits
            // Create three 5-digit random numbers
            int int1 = rand.Next(10000, 99999);
            int int2 = rand.Next(10000, 99999);
            int int3 = rand.Next(10000, 99999);

            // Combine the three numbers to form a 15-digit number
            string str = int1.ToString() + int2.ToString() + int3.ToString();

            IMEI = long.Parse(str);

            return IMEI;
        }

        #endregion

        // Methods

        // Constructors
        public Phone(string name, string brand, decimal price)
        {
            Name = name;
            Brand = brand;
            Price = price;
            IMEI = _imei(IMEI);
        }
    }
}
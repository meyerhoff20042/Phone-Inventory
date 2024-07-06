using System;

namespace ElectronicsInventory
{
    internal class Television
    {
        // Fields
        private int _size;
        private string _brand;
        private decimal _cost;

        #region Properties
        public int Size
        {
            get
            {
                return _size;
            }
            set
            {
                _size = value;
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

        public decimal Cost
        {
            get
            {
                return _cost;
            }
            set
            {
                _cost = value;
            }
        }
        #endregion

        // Methods

        // Constructors
        public Television (int size, string brand, decimal cost)
        {
            Size = size;
            Brand = brand;
            Cost = cost;
        }
    }
}

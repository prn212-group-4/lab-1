using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BusinessObjects
{
    public partial class Product : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));

        public int ProductId { get; set; }

        private string _productName;
        public string ProductName
        {
            get => _productName;
            set { if (_productName != value) { _productName = value; OnPropertyChanged(); } }
        }

        private int? _categoryId;
        public int? CategoryId
        {
            get => _categoryId;
            set { if (_categoryId != value) { _categoryId = value; OnPropertyChanged(); } }
        }

        private short? _unitsInStock;
        public short? UnitsInStock
        {
            get => _unitsInStock;
            set { if (_unitsInStock != value) { _unitsInStock = value; OnPropertyChanged(); } }
        }

        private decimal? _unitPrice;
        public decimal? UnitPrice
        {
            get => _unitPrice;
            set { if (_unitPrice != value) { _unitPrice = value; OnPropertyChanged(); } }
        }

        private Category _category;
        public virtual Category Category
        {
            get => _category;
            set { if (_category != value) { _category = value; OnPropertyChanged(); } }
        }

        public string CategoryName { get; set; }

        public Product() { }

        public Product(int id, string name, int catId, short unitInStock, decimal price)
        {
            ProductId = id;
            ProductName = name;
            CategoryId = catId;
            UnitsInStock = unitInStock;
            UnitPrice = price;
        }
    }
}

using BusinessObjects;
using System.ComponentModel;
using System.Runtime.CompilerServices;

public partial class Product : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private string productName;
    public string ProductName
    {
        get => productName;
        set
        {
            if (productName != value) { productName = value; OnPropertyChanged(); }
        }
    }

    private int? categoryId;
    public int? CategoryId
    {
        get => categoryId;
        set
        {
            if (categoryId != value) { categoryId = value; OnPropertyChanged(); }
        }
    }

    private short? unitsInStock;
    public short? UnitsInStock
    {
        get => unitsInStock;
        set
        {
            if (unitsInStock != value) { unitsInStock = value; OnPropertyChanged(); }
        }
    }

    private decimal? unitPrice;
    public decimal? UnitPrice
    {
        get => unitPrice;
        set
        {
            if (unitPrice != value) { unitPrice = value; OnPropertyChanged(); }
        }
    }

    private Category category;
    public virtual Category Category
    {
        get => category;
        set
        {
            if (category != value) { category = value; OnPropertyChanged(); }
        }
    }
}

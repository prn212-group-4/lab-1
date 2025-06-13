using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using BusinessObjects;
using Services;

namespace WPFApp
{
    /// <summary> Interaction logic for MainWindow.xaml
    public partial class MainWindow : Window
    {
        private readonly IProductService iProductService;
        private readonly ICategoryService iCategoryService;
        private ObservableCollection<Product> ProductList = new ObservableCollection<Product>();

        public MainWindow()
        {
            InitializeComponent();
            iProductService = new ProductService();
            iCategoryService = new CategoryService();
            dgData.ItemsSource = ProductList;
            LoadProductList();
            LoadCategoryList();
        }

        public void LoadCategoryList()
        {
            try
            {
                var catList = iCategoryService.GetCategories();
                cboCategory.ItemsSource = catList;
                cboCategory.DisplayMemberPath = "CategoryName";
                cboCategory.SelectedValuePath = "CategoryId";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error on load list of categories");
            }
        }

        public void LoadProductList()
        {
            try
            {
                var products = iProductService.GetProducts();
                var categories = iCategoryService.GetCategories();

                ProductList.Clear();

                foreach (var p in products)
                {
                    p.Category = categories.FirstOrDefault(c => c.CategoryId == p.CategoryId);
                    ProductList.Add(p);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error on load list of products");
            }
            finally
            {
                resetInput();
            }
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCategoryList();
            LoadProductList();
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show("Please enter a product name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtProductName.Focus();
                return;
            }
            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Please enter a valid non-negative price.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPrice.Focus();
                return;
            }
            if (!short.TryParse(txtUnitsInStock.Text, out short units) || units < 0)
            {
                MessageBox.Show("Please enter a valid non-negative stock quantity.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUnitsInStock.Focus();
                return;
            }
            if (cboCategory.SelectedValue == null
                || !int.TryParse(cboCategory.SelectedValue.ToString(), out int catId))
            {
                MessageBox.Show("Please select a category.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                cboCategory.Focus();
                return;
            }
            var product = new Product
            {
                ProductName = txtProductName.Text.Trim(),
                UnitPrice = price,
                UnitsInStock = units,
                CategoryId = catId
            };

            try
            {
                iProductService.SaveProduct(product);
                product.Category = iCategoryService
                    .GetCategories()
                    .FirstOrDefault(c => c.CategoryId == product.CategoryId);
                ProductList.Add(product);
                MessageBox.Show("Product created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                resetInput();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating product:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnDump_Click(object sender, RoutedEventArgs e)
        {
            var products = iProductService.GetProducts();
            foreach (var p in products)
                Debug.WriteLine($"ID={p.ProductId}, Name={p.ProductName}, CatID={p.CategoryId}, Stock={p.UnitsInStock}, Price={p.UnitPrice}");
            MessageBox.Show("Products dumped to Debug output.");
        }

        private void dgData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (dgData.SelectedItems == null) return;
                DataGrid dataGrid = sender as DataGrid;
                DataGridRow row = (DataGridRow)dataGrid.ItemContainerGenerator.ContainerFromIndex(dataGrid.SelectedIndex);
                if (row == null) return;
                var cellContent = dataGrid.Columns[0].GetCellContent(row);
                if (cellContent == null) return;

                var textBlock = cellContent as TextBlock;
                if (textBlock == null) return;

                string id = textBlock.Text;
                if (!int.TryParse(id, out int productId)) return;

                Product product = iProductService.GetProductById(productId);
                if (product == null) return;

                txtProductID.Text = product.ProductId.ToString();
                txtProductName.Text = product.ProductName;
                txtPrice.Text = product.UnitPrice.ToString();
                txtUnitsInStock.Text = product.UnitsInStock.ToString();
                cboCategory.SelectedValue = product.CategoryId;
            }
            catch (Exception)
            {
                MessageBox.Show("Cannot choose this row !");
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtProductID.Text, out int id))
            {
                MessageBox.Show("You must select a Product!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var product = ProductList.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                MessageBox.Show("Product not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Cập nhật dữ liệu từ UI
            product.ProductName = txtProductName.Text.Trim();
            product.UnitPrice = decimal.Parse(txtPrice.Text);
            product.UnitsInStock = short.Parse(txtUnitsInStock.Text);
            var newCatId = (int)cboCategory.SelectedValue;
            product.CategoryId = newCatId;

            // Lấy Category từ danh sách (có thể thay bằng list đã load sẵn)
            product.Category = iCategoryService
                .GetCategories()
                .FirstOrDefault(c => c.CategoryId == newCatId);

            try
            {
                iProductService.UpdateProduct(product);
                MessageBox.Show("Product updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                resetInput();
                // Không gọi LoadProductList() => UI tự refresh vì INotifyPropertyChanged
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating product:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtProductID.Text, out int id))
            {
                MessageBox.Show("You must select a Product!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var product = ProductList.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                MessageBox.Show("Product not found!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show($"Are you sure you want to delete '{product.ProductName}'?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                iProductService.DeleteProduct(product);
                ProductList.Remove(product);

                MessageBox.Show("Product deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                resetInput();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting product:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void resetInput()
        {
            txtProductID.Text = "";
            txtProductName.Text = "";
            txtPrice.Text = "";
            txtUnitsInStock.Text = "";
            cboCategory.SelectedValue = 0;
        }
    }
}

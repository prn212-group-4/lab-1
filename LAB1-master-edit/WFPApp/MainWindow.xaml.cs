using System;
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

        public MainWindow()
        {
            InitializeComponent();
            iProductService = new ProductService();
            iCategoryService = new CategoryService();
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
                foreach (var p in products)
                {
                    p.Category = categories
                                 .FirstOrDefault(c => c.CategoryId == p.CategoryId);
                }
                dgData.ItemsSource = products;
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
                MessageBox.Show("Product created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating product:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                LoadProductList();
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
            try
            {
                if (txtProductID.Text.Length > 0)
                {
                    Product product = new Product();
                    product.ProductId = Int32.Parse(txtProductID.Text);
                    product.ProductName = txtProductName.Text;
                    product.UnitPrice = Decimal.Parse(txtPrice.Text);
                    product.UnitsInStock = short.Parse(txtUnitsInStock.Text);
                    product.CategoryId = Int32.Parse(cboCategory.SelectedValue.ToString());
                    iProductService.UpdateProduct(product);
                }
                else
                {
                    MessageBox.Show("You must select a Product !");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                LoadProductList();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtProductID.Text.Length > 0)
                {
                    Product product = new Product();
                    product.ProductId = Int32.Parse(txtProductID.Text);
                    iProductService.DeleteProduct(product);
                }
                else
                {
                    MessageBox.Show("You must select a Product !");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                LoadProductList();
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

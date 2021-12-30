using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DeThi2.Models;

namespace DeThi2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private SALESMANAGEMENTContext conn;

        private bool isUpdate;
        public MainWindow()
        {
            conn = new SALESMANAGEMENTContext();
            InitializeComponent();
            HienThiCombobox();
        }

        private void HienThiCombobox()
        {
            var Category = from record in conn.Categories
                             select record;
            ComCategory.ItemsSource = Category.ToList();
            ComCategory.DisplayMemberPath = "CatName";
            ComCategory.SelectedValuePath = "CatId";
        }

        private void checkMode()
        {
            if(isUpdate)
            {
                txtProductID.IsEnabled = false;
            }
            else
            {
                txtProductID.IsEnabled = true;
                txtProductID.Text = "";
                txtProductName.Text = "";
                txtQuantity.Text = "";
                txtUnitPrice.Text = "";
            }
        }
        private void LoadDataGrid()
        {
            var query = from item in conn.Products
                        where item.Quantity <= 150
                        orderby item.ProductName descending
                        select new
                        {
                            ProductId = item.ProductId,
                            ProductName = item.ProductName,
                            CategoryId = item.CatId,
                            UnitPrice = item.UnitPrice,
                            Quantity = item.Quantity,
                            Amount = item.UnitPrice * item.Quantity
                        };
            gridData.ItemsSource = query.ToList();
        }

        private void btnAdd(object sender, RoutedEventArgs e)
        {
            if(isUpdate)
            {
                isUpdate = false;
                checkMode();
            }
            else
            {
                try
                {
                    if (validate())
                    {
                        var ProductInDB = (from item in conn.Products
                                           where item.ProductId.Equals(txtProductID.Text)
                                           select item).SingleOrDefault();
                        if (!(ProductInDB == null))
                        {
                            throw new Exception("Mã sản phẩm không được để trùng");
                        }

                        var sp = new Product();
                        sp.ProductId = txtProductID.Text;
                        sp.ProductName = txtProductName.Text;
                        sp.UnitPrice = int.Parse(txtUnitPrice.Text);
                        sp.Quantity = int.Parse(txtQuantity.Text);
                        sp.CatId = ComCategory.SelectedValue.ToString();

                        conn.Products.Add(sp);
                        conn.SaveChanges();
                        LoadDataGrid();

                        Alert("Tạo thành công");
                    };
                }
                catch (Exception ex)
                {
                    Alert(ex.Message);
                }

            }
        }

        private bool validate()
        {
            string errors = "";
            if (string.IsNullOrWhiteSpace(txtProductID.Text)) errors += "Không được để trống Product ID \n";
            if (string.IsNullOrWhiteSpace(txtProductName.Text)) errors += "Không được để trống Product Name \n";
            if (ComCategory.SelectedValue == null) errors += "Không được để trống Category \n";
            if (string.IsNullOrWhiteSpace(txtUnitPrice.Text)) errors += "Không được để trống Unit Price \n";
            if (string.IsNullOrWhiteSpace(txtQuantity.Text)) errors += "Không được để trống Quantity \n";

            /*check Quantity*/
            if (!Regex.IsMatch(txtQuantity.Text, @"\d+"))
            {
                errors += "Quantity phải là số\n";
            }
            else
            {
                int Quantity = int.Parse(txtQuantity.Text);
                if (Quantity < 0) errors += "Quantity phải là số dương \n";
            }

            /*check Unit Price*/
            if (!Regex.IsMatch(txtUnitPrice.Text, @"\d+"))
            {
                errors += "Unit price phải là số";
            }
            else
            {
                int UnitPrice = int.Parse(txtUnitPrice.Text);
                if (UnitPrice < 0) errors += "Unit price phải là số dương \n";
            }

            if (string.IsNullOrWhiteSpace(errors))
            {
                return true;
            }
            else
            {
                Alert(errors);
                return false;
            }
        }

        private void Alert(string errors)
        {
            MessageBox.Show(errors, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LoadWindow(object sender, RoutedEventArgs e)
        {
            LoadDataGrid();
        }

        private void SelectItem(object sender, SelectionChangedEventArgs e)
        {
            isUpdate = true;
            checkMode();
            var item = gridData.SelectedItem;

            if(item != null)
            {
                string MaSP = ((TextBlock)gridData.SelectedCells[0].Column.GetCellContent(item)).Text;

                var product = (from record in conn.Products
                               where record.ProductId.Equals(MaSP)
                               select record).SingleOrDefault();
                if (product != null)
                {
                    txtProductID.Text = product.ProductId;
                    txtProductName.Text = product.ProductName;
                    txtUnitPrice.Text = product.UnitPrice.ToString();
                    txtQuantity.Text = product.Quantity.ToString();

                    var ListCategory = (from record in conn.Categories
                                        select record).ToList();
                    for (int i = 0; i < ListCategory.Count(); i++)
                    {
                        if(product.CatId == ListCategory[i].CatId)
                        {
                            ComCategory.SelectedIndex = i;
                        }
                    }
                }
            }
        }

        private void btnUpdate(object sender, RoutedEventArgs e)
        {
            try
            {
                if (validate())
                {
                    var ProductInDB = (from item in conn.Products
                                       where item.ProductId.Equals(txtProductID.Text)
                                       select item).SingleOrDefault();

                    var sp = ProductInDB;
                    sp.ProductId = txtProductID.Text;
                    sp.ProductName = txtProductName.Text;
                    sp.UnitPrice = int.Parse(txtUnitPrice.Text);
                    sp.Quantity = int.Parse(txtQuantity.Text);
                    sp.CatId = ComCategory.SelectedValue.ToString();

                    conn.SaveChanges();
                    LoadDataGrid();

                    Alert("Sửa thành công");
                };
            }
            catch (Exception ex)
            {
                Alert(ex.Message);
            }
        }

        private void btnDelete(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dialogResult = MessageBox.Show("Bạn chắc chắn muốn xóa sản phẩm này không ?", "Thông Báo", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (dialogResult == MessageBoxResult.Yes)
            {
                var sanPhamInDB = (from record in conn.Products
                                   where record.ProductId.Equals(txtProductID.Text)
                                   select record).SingleOrDefault();

                if (sanPhamInDB == null)
                {
                    Alert("Sản phẩm này không tồn tại trong Database");
                    return;
                }
                else
                {
                    conn.Products.Remove(sanPhamInDB);
                    conn.SaveChanges();
                    LoadDataGrid();
                    Alert("Xóa thành công");
                    isUpdate = false;
                    checkMode();
                }
            }
        }

        private void btnSearch(object sender, RoutedEventArgs e)
        {
            var window2 = new window2();
            window2.Show();
        }
    }
}

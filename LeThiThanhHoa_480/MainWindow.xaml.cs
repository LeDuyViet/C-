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
using LeThiThanhHoa_480.Models;

namespace LeThiThanhHoa_480
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private QLNhanvienContext conn;
        public MainWindow()
        {
            conn = new QLNhanvienContext();
            InitializeComponent();
            loadDataGrid();
            HienThiCombobox();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
        private void HienThiCombobox()
        {
            var Category = from record in conn.PhongBans
                           select record;
            ComMaPhong.ItemsSource = Category.ToList();
            ComMaPhong.DisplayMemberPath = "TenPhong";
            ComMaPhong.SelectedValuePath = "MaPhong";
        }

        private void loadDataGrid()
        {
            var query = from item in conn.Nhanviens
                        where item.Luong > 150
                        orderby item.Luong ascending
                        select new
                        {
                            MaPhong = item.MaPhong,
                            MaNV = item.MaNv,
                            HoTen = item.Hoten,
                            Luong = item.Luong,
                            Thuong = item.Thuong,
                            TongTien = item.Luong * item.Thuong
                        };
            DataNhanVien.ItemsSource = query.ToList();
        }

        private void btnThem(object sender, RoutedEventArgs e)
        {
            try
            {
                if (validate())
                {
                    var NhanVienInDB = (from item in conn.Nhanviens
                                       where item.MaNv.Equals(txtMaNhanVien.Text)
                                       select item).SingleOrDefault();
                    if (!(NhanVienInDB == null))
                    {
                        throw new Exception("Mã Nhân Viên không được để trùng");
                    }

                    var nv = new Nhanvien();
                    nv.MaNv = txtMaNhanVien.Text;
                    nv.Hoten = txtHoTen.Text;
                    nv.Luong = int.Parse(txtLuong.Text);
                    nv.Thuong = int.Parse(txtThuong.Text);
                    nv.MaPhong = ComMaPhong.SelectedValue.ToString();

                    conn.Nhanviens.Add(nv);
                    conn.SaveChanges();
                    loadDataGrid();

                    Alert("Tạo thành công");
                };
            }
            catch (Exception ex)
            {
                Alert(ex.Message);
            }
        }

        private void Alert(string errors)
        {
            MessageBox.Show(errors, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool validate()
        {
            string errors = "";
            if (string.IsNullOrWhiteSpace(txtMaNhanVien.Text)) errors += "Không được để trống Mã Nhân Viên \n";
            if (string.IsNullOrWhiteSpace(txtHoTen.Text)) errors += "Không được để trống Họ Tên \n";
            if (ComMaPhong.SelectedValue == null) errors += "Không được để trống Mã Phòng \n";
            if (string.IsNullOrWhiteSpace(txtLuong.Text)) errors += "Không được để trống Lương \n";
            if (string.IsNullOrWhiteSpace(txtThuong.Text)) errors += "Không được để trống Thưởng \n";

            /*check Luong*/
            if (!Regex.IsMatch(txtThuong.Text, @"\d+"))
            {
                errors += "Thưởng phải là số\n";
            }
            else
            {
                int Thuong = int.Parse(txtThuong.Text);
                if (Thuong < 100 || Thuong > 900 ) errors += "Thưởng phải từ 100 đến 900 \n";
            }

            /*check Luong*/
            if (!Regex.IsMatch(txtLuong.Text, @"\d+"))
            {
                errors += "Lương phải là số";
            }
            else
            {
                int Luong = int.Parse(txtLuong.Text);
                if (Luong < 3000 || Luong > 9000) errors += "Lương phải từ 3000 đến 9000 \n";
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

        private void btnSua(object sender, RoutedEventArgs e)
        {
            try
            {
                if (validate())
                {
                    var NhanVienInDB = (from item in conn.Nhanviens
                                        where item.MaNv.Equals(txtMaNhanVien.Text)
                                        select item).SingleOrDefault();

                    var nv = NhanVienInDB;
                    nv.MaNv = txtMaNhanVien.Text;
                    nv.Hoten = txtHoTen.Text;
                    nv.Luong = int.Parse(txtLuong.Text);
                    nv.Thuong = int.Parse(txtThuong.Text);
                    nv.MaPhong = ComMaPhong.SelectedValue.ToString();

                    conn.SaveChanges();
                    loadDataGrid();

                    Alert("Sửa thành công");
                };
            }
            catch (Exception ex)
            {
                Alert(ex.Message);
            }
        }

        private void btnXoa(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dialogResult = MessageBox.Show("Bạn chắc chắn muốn xóa sản phẩm này không ?", "Thông Báo", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (dialogResult == MessageBoxResult.Yes)
            {
                var NhanVienInDB = (from record in conn.Nhanviens
                                   where record.MaNv.Equals(txtMaNhanVien.Text)
                                   select record).SingleOrDefault();

                if (NhanVienInDB == null)
                {
                    Alert("Nhân viên này không tồn tại trong Database");
                    return;
                }
                else
                {
                    conn.Nhanviens.Remove(NhanVienInDB);
                    conn.SaveChanges();
                    loadDataGrid();
                    Alert("Xóa thành công");
                }
            }
        }

        private void btnSearch(object sender, RoutedEventArgs e)
        {

        }

        private void SelectItem(object sender, SelectionChangedEventArgs e)
        {
            /*isUpdate = true;
            checkMode();*/
            var item = DataNhanVien.SelectedItem;

            if (item != null)
            {
                string MaNv = ((TextBlock)DataNhanVien.SelectedCells[1].Column.GetCellContent(item)).Text;
                Alert(MaNv);
                var nv = (from record in conn.Nhanviens
                               where record.MaNv.Equals(MaNv)
                               select record).SingleOrDefault();
                if (nv != null)
                {
                    txtMaNhanVien.Text = nv.MaNv;
                    txtHoTen.Text = nv.Hoten;
                    txtLuong.Text = nv.Luong.ToString();
                    txtThuong.Text = nv.Thuong.ToString();

                    var ListPhongBan = (from record in conn.PhongBans
                                        select record).ToList();
                    for (int i = 0; i < ListPhongBan.Count(); i++)
                    {
                        if (nv.MaPhong == ListPhongBan[i].MaPhong)
                        {
                            ComMaPhong.SelectedIndex = i;
                        }
                    }
                }
            }
        }
    }
}

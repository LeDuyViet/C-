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
using DeThi1.Models;

namespace DeThi1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private QLBanHangContext conn;
        private SanPham? sanPhamDuocChon;
        private bool isUpdate = false;

        public MainWindow()
        {
            InitializeComponent();
            this.conn = new QLBanHangContext();
            HienThiCombobox();
        }

        private void checkStatus()
        {
            if(isUpdate)
            {
                MaSp.IsEnabled = false;
            }
            else
            {
                MaSp.IsEnabled = true;
                MaSp.Text = "";
                TenSp.Text = "";
                SoLuongCo.Text = "";
                DonGia.Text = "";
            }    
        }

        private void LoadWindow(object sender, RoutedEventArgs e)
        {
            LoadDataGrid();
        }

        private void HienThiCombobox()
        {
            var queryUpdateCombobox = from item in conn.LoaiSps
                                      select item;

            LoaiSp.ItemsSource = queryUpdateCombobox.ToList();
            LoaiSp.DisplayMemberPath = "TenLoai";
            LoaiSp.SelectedValuePath = "MaLoai";
        }

        private void LoadDataGrid()
        {
            var query = from item in conn.SanPhams
                        where item.DonGia > 100
                        orderby item.TenSp descending
                        select new
                        {
                            MaSp = item.MaSp,
                            TenSp = item.TenSp,
                            MaLoai = item.MaLoai,
                            SoLuongCo = item.SoLuongCo,
                            DonGia = item.DonGia,
                            ThanhTien = item.SoLuongCo * item.DonGia
                        };
            DataSanPham.ItemsSource = query.ToList();
        }

        private void btnThem(object sender, RoutedEventArgs e)
        {
            if(isUpdate == true)
            {
                isUpdate = false;
                checkStatus();
            }    
            try
            {
                if (validate())
                {
                    var ProductInDB = (from item in conn.SanPhams
                                       where item.MaSp.Equals(MaSp.Text)
                                       select item).SingleOrDefault();
                    if (!(ProductInDB == null))
                    {
                        MessageBoxs("Mã sản phẩm không được để trùng");
                        return;
                    }

                    /*MessageBoxs((string)(((bool)nam.IsChecked) ? nam.Content : nu.Content));
                    return;*/

                    List<RadioButton> radioButtons = new List<RadioButton>();
                    radioButtons.Add(nam);
                    radioButtons.Add(nu);
                    radioButtons.Add(khac);

                    foreach (var item in radioButtons)
                    {
                        if(item.IsChecked == true)
                        {
                            MessageBoxs((String)item.Content);
                            return;
                        }    
                    }

                    var sp = new SanPham();
                    sp.MaSp = MaSp.Text;
                    sp.TenSp = TenSp.Text;
                    sp.MaLoai = LoaiSp.SelectedValue.ToString();
                    sp.DonGia = int.Parse(DonGia.Text);
                    sp.SoLuongCo = int.Parse(SoLuongCo.Text);
                    conn.SanPhams.Add(sp);
                    conn.SaveChanges();
                    LoadDataGrid();
                    MessageBoxs("Tạo thành công");
                };
            }
            catch (Exception ex)
            {
                MessageBoxs(ex.Message);
            }
            
        }


        
        private void MessageBoxs(string mess)
        {
            MessageBox.Show(mess, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool validate()
        {
            string errors = "";
            if(string.IsNullOrWhiteSpace(MaSp.Text)) errors += "Không được để trống mã sản phẩm \n";
            if (string.IsNullOrWhiteSpace(TenSp.Text)) errors += "Không được để trống tên sản phẩm \n";
            if (LoaiSp.SelectedValue == null) errors += "Không được để trống loại sản phẩm \n";
            if (string.IsNullOrWhiteSpace(DonGia.Text)) errors += "Không được để trống đơn giá sản phẩm \n";
            if (string.IsNullOrWhiteSpace(SoLuongCo.Text)) errors += "Không được để trống số lượng sản phẩm \n";

            /*check don gia*/
            if(!Regex.IsMatch(DonGia.Text, @"\d+"))
            {
                errors += "Đơn giá phải là số\n";
            }
            else
            {
                int DonGiaVal = int.Parse(DonGia.Text);
                if (DonGiaVal < 100) errors += "Đơn giá phải lớn hơn 100 \n";
            }

            /*check so luong co*/
            if(!Regex.IsMatch(SoLuongCo.Text, @"\d+"))
            {
                errors += "Số lượng phải là số";
            }
            else
            {
                int SoLuongCoVal = int.Parse(SoLuongCo.Text);
                if (SoLuongCoVal < 0) errors += "Số lượng phải lớn hơn 100 \n";
            }

            if (string.IsNullOrWhiteSpace(errors))
            {
                return true;
            }
            else
            {
                MessageBoxs(errors);
                return false;
            }   
            
        }

        private void btnSua(object sender, RoutedEventArgs e)
        {
            try
            {
                if (validate())
                {
                    var ProductInDB = (from item in conn.SanPhams
                                       where item.MaSp.Equals(MaSp.Text)
                                       select item).SingleOrDefault();
                    var sp = ProductInDB;
                    sp.TenSp = TenSp.Text;
                    sp.MaLoai = LoaiSp.SelectedValue.ToString();
                    sp.DonGia = int.Parse(DonGia.Text);
                    sp.SoLuongCo = int.Parse(SoLuongCo.Text);

                    conn.SaveChanges();
                    LoadDataGrid();
                    MessageBoxs("Sửa thành công");
                };
            }
            catch (Exception ex)
            {
                MessageBoxs(ex.Message);
            }
        }

        private void selectItem(object sender, SelectionChangedEventArgs e)
        {
            /*get item select*/

            var item = DataSanPham.SelectedItem;

            isUpdate = true;
            checkStatus();
            if(item != null)
            {
                string MaSP = ((TextBlock)DataSanPham.SelectedCells[0].Column.GetCellContent(item)).Text;

                var sp = (from record in conn.SanPhams
                              where record.MaSp.Equals(MaSP)
                              select record).SingleOrDefault();

                if(sp != null)
                {
                    MaSp.Text = sp.MaSp;
                    TenSp.Text = sp.TenSp;

                    var danhSachLoaiSanPham = (from record in conn.LoaiSps
                                                  select record).ToList();

                    for (int i = 0; i < danhSachLoaiSanPham.Count(); i++)
                    {
                        if(danhSachLoaiSanPham[i].MaLoai.Equals(sp.MaLoai))
                        {
                            LoaiSp.SelectedIndex = i;
                        }
                    }
                    DonGia.Text = sp.DonGia.ToString();
                    SoLuongCo.Text = sp.SoLuongCo.ToString();
                    sanPhamDuocChon = sp;
                }
            }
        }

        private void btnXoa(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dialogResult = MessageBox.Show("Bạn chắc chắn muốn xóa sản phẩm này không ?", "Thông Báo", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if(dialogResult == MessageBoxResult.Yes)
            {
                var sanPhamInDB = (from record in conn.SanPhams
                                   where record.MaSp.Equals(MaSp.Text)
                                   select record).SingleOrDefault();

                if(sanPhamInDB == null)
                {
                    MessageBoxs("Sản phẩm này không tồn tại trong Database");
                    return;
                }
                else
                {
                    var hoaDon = (from record in conn.HoaDonChiTiets
                                  where record.MaSp.Equals(sanPhamInDB.MaSp)
                                  select record).ToList();
                    conn.HoaDonChiTiets.RemoveRange(hoaDon);
                    conn.SanPhams.Remove(sanPhamInDB);
                    conn.SaveChanges();
                    LoadDataGrid();
                    sanPhamDuocChon = null;
                    MessageBoxs("Xóa thành công");
                }
            }
        }

        private void btnSearch(object sender, RoutedEventArgs e)
        {
            var window2 = new Window2();
            window2.Show();
        }
    }
}

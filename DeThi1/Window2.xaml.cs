using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DeThi1.Models;

namespace DeThi1
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        public Window2()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            loadDataGrid();
        }

        private void loadDataGrid()
        {
            var conn = new QLBanHangContext();
            var sanPhamInHoaDon = from record in conn.HoaDonChiTiets
                                  group record by record.MaSp into GroupSanPham
                                  select new
                                  {
                                      MaSp = GroupSanPham.Key,
                                      SoLuongDaBan = GroupSanPham.Sum((sp) => sp.SoLuongMua)
                                  };
            var result = from record1 in sanPhamInHoaDon
                         join record2 in conn.SanPhams
                         on record1.MaSp equals record2.MaSp
                         select new
                         {
                             MaSp = record1.MaSp,
                             TenSp = record2.TenSp,
                             TenLoaiSp = record2.MaLoaiNavigation.TenLoai,
                             SoLuongCo = record1.SoLuongDaBan,
                             TongTienBan = record1.SoLuongDaBan * record2.DonGia
                         };

            DataSanPham.ItemsSource = result.ToList();
        }

        private void btnDong(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

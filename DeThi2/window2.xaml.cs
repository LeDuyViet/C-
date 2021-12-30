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
using DeThi2.Models;

namespace DeThi2
{
    /// <summary>
    /// Interaction logic for window2.xaml
    /// </summary>
    public partial class window2 : Window
    {
        public window2()
        {
            InitializeComponent();
            LoadDataGrid2();
        }

        private void LoadDataGrid2()
        {
            var conn = new SALESMANAGEMENTContext();
            var productGroupByCat = from record in conn.Products
                                    group record by record.CatId into GroupByCat
                                    select new
                                    {
                                        CatId = GroupByCat.Key,
                                        Total = GroupByCat.Sum((product) => product.Quantity * product.UnitPrice )
                                    };
            var category = from record1 in productGroupByCat
                           join record2 in conn.Categories
                           on record1.CatId equals record2.CatId
                           select new
                           {
                               CategoryId = record1.CatId,
                               CategoryName = record2.CatName,
                               Total = record1.Total
                           };

            gridData.ItemsSource = category.ToList();

        }

        private void btnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

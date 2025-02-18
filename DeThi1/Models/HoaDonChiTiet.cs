﻿using System;
using System.Collections.Generic;

#nullable disable

namespace DeThi1.Models
{
    public partial class HoaDonChiTiet
    {
        public string MaHd { get; set; }
        public string MaSp { get; set; }
        public DateTime? NgayBan { get; set; }
        public int? SoLuongMua { get; set; }

        public virtual SanPham MaSpNavigation { get; set; }
    }
}

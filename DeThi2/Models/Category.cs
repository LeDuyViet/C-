using System;
using System.Collections.Generic;

#nullable disable

namespace DeThi2.Models
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public string CatId { get; set; }
        public string CatName { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}

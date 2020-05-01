using BobaShop.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BobaShop.ViewModels
{
    public class ProductCreateViewModel
    {
        [Required]
        [Column("name")]
        [StringLength(100)]
        public string Name { get; set; }
        [Column("description")]
        [StringLength(8000)]
        public string Description { get; set; }
        [DataType(DataType.Currency)]
        [Column("price", TypeName = "decimal(10,0)")]
        public decimal Price { get; set; }
        [Column("photo")]
        [StringLength(255)]
        public IFormFile Photo { get; set; }
        [Column("category_id")]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        [InverseProperty("Product")]
        public virtual Category Category { get; set; }
        [InverseProperty("Product")]
        public virtual ICollection<Cart> Cart { get; set; }
        [InverseProperty("Product")]
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
    }
}

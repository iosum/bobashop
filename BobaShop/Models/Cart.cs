using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BobaShop.Models
{
    [Table("cart")]
    public partial class Cart
    {
        [Key]
        [Column("cart_id")]
        public int CartId { get; set; }
        [Column("product_id")]
        public int ProductId { get; set; }
        [Column("quantity")]
        public int Quantity { get; set; }
        [Column("price", TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }
        [Required]
        [Column("username")]
        [StringLength(100)]
        public string Username { get; set; }

        [ForeignKey(nameof(ProductId))]
        [InverseProperty("Cart")]
        public virtual Product Product { get; set; }
    }
}

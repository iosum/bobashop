using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BobaShop.Models
{
    [Table("order")]
    public partial class Order
    {
        public Order()
        {
            OrderDetail = new HashSet<OrderDetail>();
        }

        [Key]
        [Column("order_id")]
        public int OrderId { get; set; }
        [Column("order_date", TypeName = "datetime")]
        public DateTime OrderDate { get; set; }
        [Required]
        [Column("user_id")]
        [StringLength(100)]
        public string UserId { get; set; }
        [Column("total", TypeName = "decimal(10, 2)")]
        public decimal Total { get; set; }
        [Required]
        [Column("first_name")]
        [StringLength(100)]
        public string FirstName { get; set; }
        [Required]
        [Column("last_name")]
        [StringLength(100)]
        public string LastName { get; set; }
        [Required]
        [Column("address")]
        [StringLength(255)]
        public string Address { get; set; }
        [Required]
        [Column("city")]
        [StringLength(100)]
        public string City { get; set; }
        [Required]
        [Column("province")]
        [StringLength(10)]
        public string Province { get; set; }
        [Required]
        [Column("postal_code")]
        [StringLength(10)]
        public string PostalCode { get; set; }
        [Required]
        [Column("phone")]
        [StringLength(15)]
        public string Phone { get; set; }

        [InverseProperty("Order")]
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
    }
}

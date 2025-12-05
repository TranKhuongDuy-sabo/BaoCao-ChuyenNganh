using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace project.Models;

public partial class Brand
{
    [Key]
    [Column("BrandID")]
    [Required(ErrorMessage = "Mã thương hiệu tự tăng")]
    [DisplayName("Mã thương hiệu")]
    public int BrandId { get; set; }

    [StringLength(100)]
    [Required(ErrorMessage = "Tên thương hiệu không được để trống")]
    [DisplayName("Tên thương hiệu")]
    public string BrandName { get; set; } = null!;

    [StringLength(100)]
    [Required(ErrorMessage = "Xuất xứ không được để trống")]
    [DisplayName("Xuất xứ")]
    public string? Origin { get; set; }

    [InverseProperty("Brand")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}

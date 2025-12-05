using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace project.Models;

public partial class Category
{
    [Key]
    [Column("CategoryID")]
    [Required(ErrorMessage = "Mã danh mục tự tăng")]
    [DisplayName("Mã danh mục")]
    public int CategoryId { get; set; }

    [StringLength(100)]
    [Required(ErrorMessage = "Tên danh mục không được bỏ trống")]
    [DisplayName("Tên danh mục")]
    public string CategoryName { get; set; } = null!;

    [InverseProperty("Category")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}

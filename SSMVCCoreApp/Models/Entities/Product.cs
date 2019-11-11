using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SSMVCCoreApp.Models.Entities
{
  public class Product
  {
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProductId { get; set; }

    [Required]
    [StringLength(100)]
    public string ProductName { get; set; }

    [Required]
    [StringLength(250)]
    public string Description { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")] //For EF Core the create the column on this base or it will create a default in the schema class
    public decimal Price { get; set; }

    [Required]
    [StringLength(100)]
    public string Category { get; set; }

    public string PhotoUrl { get; set; }
  }
}

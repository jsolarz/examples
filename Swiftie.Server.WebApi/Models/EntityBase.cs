using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Swiftie.Server.WebApi.Models
{
    /// <summary>
    /// abstract base class with common properties for models
    /// </summary>
    public abstract class EntityBase
    {
        [Key]
        public int Id { get; set; }
        [Required, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime DateCreated { get; set; }
    }
}
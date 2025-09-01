using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TagNameMapper.Models.TableMetadatas;

[Table("Tag_PollGroups")]
public class PollGroup
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "PollGroup Name is required")]
    public required string Name { get; set; } 
    
    [Range(100, int.MaxValue, ErrorMessage = "PollInterval must be at least 100")]
    public required int PollInterval { get; set; } 
    
}

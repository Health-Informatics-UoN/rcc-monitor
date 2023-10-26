using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Monitor.Data.Entities;

public class Study
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int RedCapId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public ICollection<StudyUser> Users { get; set; } = null!;
}

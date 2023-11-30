using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Monitor.Data.Entities;

public class StudyGroup
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int Id { get; set; }
    public Study Study { get; set; }
    public string Name { get; set; } = string.Empty;
    public int PlannedSize { get; set; }
}

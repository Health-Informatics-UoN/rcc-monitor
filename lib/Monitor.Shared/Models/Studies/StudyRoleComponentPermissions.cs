using Monitor.Shared.Constants;

namespace Monitor.Shared.Models.Studies;

/// <summary>
/// A RedCap class to describe the permissions for a Study Role.
/// </summary>
public class StudyRoleComponentPermissions
{
    public int Id { get; set; }
    /// <summary>
    /// List of the possible permissions.
    /// </summary>
    public List<GenericItem> AllowedPermissions { get; set; } = [];
    public string ComponentName { get; set; } = string.Empty;
    public string ComponentShortName { get; set; } = string.Empty;
    public bool Enabled { get; set; } = true;
    /// <summary>
    /// List of the permissions that role actually has, indexing <see cref="AllowedPermissions"/>
    /// </summary>
    public List<int> Permissions { get; set; } = [];
}

/// <summary>
/// A RedCap class to describe the allowed permissions on a Study Role Component.
/// </summary>
public class GenericItem
{
    public int Id { get; set; }
    /// <summary>
    /// See <seealso cref="AllowedPermissions"/>
    /// </summary>
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

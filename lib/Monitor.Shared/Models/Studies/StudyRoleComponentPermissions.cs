using Monitor.Shared.Constants;

namespace Monitor.Shared.Models.Studies;

/// <summary>
/// A RedCap class to describe the permissions for a Study Role.
/// </summary>
public class StudyRoleComponentPermissions
{
    public int id { get; set; }
    /// <summary>
    /// List of the possible permissions.
    /// </summary>
    public List<GenericItem> allowedPermissions { get; set; } = [];
    public string componentName { get; set; } = string.Empty;
    public string componentShortName { get; set; } = string.Empty;
    public bool enabled { get; set; } = true;
    /// <summary>
    /// List of the permissions that role actually has, indexing <see cref="allowedPermissions"/>
    /// </summary>
    public List<int> permissions { get; set; } = [];
}

/// <summary>
/// A RedCap class to describe the allowed permissions on a Study Role Component.
/// </summary>
public class GenericItem
{
    public int id { get; set; }
    /// <summary>
    /// See <seealso cref="AllowedPermissions"/>
    /// </summary>
    public string name { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;
}

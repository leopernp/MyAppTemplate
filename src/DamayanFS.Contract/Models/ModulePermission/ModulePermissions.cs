namespace DamayanFS.Contract.Models.ModulePermission;

public record ModulePermissions(bool CanView, bool CanCreate, bool CanUpdate, bool CanDelete)
{
    public static ModulePermissions FullAccess => new(true, true, true, true);
    public static ModulePermissions NoAccess   => new(false, false, false, false);
}

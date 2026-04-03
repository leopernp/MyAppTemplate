using MyAppTemplate.Contract.Models.Role;

namespace MyAppTemplate.App.ViewModels.Role;

public class RoleIndexViewModel : BaseViewModel
{
    // Role list is not loaded server-side; handled client-side via DataTables

    public RoleUpsertModel? Role { get; set; }
}

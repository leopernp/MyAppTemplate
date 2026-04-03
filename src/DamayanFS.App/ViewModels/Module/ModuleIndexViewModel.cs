using DamayanFS.Contract.Models.Module;

namespace DamayanFS.App.ViewModels.Module;

public class ModuleIndexViewModel : BaseViewModel
{
    // Module list is not loaded server-side; handled client-side via DataTables
    public ModuleUpsertModel? Module { get; set; }
}

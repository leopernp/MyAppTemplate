using MyAppTemplate.Contract.Models.User;

namespace MyAppTemplate.App.ViewModels.User;

public class UserIndexViewModel : BaseViewModel
{
    // User List is not loaded on server side but handled in client side

    public UserUpsertModel? User { get; set; }

    // Role list is not loaded on server side but handled in client side
}

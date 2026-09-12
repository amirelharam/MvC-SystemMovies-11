namespace MvC_SystemMovies.Areas.Admin.Models
{
    public class UserListItemViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool IsBlocked { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }

    public class ChangeRoleViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string CurrentRole { get; set; } = string.Empty;
        public string SelectedRole { get; set; } = string.Empty;
        public List<string> AvailableRoles { get; set; } = new();
    }
}

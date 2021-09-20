using AutoUp.Enums;
using AutoUp.Models;

namespace AutoUp.ViewModels
{
    public class SortUserViewModel
    {
        public UserSortState LoginSort { get; private set; }
        public UserSortState EmailSort { get; private set; }
        public UserSortState BalanceSort { get; private set; }
        public UserSortState RoleSort { get; private set; }
        public UserSortState Current { get; private set; }

        public SortUserViewModel(UserSortState sortOrder)
        {
            LoginSort = sortOrder == UserSortState.LoginAsc ? UserSortState.LoginDesc : UserSortState.LoginAsc;
            EmailSort = sortOrder == UserSortState.EmailAsc ? UserSortState.EmailDesc : UserSortState.EmailAsc;
            BalanceSort = sortOrder == UserSortState.BalanceAsc ? UserSortState.BalanceDesc : UserSortState.BalanceAsc;
            RoleSort = sortOrder == UserSortState.RoleAsc ? UserSortState.RoleDesc : UserSortState.RoleAsc;
            Current = sortOrder;
        }
    }
}

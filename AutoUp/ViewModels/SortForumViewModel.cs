using AutoUp.Enums;

namespace AutoUp.ViewModels
{
    public class SortForumViewModel
    {
        public SortState NameSort { get; private set; } 
        public SortState UpSort { get; private set; }
        public SortState UrlSort { get; private set; }
        public SortState Current { get; private set; }  

        public SortForumViewModel(SortState sortOrder)
        {
            NameSort = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            UpSort = sortOrder == SortState.UpAsc ? SortState.UpDesc : SortState.UpAsc;
            UrlSort = sortOrder == SortState.UrlAsc ? SortState.UrlDesc : SortState.UrlAsc;
            Current = sortOrder;
        }
    }
}

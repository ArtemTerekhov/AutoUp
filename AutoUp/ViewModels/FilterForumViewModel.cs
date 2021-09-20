using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using AutoUp.Models;

namespace AutoUp.ViewModels
{
    public class FilterForumViewModel
    {
       public FilterForumViewModel(List<Forum> forums, int? forum, string name)
            {
                // устанавливаем начальный элемент, который позволит выбрать всех
                forums.Insert(0, new Forum { Name = "Все", ForumId = 0 });
                Forums = new SelectList(forums, "ForumId", "Name", forum);
                SelectedForum = forum;
                SelectedName = name;
            }
            public SelectList Forums { get; private set; } 
            public int? SelectedForum { get; private set; }  
            public string SelectedName { get; private set; }    
        }
 }

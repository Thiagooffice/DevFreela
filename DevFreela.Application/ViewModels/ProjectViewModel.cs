using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevFreela.Application.ViewModels
{
    public class ProjectViewModel
    {
        private int id;

        public ProjectViewModel(string id, string title, DateTime createdAt)
        {
            Id = id;
            Title = title;
            CreatedAt = createdAt;
        }

        public ProjectViewModel(int id, string title, DateTime createdAt)
        {
            this.id = id;
            Title = title;
            CreatedAt = createdAt;
        }

        public string Id { get; private set; }
        public string Title { get; private  set; }
        public DateTime CreatedAt { get; private set; }
    }
}

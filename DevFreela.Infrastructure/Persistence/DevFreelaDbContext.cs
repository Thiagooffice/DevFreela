using DevFreela.Core.Entities;
using System;
using System.Collections.Generic;

namespace DevFreela.Infrastructure.Persistence
{
    public class DevFreelaDbContext
    {
        public DevFreelaDbContext()
        {
            Projects = new List<Project>
            {
                new Project("Meu projeto aspnet core 1", "Minha descrição 1", 1, 1, 10000),
                new Project("Meu projeto aspnet core 2", "Minha descrição 2", 1, 1, 20000),
                new Project("Meu projeto aspnet core 3", "Minha descrição 3", 1, 1, 30000)
            };
            Users = new List<User>
            {
                new User("Thiago leal", "thiago@gmail.com", new DateTime(1999,1,1)),
                new User("Roberto leal", "roberto@gmail.com", new DateTime(2003,1,1)),
                new User("Victor leal", "victor@gmail.com", new DateTime(1996,1,1)),
            };
            Skills = new List<Skill>
            {
                new Skill(".NET Core"),
                new Skill("React"),
                new Skill("AWS")
            };
        }
        public List<Project> Projects { get;  set; }
        public List<User> Users { get;  set; }
        public List<Skill> Skills { get;  set; }
        public List<ProjectComment> ProjectComments { get; set; }
    }
}

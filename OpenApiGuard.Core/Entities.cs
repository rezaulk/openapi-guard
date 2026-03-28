using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OpenApiGuard.Core.Entities
{
    public class Project
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<ProjectRepoLink> RepoLinks { get; set; }
    }

    public class ProjectRepoLink
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Url { get; set; }
    }

    public class ApiSpecVersion
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Version { get; set; }
    }

    public class DiffReport
    {
        [Key]
        public Guid Id { get; set; }
        public string Changes { get; set; }
    }

    public class RuleSet
    {
        [Key]
        public Guid Id { get; set; }
        public string Rules { get; set; }
    }
}
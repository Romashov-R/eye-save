﻿using System;
using System.Collections.Generic;

namespace EyeSave.Entities
{
    public partial class AgentType
    {
        public AgentType()
        {
            Agents = new HashSet<Agent>();
        }

        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Image { get; set; }

        public virtual ICollection<Agent> Agents { get; set; }
    }
}

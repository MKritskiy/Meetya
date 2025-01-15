using Events.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Events.Application.Models;

public class EventDto
{
    public Event? Event { get; set; }
    
    public Profile? Profile { get; set; }
}

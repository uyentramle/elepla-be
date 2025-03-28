﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elepla.Service.Models.ViewModels.TeachingScheduleModels
{
    public class CreateTeachingScheduleDTO
    {
        public string Title { get; set; } 
        public string? Description { get; set; }  
        public DateTime Date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string ClassName { get; set; }
        public string TeacherId { get; set; }
        public string PlanbookId { get; set; }
    }

}

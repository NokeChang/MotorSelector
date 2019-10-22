using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MotorSelector.Models
{
    public class Motor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Torque { get; set; }
        public int Rpm { get; set; }
    }
}
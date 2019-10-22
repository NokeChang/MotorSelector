using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MotorSelector.Models;

namespace MotorSelector.Controllers
{
    public class SelectorController : Controller
    {
        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\User\source\repos\MotorSelector\MotorSelector\App_Data\Database1.mdf;Integrated Security=True";
        public ActionResult Index()
        {
            var motors = new ModelHelper<Motor>(connectionString);
            var motorList = motors.GetAllRecords();
            return View(motorList);
        }
        // GET: Selector
        public ActionResult AjaxResult(List<string> numbers)
        {
            var result = new ResultPackage
            {
                LineSpeed = Convert.ToDouble(numbers[0]),
                PullyDiameter = Convert.ToDouble(numbers[1]),
                FrictionForce = Convert.ToDouble(numbers[2]),
                GearRatio = Convert.ToDouble(numbers[3]),
                GearEfficiency = Convert.ToDouble(numbers[4])
            };
            result.GetLineSpeed();
            result.GetMotorTorque();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetMotor(int index)
        {
            var motors = new ModelHelper<Motor>(connectionString);
            var motor = motors.GetRecord(index);
            return Json(motor, JsonRequestBehavior.AllowGet);
        }
        class ResultPackage
        {
            public double MotorRevolution { get; private set; } = 0.0;
            public double MotorTorque { get; private set; } = 0.0;
            public double LineSpeed { private get; set; }
            public double PullyDiameter { private get; set; }
            public double FrictionForce { private get; set; }
            public double GearRatio { private get; set; }
            public double GearEfficiency { private get; set; }

            public void GetLineSpeed()
            {
                MotorRevolution = LineSpeed * 60 * GearRatio / PullyDiameter / Math.PI;
            }
            public void GetMotorTorque()
            {
                MotorTorque = FrictionForce * PullyDiameter / 2 / 1000 / GearRatio / GearEfficiency;
            }
        }
    }
}
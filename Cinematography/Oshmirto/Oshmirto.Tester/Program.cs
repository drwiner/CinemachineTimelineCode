using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oshmirto;

namespace Oshmirto.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            CameraPlan plan = Parser.Parse("../../../../FireBoltUnity/cameraPlans/defaultCamera.xml");
        }
    }
}

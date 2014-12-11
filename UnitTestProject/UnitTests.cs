using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WatchCAD.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WatchCAD.View;
using WatchCAD.ViewModel;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;


namespace UnitTestProject
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void TestMethodCylinder()
        {
            WatchCAD.Model.Cylinder cylinder = new WatchCAD.Model.Cylinder();
            cylinder.Diameter = 1;
            cylinder.Height = 10;
            Assert.AreEqual(cylinder.Diameter, 1);
            Assert.AreEqual(cylinder.Height, 10);
        }

        [TestMethod]
        public void TestMethodBody()
        {
           
            WatchCAD.Model.Body body = new WatchCAD.Model.Body();
            body.Diameter = 1;
            body.Height = 10;
            body.BootstrapLength = 100;
            body.HasChronograph = false;
            
            Assert.AreEqual(body.Diameter, 1);
            Assert.AreEqual(body.Height, 10);
            Assert.AreEqual(body.BootstrapLength, 100);
            Assert.AreEqual(body.HasChronograph, false);
        }

        [TestMethod]
        public void TestMethodStrap()
        {
            Strap strap = new Strap();
            strap.Width = 1;
            strap.PerforationRadius = 10;
            strap.NumberOfPerforations = 100;
            strap.Length = 1000;

            Assert.AreEqual(strap.Width, 1);
            Assert.AreEqual(strap.PerforationRadius, 10);
            Assert.AreEqual(strap.NumberOfPerforations, 100);
            Assert.AreEqual(strap.Length, 1000);
        }

        [TestMethod]
        public void TestMethodWatchData()
        {
            WatchData WatchData = new WatchData()
            {
                BodyDiameter = 10,
                BodyHeight = 6,
                BootstrapLength = 10,
                CrownDiameter = 4,
                CrownHeight = 5,
                HasChronograph = true,
                NumberOfPerforations = 7,
                StrapLength = 300,
                StrapWidth = 9,
                StrapPerforationRadius = 10
            };


            Assert.AreEqual(WatchData.BodyDiameter, 30);
            WatchData.BodyDiameter = 32;
            Assert.AreEqual(WatchData.BodyDiameter, 32);
            WatchData.BodyDiameter = 80;
            Assert.AreEqual(WatchData.BodyDiameter, 70);

            Assert.AreEqual(WatchData.BodyHeight, 6);
            Assert.AreEqual(WatchData.BootstrapLength, 10);
            Assert.AreEqual(WatchData.CrownDiameter, 4);
            Assert.AreEqual(WatchData.CrownHeight, 5);
            Assert.AreEqual(WatchData.HasChronograph, true);
            Assert.AreEqual(WatchData.NumberOfPerforations, 7);
            Assert.AreEqual(WatchData.StrapLength, 300);
            Assert.AreEqual(WatchData.StrapWidth, 9);
            Assert.AreEqual(WatchData.StrapPerforationRadius, 10);
        }
    }
}

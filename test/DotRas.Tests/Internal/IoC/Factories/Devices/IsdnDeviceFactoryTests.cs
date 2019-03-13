﻿using DotRas.Devices;
using DotRas.Internal.IoC.Factories.Devices;
using NUnit.Framework;

namespace DotRas.Tests.Internal.IoC.Factories.Devices
{
    [TestFixture]
    public class IsdnDeviceFactoryTests
    {
        [Test]
        public void ReturnADeviceInstance()
        {
            var target = new IsdnDeviceFactory();
            var result = target.Create("Test");

            Assert.AreEqual("Test", result.Name);
            Assert.IsAssignableFrom<Isdn>(result);
        }
    }
}
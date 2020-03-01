﻿using System;
using DotRas.Diagnostics;
using DotRas.Diagnostics.Events;
using DotRas.Internal.Abstractions.Primitives;
using DotRas.Internal.Infrastructure.Advice;
using DotRas.Internal.Interop;
using Moq;
using NUnit.Framework;
using static DotRas.Internal.Interop.NativeMethods;
using static DotRas.Internal.Interop.Ras;
using static DotRas.Internal.Interop.WinError;

namespace DotRas.Tests.Internal.Infrastructure.Advice
{
    [TestFixture]
    public class RasApi32LoggingAdviceTests
    {
        private delegate void LogEventCallback(EventLevel eventLevel, TraceEvent eventData);

        private Mock<IRasApi32> api;
        private Mock<IEventLoggingPolicy> eventLoggingPolicy;

        [SetUp]
        public void Setup()
        {
            api = new Mock<IRasApi32>();
            eventLoggingPolicy = new Mock<IEventLoggingPolicy>();
        }

        [Test]
        public void RasClearConnectionStatisticsAsExpected()
        {
            var hRasConn = new IntPtr(1);

            api.Setup(o => o.RasClearConnectionStatistics(hRasConn)).Returns(SUCCESS);
            eventLoggingPolicy.Setup(o => o.LogEvent(It.IsAny<EventLevel>(), It.IsAny<PInvokeInt32CallCompletedTraceEvent>())).Callback(new LogEventCallback(
                (level, o1) =>
                {
                    Assert.AreEqual(EventLevel.Verbose, level);

                    var eventData = (PInvokeInt32CallCompletedTraceEvent)o1;
                    Assert.AreEqual(hRasConn, (IntPtr)eventData.Args[nameof(hRasConn)]);
                    Assert.True(eventData.Duration > TimeSpan.Zero);
                    Assert.AreEqual(SUCCESS, eventData.Result);
                })).Verifiable();

            var target = new RasApi32LoggingAdvice(api.Object, eventLoggingPolicy.Object);
            var result = target.RasClearConnectionStatistics(hRasConn);

            eventLoggingPolicy.Verify();
            Assert.AreEqual(SUCCESS, result);
        }

        [Test]
        public void RasConnectionNotificationAsExpected()
        {
            var hRasConn = new IntPtr(1);
            var hEvent = new Mock<ISafeHandleWrapper>();
            var dwFlags = RASCN.Connection;

            api.Setup(o => o.RasConnectionNotification(hRasConn, hEvent.Object, dwFlags)).Returns(SUCCESS);
            eventLoggingPolicy.Setup(o => o.LogEvent(It.IsAny<EventLevel>(), It.IsAny<PInvokeInt32CallCompletedTraceEvent>())).Callback(new LogEventCallback(
                (level, o1) =>
                {
                    Assert.AreEqual(EventLevel.Verbose, level);

                    var eventData = (PInvokeInt32CallCompletedTraceEvent)o1;
                    Assert.AreEqual(hRasConn, (IntPtr)eventData.Args[nameof(hRasConn)]);
                    Assert.AreEqual(hEvent.Object, (ISafeHandleWrapper)eventData.Args[nameof(hEvent)]);
                    Assert.AreEqual(dwFlags, (RASCN)eventData.Args[nameof(dwFlags)]);
                    Assert.True(eventData.Duration > TimeSpan.Zero);
                    Assert.AreEqual(SUCCESS, eventData.Result);
                })).Verifiable();

            var target = new RasApi32LoggingAdvice(api.Object, eventLoggingPolicy.Object);
            var result = target.RasConnectionNotification(hRasConn, hEvent.Object, dwFlags);

            eventLoggingPolicy.Verify();
            Assert.AreEqual(SUCCESS, result);
        }

        [Test]
        public void RasEnumConnectionsAsExpected()
        {
            var lpCb = 0;
            var lpConnections = 1;
            var lpRasConn = new RASCONN[0];

            api.Setup(o => o.RasEnumConnections(lpRasConn, ref It.Ref<int>.IsAny, ref It.Ref<int>.IsAny)).Returns(SUCCESS);
            eventLoggingPolicy.Setup(o => o.LogEvent(It.IsAny<EventLevel>(), It.IsAny<PInvokeInt32CallCompletedTraceEvent>())).Callback(new LogEventCallback(
                (level, o1) =>
                {
                    Assert.AreEqual(EventLevel.Verbose, level);

                    var eventData = (PInvokeInt32CallCompletedTraceEvent)o1;
                    Assert.True(eventData.Args.ContainsKey(nameof(lpRasConn)));
                    Assert.True(eventData.Args.ContainsKey(nameof(lpCb)));
                    Assert.True(eventData.Args.ContainsKey(nameof(lpConnections)));
                    Assert.True(eventData.OutArgs.ContainsKey(nameof(lpCb)));
                    Assert.True(eventData.OutArgs.ContainsKey(nameof(lpConnections)));
                    Assert.True(eventData.Duration > TimeSpan.Zero);
                    Assert.AreEqual(SUCCESS, eventData.Result);
                })).Verifiable();

            var target = new RasApi32LoggingAdvice(api.Object, eventLoggingPolicy.Object);
            var result = target.RasEnumConnections(lpRasConn, ref lpCb, ref lpConnections);

            eventLoggingPolicy.Verify();
            Assert.AreEqual(SUCCESS, result);
        }

        [Test]
        public void RasEnumDevicesAsExpected()
        {
            var lpCb = 0;
            var lpcDevices = 1;
            var lpRasDevInfo = new RASDEVINFO[0];

            api.Setup(o => o.RasEnumDevices(lpRasDevInfo, ref It.Ref<int>.IsAny, ref It.Ref<int>.IsAny)).Returns(SUCCESS);
            eventLoggingPolicy.Setup(o => o.LogEvent(It.IsAny<EventLevel>(), It.IsAny<PInvokeInt32CallCompletedTraceEvent>())).Callback(new LogEventCallback(
                (level, o1) =>
                {
                    Assert.AreEqual(EventLevel.Verbose, level);

                    var eventData = (PInvokeInt32CallCompletedTraceEvent)o1;
                    Assert.True(eventData.Args.ContainsKey(nameof(lpRasDevInfo)));
                    Assert.True(eventData.Args.ContainsKey(nameof(lpCb)));
                    Assert.True(eventData.Args.ContainsKey(nameof(lpcDevices)));
                    Assert.True(eventData.OutArgs.ContainsKey(nameof(lpCb)));
                    Assert.True(eventData.OutArgs.ContainsKey(nameof(lpcDevices)));
                    Assert.True(eventData.Duration > TimeSpan.Zero);
                    Assert.AreEqual(SUCCESS, eventData.Result);
                })).Verifiable();

            var target = new RasApi32LoggingAdvice(api.Object, eventLoggingPolicy.Object);
            var result = target.RasEnumDevices(lpRasDevInfo, ref lpCb, ref lpcDevices);

            eventLoggingPolicy.Verify();
            Assert.AreEqual(SUCCESS, result);
        }
    }
}
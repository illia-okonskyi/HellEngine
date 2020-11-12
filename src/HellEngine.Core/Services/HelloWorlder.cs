﻿using HellEngine.Core.Constants;
using HellEngine.Utils.Configuration.ServiceRegistrator;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IO;

namespace HellEngine.Core.Services
{
    [SdkService]
    public interface IHelloWorlder
    {
        string GetHelloString();
    }

    [ApplicationOptions("HelloWorlder")]
    public class HelloWorlderOptions
    {
        public string HelloString { get; set; }
    }

    [ApplicationService(Service = typeof(IHelloWorlder))]
    public class HelloWorlder : IHelloWorlder, IDisposable
    {
        private readonly string helloString;
        private readonly ILogger<HelloWorlder> logger;
        
        public HelloWorlder(
            IOptions<HelloWorlderOptions> options,
            ILogger<HelloWorlder> logger)
        {
            this.helloString = options.Value?.HelloString;
            this.logger = logger;
        }

        public void Dispose()
        {
            logger.LogInformation("HelloWorlder.Dispose()");
        }

        public string GetHelloString()
        {
            logger.LogInformation("GetHello request");
            return !string.IsNullOrEmpty(helloString)
                ? helloString
                : HelloWorld.HelloString;
        }
    }
}

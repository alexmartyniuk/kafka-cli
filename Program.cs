using System;
using McMaster.Extensions.CommandLineUtils;
using Kafka.Tool.Cli.Kafka;

namespace Kafka.Tool.Cli
{
    class Program
    {
        public static void Main(string[] args) 
        {            
            CommandLineApplication.Execute<KafkaCommand>(args);
        }
    }
}

using System;
using McMaster.Extensions.CommandLineUtils;
using Scania.Kafka.Tool.Cli.Kafka;

namespace Scania.Kafka.Tool.Cli
{
    class Program
    {
        public static void Main(string[] args) 
        {            
            CommandLineApplication.Execute<KafkaCommand>(args);
        }
    }
}

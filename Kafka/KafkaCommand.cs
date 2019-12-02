using McMaster.Extensions.CommandLineUtils;
using Scania.Kafka.Tool.Cli.Config;
using Scania.Kafka.Tool.Cli.Message;
using Scania.Kafka.Tool.Cli.Topic;

namespace Scania.Kafka.Tool.Cli.Kafka
{
    [Command(Name = "kafka-cli", Description = "Kafka Command Line Tool (kafka-cli) allows you to manage topics and produce/consume messages to/from Kafka cluster."),
     Subcommand(typeof(TopicCommand)), Subcommand(typeof(MessageCommand), typeof(ConfigCommand))]
    public class KafkaCommand
    {
        private int OnExecute(CommandLineApplication app, IConsole console)
        {            
            app.ShowHelp();
            return 1;
        }                
    }
}

using McMaster.Extensions.CommandLineUtils;
using Scania.Kafka.Tool.Cli.Config;
using Scania.Kafka.Tool.Cli.Message;
using Scania.Kafka.Tool.Cli.Topic;

namespace Scania.Kafka.Tool.Cli.Kafka
{
    [Command(Name = "kafka-cli", Description = "Kafka Command Line Tool allows you to manage topics and send/receive messages."),
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

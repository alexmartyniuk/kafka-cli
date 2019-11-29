using McMaster.Extensions.CommandLineUtils;

namespace Scania.Kafka.Tool.Cli.Topic
{
    [Command("topic", Description = "Manage topics"),
     Subcommand(typeof(TopicAddCommand)),
     Subcommand(typeof(TopicDeleteCommand)),
     Subcommand(typeof(TopicListCommand))]
    public class TopicCommand
    {
        private int OnExecute(IConsole console)
        {
            console.Error.WriteLine("You must specify an action. See --help for more details.");
            return 1;
        }                
    }
}

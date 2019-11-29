using McMaster.Extensions.CommandLineUtils;

namespace Scania.Kafka.Tool.Cli.Message
{    
    [Command("message", Description = "Send and receive messages"),
     Subcommand(typeof(MessageSendCommand)),
     Subcommand(typeof(MessageReceiveCommand))]
    public class MessageCommand
    {
        private int OnExecute(IConsole console)
        {
            console.Error.WriteLine("You must specify an action. See --help for more details.");
            return 1;
        }                
    }
}
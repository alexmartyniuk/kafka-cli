using McMaster.Extensions.CommandLineUtils;

namespace Scania.Kafka.Tool.Cli.Topic
{

    [Command(Description = "Add Topic"), HelpOption]
    public class TopicAddCommand
    {
        [Option(Description = "Topic name")]
        public string Name { get; }

        private void OnExecute(IConsole console)
        {
            console.WriteLine("Add topic");
        }
    }
}
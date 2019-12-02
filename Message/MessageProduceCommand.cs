using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Scania.Kafka.Tool.Cli.Kafka;

namespace Scania.Kafka.Tool.Cli.Message
{

    [Command("produce", Description = "Prodcue a message to the Kafka topic",
            AllowArgumentSeparator = true,
            ThrowOnUnexpectedArgument = false)]
    public class MessageProduceCommand
    {
        [Required(ErrorMessage = "You must specify the message")]
        [Argument(0, Description = "Text message to produce")]
        public string Message { get; }

        [Required(ErrorMessage = "You must specify the topic")]
        [Option("-t|--topic", Description = "Topic name")]
        public string TopicName { get; }


        [Option("-n|--number", Description = "Number of messages that should be produced")]
        public int Number { get; } = 1;

        [Option("-p|--pause", Description = "Pause between producing messages in milliseconds")]
        public int Pause { get; } = 0;

        private async Task<int> OnExecute(IConsole console)
        {
            try
            {
                for(var i = 1; i <= Number; i++)
                {                                        
                    var topicInfo = await KafkaClient.ProduceMessageAsync(TopicName, Message);
                    console.WriteLine($"{i} message delivered to {topicInfo}");

                    if (Pause > 0)
                    {
                        Thread.Sleep(Pause);
                    }
                }                            
                
                return await Task.FromResult(0);
            }
            catch (Exception e)
            {
                console.WriteLine($"Delivery failed: {e.Message}");
                return await Task.FromResult(1);
            }
        }
    }
}
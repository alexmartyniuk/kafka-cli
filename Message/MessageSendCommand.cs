using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Scania.Kafka.Tool.Cli.Kafka;

namespace Scania.Kafka.Tool.Cli.Message
{

    [Command("send", Description = "Send a message",
            AllowArgumentSeparator = true,
            ThrowOnUnexpectedArgument = false)]
    public class MessageSendCommand
    {
        [Required(ErrorMessage = "You must specify the message")]
        [Argument(0, Description = "Text message to send")]
        public string Message { get; }

        [Required(ErrorMessage = "You must specify the topic")]
        [Option("-t|--topic", Description = "Topic name")]
        public string TopicName { get; }


        [Option("-n|--number", Description = "Number of messages that should be send")]
        public int Number { get; } = 1;

        [Option("-p|--pause", Description = "Pause between sending in milliseconds")]
        public int Pause { get; } = 0;

        private async Task<int> OnExecute(IConsole console)
        {
            try
            {
                for(var i = 1; i <= Number; i++)
                {                    
                    if (Pause > 0)
                    {
                        Thread.Sleep(Pause);
                    }
                    var topicInfo = await KafkaClient.SendMessageAsync(TopicName, Message);
                    console.WriteLine($"{i} message delivered to {topicInfo}");
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
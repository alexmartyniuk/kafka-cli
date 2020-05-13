# Kafka Command Line Tool

Kafka Command Line Tool (kafka-cli) allows you to manage topics and produce/consume messages to/from Kafka cluster.

Usage: 
```sh
kafka-cli [options] [command]

Options:
  -?|-h|--help  Show help information

Commands:
  config        Configure Kafka CLI
  message       Produce and consume messages
  topic         Manage topics

Run 'kafka-cli [command] --help' for more information about a command.
```

# How to run Kafka locally

This quick start shows you how to get up and running with Kafka server on your local laptop using Docker containers. After successfull run you will be able to push and consume events to/from Kafka. 

## Step 1 Configure Docker
To proceed with this manual you need to have Docker Desktop installed version 1.11 or later. [Download Docker](https://download.docker.com/win/stable/Docker%20Desktop%20Installer.exe)
You need at least 4096 MB available for Docker to run Kafka. Change this in Advanced Settings of Docker Desktop.

## Step 2 Run Kafka
In the folder run from command line:
```sh
docker-compose up -d
```
This will create newtwork, load containers and run them in background. To check that all works fine run 
```
docker-compose ps
```
You should see the five run services and all them should be in Up state.
```
     Name                  Command                    State                             Ports
-------------------------------------------------------------------------------------------------------
broker            /etc/confluent/docker/run   Up                      0.0.0.0:9092->9092/tcp
connect           /etc/confluent/docker/run   Up (health: starting)   0.0.0.0:8083->8083/tcp, 9092/tcp
control-center    /etc/confluent/docker/run   Up                      0.0.0.0:9021->9021/tcp
schema-registry   /etc/confluent/docker/run   Up                      0.0.0.0:8081->8081/tcp
zookeeper         /etc/confluent/docker/run   Up                      0.0.0.0:2181->2181/tcp, 2888/tcp, 3888/tcp
```

If the state is not Up, rerun the ``docker-compose up -d`` command.
If you want to see the log from Kafka services run ``docker-compose logs -f`` command.

## Step 3 Create Topic
In this step, you create Kafka topics by using the Control Center. Control Center provides the functionality for building and monitoring data pipelines and event streaming applications.

1. Navigate to the Control Center web interface at http://localhost:9021/.
    > It may take a minute or two for Control Center to come online.
2. Select the cluster *controlcenter.cluster* then *Topics* tab and press *Add a topic* button.
3. Enter any topic name, for example *TestTopic* and press *Create with defaults*
    > Topic name is case sensitive in Kafka
4. On the *Messages* tab of the newly created topic you can see all messages in this topic. But the message browser shows messages that have arrived since this page was opened. So you need to open the page and after that generate messages to see them in the table.

If all previous steps done well you have a running Kafka with Control Center Web UI.

## Step 4 Use Kafka Command Line Tool
To install Kafka CLI (kafka-cli) run in command line:
```
dotnet tool install --global kafka-cli
You can invoke the tool using the following command: kafka-cli
Tool 'kafka-cli' (version '1.0.7') was successfully installed.
```
The version can be different. It depends on the currently actual version of the tool.

To configure the broker host and timeout run:
```
kafka-cli config
Current configuration:
...
Do you want to update configuration? [y/N] y
Broker host: localhost:9092
Timeout in ms: 2000
```

To get all available topics in Kafka run:
```
kafka-cli topic list
```

To send message to a previously created topic run:
```
kafka-cli message produce "Message" --topic TestTopic
1 message delivered to TestTopic [[0]] @0
```

To start receiving messages from the topic run:
```
kafka-cli message consume --topic TestTopic
Waiting for messages in TestTopic:
[13:36:07.31] at TestTopic [[0]] @0: Message
```
To stop receiving messages prese Ctrl-C

## Step 4 Send Message to Topic programmatically
Create a new console application that will produce messages to the Kafka topic.
```
md Producer
cd Producer
dotnet new console
dotnet add package -v 1.2.2 Confluent.Kafka
dotnet restore
```
Open *Program.cs* and replace *Main* method with the code:
```c#
public static async Task Main(string[] args)
{
    var config = new ProducerConfig { BootstrapServers = "localhost:9092" };
    using (var p = new ProducerBuilder<Null, string>(config).Build())
    {
        try
        {
            var dr = await p.ProduceAsync("TestTopic", new Message<Null, string> { Value = "Hello World!" });
            Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
        }
        catch (ProduceException<Null, string> e)
        {
            Console.WriteLine($"Delivery failed: {e.Error.Reason}");
        }
    }
}
```
Don't forget to add using:
```
using System.Threading.Tasks;
using Confluent.Kafka;
```
After running you should see the text message in console:
```
dotnet run
Delivered 'Hello World!' to 'TestTopic [[0]] @0'
```

## Step 5 Receive Message from Topic programmatically
Create a new console application that will receive messages from the Kafka topic.
```
md Receiver
cd Receiver
dotnet new console
dotnet add package -v 1.2.2 Confluent.Kafka
dotnet restore
```
Open *Program.cs* and replace *Main* method with the code:
```c#
public static void Main(string[] args)
{
    var conf = new ConsumerConfig
    { 
        GroupId = "test-consumer-group",
        BootstrapServers = "localhost:9092",
        AutoOffsetReset = AutoOffsetReset.Earliest
    };

    using (var c = new ConsumerBuilder<Ignore, string>(conf).Build())
    {
        c.Subscribe("TestTopic");

        CancellationTokenSource cts = new CancellationTokenSource();
        Console.CancelKeyPress += (_, e) => {
            e.Cancel = true; // prevent the process from terminating.
            cts.Cancel();
        };

        try
        {
            while (true)
            {
                try
                {
                    var cr = c.Consume(cts.Token);
                    Console.WriteLine($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Error occured: {e.Error.Reason}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Ensure the consumer leaves the group cleanly and final offsets are committed.
            c.Close();
        }
    }
}
```
Don't forget to add using:
```
using System.Threading.Tasks;
using Confluent.Kafka;
```
After running you should see the message in console:
```
dotnet run
Consumed message 'Hello World!' at: 'TestTopic [[0]] @0'.
```
After that the application should stuck on line 32 and wait for a new message from the topic:
```
32: var cr = c.Consume(cts.Token);
```

## Step 7 Stop Kafka
To stop Kafka you need to run in the folder with the docker-compose.yml:
```sh
docker-compose down
```

All five containers should be stoped and removed:
```
Stopping control-center  ... done
Stopping connect         ... done
Stopping schema-registry ... done
Stopping broker          ... done
Stopping zookeeper       ... done
Removing control-center  ... done
Removing connect         ... done
Removing schema-registry ... done
Removing broker          ... done
Removing zookeeper       ... done
Removing network kafka_docker_default
```

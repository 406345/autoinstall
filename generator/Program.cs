// See https://aka.ms/new-console-template for more information
using generator;

Console.WriteLine("Start generate installer....");
var arg = Environment.GetCommandLineArgs();

if(arg.Length <= 1)
{
    return;
}

string json = File.ReadAllText(arg[1]);
var configs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ConfigItem>>(json);

AutoInstallGenerator generator = new AutoInstallGenerator();
foreach (var config in configs)
{
    generator.AddFile(config);
}

generator.Generate();
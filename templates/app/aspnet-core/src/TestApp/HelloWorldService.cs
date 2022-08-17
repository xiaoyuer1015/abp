using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;

namespace TestApp;

public class HelloWorldService : ITransientDependency
{
    public ILogger<HelloWorldService> Logger { get; set; }
    
    public IIdentityUserRepository UserRepository { get; set; }

    public HelloWorldService()
    {
        Logger = NullLogger<HelloWorldService>.Instance;
    }

    public async Task SayHelloAsync()
    {
        Logger.LogInformation("Hello World!");

        foreach (var user in await UserRepository.GetListAsync())
        {
            Console.WriteLine(user.Id + " - " + user.UserName);
        }
    }
}

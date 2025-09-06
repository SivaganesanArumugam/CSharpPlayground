using Microsoft.Extensions.DependencyInjection;

namespace CSharpPlayground.DI.Practice
{
    // EXAMPLE: Basic Dependency Injection with ServiceCollection
    // Register Multiple notifiers
    public interface INotifier
    {
        void Send(string message);
    }

    public class EmailService : INotifier
    {
        public void Send(string message)
        {
            Console.WriteLine($"Email sent: {message}");
            Console.WriteLine(GetHashCode());
        }
    }

    public class SmsService : INotifier
    {
        public void Send(string message)
        {
            Console.WriteLine($"SMS sent: {message}");
        }
    }

    public class NotificationManager
    {
        private readonly IEnumerable<INotifier> _notifier;

        // Dependency is injected here
        public NotificationManager(IEnumerable<INotifier> notifier)
        {
            _notifier = notifier;
        }

        public void Notify(string message)
        {
            foreach (var notifier in _notifier)
            {
                notifier.Send(message);
            }
        }
    }
    public class MutlipleNotifiers
    {
        public void Run()
        {
            Console.WriteLine("===DI Example ===");

            // 1. create DI container
            var services = new ServiceCollection();

            // 2. Register services
            services.AddTransient<INotifier, SmsService>();
            services.AddTransient<INotifier, EmailService>();
            services.AddTransient<NotificationManager>();

            // 3. Build provider
            var provider = services.BuildServiceProvider();

            // 4. Resolve dependency
            var manager = provider.GetRequiredService<NotificationManager>();

            //5. Use it
            manager.Notify("Hello via DI!");
        }
    }
}

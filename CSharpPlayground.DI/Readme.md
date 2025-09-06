The problem DI solves

Imagine your step definitions need a LoginPage. If you create it like this:

var page = new LoginPage(new Page(...));


Your step class decides how to create Page.

If you later want to switch browsers, or add tracing, you must edit many places.

Tests become harder to maintain and less flexible.

This is tight coupling.

2. What Dependency Injection is

Dependency = a class your code depends on (e.g., LoginPage needs IPage).

Injection = instead of creating it yourself (new), you receive it (injected) from the outside.

👉 In C#:

You normally use constructor injection.

A DI container (like Microsoft.Extensions.DependencyInjection) manages object lifetimes and wiring.

3. A small analogy

Think of DI like ordering food:

Without DI: you cook every meal yourself (lots of boilerplate, hard to swap).

With DI: you order from a restaurant — you just ask for “food”, and the kitchen decides whether it’s pizza or sushi. You can swap kitchens without rewriting your whole life.

Why seniors care about DI

Centralized config: change browser, base URL, tracing → one place.

Reuse: Pages, APIs, utilities are created consistently.

Test isolation: Each scenario gets fresh objects (scoped lifetime).

Maintainability: You add new services (e.g., EmailClient) with one registration line, no hunting down new keywords.

6. Quick memory hook

DI = “Don’t call us, we’ll call you.”
Instead of you constructing everything, the container gives you what you need, when you need it.

The Three Service Lifetimes

When you register services in DI, you choose how long an object lives (its lifetime).

1. Transient

Meaning: A new instance is created every time you request it.

Analogy: Like a disposable coffee cup — you get a fresh one each time you order.

Usage:

Lightweight, stateless services (e.g., formatters, calculators).

When you don’t want any shared state between calls.

services.AddTransient<INotifier, EmailService>();


Every GetRequiredService<INotifier>() → new EmailService object.

2. Scoped

Meaning: One instance per scope (often per request, or per test scenario).

Analogy: Like a restaurant table — while you’re sitting (scope), you reuse the same table. When you leave and a new customer comes (new scope), they get a new table.

Usage:

When you want objects shared within one logical unit of work (e.g., one web request, or one test scenario).

Common for things like DbContext in ASP.NET, or Playwright IPage in automation frameworks.

services.AddScoped<INotifier, EmailService>();


Within a scope → same instance reused.

New scope → new instance created.

3. Singleton

Meaning: A single instance is created and shared for the whole application lifetime.

Analogy: Like your fridge at home — the same fridge is used for years.

Usage:

Expensive-to-create objects that should be reused (e.g., configuration readers, logging providers).

Services that are truly stateless and thread-safe.

services.AddSingleton<INotifier, EmailService>();


All calls, all scopes, all threads → same instance.

🖥 Example with HashCodes

Let’s say each service prints its GetHashCode() when created:

public class EmailService : INotifier
{
    public EmailService()
    {
        Console.WriteLine($"EmailService created with HashCode: {this.GetHashCode()}");
    }

    public void Send(string message)
    {
        Console.WriteLine($"Email sent: {message}");
    }
}


Now register as transient:

services.AddTransient<INotifier, EmailService>();


Every time you resolve INotifier, you’ll see a different HashCode.

Scoped:

Within one scope → same HashCode reused.

Across different scopes → different HashCodes.

Singleton:

Always the same HashCode, no matter what.

📌 How to Choose the Right Lifetime

Transient
✔ Simple, lightweight services.
✔ No need to share state.
❌ Don’t use if creation is expensive.

Scoped
✔ Services that represent one unit of work (web request, test scenario).
✔ Playwright IPage in tests (each scenario = one scope).
❌ Don’t use in background services that don’t have scopes.

Singleton
✔ Config, logging, static utilities.
✔ Expensive-to-create objects you want to reuse.
❌ Avoid if they hold state that can change per request/test (leads to cross-contamination).

🧪 Quick Automation Framework Mapping

Singleton → ConfigManager, Logger

Scoped → BrowserContext, IPage (one per test scenario)

Transient → PageObjects (LoginPage, HomePage) — since they’re lightweight wrappers
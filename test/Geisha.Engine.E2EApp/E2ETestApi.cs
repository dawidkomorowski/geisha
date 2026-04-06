using System;
using System.Text.Json;

namespace Geisha.Engine.E2EApp;

internal static class E2ETestApi
{
    public static void Report(string assertId, string assertName)
    {
        Console.WriteLine($"AssertId: {{{assertId}}} Name: {{{assertName}}}");
    }

    public static void PublishMessage(string id, string content, string? value = null)
    {
        var e2EMessage = new E2EMessage
        {
            Id = Guid.Parse(id),
            Content = content,
            Value = value
        };
        Console.WriteLine(SerializeMessage(e2EMessage));
    }

    private static string SerializeMessage(E2EMessage message)
    {
        return JsonSerializer.Serialize(message);
    }

    private readonly record struct E2EMessage
    {
        public Guid Id { get; init; }
        public string? Content { get; init; }
        public string? Value { get; init; }
    }
}
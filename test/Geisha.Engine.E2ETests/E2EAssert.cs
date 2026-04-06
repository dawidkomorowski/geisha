using System;
using System.Linq;
using System.Text.Json;
using NUnit.Framework;

namespace Geisha.Engine.E2ETests;

internal static class E2EAssert
{
    public static void Reported(string appOutput, string assertId, string assertName)
    {
        var lines = appOutput.Split(Environment.NewLine);

        var assertLines = lines.Where(l => l.Contains($"AssertId: {{{assertId}}}")).ToArray();

        switch (assertLines.Length)
        {
            case > 1:
                Assert.Fail($"AssertId is duplicated: {assertId}");
                break;
            case < 1:
                Assert.Fail($"AssertId not found: {assertId}");
                break;
        }

        Assert.That(assertLines[0], Contains.Substring($"AssertId: {{{assertId}}} Name: {{{assertName}}}"), "Assert not matched.");
    }

    public static void MessagePublished(string appOutput, string id, string expectedContent, string? expectedValue = null)
    {
        var lines = appOutput.Split(Environment.NewLine);
        var linesWithJson = lines.Where(l => l.StartsWith("{") && l.EndsWith("}"));
        var messages = linesWithJson.Select(DeserializeMessage).ToArray();

        var message = messages.SingleOrDefault(m => m.Id == Guid.Parse(id));

        if (message.Id == Guid.Empty)
        {
            Assert.Fail($"Message with Id {id} not found.");
        }

        Assert.That(message.Content, Is.EqualTo(expectedContent), "Message content not matched.");
        Assert.That(message.Value, Is.EqualTo(expectedValue), "Message value not matched.");
    }

    private static E2EMessage DeserializeMessage(string json)
    {
        return JsonSerializer.Deserialize<E2EMessage>(json);
    }

    private readonly record struct E2EMessage
    {
        public Guid Id { get; init; }
        public string? Content { get; init; }
        public string? Value { get; init; }
    }
}
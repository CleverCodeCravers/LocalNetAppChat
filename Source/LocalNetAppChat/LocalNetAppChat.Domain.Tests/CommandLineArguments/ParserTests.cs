using CommandLineArguments;
using NUnit.Framework;

namespace LocalNetAppChat.Domain.Tests.CommandLineArguments;

[TestFixture]
public class ParserTests
{
    [Test]
    public void Parse_string_option_returns_correct_value()
    {
        var option = new StringCommandLineOption("--name", "A name");
        var parser = new Parser(new ICommandLineOption[] { option });

        var result = parser.TryParse(new[] { "--name", "Alice" }, false);

        Assert.That(result, Is.True);
        Assert.That(parser.GetOptionWithValue<string>("--name"), Is.EqualTo("Alice"));
    }

    [Test]
    public void Parse_bool_option_returns_true_when_present()
    {
        var option = new BoolCommandLineOption("--verbose", "Enable verbose output");
        var parser = new Parser(new ICommandLineOption[] { option });

        var result = parser.TryParse(new[] { "--verbose" }, false);

        Assert.That(result, Is.True);
        Assert.That(parser.GetBoolOption("--verbose"), Is.True);
    }

    [Test]
    public void Bool_option_defaults_to_false_when_not_present()
    {
        var option = new BoolCommandLineOption("--verbose", "Enable verbose output");
        var parser = new Parser(new ICommandLineOption[] { option });

        parser.TryParse(Array.Empty<string>(), false);

        Assert.That(parser.GetBoolOption("--verbose"), Is.False);
    }

    [Test]
    public void Parse_int_option_returns_correct_value()
    {
        var option = new Int32CommandLineOption("--port", "Port number", defaultValue: 80);
        var parser = new Parser(new ICommandLineOption[] { option });

        var result = parser.TryParse(new[] { "--port", "8080" }, false);

        Assert.That(result, Is.True);
        Assert.That(parser.GetOptionWithValue<int>("--port"), Is.EqualTo(8080));
    }

    [Test]
    public void Int_option_uses_default_value_when_not_provided()
    {
        var option = new Int32CommandLineOption("--port", "Port number", defaultValue: 80);
        var parser = new Parser(new ICommandLineOption[] { option });

        parser.TryParse(Array.Empty<string>(), false);

        Assert.That(parser.GetOptionWithValue<int>("--port"), Is.EqualTo(80));
    }

    [Test]
    public void Int_option_rejects_value_below_min()
    {
        var option = new Int32CommandLineOption("--port", "Port number", min: 1, max: 65535);
        var parser = new Parser(new ICommandLineOption[] { option });

        var result = parser.TryParse(new[] { "--port", "0" }, false);

        Assert.That(result, Is.False);
    }

    [Test]
    public void Int_option_rejects_value_above_max()
    {
        var option = new Int32CommandLineOption("--port", "Port number", min: 1, max: 65535);
        var parser = new Parser(new ICommandLineOption[] { option });

        var result = parser.TryParse(new[] { "--port", "70000" }, false);

        Assert.That(result, Is.False);
    }

    [Test]
    public void Int_option_rejects_non_numeric_value()
    {
        var option = new Int32CommandLineOption("--port", "Port number");
        var parser = new Parser(new ICommandLineOption[] { option });

        var result = parser.TryParse(new[] { "--port", "abc" }, false);

        Assert.That(result, Is.False);
    }

    [Test]
    public void Parse_multiple_options_together()
    {
        var nameOption = new StringCommandLineOption("--name", "A name");
        var portOption = new Int32CommandLineOption("--port", "Port number");
        var verboseOption = new BoolCommandLineOption("--verbose", "Verbose");
        var parser = new Parser(new ICommandLineOption[] { nameOption, portOption, verboseOption });

        var result = parser.TryParse(new[] { "--name", "Alice", "--port", "8080", "--verbose" }, false);

        Assert.That(result, Is.True);
        Assert.That(parser.GetOptionWithValue<string>("--name"), Is.EqualTo("Alice"));
        Assert.That(parser.GetOptionWithValue<int>("--port"), Is.EqualTo(8080));
        Assert.That(parser.GetBoolOption("--verbose"), Is.True);
    }

    [Test]
    public void Parse_empty_args_returns_true()
    {
        var option = new StringCommandLineOption("--name", "A name");
        var parser = new Parser(new ICommandLineOption[] { option });

        var result = parser.TryParse(Array.Empty<string>(), false);

        Assert.That(result, Is.True);
    }

    [Test]
    public void Unknown_argument_without_allowIncompleteness_returns_false()
    {
        var option = new StringCommandLineOption("--name", "A name");
        var parser = new Parser(new ICommandLineOption[] { option });

        var result = parser.TryParse(new[] { "--unknown", "value" }, false);

        Assert.That(result, Is.False);
    }

    [Test]
    public void Unknown_argument_with_allowIncompleteness_returns_true()
    {
        var option = new StringCommandLineOption("--name", "A name");
        var parser = new Parser(new ICommandLineOption[] { option });

        var result = parser.TryParse(new[] { "--unknown", "value" }, true);

        Assert.That(result, Is.True);
    }

    [Test]
    public void GetBoolOption_throws_when_option_not_found()
    {
        var parser = new Parser(Array.Empty<ICommandLineOption>());

        Assert.Throws<Exception>(() => parser.GetBoolOption("--nonexistent"));
    }

    [Test]
    public void GetOptionWithValue_throws_when_option_not_found()
    {
        var parser = new Parser(Array.Empty<ICommandLineOption>());

        Assert.Throws<Exception>(() => parser.GetOptionWithValue<string>("--nonexistent"));
    }

    [Test]
    public void TryGetOptionWithValue_returns_default_when_option_not_found()
    {
        var parser = new Parser(Array.Empty<ICommandLineOption>());

        var value = parser.TryGetOptionWithValue<string>("--nonexistent");

        Assert.That(value, Is.Null);
    }

    [Test]
    public void String_option_uses_default_value_when_not_provided()
    {
        var option = new StringCommandLineOption("--name", "A name", defaultValue: "default");
        var parser = new Parser(new ICommandLineOption[] { option });

        parser.TryParse(Array.Empty<string>(), false);

        Assert.That(parser.GetOptionWithValue<string>("--name"), Is.EqualTo("default"));
    }

    [Test]
    public void GetCommandsList_returns_all_options()
    {
        var opt1 = new StringCommandLineOption("--name", "A name");
        var opt2 = new BoolCommandLineOption("--verbose", "Verbose");
        var options = new ICommandLineOption[] { opt1, opt2 };
        var parser = new Parser(options);

        var commands = parser.GetCommandsList();

        Assert.That(commands.Length, Is.EqualTo(2));
    }

    [Test]
    public void Option_name_matching_is_case_insensitive()
    {
        var option = new StringCommandLineOption("--Name", "A name");
        var parser = new Parser(new ICommandLineOption[] { option });

        var result = parser.TryParse(new[] { "--name", "Alice" }, false);

        Assert.That(result, Is.True);
        Assert.That(parser.GetOptionWithValue<string>("--Name"), Is.EqualTo("Alice"));
    }

    [Test]
    public void String_option_missing_value_throws_exception()
    {
        var option = new StringCommandLineOption("--name", "A name");
        var parser = new Parser(new ICommandLineOption[] { option });

        Assert.Throws<Exception>(() => parser.TryParse(new[] { "--name" }, false));
    }

    [Test]
    public void Bool_option_IsTypeWithSeparateValue_is_false()
    {
        var option = new BoolCommandLineOption("--verbose", "Verbose");

        Assert.That(option.IsTypeWithSeparateValue, Is.False);
    }

    [Test]
    public void String_option_IsTypeWithSeparateValue_is_true()
    {
        var option = new StringCommandLineOption("--name", "A name");

        Assert.That(option.IsTypeWithSeparateValue, Is.True);
    }

    [Test]
    public void Int_option_IsTypeWithSeparateValue_is_true()
    {
        var option = new Int32CommandLineOption("--port", "Port");

        Assert.That(option.IsTypeWithSeparateValue, Is.True);
    }
}

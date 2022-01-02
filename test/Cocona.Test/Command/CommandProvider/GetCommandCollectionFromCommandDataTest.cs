using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cocona.Builder;
using Cocona.Builder.Metadata;
using Cocona.Command;
using FluentAssertions;
using Xunit;

namespace Cocona.Test.Command.CommandProvider
{
    public class GetCommandCollectionFromCommandDataTest
    {
        [Fact]
        public void Complex()
        {
            void TestMethod() { }

            var provider = new CoconaCommandProvider(new ICommandData[]
            {
                new DelegateCommandData(new Action(TestMethod).Method, this, new[] { new CommandNameMetadata("TestMethod") }),
                new TypeCommandData(typeof(CommandTestSingleCommand), Array.Empty<object>()),
            });
            var collection = provider.GetCommandCollection();
            collection.All.Should().HaveCount(2);
            collection.All[0].Name.Should().Be("TestMethod");
            collection.All[1].Name.Should().Be("A");
        }

        [Fact]
        public void SubCommands()
        {
            void TestMethod() { }

            var provider = new CoconaCommandProvider(new ICommandData[]
            {
                new SubCommandData(new ICommandData[]
                {
                    new DelegateCommandData(new Action(TestMethod).Method, this, new[] { new CommandNameMetadata("TestMethod") }),
                    new TypeCommandData(typeof(CommandTestSingleCommand), Array.Empty<object>()),
                }, new [] { new CommandNameMetadata("sub-command") }),
            });
            var collection = provider.GetCommandCollection();
            collection.All.Should().HaveCount(1);
            collection.All[0].Name.Should().Be("sub-command");
            collection.All[0].SubCommands.All.Should().HaveCount(2);
            collection.All[0].SubCommands.All[0].Name.Should().Be("TestMethod");
            collection.All[0].SubCommands.All[1].Name.Should().Be("A");
        }

        [Fact]
        public void DelegateCommandData()
        {
            void TestMethod() { }

            var provider = new CoconaCommandProvider(new[] { new DelegateCommandData(new Action(TestMethod).Method, this, new[] { new CommandNameMetadata("TestMethod") }) });
            var collection = provider.GetCommandCollection();
            collection.All.Should().HaveCount(1);
            collection.All[0].Name.Should().Be("TestMethod");
        }

        [Fact]
        public void DelegateCommandData_Static()
        {
            var provider = new CoconaCommandProvider(new[] { new DelegateCommandData(new Action<string>(CommandTest_Static.A).Method, null, new[] { new CommandNameMetadata("A") }) });
            var collection = provider.GetCommandCollection();
            collection.All.Should().HaveCount(1);
            collection.All[0].Name.Should().Be("A");
            collection.All[0].Target.Should().BeNull();
            collection.All[0].Method.IsStatic.Should().BeTrue();
        }

        [Fact]
        public void TypeCommandData_SingleCommand()
        {
            var provider = new CoconaCommandProvider(new[] { new TypeCommandData(typeof(CommandTestSingleCommand), Array.Empty<object>()) });
            var collection = provider.GetCommandCollection();
            collection.All.Should().HaveCount(1);
            collection.All[0].Name.Should().Be("A");
        }

        [Fact]
        public void TypeCommandData_PrimaryCommand_Duplicate()
        {
            var provider = new CoconaCommandProvider(new[]
            {
                new TypeCommandData(typeof(CommandTestSingleCommand), new object[] { new CommandNameMetadata("A"), new PrimaryCommandAttribute() }),
                new TypeCommandData(typeof(CommandTestSingleCommand), new object[] { new CommandNameMetadata("B"), new PrimaryCommandAttribute() }),
            });

            Assert.Throws<CoconaException>(() => provider.GetCommandCollection()).Message.Should().Contain("The commands contains more then one primary command.");
        }

        [Fact]
        public void TypeCommandData_MultipleCommands()
        {
            var provider = new CoconaCommandProvider(new[] { new TypeCommandData(typeof(CommandTestMultipleCommand), Array.Empty<object>()) });
            var collection = provider.GetCommandCollection();
            collection.All.Should().HaveCount(2);
            collection.All[0].Name.Should().Be("A");
            collection.All[1].Name.Should().Be("B");
        }

        [Fact]
        public void TypeCommandData_MultipleTypes()
        {
            var provider = new CoconaCommandProvider(new[] { new TypeCommandData(typeof(CommandTestSingleCommand), Array.Empty<object>()), new TypeCommandData(typeof(CommandTestSingleCommand2), Array.Empty<object>()) });
            var collection = provider.GetCommandCollection();
            collection.All.Should().HaveCount(2);
            collection.All[0].Name.Should().Be("A");
            collection.All[1].Name.Should().Be("A2");
        }

        [Fact]
        public void TypeCommandData_HasSameNameCommands()
        {
            var provider = new CoconaCommandProvider(new[] { new TypeCommandData(typeof(CommandTestSingleCommand), Array.Empty<object>()), new TypeCommandData(typeof(CommandTestMultipleCommand), Array.Empty<object>()) });
            Assert.Throws<CoconaException>(() => provider.GetCommandCollection());
        }
        public static class CommandTest_Static
        {
            public static void A(string name) { }
        }
        public class CommandTestSingleCommand
        {
            public void A(string name) { }
        }
        public class CommandTestSingleCommand2
        {
            public void A2(string name) { }
        }
        public class CommandTestMultipleCommand
        {
            public void A(string name) { }
            public void B(string name) { }

            [Ignore]
            public void C(string name) { }
        }
    }
}

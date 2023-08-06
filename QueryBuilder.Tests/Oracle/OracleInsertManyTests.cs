using SqlKata.Compilers;
using SqlKata.Tests.Infrastructure;
using Xunit;

namespace SqlKata.Tests.Oracle
{
    public class OracleInsertManyTests : TestSupport
    {
        private const string TableName = "Table";
        private readonly OracleCompiler compiler;

        public OracleInsertManyTests()
        {
            compiler = Compilers.Get<OracleCompiler>(EngineCodes.Oracle);
        }

        [Fact]
        public void InsertManyForOracle_ShouldRepeatColumnsAndAddSelectFromDual()
        {
            // Arrange:
            var cols = new[] { "Name", "Price" };

            var data = new[] {
                new object[] { "A", 1000 },
                new object[] { "B", 2000 },
                new object[] { "C", 3000 },
            };

            var query = new Query(TableName)
                .AsInsert(cols, data);


            // Act:
            var ctx = compiler.Compile(query);

            // Assert:
            Assert.Equal($@"INSERT ALL INTO ""{TableName}"" (""Name"", ""Price"") VALUES (?, ?) INTO ""{TableName}"" (""Name"", ""Price"") VALUES (?, ?) INTO ""{TableName}"" (""Name"", ""Price"") VALUES (?, ?) SELECT 1 FROM DUAL", ctx.RawSql);
        }

        [Fact]
        public void InsertForOracle_SingleInsertShouldNotAddALLKeywordAndNotHaveSelectFromDual()
        {
            // Arrange:
            var cols = new[] { "Name", "Price" };

            var data = new[] {
                new object[] { "A", 1000 }
            };

            var query = new Query(TableName)
                .AsInsert(cols, data);


            // Act:
            var ctx = compiler.Compile(query);

            // Assert:
            Assert.Equal($@"INSERT INTO ""{TableName}"" (""Name"", ""Price"") VALUES (?, ?)", ctx.RawSql);
        }
    }
}

using Cronus.Interfaces;
using Cronus.Parser.Queries;
using Sprache;
using System.Globalization;

namespace Cronus.Parser
{
    internal class QueryParser : IQueryParser
    {
        private static readonly Parser<string> _identifier =
            from leading in Parse.WhiteSpace.Many()
            from first in Parse.Letter.Once().Text()
            from rest in Parse.LetterOrDigit.Or(Parse.Char('_')).Many().Text()
            from trailing in Parse.WhiteSpace.Many()
            select first + rest;

        private static readonly Parser<string> _stringLiteral =
            from leading in Parse.WhiteSpace.Many()
            from open in Parse.Char('"')
            from text in Parse.CharExcept('"').Many().Text()
            from close in Parse.Char('"')
            from trailing in Parse.WhiteSpace.Many()
            select text;

        private static readonly Parser<int> _intLiteral =
            from leading in Parse.WhiteSpace.Many()
            from digits in Parse.Digit.AtLeastOnce().Text()
            from trailing in Parse.WhiteSpace.Many()
            select int.Parse(digits);

        private static readonly Parser<double> _doubleLiteral = 
            from leading in Parse.WhiteSpace.Many()
            from doubleNumber in Parse.DecimalInvariant.Token()
            from trailing in Parse.WhiteSpace.Many()
            select double.Parse(doubleNumber, CultureInfo.InvariantCulture);

        private static readonly Parser<bool> _boolLiteral = 
            Parse.IgnoreCase("true").Token().Return(true)
            .Or(Parse.IgnoreCase("false").Token().Return(false));

        private static readonly Parser<object?> _literalValue =
            _stringLiteral.Select(v => (object?)v)
            .Or(_boolLiteral.Select(v => (object?)v))
            .Or(_doubleLiteral.Select(v => (object?)v))
            .Or(_intLiteral.Select(v => (object?)v));

        private static readonly Parser<string> _operator =
            Parse.String("!=").Text()
            .Or(Parse.String("<=").Text())
            .Or(Parse.String(">=").Text())
            .Or(Parse.String("<").Text())
            .Or(Parse.String(">").Text())
            .Or(Parse.String("=").Text());

        private static Parser<string> KeyWord(string text)
        {
            return from leading in Parse.WhiteSpace.Many()
            from word in Parse.IgnoreCase(text).Text()
            from trailing in Parse.WhiteSpace.Many()
            select word.ToUpperInvariant();
        }

        private static readonly Parser<ICondition> _simpleCondition =
            from column in _identifier
            from @operator in _operator
            from value in _literalValue
            select new BinaryCondition
            {
                Left = column,
                Operator = @operator,
                Right = value
            };

        private static readonly Parser<ICondition?> _whereCondition =
            (from @where in KeyWord("WHERE")
             from condition in _simpleCondition
             select condition)
            .Optional()
            .Select(opt => opt.GetOrDefault());

        private static readonly Parser<IQuery> InsertQueryParser =
            from insertWord in KeyWord("INSERT")
            from intoWord in KeyWord("INTO")
            from table in _identifier
            from openColBracket in Parse.Char('(').Token()
            from columns in _identifier.DelimitedBy(Parse.Char(',').Token())
            from closeColBracket in Parse.Char(')').Token()
            from valuesWord in KeyWord("VALUES")
            from openValuesBracket in Parse.Char('(').Token()
            from values in _literalValue.DelimitedBy(Parse.Char(',').Token())
            from closeValuesBracket in Parse.Char(')').Token()
            select new InsertQuery(
                table,
                columns.Zip(values, (c, v) => new { c, v }).ToDictionary(x => x.c, x => x.v));


        private static readonly Parser<IQuery> SelectQueryParser =
            from selectWord in KeyWord("SELECT")
            from columns in
                Parse.Char('*').Token()
                    .Select(_ => (IReadOnlyList<string>)["*"])
                .Or(_identifier.DelimitedBy(Parse.Char(',').Token())
                    .Select(list => list.ToList()))
            from fromWord in KeyWord("FROM")
            from table in _identifier
            from whereCondition in _whereCondition
            select new SelectQuery(table, columns, whereCondition);

        private static readonly Parser<IQuery> UpdateQueryParser =
            from updateWord in KeyWord("UPDATE")
            from table in _identifier
            from set in KeyWord("SET")
            from assignments in
                (from column in _identifier
                 from equalsMark in Parse.Char('=').Token()
                 from value in _literalValue
                 select new { column, value })
                .DelimitedBy(Parse.Char(',').Token())
            from whereCondition in _whereCondition
            select new UpdateQuery(
                table,
                assignments.ToDictionary(a => a.column, a => a.value),
                whereCondition);

        private static readonly Parser<IQuery> DeleteQueryParser =
            from deleteWord in KeyWord("DELETE")
            from fromWord in KeyWord("FROM")
            from table in _identifier
            from whereClause in _whereCondition
            select new DeleteQuery(table, whereClause);

        private static readonly Parser<IQuery> RootParser =
            InsertQueryParser
            .Or(SelectQueryParser)
            .Or(UpdateQueryParser)
            .Or(DeleteQueryParser);

        public IQuery ParseQuery(string query)
        {
            try
            {
                return RootParser.Parse(query);
            }
            catch (ParseException ex)
            {
                throw new InvalidOperationException($"Parse error: {ex.Message}", ex);
            }
        }
    }
}

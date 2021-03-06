﻿using System.Linq;

namespace Sir.Store
{
    /// <summary>
    /// Parses key:value clauses where keys are separated from values by a ':' and
    /// clauses are separated by newline characters. 
    /// Clauses may be appended with a + sign (meaning AND), a - sign (meaning NOT) or nothing (meaning OR).
    /// </summary>
    public class BooleanKeyValueQueryParser : IQueryParser
    {
        public string ContentType => "*";
        private static  char[] Operators = new char[] { ' ', '+', '-' };

        public Query Parse(string query, ITokenizer tokenizer)
        {
            Query root = null;
            Query previous = null;
            var clauses = query.Split('\n');

            foreach (var clause in clauses)
            {
                var tokens = clause.Split(':');
                var key = tokens[0];
                string v;

                if (tokens.Length > 2)
                {
                    v = string.Join(" ", tokens.Skip(1));
                }
                else
                {
                    v = tokens[1];
                }

                var vals = (key[0] == '_' || tokenizer == null) ? 
                    new[] { tokenizer.Normalize(v) } : tokenizer.Tokenize(v);

                var and = root == null || key[0] == '+';
                var not = key[0] == '-';
                var or = !and && !not;

                if (Operators.Contains(key[0]))
                {
                    key = key.Substring(1);
                }

                foreach (var val in vals)
                {
                    var q = new Query { Term = new Term(key, val), Or = true };

                    if (previous == null)
                    {
                        root = q;
                        previous = q;
                    }
                    else
                    {
                        previous.Next = q;
                        previous = q;
                    }
                }
            }
            
            return root;
        }
        
        public void Dispose()
        {
        }
    }
}

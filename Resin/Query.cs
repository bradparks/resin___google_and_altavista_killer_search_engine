using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resin
{
    public class Query : Term
    {
        public IList<Query> Children { get; protected set; }
        public IDictionary<string, DocumentScore> TermResult { get; set ; }  

        public Query(string field, string token) : base(field, token)
        {
            Children = new List<Query>();
        }

        public IDictionary<string, DocumentScore> Resolve()
        {
            var result = TermResult;
            foreach (var child in Children)
            {
                if (child.And)
                {
                    var list = result.Values.ToList();
                    for(int i = 0;i<list.Count;i++)
                    {
                        var d = list[0];
                        DocumentScore score;
                        if (child.Resolve().TryGetValue(d.DocId, out score))
                        {
                            result[d.DocId] = score.Add(d);
                        }
                        else
                        {
                            result.Remove(d.DocId);
                        }
                    }
                }
                else if (child.Not)
                {
                    foreach (var d in child.Resolve())
                    {
                        result.Remove(d.Key);
                    }
                }
                else // Or
                {
                    foreach (var d in child.Resolve())
                    {
                        DocumentScore existingScore;
                        if (result.TryGetValue(d.Key, out existingScore))
                        {
                            result[d.Key] = existingScore.Add(d.Value);
                        }
                        else
                        {
                            result.Add(d);
                        }
                    }
                }
            }
            return result;
        }

        public override string ToString()
        {
            var s = new StringBuilder();
            s.AppendFormat(base.ToString());
            foreach (var child in Children)
            {
                s.AppendFormat(" {0}", child);
            }
            return s.ToString();
        }
    }
}
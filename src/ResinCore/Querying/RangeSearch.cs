﻿using DocumentTable;
using Resin.IO;
using StreamIndex;
using System.Collections.Generic;
using System.Diagnostics;

namespace Resin.Querying
{
    public class RangeSearch : Search
    {
        public RangeSearch(IReadSession session, IScoringSchemeFactory scoringFactory, PostingsReader postingsReader)
            : base(session, scoringFactory, postingsReader) { }


        public void Search(QueryContext ctx, string valueUpperBound)
        {
            var time = Stopwatch.StartNew();

            var addresses = new List<BlockInfo>();

            using (var reader = GetTreeReader(ctx.Query.Key))
            {
                var words = reader.Range(ctx.Query.Value, valueUpperBound);

                foreach (var word in words)
                {
                    addresses.Add(word.PostingsAddress.Value);
                }
            }

            Log.DebugFormat("found {0} matching terms for the query {1} in {2}",
                    addresses.Count, ctx.Query, time.Elapsed);

            var postings = PostingsReader.Read(addresses);

            ctx.Scores = Score(postings);
        }
    }
}

﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System;

namespace Resin
{
    public class TabSeparatedStream : DocumentSource, IDisposable
    {
        private readonly StreamReader Reader;
        private readonly int _take;
        private readonly int _skip;

        public TabSeparatedStream(string fileName, int skip, int take) 
            : this(File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None), skip, take)
        {
        }

        public TabSeparatedStream(Stream stream, int skip, int take)
        {
            _skip = skip;
            _take = take;

            Reader = new StreamReader(new BufferedStream(stream), Encoding.UTF8);
        }

        public override IEnumerable<Document> ReadSource()
        {
            var fieldNames = Reader.ReadLine().Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

            if (_skip > 0)
            {
                int skipped = 0;

                while (skipped++ < _skip)
                {
                    Reader.ReadLine();
                }
            }

            return ReadInternal(fieldNames).Take(_take);
        }

        private IEnumerable<Document> ReadInternal(string[] fieldNames)
        {
            while (true)
            {
                var fields = new List<Field>();

                foreach(var fieldName in fieldNames)
                {
                    var fieldValue = ReadUntilTab().ToArray();

                    fields.Add(new Field(fieldName, new string(fieldValue)));
                }

                if (fields.Count == 0) break;

                yield return new Document(fields);

                Reader.ReadLine();
            }
        }

        private IEnumerable<char> ReadUntilTab()
        {
            int c;
            while ((c = Reader.Read()) != -1)
            {
                var ch = (char)c;
                if (ch == '\t') break;
                yield return ch;
            }
        }

        private IEnumerable<Field> Parse(string document, string[] fieldNames)
        {
            var fields = document.Split(new[] { '\t' }, System.StringSplitOptions.RemoveEmptyEntries);

            for (int index = 0; index < fields.Length; index++)
            {
                yield return new Field(fieldNames[index], fields[index]);
            }
        }

        public void Dispose()
        {
            Reader.Dispose();
        }
    }
}
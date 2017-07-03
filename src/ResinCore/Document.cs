﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Resin
{
    public class Document
    {
        public int Id { get; set; }

        public UInt64 Hash { get; set; }

        private readonly IDictionary<string, Field> _fields;

        public IDictionary<string, Field> Fields { get { return _fields; } }

        public Document(int documentId, IList<Field> fields) : this(fields)
        {
            Id = documentId;
        }

        public Document(IList<Field> fields)
        {
            if (fields == null) throw new ArgumentNullException("fields");

            _fields = fields.ToDictionary(x=>x.Key);
        }

        public DocumentTableRow ToTableRow(IDictionary<string, short> keyIndex)
        {
            var fields = Fields.Values.ToDictionary(
                field=> keyIndex[field.Key], y => y);

            return new DocumentTableRow(fields);
        }
    }

    public class DocumentTableRow
    {
        public IDictionary<short, Field> Fields { get; private set; }

        public DocumentTableRow(IDictionary<short, Field> fields)
        {
            Fields = fields;
        }
    }
}
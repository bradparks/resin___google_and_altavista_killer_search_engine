﻿using System.Collections.Generic;
using System.IO;

namespace Resin.IO.Read
{
    public class PostingsReader : BlockReader<List<DocumentPosting>>
    {
        public PostingsReader(Stream stream, bool leaveOpen = false)
            : base(stream, leaveOpen)
        {
        }
        protected override List<DocumentPosting> Deserialize(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                return (List<DocumentPosting>)GraphSerializer.Serializer.Deserialize(stream);
            }
        }
    }
}
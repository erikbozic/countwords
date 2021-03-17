using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace optimized
{
    public static partial class Implementations
    {
        private const int BufferSize = 65536;

        public static void CsharpOptimized2()
        {
            var counts = new Dictionary<string, StrongBox<int>>();
            var buffer = new char[BufferSize];
            var offset = 0;
            using var streamReader = File.OpenText("/home/erik/src/sandbox/countwords/kjvbible_x10.txt");
            while (true)
            {
                // Read from stdin in chars
                var bytesRead = streamReader.Read(buffer, offset, BufferSize - offset);
                if (bytesRead == 0)
                {
                    break;
                }

                // Determine last delimiter position
                var bufferLength = offset + bytesRead;
                var lastDelimiterIndex = -1;
                for (var k = bufferLength - 1; k >= 0; --k)
                {
                    if (buffer[k] <= ' ')
                    {
                        lastDelimiterIndex = k;
                        break;
                    }
                }

                var count = lastDelimiterIndex >= 0 ? lastDelimiterIndex : bufferLength;
                var i = 0;

                while (true)
                {
                    // Skip whitespaces before each word
                    for (; i < count; ++i)
                    {
                        var c = buffer[i];
                        if (c > ' ')
                        {
                            break;
                        }
                    }

                    // Find word boundary and make it lowercase at the same time.
                    var start = i;
                    for (; i < count; ++i)
                    {
                        var c = buffer[i];
                        if (c <= ' ')
                        {
                            break;
                        }

                        // Set 6th bit to make char lowercase
                        buffer[i] = (char)(buffer[i] | 0x20);
                    }

                    if (i <= start)
                    {
                        break;
                    }

                    // Collect word statistics
                    var word = new String(buffer, start, i - start);
                    if (!counts.TryGetValue(word, out var wordCountRef))
                    {
                        counts.Add(word, new StrongBox<int>(1));
                    }
                    else
                    {
                        ++wordCountRef.Value;
                    }
                }

                // Copy remaining part to the beginning of the buffer.
                if (lastDelimiterIndex >= 0)
                {
                    offset = (offset + bytesRead - 1) - lastDelimiterIndex;
                    Array.Copy(buffer, lastDelimiterIndex + 1, buffer, 0, offset);
                }
                else
                {
                    offset = 0;
                }
            }

            // Finally list all word frequencies.
            var ordered = counts.OrderByDescending(pair => pair.Value.Value);
            var sb = new StringBuilder();
            foreach (var kv in ordered)
            {
                sb.Append(kv.Key.ToString()).Append(' ').AppendLine(kv.Value.Value.ToString());
            }
            sb.Length--;
            Console.WriteLine(sb.ToString());
        }
    }
}

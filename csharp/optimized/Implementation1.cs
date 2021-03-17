using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace optimized
{
    public static partial class Implementations
    {
        public static void CsharpOptimized1()
        {
            var wordCount = new Dictionary<string, StrongBox<int>>();
            using var streamReader = File.OpenText("/home/erik/src/sandbox/countwords/kjvbible_x10.txt");
            void AddToDictionary(string key)
            {
                if (!wordCount.TryGetValue(key, out var count))
                {
                    wordCount.Add(key, new StrongBox<int>(1));
                }
                else
                {
                    ++count.Value;
                }
            }

            char[] buffer = new char[64 * 1024];
            string leftover = null;
            var concat = false;
            int read;
            while ((read = streamReader.ReadBlock(buffer)) > 0)
            {
                if (leftover != null)
                {
                    if (buffer[0] <= ' ')
                    {
                        AddToDictionary(leftover);
                        leftover = null;
                    }
                    concat = true;
                }

                int lastPos = 0;
                for (int index = 0; index <= read; index++)
                {
                    var endOfBuffer = index == read;
                    if (endOfBuffer || buffer[index] <= ' ')
                    {
                        if (lastPos < index)
                        {
                            // Save the leftovers for next streamReader.Read() iteration
                            if (endOfBuffer)
                            {
                                leftover = new String(buffer, lastPos, index - lastPos);
                            }
                            else
                            {
                                if (concat)
                                {
                                    AddToDictionary(String.Concat(leftover, buffer.AsSpan(lastPos, index - lastPos)));
                                    leftover = null;
                                    concat = false;
                                }
                                else
                                {
                                    AddToDictionary(new String(buffer, lastPos, index - lastPos));
                                }
                            }
                        }
                        lastPos = index + 1;
                    }
                    else
                    {
                        buffer[index] = (char) (buffer[index] | 0x20); // lowercase ascii
                    }
                }
            }

            // minimal performance impact
            var ordered = wordCount.OrderByDescending(pair => pair.Value.Value);
            var sb = new StringBuilder();
            foreach (var entry in ordered)
            {
                sb.Append(entry.Key).Append(' ').AppendLine(entry.Value.Value.ToString());
            }

            sb.Length--; // remove last new line (what about \r\n?)
            Console.WriteLine(sb.ToString());
        }
    }
}

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

var wordCount = new Dictionary<string, StrongBox<int>>();

//using var streamReader = File.OpenText("/home/erik/src/sandbox/countwords/kjvbible_x10.txt");
using var stream = Console.OpenStandardInput();
using var streamReader = new StreamReader(stream, Encoding.ASCII, false, 64 * 1024);

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
            if (!wordCount.TryGetValue(leftover, out var count))
            {
                wordCount.Add(leftover, new StrongBox<int>(1));
            }
            else
            {
                ++count.Value;
            }
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
                    string word;
                    if (concat)
                    {
                        word = String.Concat(leftover, buffer.AsSpan(lastPos, index - lastPos));
                        leftover = null;
                        concat = false;
                    }
                    else
                    {
                        word = new String(buffer, lastPos, index - lastPos);
                    }

                    if (!wordCount.TryGetValue(word, out var count))
                    {
                        wordCount.Add(word, new StrongBox<int>(1));
                    }
                    else
                    {
                        ++count.Value;
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


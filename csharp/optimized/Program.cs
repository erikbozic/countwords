using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

var wordCount = new Dictionary<string, StrongBox<int>>(StringComparer.OrdinalIgnoreCase);

using var streamReader = new StreamReader(Console.OpenStandardInput()); // File.OpenText("/home/erik/src/sandbox/countwords/kjvbible_x10.txt"); // Console.OpenStandardInput();
// using var streamReader = new StreamReader(stream, System.Text.Encoding.UTF8, false, 64 * 1024);
const int bufferLength = 24 * 1024;
char[] buffer = new char[bufferLength]; // char[] instead of span to avoid cost while indexing into it in the main loop
string leftover = null;
var concat = false;

while (streamReader.ReadBlock(buffer) > 0)
{
    // prepend leftover if any and new chunk starts with whitespace
    if (leftover != null)
    {
        if (buffer[0] <= ' ')
        {
            if (!wordCount.TryGetValue(leftover, out var count)) {
                wordCount.Add(leftover, new StrongBox<int>(1));
            } else {
                ++count.Value;
            }
            leftover = null;
        }
        concat = true;
    }

    int lastPos = 0;
    for (int index = 0; index <= bufferLength; index++)
    {
        var endOfBuffer = index == bufferLength;
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
                    // TryGetValue takes ~65% of the time
                    if (!wordCount.TryGetValue(word, out var count)) {
                        wordCount.Add(word, new StrongBox<int>(1));
                    } else {
                        ++count.Value;
                    }
                }
            }
            lastPos = index + 1;
        }
    }
}

// minimal performance impact
var ordered = wordCount.OrderByDescending(pair => pair.Value.Value);
foreach (var entry in ordered)
{
    Console.WriteLine("{0} {1}", entry.Key.ToLower(), entry.Value.Value.ToString());
}


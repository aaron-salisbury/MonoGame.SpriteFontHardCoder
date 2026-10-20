using System.Collections.Generic;

namespace MonoGame.SpriteFontHardEncoder;

internal record RunLengthEncodingData
{
    internal required List<byte> Data { get; init; }
    internal required int ByteDataCount { get; init; }
    internal required int Pixels { get; init; }
    internal required int BytesTallied { get; init; }
}
